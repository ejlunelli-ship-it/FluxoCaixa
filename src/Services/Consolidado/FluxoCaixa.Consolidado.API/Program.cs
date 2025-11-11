using FluentValidation;
using FluxoCaixa.Consolidado.Application.Commands.AtualizarConsolidadoDiario;
using FluxoCaixa.Consolidado.Domain.Repositories;
using FluxoCaixa.Consolidado.Infrastructure.Data;
using FluxoCaixa.Consolidado.Infrastructure.Repositories;
using FluxoCaixa.Consolidado.Infrastructure.Consumers;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<ConsolidadoDbContext>(options => {
    options.UseSqlServer(
      builder.Configuration.GetConnectionString("DefaultConnection"),
      sqlOptions => {
          sqlOptions.MigrationsAssembly(typeof(ConsolidadoDbContext).Assembly.FullName);
          sqlOptions.EnableRetryOnFailure(
          maxRetryCount: 3,
          maxRetryDelay: TimeSpan.FromSeconds(30),
          errorNumbersToAdd: null);
          sqlOptions.CommandTimeout(30);
      });

    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

// Repositories
builder.Services.AddScoped<IConsolidadoDiarioRepository, ConsolidadoDiarioRepository>();

//Autenticação JWT (mesma configuração da API Lançamentos)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
  .AddJwtBearer(options => {
      options.TokenValidationParameters = new TokenValidationParameters
      {
          ValidateIssuer = true,
          ValidateAudience = true,
          ValidateLifetime = true,
          ValidateIssuerSigningKey = true,
          ValidIssuer = builder.Configuration["Jwt:Issuer"],
          ValidAudience = builder.Configuration["Jwt:Audience"],
          IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ??
              "ChaveSecreta_FluxoCaixa2025_DeveSerMaiorQue32Caracteres!@#")),
          ClockSkew = TimeSpan.Zero
      };
  });

//Autorização com Policies
builder.Services.AddAuthorization(options => {
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
    options.AddPolicy("Operador", policy => policy.RequireRole("Admin", "Operador"));
    options.AddPolicy("Viewer", policy => policy.RequireRole("Admin", "Operador", "Viewer"));
});

// MassTransit
builder.Services.AddMassTransit(x => {
    x.AddConsumer<LancamentoCriadoConsumer>();

    x.UsingRabbitMq((context, cfg) => {
        cfg.Host("localhost", "/", h => {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ReceiveEndpoint("consolidado-lancamentos-queue", e => {
            e.UseMessageRetry(r => r.Intervals(
              TimeSpan.FromSeconds(1),
              TimeSpan.FromSeconds(5),
              TimeSpan.FromSeconds(10)
            ));

            e.ConfigureConsumer<LancamentoCriadoConsumer>(context);
        });
    });
});

// MediatR
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(AtualizarConsolidadoDiarioCommand).Assembly);
});

// FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<AtualizarConsolidadoDiarioValidator>();

// Rate Limiting - 50 req/s com máximo 5% de perda
builder.Services.AddRateLimiter(options => {
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
      RateLimitPartition.GetFixedWindowLimiter(
        partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
        factory: _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 50,
            Window = TimeSpan.FromSeconds(1),
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = 2
        }));

    options.OnRejected = async (context, cancellationToken) => {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;

        var response = new
        {
            error = "Rate limit exceeded",
            message = "Limite de 50 requisições por segundo atingido. Por favor, aguarde antes de tentar novamente.",
            retryAfter = context.Lease.TryGetMetadata(MetadataName.RetryAfter, out
              var retryAfter) ?
            retryAfter.TotalSeconds : 1
        };

        await context.HttpContext.Response.WriteAsJsonAsync(response, cancellationToken);
    };
});

// API
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ? Swagger com suporte a JWT
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new()
    {
        Title = "Fluxo de Caixa - API de Consolidado",
        Version = "v1",
        Description = "API para consulta de consolidado diário (Rate Limit: 50 req/s) com autenticação JWT"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization. Digite apenas o token (Bearer será adicionado automaticamente)",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    }); ;

    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
    {
      new OpenApiSecurityScheme {
        Reference = new OpenApiReference {
          Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
      },
      Array.Empty < string > ()
    }
  });
});

var app = builder.Build();

// Migrations
if (app.Environment.IsDevelopment())
{
    using
    var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ConsolidadoDbContext>();

    try
    {
        await dbContext.Database.MigrateAsync();
        app.Logger.LogInformation("? Migrations aplicadas com sucesso!");
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "? Erro ao aplicar migrations");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Consolidado API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

// ? Autenticação e Autorização (ANTES do Rate Limiter)
app.UseAuthentication();
app.UseAuthorization();

app.UseRateLimiter();

app.UseCors("AllowAll");
app.MapControllers();

app.MapGet("/health", () => Results.Ok(new
{
    status = "healthy",
    timestamp = DateTime.UtcNow,
    service = "Consolidado API",
    rateLimit = "50 req/s"
})).AllowAnonymous().WithName("HealthCheck");

app.Logger.LogInformation("?? Consolidado API iniciada com JWT + Rate Limiting!");
app.Run();

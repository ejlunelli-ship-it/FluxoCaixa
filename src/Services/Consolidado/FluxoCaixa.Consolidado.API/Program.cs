using FluentValidation;
using FluxoCaixa.Consolidado.Application.Commands.AtualizarConsolidadoDiario;

var builder = WebApplication.CreateBuilder(args);

// Configurar MediatR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(AtualizarConsolidadoDiarioCommand).Assembly);
});

// Configurar FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<AtualizarConsolidadoDiarioValidator>();

// API
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "Fluxo de Caixa - API de Consolidado",
        Version = "v1",
        Description = "API para consulta de consolidado diário"
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
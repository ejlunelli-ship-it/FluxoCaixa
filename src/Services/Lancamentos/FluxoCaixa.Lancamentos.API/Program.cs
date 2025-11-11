using FluentValidation;
using FluxoCaixa.Lancamentos.Application.Commands.CriarLancamento;

var builder = WebApplication.CreateBuilder(args);

//MediatR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(CriarLancamentoCommand).Assembly);
});

//FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<CriarLancamentoValidator>();

// API
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "Fluxo de Caixa - API de Lançamentos",
        Version = "v1",
        Description = "API para controle de lançamentos financeiros"
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
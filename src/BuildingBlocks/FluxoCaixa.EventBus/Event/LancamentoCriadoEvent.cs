namespace FluxoCaixa.EventBus.Events;

public record LancamentoCriadoEvent
{
    public Guid LancamentoId { get; init; }
    public DateOnly Data { get; init; }
    public int Tipo { get; init; } // 1=Credito, 2=Debito
    public decimal Valor { get; init; }
    public DateTime CriadoEm { get; init; }
}
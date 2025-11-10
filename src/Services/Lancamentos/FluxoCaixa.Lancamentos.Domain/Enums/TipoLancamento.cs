namespace FluxoCaixa.Lancamentos.Domain.Enums;

/// <summary>
/// Tipo de lançamento financeiro
/// </summary>
public enum TipoLancamento
{
    /// <summary>
    /// Entrada de dinheiro (receita)
    /// </summary>
    Credito = 1,

    /// <summary>
    /// Saída de dinheiro (despesa)
    /// </summary>
    Debito = 2
}
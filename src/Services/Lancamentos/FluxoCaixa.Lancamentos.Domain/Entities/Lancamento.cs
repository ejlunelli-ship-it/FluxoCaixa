using FluxoCaixa.Core.Domain;
using FluxoCaixa.Lancamentos.Domain.Enums;

namespace FluxoCaixa.Lancamentos.Domain.Entities;

public class Lancamento : Entity, IAggregateRoot
{
    public DateTime DataLancamento { get; private set; }
    public TipoLancamento Tipo { get; private set; }
    public decimal Valor { get; private set; }
    public string Descricao { get; private set; }
    public string? Observacao { get; private set; }

    private Lancamento() { }

    public Lancamento(
        DateTime dataLancamento,
        TipoLancamento tipo,
        decimal valor,
        string descricao,
        string? observacao = null)
    {
        ValidarDados(dataLancamento, valor, descricao);

        DataLancamento = dataLancamento;
        Tipo = tipo;
        Valor = valor;
        Descricao = descricao;
        Observacao = observacao;
    }

    public void Atualizar(
        DateTime dataLancamento,
        TipoLancamento tipo,
        decimal valor,
        string descricao,
        string? observacao = null)
    {
        ValidarDados(dataLancamento, valor, descricao);

        DataLancamento = dataLancamento;
        Tipo = tipo;
        Valor = valor;
        Descricao = descricao;
        Observacao = observacao;
        AtualizadoEm = DateTime.UtcNow;
    }

    private void ValidarDados(DateTime dataLancamento, decimal valor, string descricao)
    {
        if (dataLancamento > DateTime.UtcNow.AddDays(1))
            throw new DomainException("Data de lançamento não pode ser futura");

        if (valor <= 0)
            throw new DomainException("Valor deve ser maior que zero");

        if (string.IsNullOrWhiteSpace(descricao))
            throw new DomainException("Descrição é obrigatória");

        if (descricao.Length < 3)
            throw new DomainException("Descrição deve ter no mínimo 3 caracteres");

        if (descricao.Length > 200)
            throw new DomainException("Descrição deve ter no máximo 200 caracteres");
    }

    public bool IsCredito() => Tipo == TipoLancamento.Credito;
    public bool IsDebito() => Tipo == TipoLancamento.Debito;
}
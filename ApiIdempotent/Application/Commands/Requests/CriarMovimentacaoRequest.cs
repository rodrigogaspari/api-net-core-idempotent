namespace ApiIdempotent.Application.Commands.Requests
{
    [Serializable]
    public class CriarMovimentacaoRequest
    {
        public string? IdRequisicao { get; set; }

        public string? TipoMovimento { get; set; }

        public decimal? Valor { get; set; }
    }
}

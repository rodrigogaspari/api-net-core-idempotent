namespace ApiIdempotent.Application.Queries.Responses
{
    public class ConsultaSaldoResponse
    {
        public int Numero { get; set; }

        public string? Nome { get; set; }

        public DateTime SaldoDataHora { get; set; }

        public decimal Saldo { get; set; }
    }
}

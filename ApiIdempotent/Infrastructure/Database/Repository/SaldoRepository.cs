using Dapper;
using ApiIdempotent.Application.Queries.Responses;

namespace ApiIdempotent.Infrastructure.Database.Repository
{
    public class SaldoRepository
    {
        private DbSession _session;

        public SaldoRepository(DbSession session)
        {
            _session = session;
        }

        public IEnumerable<ConsultaSaldoResponse> GetSaldo(string idContaCorrente)
        {
            return _session.Connection.Query<ConsultaSaldoResponse>(
                    @"
                    SELECT 
                     r.Numero
                    ,r.Nome
                    ,DATETIME('now') SaldoDataHora
                    ,Sum(r.Saldo) Saldo 
                    FROM 
                    (
                         SELECT 
                         c.Numero 
                        ,c.Nome
                        ,CASE WHEN (m.tipomovimento='C') 
	                          THEN m.valor
                              ELSE m.valor * -1 END 
                        as Saldo 
                    
                        FROM movimento m INNER JOIN  
                        contacorrente c on m.idcontacorrente = c.idcontacorrente 
                    
                        WHERE 
                        m.idcontacorrente=@idContaCorrente
                    )as r", new { idContaCorrente }, _session.Transaction);
        }
    }
}

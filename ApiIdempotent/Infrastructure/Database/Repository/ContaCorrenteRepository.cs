using Dapper;

namespace ApiIdempotent.Infrastructure.Database.Repository
{
    public class ContaCorrenteRepository
    {
        private DbSession _session;

        public ContaCorrenteRepository(DbSession session)
        {
            _session = session;
        }

        public bool IsValidAccount(string? idContaCorrente)
        {
            return _session.Connection.Query<bool>(
                @"SELECT 
                count(0) as BIT 
                   
                FROM contacorrente c 

                WHERE c.idcontacorrente=@idContaCorrente", new { idContaCorrente }, _session.Transaction).FirstOrDefault();
        }

        public bool IsActiveAccount(string? idContaCorrente)
        {
            return _session.Connection.Query<bool>(
                @"SELECT 
                ativo as BIT 
                   
                FROM contacorrente c 

                WHERE c.idcontacorrente=@idContaCorrente", new { idContaCorrente }, _session.Transaction).FirstOrDefault();
        }

    }
}

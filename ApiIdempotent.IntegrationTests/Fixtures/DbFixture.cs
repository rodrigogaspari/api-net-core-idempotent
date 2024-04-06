using ApiIdempotent.Infrastructure.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ApiIdempotent.IntegrationTests.Fixtures
{
    public class DbFixture : IDisposable
    {
        IDatabaseBootstrap databaseBootstrap;
        private bool disposedValue;
        public readonly string DatabaseName = $"ApiIdempotentIntegrationTests-{Guid.NewGuid()}-database.sqlite";
        public DbFixture() 
        {
            var config = new DatabaseConfig { Name = $"DataSource = {DatabaseName}", DatabaseName = DatabaseName };
            databaseBootstrap = new DatabaseBootstrap(config);
            databaseBootstrap.Setup();
        }

        #region Disposable

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.databaseBootstrap.EnsureDeleted();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}

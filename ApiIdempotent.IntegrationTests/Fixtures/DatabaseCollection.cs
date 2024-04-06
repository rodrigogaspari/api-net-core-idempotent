using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiIdempotent.IntegrationTests.Fixtures
{
    [CollectionDefinition("Database")]
    public class DatabaseCollection : ICollectionFixture<DbFixture> 
    {

    }
}

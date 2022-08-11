using Microsoft.EntityFrameworkCore;
using Respawn;
using Respawn.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldBank.Entities;

namespace WorldBank.API.Test.Fixture
{
    public class TestDatabaseFixture:IDisposable
    {
        private const string ConnectionString = @"Data Source=LAPTOP-UL22A35E\HTETLIN;Initial Catalog=world_bank_test;User ID=sa;Password=codigo180;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;Pooling=True;";

        private static readonly object _lock = new();
        private static bool _databaseInitialized;

        private static Checkpoint checkpoint = new Checkpoint
        {
            TablesToIgnore = new Table[]
        {
                "audit_types",
                "bank_account_types",
                "currency",
                "transaction_charges",
                "transaction_types",
        },

        };

        public TestDatabaseFixture()
        {
            lock (_lock)
            {
                if (!_databaseInitialized)
                {
                    using (var context = CreateContext())
                    {
                        
                        checkpoint.Reset(ConnectionString);
                        //context.Database.EnsureDeleted();
                        //context.Database.EnsureCreated();

                       
                        context.SaveChanges();
                    }

                    _databaseInitialized = true;
                }
            }
        }

        public WorldBankDBContext CreateContext()
            => new WorldBankDBContext(
                new DbContextOptionsBuilder<WorldBankDBContext>()
                    .UseSqlServer(ConnectionString)
                    .Options);

       
        public void Cleanup()
        {
            using var context = CreateContext();

            //context.SaveChanges();
            checkpoint.Reset(ConnectionString);

        }

        public void Dispose()
        {
            Cleanup();
        }
    }
   
}

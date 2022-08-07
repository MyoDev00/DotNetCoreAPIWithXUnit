using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using WorldBank.Entities;
using WorldBank.Entities.DataModel;

namespace WorldBank.API.Test.Mock
{
    public class TestDBContext : WorldBankDBContext
    {
        public TestDBContext()
        {

        }

        public virtual DbSet<BankAccount> BankAccount { get; set; }
        public virtual DbSet<BankAccountLedger> BankAccountLedger { get; set; }
        public virtual DbSet<BankAccountTypes> BankAccountTypes { get; set; }
        public virtual DbSet<Currency> Currency { get; set; }
        public virtual DbSet<Customer> Customer { get; set; }
        public int SaveChangesCount { get; private set; }
        public int SaveChanges()
        {
            SaveChangesCount++;
            return 1;
        }
    }
}

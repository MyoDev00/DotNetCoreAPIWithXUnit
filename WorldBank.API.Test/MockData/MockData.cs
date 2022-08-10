using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldBank.Entities.DataModel;
using WorldBank.Shared.Constant;
using static WorldBank.Shared.Constant.Constant;

namespace WorldBank.API.Test
{
    public class MockData
    {
        public List<Customer> Customers { get; set; }
        public List<BankAccountTypes> BankAccountTypes { get; set; }
        public List<Currency> Currencies { get; set; }
        public List<BankAccount> BankAccounts { get; set; }
        public List<TransactionTypes> TransactionTypes { get; set; }
        public List<TransactionCharges> TransactionCharges { get; set; }
        public List<AuditTypes> AuditTypes { get; set; }
        public List<GeneratedTransactionNumber> GeneratedTransactionNumbers { get; set; }
        public List<Transaction> Transactions { get; set; }
        public List<BankAccountLedger> BankAccountLedgers { get; set; }
        public List<StaffAuditLog> StaffAuditLogs { get; set; }
        public List<Staff> Staff { get; set; }
        public MockData()
        {
            BankAccountTypes = new List<BankAccountTypes>{
                new BankAccountTypes
                {
                    BankAccountTypeId = Guid.NewGuid(),
                    BankAccountType="Saving",
                    Description="Saving Account"
                },
                new BankAccountTypes
                {
                    BankAccountTypeId = Guid.NewGuid(),
                    BankAccountType="FixDeposit",
                    Description="Fixed deposit account"
                }
            };
            Currencies = new List<Currency>
            {
                new Currency
                {
                    CurrencyId = Guid.Parse("BA111111-1111-1111-1111-111111111111"),
                    CurrencyCode="THB",
                    CurrencySymbol = "฿",
                    Description = "Thai Baht"
                },
                new Currency
                {
                    CurrencyId = Guid.Parse("2D111111-1111-1111-1111-111111111111"),
                    CurrencyCode="USD",
                    CurrencySymbol = "$",
                    Description = "United State Dollar"
                },
            };

            BankAccounts = new List<BankAccount>()
            {
                new BankAccount()
                {
                    BankAccountId = Guid.Parse("BA111111-1111-1111-1111-111111111111"),
                    BankAccountTypeId =BankAccountTypes[0].BankAccountTypeId,
                    CurrencyId= Currencies[0].CurrencyId,
                    IbanNumber = "NL24ABNA3526558027",
                    CustomerId = Guid.Parse("CA111111-1111-1111-1111-111111111111"),
                    Status = (int)CommonStatus.Active,
                    ClosingBalance=1000,
                    TotalCredit =0,
                    TotalDebit =0,
                },
                new BankAccount()
                {
                    BankAccountId = Guid.Parse("BA222222-1111-1111-1111-111111111111"),
                    BankAccountTypeId =BankAccountTypes[0].BankAccountTypeId,
                    CurrencyId= Currencies[0].CurrencyId,
                    IbanNumber = "NL25ABNA3526558028",
                    CustomerId = Guid.Parse("CA222222-1111-1111-1111-111111111111"),
                    Status = (int)CommonStatus.Active,
                    ClosingBalance=1000,
                    TotalCredit =0,
                    TotalDebit =0,
                },
                new BankAccount()
                {
                    BankAccountId =Guid.Parse("BA333333-1111-1111-1111-111111111111"),
                    BankAccountTypeId =BankAccountTypes[0].BankAccountTypeId,
                    CurrencyId= Currencies[0].CurrencyId,
                    IbanNumber = "NL25ABNA3526558029",
                    CustomerId = Guid.Parse("CA333333-1111-1111-1111-111111111111"),
                    Status = (int)CommonStatus.Inactive,
                    ClosingBalance=1000,
                    TotalCredit =1000,
                    TotalDebit =1000,
                },
                new BankAccount()
                {
                    BankAccountId =Guid.Parse("BA444444-1111-1111-1111-111111111111"),
                    BankAccountTypeId =BankAccountTypes[0].BankAccountTypeId,
                    CurrencyId= Currencies[1].CurrencyId,
                    IbanNumber = "NL25ABNA3526558030",
                    CustomerId = Guid.Parse("CA444444-1111-1111-1111-111111111111"),
                    Status = (int)CommonStatus.Inactive,
                    ClosingBalance=1000,
                    TotalCredit =1000,
                    TotalDebit =1000,
                },
                new BankAccount()
                {
                    BankAccountId =Guid.Parse("BA555555-1111-1111-1111-111111111111"),
                    BankAccountTypeId =BankAccountTypes[0].BankAccountTypeId,
                    CurrencyId= Currencies[1].CurrencyId,
                    IbanNumber = "NL25ABNA3526558030",
                    CustomerId = Guid.Parse("CA555555-1111-1111-1111-111111111111"),
                    Status = (int)CommonStatus.Inactive,
                    ClosingBalance=1000,
                    TotalCredit =1000,
                    TotalDebit =1000,
                },
                 new BankAccount()
                {
                    BankAccountId = Guid.Parse("BA666666-1111-1111-1111-111111111111"),
                    BankAccountTypeId =BankAccountTypes[0].BankAccountTypeId,
                    CurrencyId= Currencies[0].CurrencyId,
                    IbanNumber = "NL22ABNA2222222222",
                    CustomerId = Guid.Parse("CA111111-1111-1111-1111-111111111111"),
                    Status = (int)CommonStatus.Active,
                    ClosingBalance=1000,
                    TotalCredit =0,
                    TotalDebit =0,
                },
            };
            Customers = new List<Customer>
            {
                new Customer{
                    CustomerId=Guid.Parse("CA111111-1111-1111-1111-111111111111")
                    ,FullName="C 1"
                    ,Email="c1@gmail.com"
                    ,MobileCode="+95"
                    ,Mobile="0111"
                    ,IdentityCardNo="IC0001"
                    ,Status=(int)CommonStatus.Active
                    ,CreatedOn=DateTime.Now
                    ,UpdatedOn=DateTime.Now
                    ,Password="ZB8Qr/BBCabIbKNo6uPBZ9VVi1M="
                    ,SaltPassword="shHWTKuHhHEeWnUydFg3Gnlr30AfyTQ8"
                    ,BankAccount = new List<BankAccount>()
                    {
                        BankAccounts[0]
                    }
                },

                new Customer{
                    CustomerId=Guid.Parse("CA222222-1111-1111-1111-111111111111")
                    ,FullName="C 2"
                    ,Email="c2@gmail.com"
                    ,MobileCode="+95"
                    ,Mobile="0222"
                    ,IdentityCardNo="IC0002"
                    ,Status=(int)CommonStatus.Active
                    ,CreatedOn=DateTime.Now.AddDays(1)
                    ,UpdatedOn=DateTime.Now.AddDays(1)
                    ,Password="ZB8Qr/BBCabIbKNo6uPBZ9VVi1M="
                    ,SaltPassword="shHWTKuHhHEeWnUydFg3Gnlr30AfyTQ8"
                    ,BankAccount = new List<BankAccount>()
                    {
                        BankAccounts[1]
                    }
                },

                new Customer{
                    CustomerId=Guid.Parse("CA333333-1111-1111-1111-111111111111")
                    ,FullName="C 3"
                    ,Email="c3@gmail.com"
                    ,MobileCode="+95"
                    ,Mobile="0333"
                    ,IdentityCardNo="IC0003"
                    ,Status=(int)CommonStatus.Active
                    ,CreatedOn=DateTime.Now.AddDays(2)
                    ,UpdatedOn=DateTime.Now.AddDays(2)
                    ,Password="ZB8Qr/BBCabIbKNo6uPBZ9VVi1M="
                    ,SaltPassword="shHWTKuHhHEeWnUydFg3Gnlr30AfyTQ8"
                    ,BankAccount = new List<BankAccount>()
                    {
                        BankAccounts[2]
                    }
                },

                new Customer{
                    CustomerId=Guid.Parse("CA444444-1111-1111-1111-111111111111")
                    ,FullName="C 4"
                    ,Email="c4@gmail.com"
                    ,MobileCode="+95"
                    ,Mobile="0444"
                    ,IdentityCardNo="IC0004"
                    ,Status=(int)CommonStatus.Inactive
                    ,CreatedOn=DateTime.Now.AddDays(3)
                    ,UpdatedOn=DateTime.Now.AddDays(3)
                    ,Password="ZB8Qr/BBCabIbKNo6uPBZ9VVi1M="
                    ,SaltPassword="shHWTKuHhHEeWnUydFg3Gnlr30AfyTQ8"
                    ,BankAccount = new List<BankAccount>()
                    {
                        BankAccounts[3]
                    }
                },
                 new Customer{
                    CustomerId=Guid.Parse("CA555555-1111-1111-1111-111111111111")
                    ,FullName="C 5"
                    ,Email="c5@gmail.com"
                    ,MobileCode="+95"
                    ,Mobile="0555"
                    ,IdentityCardNo="IC0005"
                    ,Status=(int)CommonStatus.Inactive
                    ,CreatedOn=DateTime.Now.AddDays(4)
                    ,UpdatedOn=DateTime.Now.AddDays(4)
                    ,Password="ZB8Qr/BBCabIbKNo6uPBZ9VVi1M="
                    ,SaltPassword="shHWTKuHhHEeWnUydFg3Gnlr30AfyTQ8"
                    ,BankAccount = new List<BankAccount>()
                    {
                        BankAccounts[4]
                    }
                },
            };

            AuditTypes = new List<AuditTypes>
            {
                new AuditTypes()
                {
                    AuditTypeId= Guid.NewGuid(),
                    AuditType="BankDeposit",
                    Description="Customer deposit money at bank"
                },
                new AuditTypes()
                {
                    AuditTypeId= Guid.NewGuid(),
                    AuditType="FundTransfer",
                    Description="Transfer money from one account to another,or own account"
                }
            };

            TransactionTypes = new List<TransactionTypes>()
            {
                new TransactionTypes()
                {
                    TransactionTypeId=Guid.Parse("DA111111-1111-1111-1111-111111111111"),
                    TransactionType = "BankDeposit",
                    Description="Money Deposit"
                },
                new TransactionTypes()
                {
                    TransactionTypeId=Guid.Parse("FA222222-1111-1111-1111-111111111111"),
                    TransactionType = "FundTransfer",
                    Description="Money Transfer"
                }
            };

            TransactionCharges = new List<TransactionCharges>
            {
                new TransactionCharges()
                {
                    TransactionChargesId=Guid.NewGuid(),
                    ChargesType=Constant.TransactionType.BankDeposit,
                    Description = "Deposit",
                    Percentage = 0.1m
                },
                  new TransactionCharges()
                {
                    TransactionChargesId=Guid.NewGuid(),
                    ChargesType=Constant.TransactionType.FundTransfer,
                    Description = "Money Transfer",
                    Percentage = 0
                }
            };
            
            GeneratedTransactionNumbers = new List<GeneratedTransactionNumber>()
            {
                new GeneratedTransactionNumber()
                {
                    TransactionNo=0,
                    GeneratedNo="0"
                }
            };
            Transactions = new List<Transaction>();
            BankAccountLedgers = new List<BankAccountLedger>();
            StaffAuditLogs = new List<StaffAuditLog>();
            Staff = new List<Staff>()
            {
                new Staff
                {
                    StaffId = Guid.NewGuid(),
                    LoginId = "Admin",
                    FullName ="Admin",
                    Password="ZB8Qr/BBCabIbKNo6uPBZ9VVi1M=",
                    SaltPassword="shHWTKuHhHEeWnUydFg3Gnlr30AfyTQ8",
                    Status = (int)CommonStatus.Active,
                    CreatedOn = DateTime.Now,
                    UpdatedOn = DateTime.Now,
                }
            };
        }


    }
}

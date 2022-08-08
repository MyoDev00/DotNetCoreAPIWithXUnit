using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldBank.Entities.DataModel;
using static WorldBank.Shared.Constant.Constant;

namespace WorldBank.API.Test
{
    public class MockData
    {
        public List<Customer> Customers { get; set; }
        public List<BankAccountTypes> BankAccountTypes { get; set; }
        public List<Currency> Currencies { get; set; }
        public List<BankAccount> BankAccounts { get; set; }

        public MockData()
        {
            Customers = new List<Customer>
            {
                new Customer{ CustomerId=Guid.Parse("1A111111-1111-1111-1111-111111111111")
                    ,FullName="C 1"
                    ,Email="c1@gmail.com"
                    ,MobileCode="+95"
                    ,Mobile="0111"
                    ,IdentityCardNo="IC0001"
                    ,Status=(int)CommonStatus.Active
                    ,CreatedOn=DateTime.Now
                    ,UpdatedOn=DateTime.Now
                },

                new Customer{ CustomerId=Guid.Parse("2A222222-2222-2222-2222-222222222222")
                    ,FullName="C 2"
                    ,Email="c2@gmail.com"
                    ,MobileCode="+95"
                    ,Mobile="0222"
                    ,IdentityCardNo="IC0002"
                    ,Status=(int)CommonStatus.Active
                    ,CreatedOn=DateTime.Now.AddDays(1)
                    ,UpdatedOn=DateTime.Now.AddDays(1)},

                new Customer{ CustomerId=Guid.NewGuid()
                    ,FullName="C 3"
                    ,Email="c3@gmail.com"
                    ,MobileCode="+95"
                    ,Mobile="0333"
                    ,IdentityCardNo="IC0003"
                    ,Status=(int)CommonStatus.Inactive
                    ,CreatedOn=DateTime.Now.AddDays(2)
                    ,UpdatedOn=DateTime.Now.AddDays(2)},

                new Customer{ CustomerId=Guid.NewGuid()
                    ,FullName="C 4"
                    ,Email="c4@gmail.com"
                    ,MobileCode="+95"
                    ,Mobile="0444"
                    ,IdentityCardNo="IC0004"
                    ,Status=(int)CommonStatus.Deleted
                    ,CreatedOn=DateTime.Now.AddDays(3)
                    ,UpdatedOn=DateTime.Now.AddDays(3)},
            };

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
                    CurrencySymbol = "USD",
                    Description = "United State Dollar"
                },
            };

            BankAccounts = new List<BankAccount>()
        {
             new BankAccount()
                {
                    BankAccountId = Guid.NewGuid(),
                    BankAccountTypeId =BankAccountTypes[0].BankAccountTypeId,
                    CurrencyId= Currencies[0].CurrencyId,
                    IbanNumber = "NL24ABNA3526558027",
                    CustomerId = Customers[0].CustomerId,
                    Status = (int)CommonStatus.Active,
                    ClosingBalance=0,
                    TotalCredit =0,
                    TotalDebit =0,
                }
        };
        }


    }
}

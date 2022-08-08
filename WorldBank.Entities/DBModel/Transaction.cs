
#nullable disable
using System;
using System.Collections.Generic;

namespace WorldBank.Entities.DataModel
{
    public partial class Transaction
    {
        public Guid TransactionId { get; set; }
        public string TransactionNo { get; set; }
        public Guid TransactionTypeId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid BankAccountId { get; set; }
        public Guid ReceiverCustomerId { get; set; }
        public Guid ReceiverBankAccountId { get; set; }
        public decimal Charges { get; set; }
        public decimal ChargesPercentage { get; set; }
        public decimal Amount { get; set; }
        public decimal NetAmount { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedOn { get; set; }

        public virtual BankAccount BankAccount { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual TransactionTypes TransactionType { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            else
            {
                var data = (Transaction)obj;

                var isEqual = (TransactionTypeId == data.TransactionTypeId) && (CustomerId == data.CustomerId)
                            && (BankAccountId == data.BankAccountId) && (ReceiverCustomerId == data.ReceiverCustomerId)
                            && (ReceiverBankAccountId == data.ReceiverBankAccountId)
                            && (Charges == data.Charges) && (ChargesPercentage == data.ChargesPercentage)
                            && (NetAmount == data.NetAmount) && (Amount == data.Amount)
                            && (Notes == data.Notes) ;
                return isEqual;
            }
        }
    }
}
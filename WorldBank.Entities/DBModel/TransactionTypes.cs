
#nullable disable
using System;
using System.Collections.Generic;

namespace WorldBank.Entities.DataModel
{
    public partial class TransactionTypes
    {
        public TransactionTypes()
        {
        }

        public Guid TransactionTypeId { get; set; }
        public string TransactionType { get; set; }
        public string Description { get; set; }

    }
}
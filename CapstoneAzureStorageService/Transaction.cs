using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapstoneAzureStorageService
{
    public class Transaction:TableEntity
    {
        public double Amount { get; set; }

        public string DriverId { get; set; }

        public DateTime TransactionDate { get; set; }

        public string Type { get; set; }

        public Transaction()
        {

        }
        public Transaction(string customerId, string transactionId)
        {
            this.PartitionKey = customerId;
            this.RowKey = transactionId;
        }
    }
}

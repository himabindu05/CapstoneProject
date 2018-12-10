using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapstoneAzureStorageService
{
    public class Customer:TableEntity
    {
        public string Address { get; set; }

        public double Balance { get; set; }

        public string CustomerName { get; set; }

        public int Offer { get; set; }

        public Customer()
        {
            
        }
        public Customer(string customerId)
        {
            this.PartitionKey = "Customer";
            this.RowKey = customerId;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace CapstoneAzureStorageService
{
    public class BookMyCabRepository
    {
        public CloudTable customerTable { get; set; }

        public CloudStorageAccount storageAccount { get; set; }

        public CloudTableClient tableClient { get; set; }

        public CloudTable transactionTable { get; set; }

        public BookMyCabRepository()
        {
            storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageAccountConnectionString"]);
            tableClient = storageAccount.CreateCloudTableClient();
            customerTable = tableClient.GetTableReference("Customer");
            transactionTable = tableClient.GetTableReference("Transaction");
        }

        public int CountTransactions(string customerId)
        {
            int noOfTransactions = 0;
            try
            {
                TableQuery<Customer> query = new TableQuery<Customer>().Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, customerId));
                List<Customer> customerList = customerTable.ExecuteQuery(query)?.ToList();
                if (customerList != null)
                {
                    noOfTransactions = customerList.Count();
                }
            }
            catch(Exception)
            {
                noOfTransactions = -99;
            }
            return noOfTransactions;
        }

        public bool CreateCustomerTable()
        {
            try
            {
                customerTable.CreateIfNotExists();
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

        public bool CreateTransactionTable()
        {
            try
            {
                transactionTable.CreateIfNotExists();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public double FetchBalance(string customerId)
        {
            double balance = 0.0;
            try
            {                
                //TableQuery<DynamicTableEntity> query = new TableQuery<DynamicTableEntity>().Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, customerId)).Select(new string[] { "Balance" });
                //EntityResolver<string> resolver = (pk, rk, ts, props,etag) => props.ContainsKey("Balance")?props["Balance"].StringValue:null;
                //customerTable.ExecuteQuery(query, resolver);

                TableQuery<Customer> query = new TableQuery<Customer>().Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, customerId));                
                Customer customerData = customerTable.ExecuteQuery(query)?.FirstOrDefault();
                if(customerData != null)
                {
                    balance = customerData.Balance;
                }
            }
            catch
            {
                balance = -99;
            }
            return balance;
        }

        public string GenerateNewTransactionId()
        {
            string newTransId = "";
            try
            {
                TableQuery<Transaction> query = new TableQuery<Transaction>();
                Transaction transactionData = transactionTable.ExecuteQuery(query).OrderBy(x => x.TransactionDate).LastOrDefault();
                if (transactionData != null)
                {
                    newTransId = transactionData.RowKey;
                }
                else
                {
                    newTransId = "1";
                }
            }
            catch
            {
                newTransId = "NA";
            }
            return newTransId;
        }

        public List<Customer> GetAllCustomers()
        {
            List<Customer> customerList;
            try
            {
                TableQuery<Customer> query = new TableQuery<Customer>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Customer"));
                customerList = customerTable.ExecuteQuery(query)?.ToList();
            }
            catch
            {
                customerList = null;
            }
            return customerList;
        }

        public Customer GetCustomer(string customerId)
        {
            Customer customerData;
            try
            {
                TableQuery<Customer> query = new TableQuery<Customer>().Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, customerId));
                customerData = customerTable.ExecuteQuery(query).FirstOrDefault();
            }
            catch
            {
                customerData = null;
            }
            return customerData;
        }

        public bool InsertEntityIntoCustomerTable(string customerId, string customerName, double balance, string address, int offer)
        {
            try
            {
                Customer customerData = new Customer(customerId);
                customerData.CustomerName = customerName;
                customerData.Balance = balance;
                customerData.Address = address;
                customerData.Offer = offer;
                TableOperation insertOperation = TableOperation.Insert(customerData);
                customerTable.Execute(insertOperation);
                return true;
            }
            catch
            {
                return false;
            }            
        }

        public void UpdateOfferForCustomer(string customerId, int offerPercentage)
        {
            try
            {
                Customer customerData = GetCustomer(customerId);
                customerData.Offer = offerPercentage;
                TableOperation updateOperation = TableOperation.InsertOrReplace(customerData);
                customerTable.Execute(updateOperation);
            }
            catch
            {

            }
        }

        public bool UpdateWallet(string customerId, double amount, string type, string driverId = "0")
        {
            bool isUpdateSuccess = false;
            try
            {
                Customer customerData = GetCustomer(customerId);
                if (customerData != null)
                {
                    if (type.Equals("CREDIT"))
                    {
                        if (amount >= 100 && amount <= 5000)
                        {
                            customerData.Balance = customerData.Balance + amount;
                            TableOperation updateOperation = TableOperation.InsertOrReplace(customerData);
                            customerTable.Execute(updateOperation);
                            string transactionId = GenerateNewTransactionId();
                            if (transactionId != "NA")
                            {
                                Transaction transactionData = new Transaction(customerId, transactionId);
                                transactionData.Amount = amount;
                                transactionData.Type = type;
                                transactionData.TransactionDate = DateTime.Now;
                                transactionData.DriverId = driverId;
                                TableOperation insertOperation = TableOperation.Insert(transactionData);
                                transactionTable.Execute(insertOperation);
                                isUpdateSuccess = true;
                            }
                        }
                    }
                    if (type.Equals("DEBIT"))
                    {
                        if(amount <= customerData.Balance)
                        {
                            double offerAmount = customerData.Offer * amount / 100;
                            customerData.Balance = customerData.Balance - (amount - offerAmount);
                            TableOperation updateOperation = TableOperation.InsertOrReplace(customerData);
                            customerTable.Execute(updateOperation);
                            string transactionId = GenerateNewTransactionId();
                            if (transactionId != "NA")
                            {
                                Transaction transactionData = new Transaction(customerId, transactionId);
                                transactionData.Amount = amount;
                                transactionData.Type = type;
                                transactionData.TransactionDate = DateTime.Now;
                                transactionData.DriverId = driverId;
                                TableOperation insertOperation = TableOperation.Insert(transactionData);
                                transactionTable.Execute(insertOperation);
                                isUpdateSuccess = true;
                            }
                        }
                    }
                }
            }
            catch
            {
                isUpdateSuccess = false;
            }
            return isUpdateSuccess;
        }
    }
}

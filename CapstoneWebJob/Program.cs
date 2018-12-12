using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapstoneAzureStorageService;
using Microsoft.Azure.WebJobs;

namespace CapstoneWebJob
{
    // To learn more about Microsoft Azure WebJobs SDK, please see https://go.microsoft.com/fwlink/?LinkID=320976
    class Program
    {
        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        static void Main()
        {
            var config = new JobHostConfiguration();

            if (config.IsDevelopment)
            {
                config.UseDevelopmentSettings();
            }

            var host = new JobHost(config);
            // The following code ensures that the WebJob will be running continuously
            host.RunAndBlock();

            BookMyCabRepository repoObj = new BookMyCabRepository();
            List<Customer> custList = repoObj.GetAllCustomers();
            if (custList != null && custList.Count()>0)
            {
                foreach (Customer custObj in custList)
                {
                    int transCount = repoObj.CountTransactions(custObj.RowKey);
                    int offer = 0;
                    if (transCount >= 2 && transCount <=4)
                    {
                        offer = 5;
                    }
                    if(transCount>4)
                    {
                        offer = 10;
                    }
                    if(offer !=0)
                    {
                        repoObj.UpdateOfferForCustomer(custObj.RowKey, offer);
                    }
                }
            }
        }
    }
}

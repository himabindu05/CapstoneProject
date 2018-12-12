using CabDriversDAL;
using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CapstoneServiceBus
{
    class Program
    {
        static IQueueClient queueClient;
        static void Main(string[] args)
        {
            UploadDriverDetails().GetAwaiter().GetResult();
        }
        public static async Task UploadDriverDetails()
        {
            CabDriversRepository dalObj = new CabDriversRepository();
            List<DriverDetail> lst = dalObj.GetDrivers();

            string svcBusConnectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"].ToString();
            string queueName = "himacapstonedrivers";
            queueClient = new QueueClient(svcBusConnectionString, queueName);
            var message = new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(lst)));
            await queueClient.SendAsync(message);

            Console.WriteLine("Driver details uploaded successfully");
        }
    }
}

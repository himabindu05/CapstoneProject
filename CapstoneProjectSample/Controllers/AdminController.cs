using CabDriversDAL;
using CapstoneAzureStorageService;
//using Microsoft.Azure.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CapstoneProjectSample.Controllers
{
    public class AdminController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult ViewAllCustomers()
        {
            BookMyCabRepository repoObj = new BookMyCabRepository();
            List<Customer> custList = repoObj.GetAllCustomers();
            if (custList==null || custList.Count == 0)
            {
                custList = new List<Customer>();
                Customer cust = new Customer();
                cust.RowKey = "testc";
                cust.CustomerName = "testname";
                cust.Balance = 2020;
                cust.Address = "india";
                cust.Offer = 5;
                custList.Add(cust);
            }
            return View("ViewAllCustomers",custList);
        }

        public void ViewAllDrivers()
        {            
            string svcBusConnectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
            string queueName = "himacapstonedrivers";
            QueueClient queueClient = QueueClient.CreateFromConnectionString(svcBusConnectionString, queueName);
            BrokeredMessage message = queueClient.Receive();
            if (message!=null)
            {
                List<DriverDetail> drivers = JsonConvert.DeserializeObject<List<DriverDetail>>( message.GetBody<string>());
            }
        }
        
        [HttpPost]
        [Route("[controller]/BookCab")]
        public ActionResult BookCab(HttpRequest request)
        {
            List<DriverDetail> drivers = new List<DriverDetail>();
            string svcBusConnectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
            string queueName = "himacapstonedrivers";
            QueueClient queueClient = QueueClient.CreateFromConnectionString(svcBusConnectionString, queueName);
            BrokeredMessage message = queueClient.Receive();
            if (message != null)
            {
                drivers = JsonConvert.DeserializeObject<List<DriverDetail>>(message.GetBody<string>());
                if (drivers != null && drivers.Count != 0)
                {
                    
                }
                else
                {
                    drivers = new List<DriverDetail>();
                    DriverDetail driver = new DriverDetail();
                    driver.DriverId = "101";
                    driver.Rating = 2;
                    driver.Status = "tste";
                    drivers.Add(driver);
                }
            }
            return View("BookCab",drivers);
        }
    }
}
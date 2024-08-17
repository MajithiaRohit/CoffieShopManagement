using CoffieShop.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;

namespace CoffieShop.Controllers
{
    public class BillsController : Controller
    {
        //List<BillsModel> bills = new List<BillsModel>
        //{
        //    new BillsModel { BillID = 1, BillNumber = "B1001", BillDate = DateTime.Now, OrderID = 1, TotalAmount = 399.98m, Discount = 20.00m, NetAmount = 379.98m, UserID = 101 },
        //    new BillsModel { BillID = 2, BillNumber = "B1002", BillDate = DateTime.Now.AddDays(-1), OrderID = 2, TotalAmount = 29.99m, Discount = null, NetAmount = 29.99m, UserID = 101 },
        //    new BillsModel { BillID = 3, BillNumber = "B1003", BillDate = DateTime.Now.AddDays(-2), OrderID = 3, TotalAmount = 119.97m, Discount = 10.00m, NetAmount = 109.97m, UserID = 101 },
        //    new BillsModel { BillID = 4, BillNumber = "B1004", BillDate = DateTime.Now.AddDays(-3), OrderID = 4, TotalAmount = 199.96m, Discount = 15.00m, NetAmount = 184.96m, UserID = 101 },
        //    new BillsModel { BillID = 5, BillNumber = "B1005", BillDate = DateTime.Now.AddDays(-4), OrderID = 5, TotalAmount = 119.98m, Discount = null, NetAmount = 119.98m, UserID = 101 },
        //    new BillsModel { BillID = 6, BillNumber = "B1006", BillDate = DateTime.Now.AddDays(-5), OrderID = 6, TotalAmount = 69.99m, Discount = 5.00m, NetAmount = 64.99m, UserID = 101 },
        //    new BillsModel { BillID = 7, BillNumber = "B1007", BillDate = DateTime.Now.AddDays(-6), OrderID = 7, TotalAmount = 239.97m, Discount = 30.00m, NetAmount = 209.97m, UserID = 101 },
        //    new BillsModel { BillID = 8, BillNumber = "B1008", BillDate = DateTime.Now.AddDays(-7), OrderID = 8, TotalAmount = 179.98m, Discount = null, NetAmount = 179.98m, UserID = 101 },
        //    new BillsModel { BillID = 9, BillNumber = "B1009", BillDate = DateTime.Now.AddDays(-8), OrderID = 9, TotalAmount = 99.99m, Discount = 8.00m, NetAmount = 91.99m, UserID = 101 },
        //    new BillsModel { BillID = 10, BillNumber = "B1010", BillDate = DateTime.Now.AddDays(-9), OrderID = 10, TotalAmount = 439.96m, Discount = 50.00m, NetAmount = 389.96m, UserID = 101 }
        //};

        private IConfiguration configuration;
        public BillsController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }
        /*Methods*/
        public IActionResult BillsList()
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "SP_Bills_SelectAll";
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            return View(table);
        }
        public IActionResult AddEditBillsForm()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddEditBills(BillsModel bm)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("BillsList");
            }
            else
            {
                return View("AddEditBillsForm");
            }
        }
    }
}

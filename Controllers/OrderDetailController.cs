using CoffieShop.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;

namespace CoffieShop.Controllers
{
    public class OrderDetailController : Controller
    {
        //List<OrderDetailModel> orderDetails = new List<OrderDetailModel>
        //{
        //    new OrderDetailModel { OrderDetailID = 1, OrderID = 1, ProductID = 1, Quantity = 2, Amount = 19.99m, TotalAmount = 39.98m, UserID = 101 },
        //    new OrderDetailModel { OrderDetailID = 2, OrderID = 2, ProductID = 2, Quantity = 1, Amount = 29.99m, TotalAmount = 29.99m, UserID = 101 },
        //    new OrderDetailModel { OrderDetailID = 3, OrderID = 3, ProductID = 3, Quantity = 3, Amount = 39.99m, TotalAmount = 119.97m, UserID = 101 },
        //    new OrderDetailModel { OrderDetailID = 4, OrderID = 4, ProductID = 4, Quantity = 4, Amount = 49.99m, TotalAmount = 199.96m, UserID = 101 },
        //    new OrderDetailModel { OrderDetailID = 5, OrderID = 5, ProductID = 5, Quantity = 2, Amount = 59.99m, TotalAmount = 119.98m, UserID = 101 },
        //    new OrderDetailModel { OrderDetailID = 6, OrderID = 6, ProductID = 6, Quantity = 1, Amount = 69.99m, TotalAmount = 69.99m, UserID = 101 },
        //    new OrderDetailModel { OrderDetailID = 7, OrderID = 7, ProductID = 7, Quantity = 3, Amount = 79.99m, TotalAmount = 239.97m, UserID = 101 },
        //    new OrderDetailModel { OrderDetailID = 8, OrderID = 8, ProductID = 8, Quantity = 2, Amount = 89.99m, TotalAmount = 179.98m, UserID = 101 },
        //    new OrderDetailModel { OrderDetailID = 9, OrderID = 9, ProductID = 9, Quantity = 1, Amount = 99.99m, TotalAmount = 99.99m, UserID = 101 },
        //    new OrderDetailModel { OrderDetailID = 10, OrderID = 10, ProductID = 10, Quantity = 4, Amount = 109.99m, TotalAmount = 439.96m, UserID = 101 }
        //};

        private IConfiguration configuration;
        public OrderDetailController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }
        /*Methods*/
        public IActionResult OrderDetailsList()
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "SP_OrderDetails_SelectAll";
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            return View(table);
        }
        public IActionResult AddEditOrderDetailForm() {
          return View();
        }

        [HttpPost]
        public IActionResult AddEditOrderDetail(OrderDetailModel odm) {
            if (ModelState.IsValid)
            {
                return RedirectToAction("OrderDetailsList");
            }
            else
            {
                return View("AddEditOrderDetailForm");
            }
        }

    }
}

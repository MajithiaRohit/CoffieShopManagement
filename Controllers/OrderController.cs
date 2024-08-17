using CoffieShop.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace CoffieShop.Controllers
{
    public class OrderController : Controller
    {
        /*Statiic Data*/
        //public List<OrderModel> orders = new List<OrderModel>
        //{
        //    new OrderModel { OrderID = 1, OrderDate = DateTime.Now, CustomerName = "Alice", PaymentMode = "Credit Card", TotalAmount = 199.99m, ShippingAddress = "123 Main St", UserID = 101 },
        //    new OrderModel { OrderID = 2, OrderDate = DateTime.Now.AddDays(-1), CustomerName = "Bob", PaymentMode = "PayPal", TotalAmount = 299.99m, ShippingAddress = "124 Main St", UserID = 101 },
        //    new OrderModel { OrderID = 3, OrderDate = DateTime.Now.AddDays(-2), CustomerName = "Charlie", PaymentMode = "Cash", TotalAmount = 399.99m, ShippingAddress = "125 Main St", UserID = 101 },
        //    new OrderModel { OrderID = 4, OrderDate = DateTime.Now.AddDays(-3), CustomerName = "David", PaymentMode = null, TotalAmount = 499.99m, ShippingAddress = "126 Main St", UserID = 101 },
        //    new OrderModel { OrderID = 5, OrderDate = DateTime.Now.AddDays(-4), CustomerName = "Eva", PaymentMode = "Credit Card", TotalAmount = 599.99m, ShippingAddress = "127 Main St", UserID = 101 },
        //    new OrderModel { OrderID = 6, OrderDate = DateTime.Now.AddDays(-5), CustomerName = "Frank", PaymentMode = "Debit Card", TotalAmount = 699.99m, ShippingAddress = "128 Main St", UserID = 101 },
        //    new OrderModel { OrderID = 7, OrderDate = DateTime.Now.AddDays(-6), CustomerName = "Grace", PaymentMode = null, TotalAmount = 799.99m, ShippingAddress = "129 Main St", UserID = 101 },
        //    new OrderModel { OrderID = 8, OrderDate = DateTime.Now.AddDays(-7), CustomerName = "Hank", PaymentMode = "Credit Card", TotalAmount = 899.99m, ShippingAddress = "130 Main St", UserID = 101 },
        //    new OrderModel { OrderID = 9, OrderDate = DateTime.Now.AddDays(-8), CustomerName = "Ivy", PaymentMode = "PayPal", TotalAmount = 999.99m, ShippingAddress = "131 Main St", UserID = 101 },
        //    new OrderModel { OrderID = 10, OrderDate = DateTime.Now.AddDays(-9), CustomerName = "Jack", PaymentMode = "Cash", TotalAmount = 1099.99m, ShippingAddress = "132 Main St", UserID = 101 }
        //};

        #region Configuration
        private IConfiguration configuration;
        public OrderController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }
        #endregion

        #region Display OrderList
        /*Methods*/
        public IActionResult OrderList()
        {
            string? connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "SP_Orders_SelectAll";
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            return View(table);
        }
        #endregion

        #region AddEditOrderForm
        public IActionResult OrderForm(int OrderID)
        {
            string? connectionString = this.configuration.GetConnectionString("ConnectionString");

            #region Display User by thir id DropDownList
            SqlConnection connection1 = new SqlConnection(connectionString);
            connection1.Open();
            SqlCommand command1 = connection1.CreateCommand();
            command1.CommandType = CommandType.StoredProcedure;
            command1.CommandText = "Sp_SelectUsers_By_DropDown";
            SqlDataReader reader1 = command1.ExecuteReader();
            DataTable dataTable1 = new DataTable();
            dataTable1.Load(reader1);
            connection1.Close();

            List<UserDropDownModel> users = new List<UserDropDownModel>();

            foreach (DataRow dataRow in dataTable1.Rows)
            {
                UserDropDownModel userDropDownModel = new UserDropDownModel();
                userDropDownModel.UserID = Convert.ToInt32(dataRow["UserID"]);
                userDropDownModel.UserName = dataRow["UserName"].ToString();
                users.Add(userDropDownModel);
            }

            ViewBag.UserList = users;
            #endregion

            #region Display Payment Modes DropDownList

            SqlConnection connection2 = new SqlConnection(connectionString);
            connection2.Open();
            SqlCommand command2 = connection2.CreateCommand();
            command2.CommandType = CommandType.StoredProcedure;
            command2.CommandText = "Sp_SelectPaymentModes";
            SqlDataReader reader2 = command2.ExecuteReader();
            DataTable dataTable2 = new DataTable();
            dataTable2.Load(reader2);
            connection2.Close();

            List<PaymentModeModel> paymentModes = new List<PaymentModeModel>();

            foreach (DataRow dataRow in dataTable2.Rows)
            {
                PaymentModeModel paymentMode = new PaymentModeModel();
                paymentMode.PaymentModeID = Convert.ToInt32(dataRow["PaymentModeID"]);
                paymentMode.PaymentModeName = dataRow["PaymentModeName"].ToString();
                paymentModes.Add(paymentMode);
            }

            ViewBag.PaymentModeList = paymentModes;
            #endregion

            #region Display ProductByID Aad set value in textbox
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "SP_Orders_SelectByPK";
            command.Parameters.AddWithValue("@OrderID", OrderID);
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            OrderModel orderModel = new OrderModel();

            foreach (DataRow dataRow in table.Rows)
            {
                orderModel.OrderID = Convert.ToInt32(@dataRow["OrderID"]);
                orderModel.OrderDate = Convert.ToDateTime(@dataRow["OrderDate"]);
                orderModel.CustomerID =Convert.ToInt32(@dataRow["CustomerID"]);
                orderModel.PaymentModeID = Convert.ToInt32(@dataRow["PaymentModeID"]);
                orderModel.TotalAmount = Convert.ToDecimal(@dataRow["TotalAmount"]);
                orderModel.ShippingAddress = @dataRow["ShippingAddress"].ToString();
                orderModel.UserID = Convert.ToInt32(@dataRow["UserID"]);
            }

            #endregion


            return View(orderModel);
        }
        #endregion


        #region AddEdit Orders Logic
        public IActionResult saveOrder(OrderModel orderModel)
        {
            string? connectionString = this.configuration.GetConnectionString("ConnectionString");
            if (ModelState.IsValid)
            {
                try
                {

                    SqlConnection connection = new SqlConnection(connectionString);

                    connection.Open();
                    SqlCommand command = connection.CreateCommand();

                    command.CommandType = CommandType.StoredProcedure;

                    if (orderModel.OrderID <= 0)
                    {
                        command.CommandText = "SP_Orders_Insert";
                    }
                    else
                    {
                        command.CommandText = "SP_Orders_UpdateByPK";
                        command.Parameters.Add("@OrderID", SqlDbType.Int).Value = orderModel.OrderID;
                    }

                    // Add Parameters
                    command.Parameters.Add("@OrderDate", SqlDbType.DateTime).Value = orderModel.OrderDate;
                    command.Parameters.Add("@CustomerID", SqlDbType.Int).Value = orderModel.CustomerID;
                    command.Parameters.Add("@PaymentModeID", SqlDbType.Int).Value = orderModel.PaymentModeID;
                    command.Parameters.Add("@TotalAmount", SqlDbType.Decimal).Value = orderModel.TotalAmount;
                    command.Parameters.Add("@ShippingAddress", SqlDbType.VarChar).Value = orderModel.ShippingAddress;
                    command.Parameters.Add("@UserID", SqlDbType.Int).Value = orderModel.UserID;

                    // Execute the command
                    command.ExecuteNonQuery();


                    TempData["SuccessMessageAdd"] = "Order placed successfully!";
                    TempData["SuccessMessageEdit"] = "Order Edit  successfully!";
                    TempData["ProductID"] = orderModel.OrderID;
                    return RedirectToAction("OrderList");
                }
                catch (Exception ex)
                {

                    Console.WriteLine("An error occurred: " + ex.Message);
                    TempData["error"] = ex;
                    return View("OrderForm");
                }
            }
            else
            {
                return View("OrderForm");
            }
        }
        #endregion

        #region Delete Order
        public IActionResult DeleteOrder(int OrderID)
        {

            try
            {
                string? connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "SP_Orders_DeleteByPK";
                // Add the OrderID parameter
                command.Parameters.Add("@OrderID", SqlDbType.Int).Value = OrderID;
                // Execute the command
                command.ExecuteNonQuery();
                TempData["SuccessMassage"] = "Order Deleted Successfuly";
                return RedirectToAction("OrderList");
            }
            catch (Exception ex)
            {
                TempData["errorEx"] = ex;
                TempData["error"] = "Can not Delete.";
            }

            return RedirectToAction("OrderList");
        }
        #endregion
    }

}

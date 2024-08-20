using CoffieShop.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using Microsoft.CodeAnalysis;

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

        #region configuration
        private IConfiguration configuration;
        public OrderDetailController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }
        #endregion

        #region OrderDetailsList
        /*Methods*/
        public IActionResult OrderDetailsList()
        {
            string? connectionString = this.configuration.GetConnectionString("ConnectionString");
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
        #endregion


        #region AddEdit Order Detail Form
        public IActionResult AddEditOrderDetailForm(int OrderDetailID) {
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

            #region Display ProductByID Aad set value in textbox
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "SP_OrderDetails_SelectByPK";
            command.Parameters.AddWithValue("@OrderDetailID", OrderDetailID);
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            OrderDetailModel orderDetailModel = new OrderDetailModel();

            foreach (DataRow dataRow in table.Rows)
            {
                orderDetailModel.OrderID = Convert.ToInt32(@dataRow["OrderID"]);
                orderDetailModel.ProductID = Convert.ToInt32(@dataRow["ProductID"]);
                orderDetailModel.Quantity = Convert.ToInt32(@dataRow["Quantity"]);
                orderDetailModel.Amount = Convert.ToDecimal(@dataRow["Amount"]);
                orderDetailModel.TotalAmount = Convert.ToDecimal(@dataRow["TotalAmount"]);
                orderDetailModel.UserID = Convert.ToInt32(@dataRow["UserID"]);
            }

            #endregion

            return View(orderDetailModel);
        }
        #endregion


        #region Save Order Detail
        public IActionResult SaveOrderDetail(OrderDetailModel orderDetailModel) {
            string? connectionString = this.configuration.GetConnectionString("ConnectionString");

            #region SaveOrderDetail
            if (ModelState.IsValid)
            {
                try
                {

                    SqlConnection connection = new SqlConnection(connectionString);

                    connection.Open();
                    SqlCommand command = connection.CreateCommand();

                    command.CommandType = CommandType.StoredProcedure;

                    if (orderDetailModel.OrderDetailID == null)
                    {
                        command.CommandText = "SP_OrderDetails_Insert";
                    }
                    else
                    {
                        command.CommandText = "SP_OrderDetails_UpdateByPK";
                        command.Parameters.Add("@OrderDetailID", SqlDbType.Int).Value = orderDetailModel.OrderDetailID;
                    }

                    // Add Parameters
                    command.Parameters.Add("@OrderID", SqlDbType.Int).Value = orderDetailModel.OrderID;
                    command.Parameters.Add("@ProductID", SqlDbType.Int).Value = orderDetailModel.ProductID;
                    command.Parameters.Add("@Quantity", SqlDbType.Int).Value = orderDetailModel.Quantity;
                    command.Parameters.Add("@Amount", SqlDbType.Decimal).Value = orderDetailModel.Amount; 
                    command.Parameters.Add("@TotalAmount", SqlDbType.Decimal).Value = orderDetailModel.TotalAmount;
                    command.Parameters.Add("@UserID", SqlDbType.Int).Value = orderDetailModel.UserID;

                    // Execute the command
                    command.ExecuteNonQuery();


                    TempData["SuccessMessageAdd"] = "OrderDetail added successfully!";
                    TempData["SuccessMessageEdit"] = "OrderDetail Edit  successfully!";
                    TempData["OrderDetailID"] = orderDetailModel.OrderDetailID;
                    return RedirectToAction("OrderDetailsList");
                }
                catch (Exception ex)
                {

                    Console.WriteLine("An error occurred: " + ex.Message);
                    return View("AddEditOrderDetailForm", orderDetailModel);
                }
            }
            else
            {

                return View("AddEditOrderDetailForm", orderDetailModel);
            }
            #endregion
        }
        #endregion

        #region DeleteOrderDetail
        public ActionResult DeleteOrderDetail(int OrderDetailID) {

            try
            {
                string? connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "SP_OrderDetails_DeleteByPK";
                // Add the UserID parameter
                command.Parameters.Add("@OrderDetailID", SqlDbType.Int).Value = OrderDetailID;
                // Execute the command
                command.ExecuteNonQuery();
                return RedirectToAction("OrderDetailsList");
            }
            catch (Exception ex)
            {
                TempData["errorEx"] = ex;
                TempData["error"] = "Can not Delete.";
                return RedirectToAction("OrderDetailsList");
            }

           
        }
        #endregion

    }
}

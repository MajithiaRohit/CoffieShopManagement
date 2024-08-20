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

        #region configuration
        private IConfiguration configuration;
        public BillsController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }
        #endregion

        
        #region Bill List Display
        public IActionResult BillsList()
        {
            string? connectionString = this.configuration.GetConnectionString("ConnectionString");
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
        #endregion



        #region AddEdit Bills
        public IActionResult AddEditBillsForm(int BillID)
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
            #region Display Bill Aad set value in textbox
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "SP_Bills_SelectByPK";
            command.Parameters.AddWithValue("@BillID", BillID);
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            BillsModel billsModel = new BillsModel();

            foreach (DataRow dataRow in table.Rows)
            {
                billsModel.BillNumber = @dataRow["BillNumber"].ToString();
                billsModel.BillDate = Convert.ToDateTime(@dataRow["BillDate"]);
                billsModel.OrderID = Convert.ToInt32(@dataRow["OrderID"]);
                billsModel.TotalAmount = Convert.ToDecimal(@dataRow["TotalAmount"]);
                billsModel.Discount = Convert.ToDecimal(@dataRow["Discount"]);
                billsModel.NetAmount = Convert.ToDecimal(@dataRow["NetAmount"]);
                billsModel.UserID = Convert.ToInt32(@dataRow["UserID"]);
            }

            #endregion

            return View(billsModel);
        }
        #endregion

        #region Save Bill
        public IActionResult saveBills(BillsModel billsModel)
        {
            string? connectionString = this.configuration.GetConnectionString("ConnectionString");

            #region saveBills
            if (ModelState.IsValid)
            {
                try
                {

                    SqlConnection connection = new SqlConnection(connectionString);

                    connection.Open();
                    SqlCommand command = connection.CreateCommand();

                    command.CommandType = CommandType.StoredProcedure;

                    if (billsModel.BillID == null)
                    {
                        command.CommandText = "SP_Bills_Insert";
                    }
                    else
                    {
                        command.CommandText = "SP_Bills_UpdateByPK";
                        command.Parameters.Add("@BillID", SqlDbType.Int).Value = billsModel.BillID;
                    }

                    // Add Parameters
                    command.Parameters.Add("@BillNumber", SqlDbType.VarChar).Value = billsModel.BillNumber;
                    command.Parameters.Add("@BillDate", SqlDbType.Date).Value = billsModel.BillDate;
                    command.Parameters.Add("@OrderID", SqlDbType.Int).Value = billsModel.OrderID;
                    command.Parameters.Add("@TotalAmount", SqlDbType.Decimal).Value = billsModel.TotalAmount;
                    command.Parameters.Add("@Discount", SqlDbType.Decimal).Value = billsModel.Discount;
                    command.Parameters.Add("@NetAmount", SqlDbType.Decimal).Value = billsModel.@NetAmount;
                    command.Parameters.Add("@UserID", SqlDbType.Int).Value = billsModel.UserID;

                    // Execute the command
                    command.ExecuteNonQuery();


                    TempData["SuccessMessageAdd"] = "OrderDetail added successfully!";
                    TempData["SuccessMessageEdit"] = "OrderDetail Edit  successfully!";
                    TempData["OrderDetailID"] = billsModel.BillID;
                    return RedirectToAction("BillsList");
                }
                catch (Exception ex)
                {

                    Console.WriteLine("An error occurred: " + ex.Message);
                    return View("AddEditBillsForm", billsModel);
                }
            }
            else
            {

                return View("AddEditBillslForm", billsModel);
            }
            #endregion
        }
        #endregion


        #region DeleteOrderDetail
        public ActionResult DeleteOrderDetail(int BillID)
        {

            try
            {
                string? connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "SP_Bills_DeleteByPK";
                // Add the UserID parameter
                command.Parameters.Add("@BillID", SqlDbType.Int).Value = BillID;
                // Execute the command
                command.ExecuteNonQuery();
                return RedirectToAction("BillsList");
            }
            catch (Exception ex)
            {
                TempData["errorEx"] = ex;
                TempData["error"] = "Can not Delete.";
                return RedirectToAction("BillsList");
            }


        }
        #endregion
    }
}

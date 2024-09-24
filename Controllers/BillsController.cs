using CoffieShop.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using CoffieShop.Session;

namespace CoffieShop.Controllers
{
    public class BillsController : Controller
    {

		#region configuration
		private IConfiguration configuration;
        public BillsController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }
        #endregion

		public List<UserDropDownModel> setUserDropDown()
		{
			#region Display User by thir id DropDownList
			string? connectionString = this.configuration.GetConnectionString("ConnectionString");
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

			return users;
			#endregion

		}


        [LoginCheckAccess]
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


        [LoginCheckAccess]
        #region AddEdit Bills
        public IActionResult AddEditBillsForm(int BillID)
        {
            string? connectionString = this.configuration.GetConnectionString("ConnectionString");

			ViewBag.UserList = setUserDropDown();

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
                billsModel.Discount = dataRow["Discount"] == DBNull.Value ? 0 : Convert.ToDecimal(dataRow["Discount"]);
                billsModel.NetAmount = Convert.ToDecimal(@dataRow["NetAmount"]);
                billsModel.UserID = Convert.ToInt32(@dataRow["UserID"]);
            }

            #endregion

            return View(billsModel);
        }
        #endregion

        [LoginCheckAccess]
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
                    command.Parameters.Add("@Discount", SqlDbType.Decimal).Value = billsModel.Discount ?? (object)DBNull.Value;
                    command.Parameters.Add("@NetAmount", SqlDbType.Decimal).Value = billsModel.@NetAmount;
                    command.Parameters.Add("@UserID", SqlDbType.Int).Value = billsModel.UserID;

                    // Execute the command
                    command.ExecuteNonQuery();
                   

                    TempData["SuccessMessageAdd"] = "Bill added successfully!";
                    TempData["SuccessMessageEdit"] = "Bill Edit  successfully!";
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
                ViewBag.UserList = setUserDropDown();   
                return View("AddEditBillsForm", billsModel);
            }
            #endregion
        }
        #endregion

        [LoginCheckAccess]
        #region DeleteOrderDetail
        public ActionResult DeleteBill(int BillID)
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

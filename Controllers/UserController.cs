using CoffieShop.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;


namespace CoffieShop.Controllers
{
    public class UserController : Controller
    {
        #region configuration varible set
        private IConfiguration configuration;
      
        public UserController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }
        #endregion

        #region Display User List
        //User List Page
        public IActionResult UserList()
        {
            string? connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "SP_Users_SelectAll";
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            return View(table);
        }
        #endregion

        #region AddEditForm & PopulateData in TextFeild
        //AddEdit User Form Page
        public IActionResult AddEditUser(int UserID)
        {
            string? connectionString = this.configuration.GetConnectionString("ConnectionString");

            #region set value in textfeild for update 
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType= CommandType.StoredProcedure;
            command.CommandText= "SP_Users_SelectById";
            command.Parameters.Add("UserID",SqlDbType.Int).Value=UserID;
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);

            /*For set value in model*/
            UserModel userModel = new UserModel();
            foreach (DataRow row in table.Rows) {
                userModel.UserID = Convert.ToInt32(row["UserID"]);
                userModel.UserName = row["UserName"].ToString();
                userModel.Email = row["Email"].ToString() ;
                userModel.Password = row["Password"].ToString();
                userModel.MobileNo = row["MobileNo"].ToString();
                userModel.Address = row["address"].ToString();
                userModel.IsActive = Convert.ToBoolean(row["IsActive"]);
            }
            #endregion

            return View(userModel);
        }
        #endregion

        #region AddEditUser
        //Save User Page
        public IActionResult saveUser(UserModel user)
        {
            if (ModelState.IsValid)
            {
                
                string? connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                Console.WriteLine(user.IsActive);
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                if(user.UserID <=0)
                {
                    command.CommandText = "SP_Users_Insert";
                }
                else
                {
                    command.CommandText = "SP_Users_UpdateByPK";
                    command.Parameters.Add("@UserID",SqlDbType.Int).Value = user.UserID;
                }
                    
                //Add Paramiters
                command.Parameters.Add("@UserName",SqlDbType.VarChar).Value = user.UserName;
                command.Parameters.Add("@Email", SqlDbType.VarChar).Value = user.Email;
                command.Parameters.Add("@Password", SqlDbType.VarChar).Value = user.Password;
                command.Parameters.Add("@MobileNo", SqlDbType.VarChar).Value = user.MobileNo;
                command.Parameters.Add("@Address", SqlDbType.VarChar).Value = user.Address;
                command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = user.IsActive;
        

                // Execute the command
                command.ExecuteNonQuery();
                    
                
                return RedirectToAction("UserList");
            }
            else
            {
                return View("AddEditUser");
            }
        }
        #endregion

        #region DeleteUser
        //Delete User Page 
        public IActionResult DeleteUser(int UserID)
        {
           
            try{
                string? connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;    
                command.CommandText = "SP_Users_DeleteByPK";
                // Add the UserID parameter
                command.Parameters.Add("@UserID",SqlDbType.Int).Value = UserID;
                // Execute the command
                command.ExecuteNonQuery();
                return RedirectToAction("UserList");
            }
            catch(Exception ex)
            {
                ViewBag.Error = ex.Message;
                TempData["error"] = "Can not Delete Data";    
                return RedirectToAction("UserList");
            }
            
            
        }
        #endregion
    }
}

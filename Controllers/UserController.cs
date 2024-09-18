using CoffieShop.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using CoffieShop.Session;


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

        [LoginCheckAccess]
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

        [LoginCheckAccess]
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

        [LoginCheckAccess]
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
                if(user.UserID == null)
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

                TempData["SuccessMessageAdd"] = "User Add Successfuly";
                TempData["SuccessMessageEdit"] = "User Edit Successfuly";
                return RedirectToAction("UserList");
            }
            else
            {
                return View("AddEditUser" , user);
            }
        }
        #endregion

        [LoginCheckAccess]
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
                TempData["SuccessMassage"] = "User Delete Successfuly";
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


        public IActionResult UserLogin(UserLoginModel userLoginModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string connectionString = this.configuration.GetConnectionString("ConnectionString");
                    using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                    {
                        sqlConnection.Open();
                        using (SqlCommand sqlCommand = sqlConnection.CreateCommand())
                        {
                            sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                            sqlCommand.CommandText = "SP_User_Login";
                            sqlCommand.Parameters.Add("@UserName", SqlDbType.VarChar).Value = userLoginModel.UserName;
                            sqlCommand.Parameters.Add("@Password", SqlDbType.VarChar).Value = userLoginModel.Password;

                            using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                            {
                                DataTable dataTable = new DataTable();
                                dataTable.Load(sqlDataReader);

                                if (dataTable.Rows.Count > 0)
                                {
                                    foreach (DataRow dr in dataTable.Rows)
                                    {
                                        HttpContext.Session.SetString("UserID", dr["UserID"].ToString());
                                        HttpContext.Session.SetString("UserName", dr["UserName"].ToString());
                                    }
                                    // Redirect to ProductList if login is successful
                                    return RedirectToAction("UserList", "User");
                                }
                                else
                                {
                                    // No user found, invalid login
                                    ModelState.AddModelError("", "Invalid username or password.");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception (you can also log it to a file or database)
                TempData["ErrorMessage"] = "Login failed due to an unexpected error. Please try again later.";
            }

            // Return the login view with the error message
            return View(userLoginModel); // Ensure this view is set to handle form resubmission
        }

        [HttpPost]
        [LoginCheckAccess]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("UserLogin", "User");
        }

    }
}

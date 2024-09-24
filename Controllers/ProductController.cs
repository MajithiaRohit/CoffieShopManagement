using CoffieShop.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using Microsoft.CodeAnalysis;

namespace CoffieShop.Controllers
{

    public class ProductController : Controller
    {
        #region configuration
        private IConfiguration configuration;

        public ProductController(IConfiguration _configuration)
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
                UserDropDownModel userDropDownModel = new UserDropDownModel
                {
                    UserID = Convert.ToInt32(dataRow["UserID"]),
                    UserName = dataRow["UserName"].ToString()
                };
                users.Add(userDropDownModel);
            }
            return users;
            #endregion

        }

        #region Display Product List
        public IActionResult ProductList()
        {
            string? connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "SP_Products_SelectAll";
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            return View(table);
        }
        #endregion

        #region AddEditProduct And Set textbox feild
        public IActionResult AddEditProduct(int ProductID)
        {
          
            ViewBag.UserList = setUserDropDown();

            
            if (ProductID > 0)
            {
                string? connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "SP_Products_SelectByPK";
                command.Parameters.AddWithValue("@ProductID", ProductID);
                SqlDataReader reader = command.ExecuteReader();
                DataTable table = new DataTable();
                table.Load(reader);
                connection.Close(); 

                ProductModel productModel = new ProductModel();

                foreach (DataRow dataRow in table.Rows)
                {
                    productModel.ProductID = Convert.ToInt32(dataRow["ProductID"]);
                    productModel.ProductName = dataRow["ProductName"].ToString();
                    productModel.ProductCode = dataRow["ProductCode"].ToString();
                    productModel.ProductPrice = Convert.ToDecimal(dataRow["ProductPrice"]);
                    productModel.Description = dataRow["Description"].ToString();
                    productModel.UserID = Convert.ToInt32(dataRow["UserID"]); 
                }

                return View(productModel);
            }
            else
            {
                return View(new ProductModel());
            }
        }

        #endregion

        #region saveAddEditProduct
        public IActionResult SaveProduct(ProductModel product)
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

                    if (product.ProductID == null) 
                    {
                        command.CommandText = "SP_Products_Insert";
                    }
                    else 
                    {
                        command.CommandText = "SP_Products_UpdateByPK";
                        command.Parameters.AddWithValue("@ProductID", product.ProductID);
                    }

                    command.Parameters.AddWithValue("@ProductName", product.ProductName);
                    command.Parameters.AddWithValue("@ProductPrice", product.ProductPrice);
                    command.Parameters.AddWithValue("@ProductCode", product.ProductCode);
                    command.Parameters.AddWithValue("@Description", product.Description);
                    command.Parameters.AddWithValue("@UserID", product.UserID);

                    command.ExecuteNonQuery();

                    connection.Close();

                    if (product.ProductID == null)
                        TempData["SuccessMessageAdd"] = "Product added successfully!";
                    else
                        TempData["SuccessMessageEdit"] = "Product edited successfully!";

                    return RedirectToAction("ProductList");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: " + ex.Message);
                    TempData["Error"] = ex.Message;
                    return View("AddEditProduct", product);
                }
            }
            else
            {
                ViewBag.UserList = setUserDropDown();
                return View("AddEditProduct", product); 
            }
        }


        #endregion


        #region DeleteProduct
        public IActionResult DeleteProduct(int ProductID)
        {
            try
            {
                string? connectionString = this.configuration.GetConnectionString("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "SP_Products_DeleteByPK";
                    command.Parameters.Add("@ProductID", SqlDbType.Int).Value = ProductID;

                    command.ExecuteNonQuery();
                }
                TempData["SuccessMessageDelete"] = "Product deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["errorEx"] = "Cannot delete product.";
            }

            return RedirectToAction("ProductList");
        }
        #endregion

    }
}

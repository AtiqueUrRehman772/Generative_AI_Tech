﻿using Generative_AI_Tech.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;
using OfficeOpenXml;
using System.Data.SqlClient;
using System.Diagnostics;

namespace Generative_AI_Tech.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _config;
        private string _constr;
        private static UserModal _current_user = new UserModal();
        private static List<string>likedWebsites = new List<string>();

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment webHostEnvironment, IConfiguration config)
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
            _config = config;
            _constr = _config.GetConnectionString("default");
        }

        public IActionResult Index()
        {
            if (_current_user != null && _current_user?.Email != "" && _current_user?.Email != null)
            {
                _current_user.GenAiSites = new List<SiteModal>();
                _current_user.GenAiSites = GetAllSites();
            }
            return View(_current_user);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }



        //////// New Added Pages  ////////

        public IActionResult GenAI(string type)
        {
            return View(new GenAIViewModel
            {
                type = type
            });
        }
        public IActionResult Jobs()
        {
            try
            {
                var data = ReadCsvFile();
                int sal2020 = 0;
                int sal2020c = 0;
                int sal2021 = 0;
                int sal2021c = 0;
                int sal2022 = 0;
                int sal2022c = 0;
                int sal2023 = 0;
                int sal2023c = 0;
                data.RemoveAt(0);
                foreach (var item in data)
                {
                    if (item[0] == "2023")
                    {
                        sal2023 += int.Parse(item[4]);
                        sal2023c++;
                    }
                    else if (item[0] == "2022")
                    {
                        sal2022 += int.Parse(item[4]);
                        sal2022c++;
                    }
                    else if (item[0] == "2021")
                    {
                        sal2021 += int.Parse(item[4]);
                        sal2021c++;
                    }
                    else if (item[0] == "2020")
                    {
                        sal2020 += int.Parse(item[4]);
                        sal2020c++;
                    }
                }
                List<string> responseData = new List<string>();
                responseData.Add((sal2023 / sal2023c).ToString());
                responseData.Add((sal2022 / sal2022c).ToString());
                responseData.Add((sal2021 / sal2021c).ToString());
                responseData.Add((sal2020 / sal2020c).ToString());
                return View(new { data = JsonConvert.SerializeObject(responseData, Formatting.Indented), excelArray = JsonConvert.SerializeObject(data) });
            }
            catch (Exception)
            {
                return Ok(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        public IActionResult Contact()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }
        public IActionResult GenAISites()
        {
            if (_current_user != null && _current_user?.Email != "" && _current_user?.Email != null)
            {
                _current_user.GenAiSites = new List<SiteModal>();
                _current_user.GenAiSites = GetAllSites();
            }
            return View(_current_user);
        }

        [HttpGet]
        public List<string> ReadDataFromExcel()
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                string wwwrootPath = _webHostEnvironment.WebRootPath;
                string excelFilePath = Path.Combine(wwwrootPath, "Book1.xlsx"); // Replace with your Excel file name

                using (var package = new ExcelPackage(new FileInfo(excelFilePath)))
                {
                    var worksheet = package.Workbook.Worksheets[0]; // Assuming you want to read the first worksheet

                    // Determine the number of rows in the worksheet
                    int rowCount = worksheet.Dimension.Rows;

                    // Read data into a string array
                    string[] dataArray = new string[rowCount];

                    for (int row = 1; row <= rowCount; row++)
                    {
                        dataArray[row - 1] = worksheet.Cells[row, 1].Value?.ToString();
                    }

                    return dataArray.ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public List<string[]> ReadCsvFile()
        {
            string wwwrootPath = _webHostEnvironment.WebRootPath;
            string filePath = Path.Combine(wwwrootPath, "salaries2023.csv"); ; // Specify the path to your CSV file

            // Initialize a list to store the CSV data
            List<string[]> csvData = new List<string[]>();

            try
            {
                using (TextFieldParser parser = new TextFieldParser(filePath))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(",");

                    while (!parser.EndOfData)
                    {
                        string[] row = parser.ReadFields();
                        csvData.Add(row);
                    }
                }
                string[][] csvArray = csvData.ToArray();

                return csvData;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public IActionResult loginUser(string email, string password)
        {
            try
            {
                SqlConnection con = new SqlConnection(_constr);
                con.Open();
                string query = "select * from tbl_users where email='" + email + "' and password='" + password + "'";
                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    _current_user.Email = email;
                    _current_user.Password = password;
                    _current_user.Role = reader["Role"].ToString()?.ToLower();
                    con.Close();
                    return RedirectToAction("Index", new { email = email });
                }
                con.Close();
                return RedirectToAction("Login", new { email = "wrongcreds" });
            }
            catch (Exception)
            {
                return RedirectToAction("Login");
                throw;
            }
        }
        public IActionResult registerUser(string name, string email, string password)
        {
            try
            {
                SqlConnection con = new SqlConnection(_constr);
                con.Open();
                string query = "insert into tbl_users(UserName,Email,Password,Role) values('" + name + "','" + email + "','" + password + "','user')";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.ExecuteNonQuery();
                return RedirectToAction("Login");
            }
            catch (Exception)
            {
                return RedirectToAction("Register");
                throw;
            }
        }
        public string Logout()
        {
            _current_user.Email = null;
            likedWebsites = new List<string>();
            return "done";
        }
        public async Task<IActionResult> UpdateWebsite(SiteModal model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img", "uploads");
                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImageFile.FileName;
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        model.Image_Name = uniqueFileName;
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await model.ImageFile.CopyToAsync(stream);
                        }
                    }
                }
                SqlConnection con = new SqlConnection(_constr);
                string query = @"update tbl_genai set site_name = '" + model.Site_Name + "'";
                if (model.Image_Name != null && model.Image_Name.Trim() != "")
                    query += ",image_name = '" + model.Image_Name + "',summary = '" + model.Summary + "' where id=" + model.Id;
                con.Open();
                SqlCommand command = new SqlCommand(query, con);
                command.ExecuteNonQuery();
                con.Close();
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                return Ok(StatusCodes.Status404NotFound);
                throw;
            }
        }
        public async Task<IActionResult> AddNewWebsite(SiteModal model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img", "uploads");
                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImageFile.FileName;
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        model.Image_Name = uniqueFileName;
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await model.ImageFile.CopyToAsync(stream);
                        }
                    }
                }
                SqlConnection con = new SqlConnection(_constr);
                string query = @"insert into tbl_genai (site_name,image_name,summary,likes) values('" + model.Site_Name + "','" + model.Image_Name + "','" + model.Summary + "',0)";

                con.Open();
                SqlCommand command = new SqlCommand(query, con);
                command.ExecuteNonQuery();
                con.Close();
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                return Ok(StatusCodes.Status404NotFound);
                throw;
            }
        }
        public async Task<IActionResult> DeleteWebsite(string Id)
        {
            try
            {
                SqlConnection con = new SqlConnection(_constr);
                string query = "delete from tbl_genai where id=" + Id;
                SqlCommand command = new SqlCommand(query, con);
                con.Open();
                command.ExecuteNonQuery();
                con.Close();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return Ok(StatusCodes.Status404NotFound);
                throw;
            }
        }
        public List<SiteModal> GetAllSites()
        {
            try
            {
                List<SiteModal> list = new List<SiteModal>();
                string image_name = "";
                SqlConnection con = new SqlConnection(_constr);
                string query = "select * from tbl_genai";
                con.Open();
                SqlCommand command = new SqlCommand(query, con);
                SqlDataReader sdr = command.ExecuteReader();
                while (sdr.Read())
                {
                    list.Add(new SiteModal()
                    {
                        Id = sdr["id"].ToString(),
                        Image_Name = sdr["image_name"].ToString(),
                        Site_Name = sdr["site_name"].ToString(),
                        Summary = sdr["summary"].ToString(),
                        Likes = int.Parse(sdr["likes"].ToString() ?? "0")
                    });
                }
                con.Close();
                return list;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IActionResult AddLike(string Id)
        {
            try
            {
                foreach (var item in likedWebsites)
                {
                    if(item.Trim() == Id.Trim())
                        return RedirectToAction("GenAISites");
                }
                using (SqlConnection con = new(_constr))
                {
                    string query = "update tbl_genai set likes = likes + 1 where id = " + Id;
                    using (SqlCommand com = new(query, con))
                    {
                        con.Open();
                        com.ExecuteNonQuery();
                        con.Close();
                        likedWebsites.Add(Id);
                    }
                }
                return RedirectToAction("GenAISites");
            }
            catch (Exception)
            {
                return NotFound();
                throw;
            }
        }
        [HttpPost]
        public string SetEmail(string email)
        {
            _current_user.Email = email;
            return "Done";
        }
    }

}
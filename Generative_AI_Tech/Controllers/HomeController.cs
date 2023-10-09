using Generative_AI_Tech.Models;
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

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
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
                return View(new { data = JsonConvert.SerializeObject(responseData,Formatting.Indented),excelArray = JsonConvert.SerializeObject(data) });
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


        [HttpGet]
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
            SqlConnection con = new SqlConnection("connection");
            con.Open();
            string query = "select * from user where email='" + email + "' and password='" + password + "'";
            SqlCommand cmd = new SqlCommand(query, con);
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return RedirectToAction("Index");
            }
            return RedirectToAction("login");
        }
        public IActionResult registerUser(string name, string email, string password)
        {
            SqlConnection con = new SqlConnection("connection");
            con.Open();
            string query = "insert into uservalues('" + name + "'," + email + "','" + password + "')";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.ExecuteNonQuery();
            return RedirectToAction("Index");
        }
    }

}
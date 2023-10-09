using Generative_AI_Tech.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Diagnostics;

namespace Generative_AI_Tech.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
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
        public IActionResult login()
        {
            return View();
        }
        public IActionResult register()
        {
            return View();
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
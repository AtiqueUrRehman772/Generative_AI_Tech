using Generative_AI_Tech.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace Generative_AI_Tech.Controllers
{
    public class GenAIsController : Controller
    {
        private readonly IConfiguration _config;
        public GenAIsController(IConfiguration config)
        {
            _config = config;
        }
        public IActionResult Create()
        {
            return View();
        }
        public IActionResult Delete(string Id)
        {
            SiteModal siteModal = getSiteById(Id);
            return View(
                new UserModal()
                {
                    GenAiSites = new List<SiteModal>()
                    {
                        siteModal
                    }
                });
        }
        public IActionResult Edit(string Id)
        {
            SiteModal siteModal = getSiteById(Id);
            return View(
                new UserModal()
                {
                    GenAiSites = new List<SiteModal>()
                    {
                        siteModal
                    }
                });
        }
        public SiteModal getSiteById(string id)
        {
            try
            {
                using (SqlConnection con = new(_config.GetConnectionString("default")))
                {
                    string query = "select * from tbl_genai where id="+id+"";
                    using (SqlCommand com = new(query, con))
                    {
                        con.Open();
                        SqlDataReader sdr = com.ExecuteReader();
                        if (sdr.Read())
                        {
                            return new SiteModal
                            {
                                Id = sdr["id"].ToString(),
                                Image_Name = sdr["image_name"].ToString(),
                                Site_Name = sdr["site_name"].ToString(),
                                Summary = sdr["summary"].ToString(),
                                Likes = int.Parse(sdr["likes"].ToString()??"0")
                            };
                        }
                        con.Close();
                    }
                }
                return new SiteModal();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using RedBerryCorporate.Data;
using RedBerryCorporate.Helpers;
//using RedBerryCorporate.Dtos;
//using RedBerryCorporate.Filters;
using RedBerryCorporate.Models;
using RedBerryCorporate.Services;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static RedBerryApi.Controllers.BaseApiController;


namespace RedBerryApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GeneralController : BaseApiController
    {
        private readonly ApplicationDbContext  _db;
        private readonly IWebHostEnvironment _env;
        //private readonly ContactUsEmailService _emailService;
        //private readonly NotificationService _notificationService;
        public GeneralController(ApplicationDbContext db, IWebHostEnvironment env)
        {
            _db = db;
           // _emailService = email;
            _env = env;
            //_notificationService = notificationService;
        }

  

        [HttpPost("ManageFile")]
        public async Task<IActionResult> ManageFile([FromForm] IFormCollection form)
        {
            try
            {
                if (!form.ContainsKey("Id") || !form.ContainsKey("FileType"))
                    return BadRequest(new { success = false, message = "Id or FileType missing." });

                int id = int.Parse(form["Id"]);
                string fileType = form["FileType"];

                if (form.Files.Count == 0)
                    return BadRequest(new { success = false, message = "No file uploaded." });

                var postedFile = form.Files[0];
                string fileExtension = Path.GetExtension(postedFile.FileName);
                string uniqueFileName = $"{fileType}{id}{DateTime.Now.Ticks}{fileExtension}";

                // Save to wwwroot/Uploads
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await postedFile.CopyToAsync(stream);
                }

                // Build URL
                var request = HttpContext.Request;
                string fileUrl = $"{request.Scheme}://{request.Host}/Uploads/{uniqueFileName}";

                // Optional: update TblEmployees if profile
                if (fileType.ToLower() == "profile")
                {
                    var emp = _db.TblEmployees.FirstOrDefault(x => x.ID == id);
                    if (emp != null)
                    {
                        emp.ProfilePicName = fileUrl;
                        _db.SaveChanges();
                    }
                }

                return Ok(new { success = true, message = "File uploaded successfully.", FileUrl = fileUrl });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

  

    }


}


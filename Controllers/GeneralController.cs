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

        // ========================
        // GET: api/General/GetUser
        // ========================
        [Authorize(Roles = "SuperAdmin,Admin,Agent,Viewer")]
        [HttpGet("GetUser")]
        public IActionResult GetUser()
        {
            //var currentRole = GetCurrentUserRole();
            //var currentEmpId = GetCurrentEmpId();

            //Console.WriteLine($"Current Role: {currentRole}");
            //Console.WriteLine($"Current EmpId: {currentEmpId}");
            var accessCheck = CheckRoleAccess(AppRoles.SuperAdmin, AppRoles.Admin, AppRoles.Agent, AppRoles.Viewer);
            if (accessCheck != null)
                return accessCheck;

            var currentRole = GetCurrentUserRole();
            var currentEmpId = GetCurrentEmpId();

            var query = from u in _db.Users
                        join e in _db.TblEmployees on u.EmpId equals e.ID
                        select new UserModal
                        {
                            id = u.ID,
                            Name = e.FULL_NAME,
                            Email = e.EMAIL_ADDRESS,
                            Role = u.RoleNames,
                            Profileimage = e.ProfilePicName,
                            EmpId = u.EmpId,
                            EmpNo = e.EMPLOYEE_NUMBER,
                            MobileNo = e.MOBILE_M,
                            Languages = e.Languages,
                            ReraNumber = e.ReraNumber,
                            WhatsappNo = e.WhatsappNo,
                            IsActive =e.IsActive ?? true
                        };

            if (currentRole == AppRoles.Admin)
            {
                query = query.Where(x => x.Role != AppRoles.SuperAdmin);
            }
            else if (currentRole == AppRoles.Agent || currentRole == AppRoles.Viewer)
            {
                query = query.Where(x => x.EmpId == currentEmpId);
            }

            return Ok(query.ToList());
        }
        
      

        [Authorize(Roles = "SuperAdmin,Admin,Agent,Viewer")]
        [HttpGet("GetUserbyId")]
        public IActionResult GetUserById(int id)
        {
            var currentRole = GetCurrentUserRole();
            var currentEmpId = GetCurrentEmpId();

            var user = (from u in _db.Users
                        join e in _db.TblEmployees on u.EmpId equals e.ID
                        where u.ID == id
                        select new UserModal
                        {
                            id = u.ID,
                            Name = e.FULL_NAME,
                            Email = e.EMAIL_ADDRESS,
                            Role = u.RoleNames,
                            Profileimage = e.ProfilePicName,
                            EmpId = u.EmpId,
                            EmpNo = e.EMPLOYEE_NUMBER,
                            ReraNumber = e.ReraNumber,
                            Languages = e.Languages,
                            MobileNo = e.MOBILE_M,
                            WhatsappNo = e.WhatsappNo,
                            IsActive = e.IsActive ?? true
                        }).FirstOrDefault();

            if (user == null)
                return NotFound("User not found");

            if (currentRole == "Admin" && user.Role == "SuperAdmin")
                return BadRequest("Admin cannot access SuperAdmin records.");

            if ((currentRole == "Agent" || currentRole == "Viewer") && user.EmpId != currentEmpId)
                return BadRequest("You can only view your own record.");

            return Ok(user);
        }

        private IActionResult SaveUser(UserModal userModal)
        {
            try
            {
                if (userModal == null)
                    return BadRequest("userModal is null");

                if (string.IsNullOrWhiteSpace(userModal.Name))
                    return BadRequest("Name is required");

                if (string.IsNullOrWhiteSpace(userModal.Email))
                    return BadRequest("Email is required");

                if (string.IsNullOrWhiteSpace(userModal.Password))
                    return BadRequest("Password is required");

                if (string.IsNullOrWhiteSpace(userModal.Role))
                    return BadRequest("Role is required");
                if (string.IsNullOrWhiteSpace(userModal.MobileNo))
                    return BadRequest("Phone is required");

                if (string.IsNullOrWhiteSpace(userModal.ReraNumber))
                    return BadRequest("RERA Number is required");
                if (!IsValidRole(userModal.Role))
                    return BadRequest("Invalid role. Allowed roles: SuperAdmin, Admin, Agent, Viewer");

                // =========================
                // CURRENT LOGGED-IN USER ROLE CHECK
                // =========================
                var currentRole = GetCurrentUserRole();

                if (string.IsNullOrWhiteSpace(currentRole))
                    return StatusCode(403, new { message = "Unable to detect current user role." });

                // Admin can only create/update Agent and Viewer
                if (currentRole == "Admin" &&
                    (userModal.Role == "SuperAdmin" || userModal.Role == "Admin"))
                {
                    return StatusCode(403, new
                    {
                        message = "Admin can only create or update Agent and Viewer users."
                    });
                }

                // Agent / Viewer cannot create/update users
                if (currentRole == "Agent" || currentRole == "Viewer")
                {
                    return StatusCode(403, new
                    {
                        message = "You are not allowed to create or update users."
                    });
                }

                // =========================
                // UPDATE EXISTING USER
                // =========================
                if (userModal.id.HasValue && userModal.id > 0)
                {
                    var userdata = _db.Users.FirstOrDefault(x => x.ID == userModal.id);
                    if (userdata == null)
                        return BadRequest("User not found");

                    // Optional extra protection:
                    // Admin cannot update existing SuperAdmin/Admin users
                    if (currentRole == "Admin" &&
                        (userdata.RoleNames == "SuperAdmin" || userdata.RoleNames == "Admin"))
                    {
                        return StatusCode(403, new
                        {
                            message = "Admin cannot update SuperAdmin or Admin users."
                        });
                    }

                    // userdata.Password = Helpers.EncryptionHelper.EncrptPassword(userModal.Password);
                    if (!string.IsNullOrWhiteSpace(userModal.Password))
                    {
                        userdata.Password = RedBerryCorporate.Helpers.EncryptionHelper.EncrptPassword(userModal.Password);
                    }
                    userdata.AppWiseRoles = userModal.Role;
                    userdata.RoleNames = userModal.Role;
                    userdata.AppIDs = "raideTime-";
                    userdata.IsActive = userModal.IsActive;
                    
                    userdata.UpdatedDate = DateTime.Now;

                    _db.SaveChanges();

                    var empdata = _db.TblEmployees.FirstOrDefault(x => x.ID == userdata.EmpId);
                    if (empdata != null)
                    {
                        empdata.FULL_NAME = userModal.Name;
                        empdata.FIRST_NAME = userModal.Name;
                        empdata.EMAIL_ADDRESS = userModal.Email;
                        empdata.MOBILE_M = userModal.MobileNo;
                        empdata.WhatsappNo = userModal.WhatsappNo;

                        empdata.ReraNumber = userModal.ReraNumber;
                        empdata.Languages = userModal.Languages;

                        if (!string.IsNullOrEmpty(userModal.Profileimage))
                            empdata.ProfilePicName = userModal.Profileimage;
                        userdata.IsActive = userModal.IsActive ?? true;
                        _db.SaveChanges();
                    }

                    return Ok(new { message = "Updated successfully" });
                }

                // =========================
                // ADD NEW USER
                // =========================
                else
                {
                    if (_db.Users.Any(x => x.UserName == userModal.Email))
                        return Ok(new { message = "UsernameExist" });

                    var emp = new TblEmployee
                    {
                        FULL_NAME = userModal.Name,
                        FIRST_NAME = userModal.Name,
                        EMAIL_ADDRESS = userModal.Email,
                        //ProfilePicName = userModal.Profileimage,
                        MOBILE_M = userModal.MobileNo,
                        WhatsappNo = userModal.WhatsappNo,

                        ReraNumber = userModal.ReraNumber,
                        Languages = userModal.Languages,
                        ProfilePicName = userModal.Profileimage,
                        IsActive = userModal.IsActive ?? true
                    };

                    _db.TblEmployees.Add(emp);
                    _db.SaveChanges();

                    emp.EMPLOYEE_NUMBER = emp.ID.ToString();
                    _db.SaveChanges();

                    var user = new User
                    {
                        UserName = userModal.Email,
                        Password = RedBerryCorporate.Helpers.EncryptionHelper.EncrptPassword(userModal.Password),
                        AppWiseRoles = userModal.Role,
                        RoleNames = userModal.Role,
                        AppIDs = "raideTime-",
                        EmpId = emp.ID,
                        IsActive = userModal.IsActive,
                        CreatedDate = DateTime.Now,
                        UpdatedDate = DateTime.Now
                    };

                    _db.Users.Add(user);
                    _db.SaveChanges();

                    return Ok(new { message = "Added successfully" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "SaveUser failed",
                    error = ex.Message,
                    inner = ex.InnerException?.Message,
                    stack = ex.StackTrace
                });
            }
        }

        [HttpPost("addUser11")]
        public IActionResult AddUser11([FromBody] UserModal userModal)
        {
            return SaveUser(userModal);
        }
        //[Authorize]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpPost("addUser")]
        public async Task<IActionResult> AddUser([FromForm] string userModal, IFormFile file)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userModal))
                    return BadRequest("userModal is required.");

                var model = JsonConvert.DeserializeObject<UserModal>(userModal);

                if (model == null)
                    return BadRequest("Invalid JSON in userModal.");

                if (string.IsNullOrWhiteSpace(model.Name))
                    return BadRequest("Name is required.");

                if (string.IsNullOrWhiteSpace(model.Email))
                    return BadRequest("Email is required.");

                if (string.IsNullOrWhiteSpace(model.Password))
                    return BadRequest("Password is required.");

                if (string.IsNullOrWhiteSpace(model.Role))
                    return BadRequest("Role is required.");

                // ===== DEBUG CHECKS =====
                if (_env == null)
                    return StatusCode(500, "_env is null");

                if (file != null)
                {
                    if (string.IsNullOrWhiteSpace(file.FileName))
                        return BadRequest("Uploaded file name is empty or null.");

                    if (string.IsNullOrWhiteSpace(_env.WebRootPath))
                        return StatusCode(500, "WebRootPath is null. Ensure wwwroot exists.");

                    string imageName = Guid.NewGuid() + Path.GetExtension(file.FileName);

                    if (string.IsNullOrWhiteSpace(imageName))
                        return StatusCode(500, "imageName could not be generated.");

                    var folderPath = Path.Combine(_env.WebRootPath, "ProfileImages");

                    if (string.IsNullOrWhiteSpace(folderPath))
                        return StatusCode(500, "folderPath is null or empty.");

                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    var fullPath = Path.Combine(folderPath, imageName);

                    if (string.IsNullOrWhiteSpace(fullPath))
                        return StatusCode(500, "fullPath is null or empty.");

                    using var fs = new FileStream(fullPath, FileMode.Create);
                    await file.CopyToAsync(fs);

                    model.Profileimage = imageName;
                }

                return SaveUser(model);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Something went wrong",
                    error = ex.Message,
                    inner = ex.InnerException?.Message,
                    stack = ex.StackTrace
                });
            }
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpGet("getUser/{id}")]
        public IActionResult GetUserByIdRoute(int id)
        {
            var user = (from u in _db.Users
                        join e in _db.TblEmployees on u.EmpId equals e.ID
                        where u.ID == id
                        select new
                        {
                            u.ID,
                            Name = e.FULL_NAME,
                            Email = u.UserName,
                            Role = u.RoleNames.Replace("-", ""),
                            ProfileImage = e.ProfilePicName,
                            reraNo = e.ReraNumber,
                            language = e.Languages,
                            phoneNo = e.MOBILE_M,
                            whatsapp = e.WhatsappNo,
                            IsActive = e.IsActive ?? true
                        }).FirstOrDefault();

            if (user == null) return NotFound("User not found");
            return Ok(user);
        }

        [Authorize(Roles = "SuperAdmin,Admin,Agent,Viewer")]
        [HttpPost("updateProfile")]
        public IActionResult UpdateProfile(UserProfile userModal)
        {
            try
            {
                if (userModal.Empid == null)
                    return BadRequest("Employee ID is required");

                var currentRole = GetCurrentUserRole();
                var currentEmpId = GetCurrentEmpId();

                // Agent and Viewer can only update their own profile
                if ((currentRole == "Agent" || currentRole == "Viewer") && currentEmpId != userModal.Empid)
                {
                    return Forbid("You are not allowed to update another user's profile.");
                }

                var empdata = new TblEmployee { ID = userModal.Empid.Value };
                _db.TblEmployees.Attach(empdata);

                if (!string.IsNullOrEmpty(userModal.Address)) empdata.Address = userModal.Address;
                if (!string.IsNullOrEmpty(userModal.Bio)) empdata.Bio = userModal.Bio;
                if (!string.IsNullOrEmpty(userModal.Facebook)) empdata.Facebook = userModal.Facebook;
                if (!string.IsNullOrEmpty(userModal.LinkedIn)) empdata.LinkedIn = userModal.LinkedIn;
                if (!string.IsNullOrEmpty(userModal.MobileNo)) empdata.MOBILE_M = userModal.MobileNo;
                if (!string.IsNullOrEmpty(userModal.WhatsappNo)) empdata.WhatsappNo = userModal.WhatsappNo;
                if (!string.IsNullOrEmpty(userModal.Email)) empdata.EMAIL_ADDRESS = userModal.Email;
                if (!string.IsNullOrEmpty(userModal.Twitter)) empdata.Twitter = userModal.Twitter;
                if (userModal.Birthday != null) empdata.DATE_OF_BIRTH = userModal.Birthday;
                if (!string.IsNullOrEmpty(userModal.Position)) empdata.Position = userModal.Position;
                if (!string.IsNullOrEmpty(userModal.ReraNumber)) empdata.ReraNumber = userModal.ReraNumber;
                if (!string.IsNullOrEmpty(userModal.Languages)) empdata.Languages = userModal.Languages;

                _db.SaveChanges();

                return Ok("Updated successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Roles = "SuperAdmin,Admin,Agent,Viewer")]
        [HttpGet("GetProfile")]
        public IActionResult GetProfile(int empId)
        {
            var currentRole = GetCurrentUserRole();
            var currentEmpId = GetCurrentEmpId();

            if ((currentRole == "Agent" || currentRole == "Viewer") && currentEmpId != empId)
            {
                return Forbid("You are not allowed to view another user's profile.");
            }

            var data = _db.TblEmployees
                .Where(e => e.ID == empId)
                .Select(e => new UserProfile
                {
                    Empid = e.ID,
                    EmpNo = e.EMPLOYEE_NUMBER,
                    Address = e.Address,
                    Bio = e.Bio,
                    Birthday = e.DATE_OF_BIRTH,
                    Facebook = e.Facebook,
                    LinkedIn = e.LinkedIn,
                    MobileNo = e.MOBILE_M,
                    WhatsappNo = e.WhatsappNo,
                   //mobile =e.MOBILE_M,
                    Position = e.Position,
                    Twitter = e.Twitter,
                    Email = e.EMAIL_ADDRESS,
                    Languages = e.Languages,
                    ReraNumber = e.ReraNumber,
                    IsActive = e.IsActive ?? true
                }).FirstOrDefault();

            return Ok(data);
        }



        [Authorize(Roles = "SuperAdmin")]
        [HttpPost("deleteUser")]
        public IActionResult DeleteUser([FromQuery] int id)
        {
            if (id <= 0)
                return BadRequest(new { message = "Invalid User ID" });

            try
            {
                // 🔹 Fetch minimal user info for notification
                var userInfo = _db.Users
                    .Where(u => u.ID == id)
                    .Select(u => new { u.ID, u.UserName, u.EmpId })
                    .FirstOrDefault();

                if (userInfo == null)
                    return NotFound(new { message = $"User with ID {id} not found" });

                // 🔹 Delete related employee if exists
                if (userInfo.EmpId.HasValue)
                {
                    var employee = new TblEmployee { ID = userInfo.EmpId.Value };
                    _db.TblEmployees.Attach(employee);
                    _db.TblEmployees.Remove(employee);
                }

                // 🔹 Delete user by key only
                var user = new User { ID = id };
                _db.Users.Attach(user);
                _db.Users.Remove(user);

                _db.SaveChanges();

                return Ok(new
                {
                    message = "User, related employee deleted and notification added successfully",
                    userId = id
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error while deleting user",
                    error = ex.InnerException?.Message ?? ex.Message
                });
            }
        }




       


        public class UserModal
        {
            public int? id { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public string Role { get; set; }
            public int? EmpId { get; set; }
            public string EmpNo { get; set; }

            public string Profileimage { get; set; }
            public string ReraNumber { get; set; }
            public string Languages { get; set; }
            public string MobileNo { get; set; }
            public string WhatsappNo { get; set; }
            public bool? IsActive { get; set; }
        }
        public class UserProfile
        {
            public int? Empid { get; set; }
            public string EmpNo { get; set; }
            public string Bio { get; set; }
            public string Email { get; set; }

            public string Position { get; set; }

            public DateTime? Birthday { get; set; }

            public string MobileNo { get; set; }
            public string WhatsappNo { get; set; }

            public string LinkedIn { get; set; }

            public string Facebook { get; set; }

            public string Twitter { get; set; }

            public string Address { get; set; }
            public string ReraNumber { get; set; }
            public string Languages { get; set; }
            public bool? IsActive { get; set; }

        }
     
        private string GetCurrentUserRole()
        {
            var roleClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
            Console.WriteLine($"Role Claim: {roleClaim?.Value}");
            return roleClaim?.Value;
        }

        private int? GetCurrentEmpId()
        {
            var empIdClaim = User.Claims.FirstOrDefault(c => c.Type == "EmpId");
            Console.WriteLine($"EmpId Claim: {empIdClaim?.Value}");
            if (int.TryParse(empIdClaim?.Value, out int empId))
                return empId;
            return null;
        }

        public static class AppRoles
        {
            public const string SuperAdmin = "SuperAdmin";
            public const string Admin = "Admin";
            public const string Agent = "Agent";
            public const string Viewer = "Viewer";
        }

        private bool IsValidRole(string role)
        {
            var allowedRoles = new[] { "SuperAdmin", "Admin", "Agent", "Viewer" };
            return allowedRoles.Contains(role);
        }
      

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out int userId))
                return userId;

            return null;
        }

        private IActionResult? CheckRoleAccess(params string[] allowedRoles)
        {
            var currentRole = GetCurrentUserRole();

            if (string.IsNullOrWhiteSpace(currentRole))
            {
                return Unauthorized(new
                {
                    message = "Unable to detect current user role. Please login again."
                });
            }

            if (!allowedRoles.Contains(currentRole))
            {
                return StatusCode(403, new
                {
                    message = $"Access denied. '{currentRole}' is not allowed to access this API."
                });
            }

            return null;
        }

    }


}


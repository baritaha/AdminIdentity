using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tasks.Models.ViewModles;

namespace Tasks.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private UserManager<IdentityUser> _userManager;
        private SignInManager<IdentityUser> _signInManager;
        private RoleManager<IdentityRole> _roleManager;
        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterVeiwModle model)
        {

            if (ModelState.IsValid)
            {
                IdentityUser user = new IdentityUser
                {
                    UserName = model.UserName,
                    PhoneNumber = model.Phone,
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("Login");
                }
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }
                return View(model);
            }


            return View(model);
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RemeberMe, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("index", "home");
                }
                ModelState.AddModelError("", "Error In Password Or User Name");
                return View(model);
            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("index", "home");
        }
        [HttpGet]
        [Authorize(Roles ="Admin")]
        public IActionResult RoleList()
        {
            return View(_roleManager.Roles);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {

            if (ModelState.IsValid)
            {
                IdentityRole Role = new IdentityRole
                {
                    Name = model.RoleName
                };
                var result = await _roleManager.CreateAsync(Role);
                if (result.Succeeded)
                {
                    return RedirectToAction("RoleList");
                }
                ModelState.AddModelError("", "Role Not Created");
                return View(model);
            }
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditRole(string id)
        {
            var data = await _roleManager.FindByIdAsync(id);
            EditRoleModel model = new EditRoleModel
            {
                RoleId = data.Id,
                RoleName = data.Name,

            };
            foreach (var user in _userManager.Users)
            {
                if (await _userManager.IsInRoleAsync(user, model.RoleName))
                {
                    model.usersNames.Add(user.UserName);
                }
            }
            return View(model);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditRole(EditRoleModel model)
        {
            if (ModelState.IsValid)
            {
                var role = await _roleManager.FindByIdAsync(model.RoleId);
                role.Name = model.RoleName;
                var result = await _roleManager.UpdateAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("RoleList");
                }
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }
                return View(model);
            }
            return View(model);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteRole(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Role ID is missing or invalid.");
            }

            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound("Role not found.");
            }

            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
             
                return RedirectToAction("RoleList");
            }
            else
            {
               
                return StatusCode(500, "Role deletion failed."); 
            }
        }
        [HttpGet]
        public async Task<IActionResult> AddRemoveUser(string id)
        {
            HttpContext.Session.SetString("roleID", id);
            var data = await _roleManager.FindByIdAsync(id);
            if(data == null)
            {
                return RedirectToAction("RoleList");
            }
            var userRolesViewModel = new List<UserRoleViewModel>();
            foreach (var user in _userManager.Users)
            {
                var userRoleVM = new UserRoleViewModel
                {
                    UserId= user.Id,
                    UserName= user.UserName,
                };
                if (await _userManager.IsInRoleAsync(user ,data.Name))
                {
                    userRoleVM.IsSelected= true;
                }
                if(!(await _userManager.IsInRoleAsync(user, data.Name)))
                {
                    userRoleVM.IsSelected= false;
                }
                userRolesViewModel.Add(userRoleVM);
            }
          


            return View(userRolesViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> AddRemoveUser(List<UserRoleViewModel> userRoles)
        {
           
            string RoleId = HttpContext.Session.GetString("roleID");
            var role=await _roleManager.FindByIdAsync(RoleId);
            if (role == null)
            {
                return RedirectToAction("RoleList");
            }
            IdentityResult result ;
            foreach (var userRole in userRoles)
            {
                var user = await _userManager.FindByIdAsync(userRole.UserId);

                if (userRole.IsSelected && !( await _userManager.IsInRoleAsync(user,role.Name)))
                {
                    result = await _userManager.AddToRoleAsync(user, role.Name);
                }
                 if(! userRole.IsSelected  && await _userManager.IsInRoleAsync(user, role.Name))
                {
                    result=await _userManager.RemoveFromRoleAsync(user, role.Name);
                }
              

            }
            return View(userRoles);
        }
    }
    }

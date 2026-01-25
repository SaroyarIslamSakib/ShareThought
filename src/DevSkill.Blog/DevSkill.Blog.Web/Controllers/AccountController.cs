using DevSkill.Blog.Domain.Utilities;
using DevSkill.Blog.Infrastructure.Extensions;
using DevSkill.Blog.Infrastructure.Identity;
using DevSkill.Blog.Web.Areas.Admin.Models;
using DevSkill.Blog.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Shared;
using System.Text;
using System.Text.Encodings.Web;

namespace DevSkill.Blog.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailUtility _emailUtility;
        private readonly IServerTime _serverTime;
        private readonly ApplicationRoleManager _roleManager;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailUtility emailUtility,
            IServerTime serverTime,
            ApplicationRoleManager roleManager)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailUtility = emailUtility;
            _serverTime = serverTime;
            _roleManager = roleManager;
        }
        public async Task<IActionResult> RegisterAsync(string returnUrl = null)
        {
            var model = new RegisterModel();
            model.ReturnUrl = returnUrl;
            model.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            return View(model);
        }
        [HttpPost,ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterAsync(RegisterModel model)
        {
            try
            {
                model.ReturnUrl ??= Url.Content("~/");
                model.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
                if (ModelState.IsValid)
                {
                    var user = CreateUser();
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.RegistrationDate = _serverTime.DateTime;
                    user.PhoneNumber = model.PhoneNumber;

                    await _userStore.SetUserNameAsync(user, model.Email, CancellationToken.None);
                    await _emailStore.SetEmailAsync(user, model.Email, CancellationToken.None);
                    var result = await _userManager.CreateAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, "Blogger");

                        _logger.LogInformation("User created a new account with password.");

                        var userId = await _userManager.GetUserIdAsync(user);
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        var callbackUrl = Url.Action(
                            "ConfirmEmail", "Account",
                            values: new { area = "", userId = userId, code = code, returnUrl = model.ReturnUrl },
                            protocol: Request.Scheme);

                        await _emailUtility.SendEmailAsync($"{model.FirstName} {model.LastName}", model.Email, 
                            "Confirm your email", $"<html><body><p>Please confirm your account by " +
                            $"<a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a></p></body></html>.");


                        if (_userManager.Options.SignIn.RequireConfirmedEmail)
                        {
                            TempData.Put("ResponseMessage", new ResponseModel
                            {
                                Message = "Registration successful. Please confirm your email to login.",
                                Response = ResponseTypes.success
                            });
                            return RedirectToAction("MailConfirmation");
                        }
                        else
                        {
                            TempData.Put("ResponseMessage", new ResponseModel
                            {
                                Message = "Registration successful. Please confirm your email to login.",
                                Response = ResponseTypes.success
                            });


                            await _signInManager.SignInAsync(user, isPersistent: false);
                            return LocalRedirect(model.ReturnUrl);
                        }
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }

                return View();
            }
            catch(Exception ex)
            {
                TempData.Put("ResponseMessage", new ResponseModel
                {
                    Message = "Registration Failed",
                    Response = ResponseTypes.danger
                });
            }
            return View();
        }
        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }
        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }

        public async Task<IActionResult> LoginAsync(string returnUrl = null)
        {
            var model = new LoginModel();
            if (!string.IsNullOrEmpty(model.ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, model.ErrorMessage);
            }

            model.ReturnUrl ??= Url.Content("~/");

            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            model.ReturnUrl = returnUrl;
            model.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            return View(model);
        }
        [HttpPost,ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginAsync(LoginModel model)
        {
            try
            {
                model.ReturnUrl ??= Url.Content("~/");

                model.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

                if (ModelState.IsValid)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);

                    if (user == null)
                    {
                        TempData.Put("ResponseMessage", new ResponseModel
                        {
                            Message = "Invalid login attempt",
                            Response = ResponseTypes.danger
                        });
                        return RedirectToAction("Index", "Home");
                    }

                    // Email confirmed
                    if (!await _userManager.IsEmailConfirmedAsync(user))
                    {
                        TempData.Put("ResponseMessage", new ResponseModel
                        {
                            Message = "Please confirm your email before login",
                            Response = ResponseTypes.danger
                        });
                        return RedirectToAction("Index", "Home");
                    }



                    var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                    if (result.Succeeded)
                    {
                        TempData.Put("ResponseMessage", new ResponseModel
                        {
                            Message = "Login Successfull",
                            Response = ResponseTypes.success
                        });
                        _logger.LogInformation("User logged in.");
                        return LocalRedirect(model.ReturnUrl);
                    }
                    if (result.RequiresTwoFactor)
                    {
                        return RedirectToPage("./LoginWith2fa", new { ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
                    }
                    if (result.IsLockedOut)
                    {
                        _logger.LogWarning("User account locked out.");
                        TempData.Put("ResponseMessage", new ResponseModel
                        {
                            Message = "Account is locked",
                            Response = ResponseTypes.danger
                        });
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        TempData.Put("ResponseMessage", new ResponseModel
                        {
                            Message = "Login Failed",
                            Response = ResponseTypes.danger
                        });
                        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    TempData.Put("ResponseMessage", new ResponseModel
                    {
                        Message = "Login Failed",
                        Response = ResponseTypes.danger
                    });
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login error");

                TempData.Put("ResponseMessage", new ResponseModel
                {
                    Message = "Something went wrong",
                    Response = ResponseTypes.danger
                });

                return View("index",model:"Home");
            }
        }
        [HttpPost,ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                return RedirectToAction();
            }
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToAction("Index", "Home");

            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);
            string msg = result.Succeeded ? "Thank you for confirming your email." : "Error confirming your email.";

            TempData.Put("ResponseMessage", new ResponseModel
            {
                Message = msg,
                Response = ResponseTypes.success
            });
            await _signInManager.SignInAsync(user, isPersistent: false);
            //return LocalRedirect(model.ReturnUrl);
            return RedirectToAction("Index", "Home");


        }
        public IActionResult AccessDenied()
        {
            return View();
        }
        public IActionResult MailConfirmation()
        {
            return View();
        }
    }
}

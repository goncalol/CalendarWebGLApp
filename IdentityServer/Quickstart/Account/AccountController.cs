// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityModel;
using IdentityServer;
using IdentityServer.Models;
using IdentityServer4;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Test;
using IdentityServerAspNetIdentity.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityServerHost.Quickstart.UI
{
    /// <summary>
    /// This sample controller implements a typical login/logout/provision workflow for local and external accounts.
    /// The login service encapsulates the interactions with the user data store. This data store is in-memory only and cannot be used for production!
    /// The interaction service provides a way for the UI to communicate with identityserver for validation and context retrieval
    /// </summary>
    [SecurityHeaders]
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IClientStore _clientStore;
        private readonly IAuthenticationSchemeProvider _schemeProvider;
        private readonly IEventService _events;
        private readonly IPersistedGrantService _persistedGrantService;
        private readonly IConfiguration _configuration;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IPersistedGrantService persistedGrantService,
            IIdentityServerInteractionService interaction,
            IClientStore clientStore,
            IAuthenticationSchemeProvider schemeProvider,
            IEventService events,
            IConfiguration configuration,
            TestUserStore users = null)
        {
            
            _userManager = userManager;
            _configuration = configuration;
            _signInManager = signInManager;
            _persistedGrantService = persistedGrantService;
            _interaction = interaction;
            _clientStore = clientStore;
            _schemeProvider = schemeProvider;
            _events = events;
        }

        /// <summary>
        /// Entry point into the login workflow
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl)
        {
            // build a model so we know what to show on the login page
            var vm = await BuildLoginViewModelAsync(returnUrl);

            if (vm.IsExternalLoginOnly)
            {
                // we only have one option for logging in and it's an external provider
                return RedirectToAction("Challenge", "External", new { scheme = vm.ExternalLoginScheme, returnUrl });
            }

            return View(vm);
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginInputModel model)
        {
            var returnUrl = model.ReturnUrl;

            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberLogin, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    //_logger.LogInformation(1, "User logged in.");
                    return RedirectToLocal(returnUrl);
                }
                //if (result.RequiresTwoFactor)
                //{
                //    return RedirectToAction(nameof(VerifyCode), new { ReturnUrl = returnUrl, RememberMe = model.RememberLogin });
                //}
                if (result.IsLockedOut)
                {
                    //_logger.LogWarning(2, "User account locked out.");
                    return View("Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(await BuildLoginViewModelAsync(model));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(await BuildLoginViewModelAsync(model));
        }

        ///// <summary>
        ///// Handle postback from username/password login
        ///// </summary>
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Login(LoginInputModel model, string button)
        //{
        //    // check if we are in the context of an authorization request
        //    var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);

        //    // the user clicked the "cancel" button
        //    if (button != "login")
        //    {
        //        if (context != null)
        //        {
        //            // if the user cancels, send a result back into IdentityServer as if they 
        //            // denied the consent (even if this client does not require consent).
        //            // this will send back an access denied OIDC error response to the client.
        //            await _interaction.DenyAuthorizationAsync(context, AuthorizationError.AccessDenied);

        //            // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
        //            if (context.IsNativeClient())
        //            {
        //                // The client is native, so this change in how to
        //                // return the response is for better UX for the end user.
        //                return this.LoadingPage("Redirect", model.ReturnUrl);
        //            }

        //            return Redirect(model.ReturnUrl);
        //        }
        //        else
        //        {
        //            // since we don't have a valid context, then we just go back to the home page
        //            return Redirect("~/");
        //        }
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        // validate username/password against in-memory store
        //        if (_users.ValidateCredentials(model.Username, model.Password))
        //        {
        //            var user = _users.FindByUsername(model.Username);
        //            await _events.RaiseAsync(new UserLoginSuccessEvent(user.Username, user.SubjectId, user.Username, clientId: context?.Client.ClientId));

        //            // only set explicit expiration here if user chooses "remember me". 
        //            // otherwise we rely upon expiration configured in cookie middleware.
        //            AuthenticationProperties props = null;
        //            if (AccountOptions.AllowRememberLogin && model.RememberLogin)
        //            {
        //                props = new AuthenticationProperties
        //                {
        //                    IsPersistent = true,
        //                    ExpiresUtc = DateTimeOffset.UtcNow.Add(AccountOptions.RememberMeLoginDuration)
        //                };
        //            };

        //            // issue authentication cookie with subject ID and username
        //            var isuser = new IdentityServerUser(user.SubjectId)
        //            {
        //                DisplayName = user.Username
        //            };

        //            await HttpContext.SignInAsync(isuser, props);

        //            if (context != null)
        //            {
        //                if (context.IsNativeClient())
        //                {
        //                    // The client is native, so this change in how to
        //                    // return the response is for better UX for the end user.
        //                    return this.LoadingPage("Redirect", model.ReturnUrl);
        //                }

        //                // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
        //                return Redirect(model.ReturnUrl);
        //            }

        //            // request for a local page
        //            if (Url.IsLocalUrl(model.ReturnUrl))
        //            {
        //                return Redirect(model.ReturnUrl);
        //            }
        //            else if (string.IsNullOrEmpty(model.ReturnUrl))
        //            {
        //                return Redirect("~/");
        //            }
        //            else
        //            {
        //                // user might have clicked on a malicious link - should be logged
        //                throw new Exception("invalid return URL");
        //            }
        //        }

        //        await _events.RaiseAsync(new UserLoginFailureEvent(model.Username, "invalid credentials", clientId:context?.Client.ClientId));
        //        ModelState.AddModelError(string.Empty, AccountOptions.InvalidCredentialsErrorMessage);
        //    }

        //    // something went wrong, show form with error
        //    var vm = await BuildLoginViewModelAsync(model);
        //    return View(vm);
        //}


        ///// <summary>
        ///// Show logout page
        ///// </summary>
        //[HttpGet]
        //public async Task<IActionResult> Logout(string logoutId)
        //{
        //    // build a model so the logout page knows what to display
        //    var vm = await BuildLogoutViewModelAsync(logoutId);

        //    if (vm.ShowLogoutPrompt == false)
        //    {
        //        // if the request for logout was properly authenticated from IdentityServer, then
        //        // we don't need to show the prompt and can just log the user out directly.
        //        return await Logout(vm);
        //    }

        //    return View(vm);
        //}

        ///// <summary>
        ///// Handle logout page postback
        ///// </summary>
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Logout(LogoutInputModel model)
        //{
        //    // build a model so the logged out page knows what to display
        //    var vm = await BuildLoggedOutViewModelAsync(model.LogoutId);

        //    if (User?.Identity.IsAuthenticated == true)
        //    {
        //        // delete local authentication cookie
        //        await HttpContext.SignOutAsync();

        //        // raise the logout event
        //        await _events.RaiseAsync(new UserLogoutSuccessEvent(User.GetSubjectId(), User.GetDisplayName()));
        //    }

        //    // check if we need to trigger sign-out at an upstream identity provider
        //    if (vm.TriggerExternalSignout)
        //    {
        //        // build a return URL so the upstream provider will redirect back
        //        // to us after the user has logged out. this allows us to then
        //        // complete our single sign-out processing.
        //        string url = Url.Action("Logout", new { logoutId = vm.LogoutId });

        //        // this triggers a redirect to the external provider for sign-out
        //        return SignOut(new AuthenticationProperties { RedirectUri = url }, vm.ExternalAuthenticationScheme);
        //    }

        //    return View("LoggedOut", vm);
        //}

        /// <summary>
        /// Show logout page
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Logout(string logoutId)
        {
            if (User.Identity.IsAuthenticated == false)
            {
                // if the user is not authenticated, then just show logged out page
                return await Logout(new LogoutViewModel { LogoutId = logoutId });
            }

            var context = await _interaction.GetLogoutContextAsync(logoutId);
            if (context?.ShowSignoutPrompt == false)
            {
                // it's safe to automatically sign-out
                return await Logout(new LogoutViewModel { LogoutId = logoutId });
            }

            // show the logout prompt. this prevents attacks where the user
            // is automatically signed out by another malicious web page.
            var vm = new LogoutViewModel
            {
                LogoutId = logoutId
            };

            return View(vm);
        }

        /// <summary>
        /// Handle logout page postback
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Logout(LogoutViewModel model)
        {
            var idp = User?.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;
            var subjectId = HttpContext.User.Identity.GetSubjectId();

            if (idp != null && idp != IdentityServerConstants.LocalIdentityProvider)
            {
                if (model.LogoutId == null)
                {
                    // if there's no current logout context, we need to create one
                    // this captures necessary info from the current logged in user
                    // before we signout and redirect away to the external IdP for signout
                    model.LogoutId = await _interaction.CreateLogoutContextAsync();
                }

                string url = "/Account/Logout?logoutId=" + model.LogoutId;
                try
                {
                    await _signInManager.SignOutAsync();
                    // await HttpContext.Authentication.SignOutAsync(idp, new AuthenticationProperties { RedirectUri = url });
                }
                catch (NotSupportedException)
                {
                }
            }

            // delete authentication cookie
            await _signInManager.SignOutAsync();

            // set this so UI rendering sees an anonymous user
            HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());

            // get context information (client name, post logout redirect URI and iframe for federated signout)
            var logout = await _interaction.GetLogoutContextAsync(model.LogoutId);

            var vm = new LoggedOutViewModel
            {
                PostLogoutRedirectUri = logout?.PostLogoutRedirectUri,
                ClientName = logout?.ClientId,
                SignOutIframeUrl = logout?.SignOutIFrameUrl
            };

            await _persistedGrantService.RemoveAllGrantsAsync(subjectId, "singleapp");

            return Redirect(_configuration["HostUrl"] + "/home/index");
            //return View("LoggedOut", vm);
        }


        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
                    // Send an email with this link
                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                    //await _emailSender.SendEmailAsync(model.Email, "Confirm your account",
                    //    $"Please confirm your account by clicking this link: <a href='{callbackUrl}'>link</a>");
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    //_logger.LogInformation(3, "User created a new account with password.");
                    return RedirectToLocal(returnUrl);
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        /*****************************************/
        /* helper APIs for the AccountController */
        /*****************************************/
        private async Task<LoginViewModel> BuildLoginViewModelAsync(string returnUrl)
        {
            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
            if (context?.IdP != null && await _schemeProvider.GetSchemeAsync(context.IdP) != null)
            {
                var local = context.IdP == IdentityServer4.IdentityServerConstants.LocalIdentityProvider;

                // this is meant to short circuit the UI and only trigger the one external IdP
                var vm = new LoginViewModel
                {
                    EnableLocalLogin = local,
                    ReturnUrl = returnUrl,
                    Email = context?.LoginHint,
                };

                if (!local)
                {
                    vm.ExternalProviders = new[] { new ExternalProvider { AuthenticationScheme = context.IdP } };
                }

                return vm;
            }

            var schemes = await _schemeProvider.GetAllSchemesAsync();

            var providers = schemes
                .Where(x => x.DisplayName != null)
                .Select(x => new ExternalProvider
                {
                    DisplayName = x.DisplayName ?? x.Name,
                    AuthenticationScheme = x.Name
                }).ToList();

            var allowLocal = true;
            if (context?.Client.ClientId != null)
            {
                var client = await _clientStore.FindEnabledClientByIdAsync(context.Client.ClientId);
                if (client != null)
                {
                    allowLocal = client.EnableLocalLogin;

                    if (client.IdentityProviderRestrictions != null && client.IdentityProviderRestrictions.Any())
                    {
                        providers = providers.Where(provider => client.IdentityProviderRestrictions.Contains(provider.AuthenticationScheme)).ToList();
                    }
                }
            }

            return new LoginViewModel
            {
                AllowRememberLogin = AccountOptions.AllowRememberLogin,
                EnableLocalLogin = allowLocal && AccountOptions.AllowLocalLogin,
                ReturnUrl = returnUrl,
                Email = context?.LoginHint,
                ExternalProviders = providers.ToArray()
            };
        }

        private async Task<LoginViewModel> BuildLoginViewModelAsync(LoginInputModel model)
        {
            var vm = await BuildLoginViewModelAsync(model.ReturnUrl);
            vm.Email = model.Email;
            vm.RememberLogin = model.RememberLogin;
            return vm;
        }

        private async Task<LogoutViewModel> BuildLogoutViewModelAsync(string logoutId)
        {
            var vm = new LogoutViewModel { LogoutId = logoutId, ShowLogoutPrompt = AccountOptions.ShowLogoutPrompt };

            if (User?.Identity.IsAuthenticated != true)
            {
                // if the user is not authenticated, then just show logged out page
                vm.ShowLogoutPrompt = false;
                return vm;
            }

            var context = await _interaction.GetLogoutContextAsync(logoutId);
            if (context?.ShowSignoutPrompt == false)
            {
                // it's safe to automatically sign-out
                vm.ShowLogoutPrompt = false;
                return vm;
            }

            // show the logout prompt. this prevents attacks where the user
            // is automatically signed out by another malicious web page.
            return vm;
        }

        private async Task<LoggedOutViewModel> BuildLoggedOutViewModelAsync(string logoutId)
        {
            // get context information (client name, post logout redirect URI and iframe for federated signout)
            var logout = await _interaction.GetLogoutContextAsync(logoutId);

            var vm = new LoggedOutViewModel
            {
                AutomaticRedirectAfterSignOut = AccountOptions.AutomaticRedirectAfterSignOut,
                PostLogoutRedirectUri = logout?.PostLogoutRedirectUri,
                ClientName = string.IsNullOrEmpty(logout?.ClientName) ? logout?.ClientId : logout?.ClientName,
                SignOutIframeUrl = logout?.SignOutIFrameUrl,
                LogoutId = logoutId
            };

            if (User?.Identity.IsAuthenticated == true)
            {
                var idp = User.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;
                if (idp != null && idp != IdentityServer4.IdentityServerConstants.LocalIdentityProvider)
                {
                    var providerSupportsSignout = await HttpContext.GetSchemeSupportsSignOutAsync(idp);
                    if (providerSupportsSignout)
                    {
                        if (vm.LogoutId == null)
                        {
                            // if there's no current logout context, we need to create one
                            // this captures necessary info from the current logged in user
                            // before we signout and redirect away to the external IdP for signout
                            vm.LogoutId = await _interaction.CreateLogoutContextAsync();
                        }

                        vm.ExternalAuthenticationScheme = idp;
                    }
                }
            }

            return vm;
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        #endregion
    }
}

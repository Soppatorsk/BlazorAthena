﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using BlazorAthenaFrontend.Data.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;
using BlazorAthenaFrontend.Models;
using BlazorAthenaFrontend.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace BlazorAthenaFrontend.Areas.Identity.Pages.Account
{

    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly TokenService _tokenService;

        public LoginModel(SignInManager<ApplicationUser> signInManager, ILogger<LoginModel> logger, IHttpClientFactory httpClientFactory, TokenService tokenService)
        {
            _signInManager = signInManager;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _tokenService = tokenService;
        }


        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string ErrorMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
              
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");

                    // Call the method in IdentityController to generate and validate JWT token
                    var token = await GetJwtTokenFromAccountController(Input.Email, Input.Password);

                    var detoken = DecodeToken(token);
                    // Store or use the token
                    _tokenService.Token = token;


                    return LocalRedirect(returnUrl);
                }

                // Handle other login failure scenarios...
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        public ClaimsPrincipal DecodeToken(string token)
        {
            // Create an instance of JwtSecurityTokenHandler to handle JWT tokens
            var handler = new JwtSecurityTokenHandler();

            // Read and parse the input JWT token
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

            // Create a new ClaimsPrincipal using the claims extracted from the JWT token
            return new ClaimsPrincipal(new ClaimsIdentity(jsonToken?.Claims));
        }
        private async Task<string> GetJwtTokenFromAccountController(string email, string password)
        {
            try
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri("https://localhost:7088");

                // Adjust the URL to the endpoint in your IdentityController responsible for token generation
                var response = await client.PostAsync("/api/Identity/token", new StringContent(JsonConvert.SerializeObject(new { Email = email, Password = password }), Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    var tokenResponse = JsonConvert.DeserializeObject<TokenModel>(await response.Content.ReadAsStringAsync());
                    return tokenResponse.Token;
                }

                // Log the unsuccessful response status code
                Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                ModelState.AddModelError(string.Empty, "Failed to retrieve JWT token.");
                return null;
            }
            catch (Exception ex)
            {
                // Log any exception that occurs during the HTTP request
                Console.WriteLine($"Exception: {ex.Message}");
                ModelState.AddModelError(string.Empty, "An error occurred while retrieving JWT token.");
                return null;
            }
        }



    }
}

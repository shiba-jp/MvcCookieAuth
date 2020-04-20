using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MvcCookieAuth.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using MvcCookieAuth.Models.Account;

namespace MvcCookieAuth.Controllers
{
        public class AccountController : Controller
    {
        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            return View(new AccountModel
            {
                ReturnUrl = returnUrl
            });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> LoginImpl(AccountModel model)
        {
            if (!ModelState.IsValid) return View("Login", model);

            //Mock用ユーザーマスタと検証
            var mockDB = new[] {
                (email: "user1@example.com", password: "password1"),
                (email: "user2@example.com", password: "password2")
            };
            var isValid = mockDB.Any(u => u.email == model.Email && u.password == model.Password);

            // ユーザーが存在しない場合はLoginのViewに遷移
            if(!isValid) return View("Login", model);

            var claims = new[] {
                // 適当なユーザ名を登録しておく
                new Claim(ClaimTypes.Name, model.Email),
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(claimsIdentity);

            // サインイン
            // 認証クッキーをレスポンスに追加
            var authProperties = new AuthenticationProperties
            {
                #region description
                //AllowRefresh = <bool>,
                // Refreshing the authentication session should be allowed.

                //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                // The time at which the authentication ticket expires. A 
                // value set here overrides the ExpireTimeSpan option of 
                // CookieAuthenticationOptions set with AddCookie.

                //IsPersistent = true,
                // Whether the authentication session is persisted across 
                // multiple requests. When used with cookies, controls
                // whether the cookie's lifetime is absolute (matching the
                // lifetime of the authentication ticket) or session-based.

                //IssuedUtc = <DateTimeOffset>,
                // The time at which the authentication ticket was issued.

                //RedirectUri = <string>
                // The full path or absolute URI to be used as an http 
                // redirect response value.
                #endregion
            };

            // サインイン
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, 
                new ClaimsPrincipal(claimsIdentity), 
                authProperties);

            // LoginViewのGet時のGetパラメータに指定されたURLに遷移
            if (IsUrlValid(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        private static bool IsUrlValid(string returnUrl)
        {
            return !string.IsNullOrWhiteSpace(returnUrl)
                && Uri.IsWellFormedUriString(returnUrl, UriKind.Relative);
        }

        public IActionResult Logout()
        {
            return View("Logout");
        }

        [HttpPost]
        public async Task<IActionResult> LogoutImpl()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            // ログアウト後はトップページへリダイレクト
            return LocalRedirect(Url.Content("~/"));
        }
    }
}
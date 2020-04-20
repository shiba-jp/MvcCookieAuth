using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MvcCookieAuth.Models.Account
{
    public class AccountModel
    {
        [DisplayName("Emailアドレス")]
        [Required(ErrorMessage = "{0}は必須です。")]
        [EmailAddress()]
        public string Email { get; set; }

        [DisplayName("パスワード")]
        [Required(ErrorMessage = "{0}は必須です。")]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }
    }
} 
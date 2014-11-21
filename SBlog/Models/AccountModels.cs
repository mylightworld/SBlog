using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SBlog.Models
{
    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "电子邮件")]
        public string Email { get; set; }
    }

    public class LoginModel
    {
        [Required]
        [Display(Name = "账户名")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [Display(Name = "记住我?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "电子邮件")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "用户名")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "确认密码")]
        [Compare("Password", ErrorMessage = "密码和确认密码不匹配。")]
        public string ConfirmPassword { get; set; }
    }

    public class ActivateModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public DateTime CreateTime { get; set; }
    }

    public class ChangePasswordModel
    {
        [Display(Name = "用户名")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "旧密码")]
        public string Password { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "新密码")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "确认新密码")]
        [Compare("ConfirmNewPassword", ErrorMessage = "新密码和确认新密码不匹配。")]
        public string ConfirmNewPassword { get; set; }
    }

    public class ResetPasswordModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "新密码")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "确认新密码")]
        [Compare("ConfirmNewPassword", ErrorMessage = "新密码和确认新密码不匹配。")]
        public string ConfirmNewPassword { get; set; }
    }

    public class ForgotPasswordVerifyModel
    {
        public int Id { get; set; }
        public DateTime CreateTime { get; set; }
    }

    public class ForgotPasswordModel
    {
        [Required(ErrorMessage="请填写邮件！")]
        [Display(Name="邮箱")]
        [EmailAddress]
        public string Email { get; set; }
    }
}

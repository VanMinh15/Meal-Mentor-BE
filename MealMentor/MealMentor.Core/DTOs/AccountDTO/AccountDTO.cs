using System.ComponentModel.DataAnnotations;

namespace MealMentor.Core.DTOs.AccountDTO
{
    public class AccountDTO
    {
    }

    public class LoginDTO
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }

    public class RegisterDTO
    {
        [Required(ErrorMessage = "Please enter your User Name is required")]
        public string UserName { get; set; }
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }

    public class ForgotPasswordDTO
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }
    }

    public class LoginResponseData
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}

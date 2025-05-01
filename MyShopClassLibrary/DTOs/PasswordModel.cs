using System.ComponentModel.DataAnnotations;

namespace MyShopClassLibrary.DTOs
{
    public class PasswordModel
    {
        public string UserKey { get; set; }
        [Required(ErrorMessage = "Old Password is required ")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Old Password")]
        public string NewPassword { get; set; } = string.Empty;
        [Required(ErrorMessage = "Password is required ")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string OldPassword { get; set; } = string.Empty;
        [Required(ErrorMessage = "Confirm Password is required ")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = "";
    }
}

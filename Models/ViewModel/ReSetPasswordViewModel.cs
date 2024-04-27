using System.ComponentModel.DataAnnotations;

namespace YYBagProgram.Models.ViewModel
{
    public class ResetPasswordViewModel
    {
        [Display(Name = "會員信箱")]
        [Required]
        [DataType(DataType.Text)]
        public string? Email { get; set; }

        [Display(Name = "登入密碼")]
        [Required(ErrorMessage = "登入密碼不可空白!!")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Display(Name = "確認密碼")]
        [Required(ErrorMessage = "確認密碼不可空白!!")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "與登入密碼輸入不相符!!")]
        public string? ConfirmPassword { get; set; }
    }
}

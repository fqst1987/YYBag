using System.ComponentModel.DataAnnotations;

namespace YYBagProgram.Models.ViewModel
{
    public class ForgetPasswordViewModel
    {
        [Display(Name = "註冊之Email")]
        [Required]
        public string? Email { get; set; }
    }
}

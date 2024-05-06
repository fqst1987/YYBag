using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YYBagProgram.Models.Token
{
    public class Tokens
    {
        [Display(Name = "令牌")]
        [Required]
        [Column(TypeName = "nvarchar(255)")]
        public string? Token {  get; set; }

        [Display(Name = "時效時間")]
        [Required]
        [Column(TypeName = "date")]
        public DateTime ExpirationTime { get; set; }
    }
}

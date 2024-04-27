using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using YYBagProgram.Enums;

namespace YYBagProgram.Models
{
    public class MemberRole
    {
        [DisplayName("會員帳號")]
        [Required]
        [Column(TypeName = "varchar(10)")]
        public string MemberId { get; set; } = string.Empty;

        [DisplayName("會員身分")]
        [Required]
        [Column(TypeName = "int")]
        public AllEnums.MemberRole Role { get; set; } = AllEnums.MemberRole.Client;
    }
}

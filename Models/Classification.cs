using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YYBagProgram.Models
{
    [Display(Name = "商品分類")]
    public class Classification
    {
        [Display(Name = "分類編號")]
        [Column(TypeName = "varchar(10)")]
        [Required]
        public string Id { get; set; }

        [Display(Name = "分類名稱")]
        [Column(TypeName = "nvarchar(50)")]
        [Required]
        public string Name { get; set; }
    }
}

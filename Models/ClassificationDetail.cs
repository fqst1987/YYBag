using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YYBagProgram.Models
{
    [Display(Name = "商品分類明細")]
    public class ClassificationDetail
    {
        [Display(Name = "分類編號")]
        [Column(TypeName = "varchar(10)")]
        [Required]
        public string Id { get; set; }

        [Display(Name = "商品編號")]
        [Column(TypeName = "varchar(8)")]
        [Required]
        public string strBagsId { get; set; }
    }
}

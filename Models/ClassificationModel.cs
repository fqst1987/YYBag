using System.ComponentModel.DataAnnotations;

namespace YYBagProgram.Models
{
    [Display(Name = "商品分類")]
    public class ClassificationModel
    {
        [Display(Name = "分類編號")]
        [Required]
        public string Id { get; set; }

        [Display(Name = "分類名稱")]
        [Required]
        public string Name { get; set; }
    }
}

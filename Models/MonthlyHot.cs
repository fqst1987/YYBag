using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YYBagProgram.Models
{
    [Display (Name="本月主打商品")]
    public class MonthlyHot
    {
        [Display(Name = "年")]
        [Column(TypeName = "int")]
        [Required]
        public int Year { get; set; }

        [Display(Name = "月")]
        [Column(TypeName = "int")]
        [Required]
        public int Month { get; set; }

        [Display(Name = "商品編號")]
        [Column(TypeName = "varchar(8)")]
        [Required]
        public string strBagsId { get; set; }

        [Display(Name = "上傳圖片")]
        [Column(TypeName = "varchar(max)")]
        public string? img { get; set; }
    }
}

using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YYBagProgram.Models
{
    [Display (Name="輪播設定")]
    public class CarouselSetting
    {
        [Display(Name = "商品編號")]
        [Column(TypeName = "varchar(8)")]
        [Required]
        public string strBagsId { get; set; }

        [Display(Name = "輪播照片")]
        [Column(TypeName = "varchar(max)")]
        [Required]
        public string imgurl { get; set; }
    }
}

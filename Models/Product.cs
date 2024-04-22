using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using YYBagProgram.Enums;

namespace YYBagProgram.Models
{
    [Display (Name ="商品")]
    public class Product
    {
        [DisplayName("編號")]
        [Column(TypeName = "varchar(8)")]
        [Required]
        public string strBagsId { get; set; } = string.Empty;

        [Required]
        [DisplayName("名稱")]
        [Column(TypeName = "nvarchar(50)")]
        public string strBagsName { get; set; }

        [Required]
        [DisplayName("類型")]
        public AllEnums.BagType BagType { get; set; }

        [Required]
        [DisplayName("敘述")]
        [Column(TypeName = "nvarchar(max)")]
        public string strDescription { get; set; }

        [Required]
        [DisplayName("長度")]
        [Column(TypeName = "decimal(6,2)")]
        public decimal dLength { get; set; }

        [Required]
        [DisplayName("寬度")]
        [Column(TypeName = "decimal(6,2)")]
        public decimal dWidth { get; set; }

        [Required]
        [DisplayName("高度")]
        [Column(TypeName = "decimal(6,2)")]
        public decimal dHigh { get; set; }

        [Required]
        [DisplayName("重量")]
        [Column(TypeName = "decimal(6,2)")]
        public decimal dWeight { get; set; }

        [Required]
        [DisplayName("材質")]
        [Column(TypeName = "nvarchar(10)")]
        public string strMaterial { get; set; }

        [Required]
        [DisplayName("價格")]
        [Column(TypeName = "int")]
        public int iPrice { get; set; }

        [Required]
        [DisplayName("縮圖照片")]
        [Column(TypeName = "varchar(max)")]
        public string strImageUrl { get; set; }

    }
}

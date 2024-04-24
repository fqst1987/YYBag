using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using YYBagProgram.Enums;

namespace YYBagProgram.Models
{
    public class ProductsColorDetail
    {
        [DisplayName("商品編號內碼")]
        [Required]
        [Column(TypeName = "varchar(20)")]
        public string strID { get; set; }

        [DisplayName("顏色")]
        [Column(TypeName = "nvarchar(10)")]
        public string strColor { get; set; }

        [DisplayName("所有數量")]
        [Required]
        [Column(TypeName = "int")]
        public int iTotal { get; set; }

        [DisplayName("剩餘數量")]
        [Required]
        [Column(TypeName = "int")]
        public int iRemain { get; set; }

        [DisplayName("價格")]
        [Required]
        [Column(TypeName = "int")]
        public int iPrice { get; set; }

        [DisplayName("上傳圖片")]
        [Column(TypeName = "varchar(max)")]
        [Required]
        public string Images {  get; set; }

        [DisplayName("出貨天數")]
        [Column(TypeName = "int")]
        [Required]
        public int iDeliveryDays { get; set; }

        [DisplayName("貨品狀態")]
        [Column(TypeName = "int")]
        [Required]
        public AllEnums.ProductStatus ProductStatus { get; set; }

        [DisplayName("是否有在賣")]
        [Column(TypeName = "boolean ")]
        [Required]
        public bool isOnline { get; set; }

    }
}

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using YYBagProgram.Enums;

namespace YYBagProgram.Models
{
    [DisplayName("訂單明細")]
    public class OrderDetail
    {
        [DisplayName("訂單編號")]
        [Required]
        [Key]
        [Column(TypeName = "varchar(20)")]
        public string strOrderGuid { get; set; }

        [DisplayName("商品編號")]
        [Required]
        [Column(TypeName = "varchar(8)")]
        public string strBagsId { get; set; }

        [DisplayName("最後價格")]
        [Required]
        [Column(TypeName = "int")]
        public int iFinalPrice { get; set; }

        [DisplayName("商品數量")]
        [Required]
        [Column(TypeName = "int")]
        public int iQuantity { get; set; }

        [DisplayName("訂單狀態")]
        [Required]
        [Column(TypeName = "int")]
        public AllEnums.OrderStatus iOrderStatus { get; set; }
    }
}

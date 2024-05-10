using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YYBagProgram.Models
{
    [DisplayName("訂單")]
    public class Order
    {
        [DisplayName("訂單編號")]
        [Required]
        [Column(TypeName ="varchar(10)")]
        public string strOrderId { get; set; }

        [DisplayName("會員編號")]
        [Required]
        [Column(TypeName = "varchar(10)")]
        public string strMemberId { get; set; }

        [DisplayName("收件人姓名")]
        [Required]
        [Column(TypeName = "nvarchar(10)")]
        public string strReceiver { get; set; } = string.Empty;

        [DisplayName("收件人電話")]
        [Required]
        [Column(TypeName = "varchar(15)")]
        public string strReceiverPhone { get; set; } = string.Empty;

        [DisplayName("收件人地址")]
        [Required]
        [Column(TypeName = "nvarchar(50)")]
        public string strReceiverAddress { get; set; } = string.Empty;

        [DisplayName("訂單日期")]
        [Required]
        [Column(TypeName = "date")]
        public DateTime OrderDate { get; set; }
    }
}

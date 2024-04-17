using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YYBagProgram.Models
{
    public class ProductColor
    {
        [DisplayName("商品編號")]
        [Required]
        [Column(TypeName = "varchar(20)")]
        public string strID { get; set; }

        //直接帶入
        [DisplayName("商品編號")]
        [Column(TypeName = "varchar(8)")]
        [Required]
        public string strBagsId { get; set; }

        [DisplayName("顏色")]
        [Column(TypeName = "nvarchar(10)")]
        [Required]
        public string strColor { get; set; }
    }
}

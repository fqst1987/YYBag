using System.ComponentModel.DataAnnotations;

namespace YYBagProgram.Enums
{
    public class AllEnums
    {
        public enum BagType
        {
            [Display(Name = "後背包")]
            Backpack = 1,
            [Display(Name = "肩背包")]
            ShoulderBag = 2,
            [Display(Name = "行李袋")]
            DuffelBag = 3,
            [Display(Name = "束口袋")]
            Drawstring = 4,
            [Display(Name = "水桶包")]
            BucketBag = 5,
            [Display(Name = "新月包")]
            HoboBag = 6,
            [Display(Name = "晚宴包")]
            Clutch = 7,
            [Display(Name = "雲朵包")]
            Cloud = 8,
            [Display(Name = "其他")]
            Others = 99,
        }

        public enum ProductStatus
        {
            [Display(Name = "現貨")]
            Available = 1,
            [Display(Name = "預購")]
            PreOrder = 2,
        }

        public enum Color
        {
            [Display(Name = "沒選擇")]
            Default = 0,
            [Display(Name = "米白色")]
            Offwhite = 1,
            [Display(Name = "白色")]
            White = 2,
            [Display(Name = "黑色")]
            Black = 3,
            [Display(Name = "黃色")]
            Yellow = 4,
            [Display(Name = "奶茶色")]
            Milktea = 5,
            [Display(Name = "棕色")]
            Brown = 6,
            [Display(Name = "綠色")]
            Green = 7,
            [Display(Name = "紅色")]
            Red = 8,
            [Display(Name = "粉紅色")]
            Pink = 9,
            [Display(Name = "藍色")]
            Blue = 10,
        }

        public enum OrderStatus
        {
            [Display(Name = "Default")]
            Default = 0,
            [Display(Name = "未成立")]
            NotEstablished = 1,
            [Display(Name = "已成立")]
            Established = 2,
            [Display(Name = "撿貨中")]
            Pickup = 3,
            [Display(Name = "已出貨")]
            Shipped = 4,
            [Display(Name = "配送中")]
            Delivering = 5,
            [Display(Name = "已送達")]
            Delivered = 6,
            [Display(Name = "已完成")]
            Completed = 7,
            [Display(Name = "退貨")]
            Return = 8,
        }
    }
}

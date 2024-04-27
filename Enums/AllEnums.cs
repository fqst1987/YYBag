using System.ComponentModel.DataAnnotations;

namespace YYBagProgram.Enums
{
    public class AllEnums
    {
        public enum BagType
        {
            [Display(Name = "請選擇包款")]
            None = 0,
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
            [Display(Name = "側背包")]
            Slide = 9,
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

        public enum MemberRole
        {
            [Display(Name = "請選擇身分")]
            Default = 0,
            [Display(Name = "一般顧客")]
            Client = 1,
            [Display(Name = "一般員工")]
            Staff = 2,
            [Display(Name = "管理者")]
            Supervisor = 3,
            [Display(Name = "工程人員")]
            Engineer = 4,
            [Display(Name = "完全功能使用者")]
            Admin = 9,
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

        //public enum MemberStatus
        //{
        //    [Display(Name = "未登入")]
        //    NoLogin = 0,
        //    [Display(Name = "已登入")]
        //    Login = 1,
        //    [Display(Name = "未驗證")]
        //    NoReview = 2,
        //    [Display(Name = "重設密碼token過期")]
        //    ResetPasswordTokenExpire = 3,
        //    [Display(Name = "驗證帳戶token過期")]
        //    ReviewTokenExpire = 4,
        //}
    }
}

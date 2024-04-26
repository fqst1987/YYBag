using System.ComponentModel;

namespace YYBagProgram.Models.ViewModel
{
    public class LoginPostViewModel
    {
        [DisplayName("會員帳號")]
        public string strMemberGuid { get; set; }

        [DisplayName("會員密碼")]
        public string strMemberPassWord { get; set; }
    }
}

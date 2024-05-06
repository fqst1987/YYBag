namespace YYBagProgram.Models.ViewModel
{
    public class MemberRoleViewModel
    {
        public IList<MemberRole>? MemberRoles { get; set; }

        public IList<Members>? Members { get; set; }

        public int? CurremtPage { get; set; }

        public int? TotalPage { get; set; }
    }
}

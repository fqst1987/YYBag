using System.Runtime.InteropServices;
using YYBagProgram.Models;

namespace YYBagProgram.Service.Interface
{
    public interface ICryptographyService 
    {
        //sha256密碼
        string HashPassword(string password);
        //確認密碼
        bool VerifyHashedPassword(string password, string hashedpassword);
    }

    public interface ISendEmailService
    {
        public Task<string> SendResetEmail(string email);

        public Task<string> SendReviewEmail(string email);
    }

    public interface IMemberService
    {
        public string GoogleApiClientId { get; }
        public string SecretKey { get; }
        public string GmailUserID { get; }
        public string GmailUserPassword { get; }

        public Task<bool> isMemberEmailExist(string email);

        public Task<bool> CheckMemberInfo(string account, string passowrd, [Optional] bool isgoogleaccount);

        public Task<bool> isMemberEnable(string account);

        public Task AddMember<T>(T model);

        public Task RemoveMember<T>(T model);

        public Task UpdateMember<T>(T member);

        public Task<IList<Members>> GetMmebers([Optional] string email);

        public Task<IList<MemberRole>> GetMmeberRoles([Optional ]string id);
    }

}

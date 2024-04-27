namespace YYBagProgram.Interface
{
   public interface IMemberScopeService
    {
        bool isAccountRegister(string? phone, string email);
        string HashPassword(string password);
        bool VerifyHashedPassword(string password, string hashedpassword);
    }

}

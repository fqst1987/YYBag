using System.Text.Json;

namespace YYBagProgram
{
    public class UserInfoService
    {

        private readonly IHttpContextAccessor _contextAccessor;

        public UserInfoService(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }
    }
}

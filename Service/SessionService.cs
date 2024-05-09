using YYBagProgram.Service.Interface;
using Newtonsoft.Json;

namespace YYBagProgram.Service
{
    public class SessionService : ISessionService
    {
        public void SetObjectAsJson(ISession session, string key,  object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public T GetObjectFromJson<T>(ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }

        public void Remove(ISession session, string key)
        {
            session.Remove(key);
        }
    }
}

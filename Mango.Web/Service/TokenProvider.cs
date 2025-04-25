using Mango.Web.Service.IService;
using Mango.Web.Utility;
using Newtonsoft.Json.Linq;

namespace Mango.Web.Service
{
    public class TokenProvider : ITokenProvider
    {
        private readonly IHttpContextAccessor _ContextAccessor;

        public TokenProvider(IHttpContextAccessor ContextAccessor)
        {
            _ContextAccessor = ContextAccessor;
        }

        public void ClearToken()
        {
            _ContextAccessor.HttpContext?.Response.Cookies.Delete(SD.TokenCokie);
        }

        public string? GetToken()
        {
            string? token = null;
            bool? hasToken = _ContextAccessor.HttpContext?.Request.Cookies.TryGetValue(SD.TokenCokie, out token);
            return hasToken is true ? token : null;

        }

        public void SetToken(string token)
        {
            _ContextAccessor.HttpContext?.Response.Cookies.Append(SD.TokenCokie, token);

        }
    }
}

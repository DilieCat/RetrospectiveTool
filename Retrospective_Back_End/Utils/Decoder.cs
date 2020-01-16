using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace Retrospective_Back_End.Utils
{
    public class Decoder : IDecoder
    {

        public string DecodeToken(string token)
        {
            if (token == null)
                return null; 

            var handler = new JwtSecurityTokenHandler();
            var tokenS = handler.ReadToken(token) as JwtSecurityToken;

            var id = tokenS.Claims.First(claim => claim.Type == "sub").Value;

            return id;
        }

    }
}

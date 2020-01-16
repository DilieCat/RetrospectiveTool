using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace Retrospective_Back_End.Utils
{
    public class Decoder : IDecoder
    {

        public string DecodeToken(string token)
        {
            if (token == null || token.Length == 0)
                return null; 

            var handler = new JwtSecurityTokenHandler();
            var tokenS = handler.ReadToken(token) as JwtSecurityToken;

            var id = tokenS.Claims.First(claim => claim.Type == "sub").Value;

            return id;
        }

    }
}

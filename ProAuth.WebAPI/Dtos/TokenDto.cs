using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProAuth.WebAPI.Dtos
{
    public class TokenDto
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
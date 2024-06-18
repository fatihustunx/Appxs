using App.Appxs.eSecurities.Usings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Features.Features.Auths.Auths
{
    public interface IAuthsFeatures
    {
        Task<AccessToken> RunToLogin(UserForLoginDto userForLoginDto);

        Task<AccessToken> RunToRegister(UserForRegisterDto userForRegisterDto);
    }

    public class UserForLoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class UserForRegisterDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}

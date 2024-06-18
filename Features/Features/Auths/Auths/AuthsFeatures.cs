using App.Appxs.Exceptions;
using Features.Features.Auths.Users;
using Microsoft.Extensions.Configuration;
using System;
using App.Appxs.eSecurities.Usings;

namespace Features.Features.Auths.Auths
{
    public class AuthsFeatures : IAuthsFeatures
    {
        private readonly IConfiguration _configuration;
        private readonly IUserFeatures _userFeatures;

        public AuthsFeatures(IUserFeatures userFeatures
            , IConfiguration configuration)
        {
            _userFeatures = userFeatures;
            _configuration = configuration;

            new JwtHelper(_configuration);
        }

        public async Task<AccessToken> RunToRegister(UserForRegisterDto userForRegisterDto)
        {
            var user = await Register(userForRegisterDto);

            var res = await CreateAccessToken(user);

            return res;
        }

        public async Task<AccessToken> RunToLogin(UserForLoginDto userForLoginDto)
        {
            var user = await Login(userForLoginDto);

            var res = await CreateAccessToken(user);

            return res;
        }

        private async Task<User> Register(UserForRegisterDto userForRegisterDto)
        {
            var isExists = await _userFeatures.isExists(userForRegisterDto.Email);

            if (isExists) { throw new BusinessException("Böyle bir user var."); }

            byte[] passwordSalt, passwordHash;
            HashingHelper.CreatePasswordHash(userForRegisterDto.Password, out passwordSalt, out passwordHash);
            var user = new User
            {
                Email = userForRegisterDto.Email,
                FirstName = userForRegisterDto.FirstName,
                LastName = userForRegisterDto.LastName,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Status = true
            };

            await _userFeatures.Add(user);

            return user;
        }

        private async Task<User> Login(UserForLoginDto userForLoginDto)
        {
            var user = await _userFeatures.GetByEmail(userForLoginDto.Email);

            if (!HashingHelper.VerifyPasswordHash
                (userForLoginDto.Password, user.PasswordSalt, user.PasswordHash))
            {
                throw new BusinessException("Girilen şifre yanlış.");
            }

            return user;
        }

        private async Task<AccessToken> CreateAccessToken(User user)
        {
            var claims = await _userFeatures.GetClaims(user);

            var accessToken = JwtHelper.CreateToken(user, claims);

            return accessToken;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Entities;
using API.Interfaces;
using API.Interfaces.Repository;
using API.Models;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    public class AccountServices : IAccountServices
    {
        private readonly IUserRepository _rep;

        private readonly SymmetricSecurityKey _key;

        private readonly IMapper _mapper;
        public AccountServices(IUserRepository rep, IConfiguration config, IMapper mapper)
        {
            _rep = rep;
            _mapper = mapper;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
        }
        public string CreateToken(AppUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString())
            };

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
        public async Task<bool> Login(AppUser user, LoginModel model)
        {
            using var hmac = new HMACSHA512(user.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(model.Password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) return false;
            }

            return await Task.FromResult(true);
        }
        public async Task<bool> Register(RegisterModel model)
        {
            using var hmac = new HMACSHA512();

            var user = _mapper.Map<AppUser>(model);

            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(model.Password));

            user.PasswordSalt = hmac.Key;

            return await _rep.Insert(user);
        }
        public async Task<AppUser> UserExists(string username)
        {
            return await _rep.GetUserByUsernameAsync(username.ToUpper());
        }
    }
}
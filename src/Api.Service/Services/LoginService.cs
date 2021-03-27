using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Api.Data.Dtos;
using Api.Domain.Entities;
using Api.Domain.Interfaces;
using Api.Domain.Interfaces.Services.User;
using Api.Domain.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Api.Service.Services
{
    public class LoginService : ILoginService
    {
        private IUserRepository _repository;
        private  SigningConfigurations _signingConfigurations;
        private  TokenConfigurations _tokenConfigurations;

        private IConfiguration _iconfiguration{get;}

        public LoginService(IUserRepository repository, SigningConfigurations signingConfigurations, TokenConfigurations tokenConfigurations, IConfiguration iconfiguration)
        {
            _repository = repository;
            _signingConfigurations = signingConfigurations;
            _tokenConfigurations = tokenConfigurations;
            _iconfiguration = iconfiguration;
        }
        public async Task<object> FindByLogin(LoginDto user)
        {
            var baseUser = new UserEntity();
            if(user != null && !string.IsNullOrWhiteSpace(user.Email))
            {           
                baseUser = await _repository.FindByLogin(user.Email);    
                if (baseUser == null)
                    return new {authenticated = false,
                        message = "Falha ao autenticar"
                    };
                else
                {
                    ClaimsIdentity identity =  new ClaimsIdentity(new GenericIdentity(baseUser.Email),
                    new[]
                    {
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.UniqueName, baseUser.Email),
                    });
                    DateTime createdate = DateTime.Now;
                    DateTime expirationDate = createdate + TimeSpan.FromSeconds(_tokenConfigurations.Seconds);

                    JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                    string token = CreateToken(identity, createdate, expirationDate, handler);
                    return SucessObject(createdate, expirationDate, token, user);
                }
            } 
            else
                return null;                     
        }

        private string CreateToken(ClaimsIdentity identity, DateTime createDate, DateTime expirationDate, JwtSecurityTokenHandler handler)
        {
            var securityToken = handler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = _tokenConfigurations.Issuer,
                Audience = _tokenConfigurations.Audience,
                SigningCredentials = _signingConfigurations.SigningCredentials,
                Subject = identity,
                NotBefore = createDate,
                Expires = expirationDate,
            });

            var token = handler.WriteToken(securityToken);
            return token;
        }

        private object SucessObject(DateTime createDate, DateTime expirationDate, string token, LoginDto user)
        {
            return new
            {
                authenticated = true,
                created = createDate.ToString("yyyy-MM-dd HH:mm:ss"),
                expirationDate = expirationDate.ToString("yyyy-MM-dd HH:mm:ss"),
                acessToken = token,
                userName = user.Email,
                message = "Usuário logado com sucesso"
            };
        }
    }
}

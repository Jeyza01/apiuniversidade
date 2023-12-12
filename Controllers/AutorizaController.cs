using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using apiUniversidade.Model;
using apiUniversidade.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Identity;
using apiUniversidade.DTO;
using ApiUniversidade.DTO;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;

namespace apiUniversidade.Controllers{

[ApiController]
[Route("[controller]")]
public class AutorizaController : Controller 
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly IConfiguration _configuration;

    public AutorizaController(UserManager<IdentityUser> userManager, 
            SignInManager<IdentityUser> signInManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
    }

    private UsuarioToken GeraToken(UsuarioDTO userInfo){

        var claims = new[]{
            new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.UniqueName,userInfo.Email),
            new Claim("IFRN", "TecInfo"),
            new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
        };

        //gerar chave através de um algoritmo de chave simética
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:key"]));
        
        //gerar a assinatura digital do token utilizando
        //a chave privada (key) e o algoritmo HMAC SHA 256
        var credentials = new SigningCredentials(key,SecurityAlgorithms.HmacSha256);

        //tempo de expiração do token
        var expiracao =_configuration["TokenConfiguration:ExpireHours"];
        var expiration = DateTime.UtcNow.AddHours(double.Parse(expiracao));

        JwtSecurityToken token = new JwtSecurityToken(
            issuer: _configuration["TokenConfiguration:Issuer"],
            audience: _configuration["TokenConfiguration:Audience"],
            claims: claims,
            expires: expiration,
            signingCredentials: credentials
        );

        return new UsuarioToken(){
            Authenticated = true,
            Expiration = expiration,
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Message = "JWT Ok."
        };
        
    }

    [HttpGet]
        public ActionResult<string> Get(){
            return "AutorizaController :: Acessado em : "
                + DateTime.Now.ToLongDateString();

        }
    
    [HttpPost("registrer")]
        public async Task<ActionResult> RegistrerUser([FromBody]UsuarioDTO model){
            var user = new IdentityUser{
                UserName = model.Email,
                Email = model.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if(!result.Succeeded)
                return BadRequest(result.Errors);

            await _signInManager.SignInAsync(user, false);
            //return Ok(GerarToken(model));
            return Ok(GeraToken(model));
        }

    [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] UsuarioDTO userInfo){

            var result = await _signInManager.PasswordSignInAsync(userInfo.Email, userInfo.Password, isPersistent: false, lockoutOnFailure: false);

            if(result.Succeeded)
                return Ok(GeraToken(userInfo));
            else{
                ModelState.AddModelError(string.Empty,"Login inválido...");
                return BadRequest(ModelState);
            }

        }
}
}
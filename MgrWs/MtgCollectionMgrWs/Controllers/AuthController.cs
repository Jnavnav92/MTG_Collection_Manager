using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MtgCollectionMgrWs.Providers;
using MtgCollectionMgrWs.SDK;
using Shared;
using Shared.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MtgCollectionMgrWs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IDataRepository _datarepo;

        public AuthController(IDataRepository datarepo)
        {
            _datarepo = datarepo;
        }

        [HttpPost]
        [Route("CreateAccount")]
        public async Task<IActionResult> CreateAccount([FromBody] AccountModel createAccount)
        {
            string sResponseJson = string.Empty;
            bool bSuccessful = false;

            try
            {
                string sConnectionString = await GetConnectionStringAsync();

                AccountSDK acctSDK = new AccountSDK(sConnectionString);

                string sCreateAccountResult = await acctSDK.CreateAccountAsync(createAccount);

                bSuccessful = true;
                sResponseJson = ResponseFormatter.FormatResponse(bSuccessful, sCreateAccountResult);
            }
            catch (Exception ex)
            {
                //log full stack trace somewhere

                bSuccessful = false;
                sResponseJson = ResponseFormatter.FormatResponse(bSuccessful, ex.Message);
            }

            if (bSuccessful == true)
            {
                return Ok(sResponseJson);
            }
            else
            {
                return BadRequest(sResponseJson);
            }
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] AccountModel loginAccount)
        {
            string sResponseJson = string.Empty;
            bool bSuccessful = false;

            try
            {
                string sConnectionString = await GetConnectionStringAsync();

                AccountSDK acctSDK = new AccountSDK(sConnectionString);

                (bool bSuccessfulLogin, string sMessage) result = await acctSDK.LoginAccountAsync(loginAccount);

                if (result.bSuccessfulLogin == false)
                {
                    bSuccessful = false;
                    //no account found for this login, prompt caller to create an account.
                    sResponseJson = ResponseFormatter.FormatResponse(bSuccessful, result.sMessage);
                }
                else
                {
                    bSuccessful = true;

                    //successful login, generate JWT token for UI to utilize.
                    string JwtToken = await GenerateJwtTokenAsync(loginAccount.EmailAddress);

                    sResponseJson = ResponseFormatter.FormatResponse(bSuccessful, result.sMessage, JwtToken);
                }
            }
            catch (Exception ex)
            {
                //log full stack trace somewhere

                bSuccessful = false;

                sResponseJson = ResponseFormatter.FormatResponse(bSuccessful, ex.Message);
            }

            if (bSuccessful == true)
            {
                return Ok(sResponseJson);
            }
            else
            {
                return BadRequest(sResponseJson);
            }
        }

        private async Task<string> GenerateJwtTokenAsync(string username)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_datarepo.GetConnectionString()));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "MtgCollectionMgr",
                audience: "MtgCollectionMgr",
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return await Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
        }



        private async Task<string> GetConnectionStringAsync()
        {
            string sConnectionString = _datarepo.GetConnectionString();

            if (string.IsNullOrEmpty(sConnectionString) == true)
            {
                throw new Exception("Unable to create account, CA_ER1");
            }

            return await Task.FromResult(sConnectionString);
        }
    }
}

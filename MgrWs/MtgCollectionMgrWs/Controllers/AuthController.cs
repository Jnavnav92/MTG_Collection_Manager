using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MtgCollectionMgrWs.Providers;
using Newtonsoft.Json.Linq;
using SDK.Classes;
using SDK.Models;
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
        public async Task<IActionResult> CreateAccount([FromBody] AccountModelUserCredentialsModel createAccount)
        {
            string sResponseJson = string.Empty;
            bool bSuccessful = false;

            try
            {
                string sConnectionString = await GetConnectionStringAsync();

                AccountSDK acctSDK = new AccountSDK()
                {
                    connString = sConnectionString,
                    smtpEmail = _datarepo.GetSMTPEmail(),
                    smtpPassword = _datarepo.GetSMTPPassword()
                };

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
        public async Task<IActionResult> Login([FromBody] AccountModelUserCredentialsModel loginAccount)
        {
            string sResponseJson = string.Empty;
            bool bSuccessful = false;

            try
            {
                string sConnectionString = await GetConnectionStringAsync();

                AccountSDK acctSDK = new AccountSDK()
                {
                    connString = sConnectionString
                };

                SDK_Auth_Return_Model result = await acctSDK.LoginAccountAsync(loginAccount);

                if (result.bSuccess == false)
                {
                    bSuccessful = false;
                    //no account found for this login, prompt caller to create an account.
                    sResponseJson = ResponseFormatter.FormatResponse(bSuccessful, result.sMessage!);
                }
                else
                {
                    bSuccessful = true;

                    //successful login for a verified account, generate JWT token for UI to utilize.
                    string JwtToken = await JWTHelper.GenerateJwtTokenAsync(loginAccount.EmailAddress, _datarepo.GetJwtSecret());

                    sResponseJson = ResponseFormatter.FormatResponse(bSuccessful, result.sMessage!, JwtToken);
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


        [HttpPost]
        [Route("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] BaseAccountModel forgotPasswordAccount)
        {
            string sResponseJson = string.Empty;
            bool bSuccessful = false;

            try
            {
                string sConnectionString = await GetConnectionStringAsync();

                AccountSDK acctSDK = new AccountSDK()
                { 
                    connString = sConnectionString,
                    smtpEmail = _datarepo.GetSMTPEmail(),
                    smtpPassword = _datarepo.GetSMTPPassword()
                };

                SDK_Auth_Return_Model result = await acctSDK.ForgotPasswordAsync(forgotPasswordAccount);

                bSuccessful = result.bSuccess;

                sResponseJson = ResponseFormatter.FormatResponse(bSuccessful, result.sMessage!);
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
        [Route("ResetPassword")]
        //[Authorize]
        public async Task<IActionResult> ResetPassword([FromBody] AccountModelUserCredentialsModel resetPasswordAccount)
        {
            string sResponseJson = string.Empty;
            bool bSuccessful = false;

            try
            {
                string sConnectionString = await GetConnectionStringAsync();

                AccountSDK acctSDK = new AccountSDK()
                {
                    connString = sConnectionString
                };

                SDK_Auth_Return_Model result = await acctSDK.ResetPasswordAsync(resetPasswordAccount);

                bSuccessful = result.bSuccess;

                sResponseJson = ResponseFormatter.FormatResponse(bSuccessful, result.sMessage!);
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
        [Route("ResendVerificationEmail")]
        public async Task<IActionResult> ResendVerificationEmailAsync([FromBody] BaseAccountModel verifyModel)
        {
            string sResponseJson = string.Empty;
            bool bSuccessful = false;

            try
            {
                string sConnectionString = await GetConnectionStringAsync();

                AccountSDK acctSDK = new AccountSDK()
                {
                    connString = sConnectionString,
                    smtpEmail = _datarepo.GetSMTPEmail(),
                    smtpPassword = _datarepo.GetSMTPPassword()
                };

                SDK_Auth_Return_Model result = await acctSDK.ResendVerificationEmailAsync(verifyModel);

                bSuccessful = result.bSuccess;

                sResponseJson = ResponseFormatter.FormatResponse(bSuccessful, result.sMessage!);
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
        [Route("VerifyAccount")]
        public async Task<IActionResult> VerifyAccountAsync([FromBody] VerifyEmailAuthModel verifyModel)
        {
            string sResponseJson = string.Empty;
            bool bSuccessful = false;

            try
            {
                string sConnectionString = await GetConnectionStringAsync();

                AccountSDK acctSDK = new AccountSDK()
                {
                    connString = sConnectionString
                };

                SDK_Auth_Return_Model result = await acctSDK.VerifyAccountAsync(verifyModel);

                bSuccessful = result.bSuccess;

                sResponseJson = ResponseFormatter.FormatResponse(bSuccessful, result.sMessage!);
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

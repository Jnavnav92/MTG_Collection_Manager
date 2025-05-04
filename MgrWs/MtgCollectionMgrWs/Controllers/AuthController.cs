using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MtgCollectionMgrWs.Providers;
using MtgCollectionMgrWs.SDK;
using Shared;
using Shared.Models;

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
        public async Task<string> CreateAccountAsync([FromBody] AccountModel createAccount)
        {
            string sResponseJson = string.Empty;

            try
            {
                string sConnectionString = _datarepo.GetConnectionString();

                if (string.IsNullOrEmpty(sConnectionString) == true)
                {
                    throw new Exception("Unable to create account, CA_ER1");
                }

                AccountSDK acctSDK = new AccountSDK(sConnectionString);

                string sCreateAccountResult = await acctSDK.CreateAccountAsync(createAccount);

                sResponseJson = ResponseFormatter.FormatResponse(true, sCreateAccountResult);
            }
            catch (Exception ex)
            {
                //log full stack trace somewhere

                sResponseJson = ResponseFormatter.FormatResponse(false, ex.Message);
            }

            return sResponseJson;
        }
    }
}

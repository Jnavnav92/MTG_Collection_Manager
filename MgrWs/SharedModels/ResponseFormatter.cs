using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft;

namespace Shared
{
    public static class ResponseFormatter
    {
        public static string FormatResponse(bool bSuccess, string Result, string JwtToken = "")
        {
            ControllerResponseModel responseModel;

            if (string.IsNullOrEmpty(JwtToken) == false)
            {
                responseModel = new ControllerResponseModelExtended()
                {
                    Success = bSuccess,
                    Message = Result,
                    JWTToken = JwtToken
                };
            }
            else
            {
                responseModel = new ControllerResponseModel()
                {
                    Success = bSuccess,
                    Message = Result
                };
            }

            return Newtonsoft.Json.JsonConvert.SerializeObject(responseModel);
        }
    }
}

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
        public static string FormatResponse(bool bSuccess, string Result)
        {
            ControllerResponseModel responseModel = new ControllerResponseModel()
            {
                Success = bSuccess,
                Message = Result
            };

            return Newtonsoft.Json.JsonConvert.SerializeObject(responseModel);
        }
    }
}

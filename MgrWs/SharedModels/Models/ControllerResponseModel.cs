using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models
{
    public class ControllerResponseModelExtended : ControllerResponseModel
    {
        public required string JWTToken { get; set; }
    }

    public class ControllerResponseModel
    {
        public bool Success { get; set; }
        public required string Message { get; set; }
    }
}

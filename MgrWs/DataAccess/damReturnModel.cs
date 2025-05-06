using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class damReturnModel
    {
        public string? QueryMessage { get; set; }
        public bool QueryResult { get; set; }

        public string? EmailAddress { get; set; }
        public string? PasswordHash { get; set; }

        public Guid? AuthorizationToken { get; set; }
    }
}

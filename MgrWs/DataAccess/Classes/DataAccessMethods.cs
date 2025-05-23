using DataAccess.Data;
using DataAccess.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Macs;
using Shared.Models;
using Shared.Statics;
using System.Collections.Generic;
using System.Data;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Transactions;

namespace DataAccess.Classes
{
    public class DataAccessMethods
    {
        public string connectionString { get; set; } = string.Empty;

        public DataAccessMethods()
        {

        }
    }
}

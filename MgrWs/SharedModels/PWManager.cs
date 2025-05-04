using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCrypt;

namespace Shared
{
    public static class PWManager
    {
        public static async Task<string> HashPasswordAsync(string sPassword)
        {
            string sPWHash = BCrypt.Net.BCrypt.EnhancedHashPassword(sPassword, 15);

            return await Task.FromResult(sPWHash);
        }
    }
}

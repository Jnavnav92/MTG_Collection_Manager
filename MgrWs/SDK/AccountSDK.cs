using Shared.Models;
using DataAccess;
using Microsoft.Data.SqlClient;
using Shared;

namespace MtgCollectionMgrWs.SDK
{
    public class AccountSDK
    {
        private string connString; 
        public AccountSDK(string sConnectionString)
        {
            connString = sConnectionString;
        }

        public async Task<string> CreateAccountAsync(AccountModel account)
        {
            //UI sends plaintext password, we will hash it via Bcrypt and save the hash.

            string sHashedPW = await PWManager.HashPasswordAsync(account.UserPW);

            DataAccessMethods dam = new DataAccessMethods(connString);

            (bool bQueryResult, string sQueryMessage) result = await dam.CreateAccountAsync(account.EmailAddress, sHashedPW);

            if (result.bQueryResult == false)
            {
                if (result.sQueryMessage.Contains("Email Already Exists"))
                {
                    throw new Exception("An account for this user already exists, please use forgot password function to retrieve it.");
                }
                else
                {
                    throw new Exception("Unable to create account, CA_ER1");
                }
            }

            return result.sQueryMessage;
        }
    }
}

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

            damReturnModel result = await dam.CreateAccountAsync(account.EmailAddress, sHashedPW);

            if (result.QueryResult == false)
            {
                if (result.QueryMessage!.Contains("Email Already Exists") == false)
                {
                    throw new Exception("Unable to create account, CA_ER1");
                }
            }

            return result.QueryMessage!;
        }

        public async Task<(bool bSuccessfulLogin, string sMessage)> LoginAccountAsync(AccountModel account)
        {
            //UI sends plaintext password, we store bcrypt hash for the user in the database.

            DataAccessMethods dam = new DataAccessMethods(connString);

            damReturnModel damResult = await dam.GetLoginRecordAsync(account.EmailAddress);

            if (damResult.QueryResult == false)
            {
                if (damResult.QueryMessage!.Contains("No Login Record") == true)
                {
                    //login doesn't exist

                    return (false, $"No account found for this user: {account.EmailAddress}, please create an account.");
                }
                else
                {
                    throw new Exception("Unable to login, CA_ER2");
                }
            }
            else
            {
                //login exists, we will compare the bcrypt hash

                if (BCrypt.Net.BCrypt.EnhancedVerify(account.UserPW, damResult.PasswordHash) == true)
                {
                    //password is valid
                    return (true, "Successful Login");
                }
                else
                {
                    throw new Exception("Login Credentials are invalid.");
                }
            }

        }
    }
}

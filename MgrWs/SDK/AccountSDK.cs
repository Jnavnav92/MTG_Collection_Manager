using Shared.Models;
using DataAccess;
using Microsoft.Data.SqlClient;
using Shared;
using SDK;

namespace MtgCollectionMgrWs.SDK
{
    public class AccountSDK
    {
        public string connString { get; set; } = string.Empty;

        public string smtpEmail { get; set; } = string.Empty;
        public string smtpPassword { get; set; } = string.Empty;
        public string smtpCallbackURL { get; set; } = string.Empty;

        public AccountSDK()
        {

        }

        public async Task<string> CreateAccountAsync(AccountModelUserCredentialsModel account)
        {
            //UI sends plaintext password, we will hash it via Bcrypt and save the hash.

            string sHashedPW = await PWManager.HashPasswordAsync(account.UserPW);

            DataAccessMethods dam = new DataAccessMethods()
            {
                connectionString = connString
            };

            damReturnModel result = await dam.CreateAccountAsync(account, sHashedPW);

            if (result.QueryResult == false)
            {
                if (result.QueryMessage!.Contains("Email Already Exists") == false)
                {
                    throw new Exception("Unable to create account, CA_ER1");
                }
            }
            else
            {
                //if we succeed in creating the account, we now need to send the verification email.
                await SendVerificationEmailAsync(account, (Guid)result.AuthorizationToken!);
            }

            return result.QueryMessage!;
        }

        public async Task<SDK_Auth_Return_Model> ForgotPasswordAsync(BaseAccountModel account)
        {
            SDK_Auth_Return_Model returnModel = new SDK_Auth_Return_Model();

            //all we do is send forgot password email. callback URl will reset password for user.
            await SMTPHelper.SendEmailForgotPasswordAsync(account, smtpEmail, smtpPassword, smtpCallbackURL);

            returnModel.bSuccess = true;
            returnModel.sMessage = "Forgot Email sent successfully!";

            return returnModel;
        }

        public async Task<SDK_Auth_Return_Model> ResetPasswordAsync(AccountModelUserCredentialsModel account)
        {
            //UI sends plaintext password, we store bcrypt hash for the user in the database.

            SDK_Auth_Return_Model returnModel = new SDK_Auth_Return_Model();

            string sHashedPW = await PWManager.HashPasswordAsync(account.UserPW);

            DataAccessMethods dam = new DataAccessMethods()
            {
                connectionString = connString
            };

            damReturnModel result = await dam.UpdatePasswordAsync(account, sHashedPW);

            if (result.QueryResult == false)
            {
                if (result.QueryMessage!.Contains("Email Does Not Exist") == true)
                {
                    //login doesn't exist

                    returnModel.bSuccess = false;
                    returnModel.sMessage = $"No account found for this user: {account.EmailAddress}, please create an account.";

                    return returnModel;
                }
                else
                {
                    throw new Exception("Unable to reset password, CA_ER4");
                }
            }
            else
            {
                //successfully reset password

                returnModel.bSuccess = true;
                returnModel.sMessage = $"Sucessfully reset password for user: {account.EmailAddress}!";

                return returnModel;
            }
        }

       
        public async Task<SDK_Auth_Return_Model> LoginAccountAsync(AccountModelUserCredentialsModel account)
        {
            //UI sends plaintext password, we store bcrypt hash for the user in the database.

            SDK_Auth_Return_Model returnModel = new SDK_Auth_Return_Model();

            DataAccessMethods dam = new DataAccessMethods()
            {
                connectionString = connString
            };

            damReturnModel damResult = await dam.GetLoginRecordAsync(account.EmailAddress);

            if (damResult.QueryResult == false)
            {
                if (damResult.QueryMessage!.Contains("No Login Record") == true)
                {
                    //login doesn't exist

                    returnModel.bSuccess = false;
                    returnModel.sMessage = $"No account found for this user: {account.EmailAddress}, please create an account.";

                    return returnModel;
                }
                else if (damResult.QueryMessage!.Contains("Unverified") == true)
                {
                    //account not verified, deny login

                    returnModel.bSuccess = false;
                    returnModel.sMessage = $"Account for user: {account.EmailAddress} has not been verified. Please check your email, verify, and try again.";

                    return returnModel;
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
                    returnModel.bSuccess = true;
                    returnModel.sMessage = "Successful Login";

                    //password is valid
                    return returnModel;
                }
                else
                {
                    throw new Exception("Login Credentials are invalid.");
                }
            }
        }

        public async Task<SDK_Auth_Return_Model> VerifyAccountAsync(VerifyEmailAuthModel verifyModel)
        {
            //UI sends plaintext password, we store bcrypt hash for the user in the database.

            SDK_Auth_Return_Model returnModel = new SDK_Auth_Return_Model();

            DataAccessMethods dam = new DataAccessMethods()
            {
                connectionString = connString
            };

            damReturnModel damResult = await dam.VerifyAccountAsync((Guid)verifyModel.AuthorizationToken!);

            if (damResult.QueryResult == false)
            {
                if (damResult.QueryMessage!.Contains("No Login Record associated with Auth Token") == true)
                {
                    //login doesn't exist

                    returnModel.bSuccess = false;
                    returnModel.sMessage = $"No account found.";

                    return returnModel;
                }
                else
                {
                    throw new Exception("Unable to login, CA_VF1");
                }
            }
            else
            {
                returnModel.bSuccess = true;
                returnModel.sMessage = "Successfully Verified Account.";

            }

            return returnModel;
        }

        public async Task<SDK_Auth_Return_Model> ResendVerificationEmailAsync(BaseAccountModel resendVerifyModel)
        {
            SDK_Auth_Return_Model returnModel = new SDK_Auth_Return_Model();

            DataAccessMethods dam = new DataAccessMethods()
            {
                connectionString = connString
            };

            damReturnModel damResult = await dam.GetAuthTokenForReVerify(resendVerifyModel.EmailAddress);

            if (damResult.QueryResult == true)
            {
                await SendVerificationEmailAsync(resendVerifyModel, (Guid)damResult.AuthorizationToken!);

                returnModel.bSuccess = true;
                returnModel.sMessage = "Re-Sent Verification Email.";
            }
            else
            {
                returnModel.bSuccess = false;
                returnModel.sMessage = damResult.QueryMessage;
            }

                return returnModel;
        }

        private async Task SendVerificationEmailAsync(BaseAccountModel account, Guid AuthorizationToken)
        {
            await SMTPHelper.SendEmailVerificationCodeAsync(account, AuthorizationToken, smtpEmail, smtpPassword, smtpCallbackURL);
        }
    }
}

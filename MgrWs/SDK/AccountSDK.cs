using Shared.Models;
using DataAccess;
using Microsoft.Data.SqlClient;
using Shared;
using SDK;
using DataAccess.Models;
using Shared.Statics;

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

            damReturnModel result = await dam.CreateAccountAsync(account, sHashedPW, null);

            if (result.QueryResult == false)
            {
                if (result.QueryMessage!.Contains(StaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_EXISTS) == false)
                {
                    throw new Exception(StaticStrings.DATAACCESS_LOGIN_ERROR_CA_ER1);
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
            returnModel.sMessage = StaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_FORGOT_EMAIL_SENT_SUCCESS_MESSAGE;

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
                if (result.QueryMessage!.Contains(StaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_UPDATE_PASSWORD_FAILURE) == true)
                {
                    //login doesn't exist

                    returnModel.bSuccess = false;
                    returnModel.sMessage = StaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_NOACCOUNT_MESSAGE.Replace(StaticStrings.DATAACCESS_TOKEN_EMAIL_ADDRESS, account.EmailAddress);

                    return returnModel;
                }
                else
                {
                    throw new Exception(StaticStrings.DATAACCESS_RESET_PASSWORD_ERROR_CA_ER4);
                }
            }
            else
            {
                //successfully reset password

                returnModel.bSuccess = true;
                returnModel.sMessage = StaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_RESET_PASSWORD_SUCCESS_MESSAGE.Replace(StaticStrings.DATAACCESS_TOKEN_EMAIL_ADDRESS, account.EmailAddress);

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
                if (damResult.QueryMessage!.Contains(StaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_RETRIEVE_FAILURE) == true)
                {
                    //login doesn't exist

                    returnModel.bSuccess = false;
                    returnModel.sMessage = StaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_NOACCOUNT_MESSAGE.Replace(StaticStrings.DATAACCESS_TOKEN_EMAIL_ADDRESS, account.EmailAddress);

                    return returnModel;
                }
                else if (damResult.QueryMessage!.Contains(StaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_UNVERIFIED) == true)
                {
                    //account not verified, deny login

                    returnModel.bSuccess = false;
                    returnModel.sMessage = StaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_UNVERIFIED_MESSAGE.Replace(StaticStrings.DATAACCESS_TOKEN_EMAIL_ADDRESS, account.EmailAddress);

                    return returnModel;
                }
                else
                {
                    throw new Exception(StaticStrings.DATAACCESS_LOGIN_ERROR_CA_ER2);
                }
            }
            else
            {
                //login exists, we will compare the bcrypt hash

                if (BCrypt.Net.BCrypt.EnhancedVerify(account.UserPW, damResult.PasswordHash) == true)
                {
                    returnModel.bSuccess = true;
                    returnModel.sMessage = StaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_LOGIN_SUCCESS_MESSAGE;

                    //password is valid
                    return returnModel;
                }
                else
                {
                    throw new Exception(StaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_LOGIN_FAILURE_MESSAGE);
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
                if (damResult.QueryMessage!.Contains(StaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_NO_MATCH_AUTH_TOKEN) == true)
                {
                    //login doesn't exist

                    returnModel.bSuccess = false;
                    returnModel.sMessage = StaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_NO_ACCOUNT_FOUND_MESSAGE;

                    return returnModel;
                }
                else
                {
                    throw new Exception(StaticStrings.DATAACCESS_VERIFY_ERROR_CA_VF1);
                }
            }
            else
            {
                returnModel.bSuccess = true;
                returnModel.sMessage = StaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_VERIFIED_ACCOUNT_MESSAGE;

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
                returnModel.sMessage = StaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_VERIFIED_RE_SEND_EMAIL_MESSAGE;
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

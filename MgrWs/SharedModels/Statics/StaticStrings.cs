using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Statics
{
    public static class StaticStrings
    {
        public const string DATAACCESS_RESPONSEQUERY_RESULT_CREATED_ACCOUNT = "Successfully Created Account";
        public const string DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_EXISTS = "Email Already Exists";

        public const string DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_DELETED = "Successfully Deleted Account";
        public const string DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_DOES_NOT_EXIST = "Email does not exist and cannot be deleted.";

        public const string DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_UPDATE_PASSWORD_SUCCESS = "Successfully Updated Password!";
        public const string DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_UPDATE_PASSWORD_FAILURE = "Email Does Not Exist";

        public const string DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_UNVERIFIED = "Unverified";
        public const string DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_RETRIEVE_SUCCESS = "Retrieved Login Record";
        public const string DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_RETRIEVE_FAILURE = "No Login Record";

        public const string DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_NOACCOUNT_MESSAGE = "No account found for this user: #EMAIL_ADDRESS#, please create an account.";
        public const string DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_UNVERIFIED_MESSAGE = $"Account for user: #EMAIL_ADDRESS# has not been verified. Please check your email, verify, and try again.";
        public const string DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_RESET_PASSWORD_SUCCESS_MESSAGE = $"Sucessfully reset password for user: #EMAIL_ADDRESS#!";


        public const string DATAACCESS_TOKEN_EMAIL_ADDRESS = "#EMAIL_ADDRESS#";

        public const string DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_LOGIN_SUCCESS_MESSAGE = "Successful Login";
        public const string DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_LOGIN_FAILURE_MESSAGE = "Login Credentials are invalid.";

        public const string DATAACCESS_RESPONSEQUERY_RESLT_ACCOUNT_VERIFY_SUCCESS = "Successfully Verified";
        public const string DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_ALREADY_VERIFIED = "Already Verified";
        public const string DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_NO_MATCH_AUTH_TOKEN = "No Login Record associated with Auth Token";

        public const string DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_NO_ACCOUNT_FOUND_MESSAGE = $"No account found.";
        public const string DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_VERIFIED_ACCOUNT_MESSAGE = "Successfully Verified Account.";
        public const string DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_VERIFIED_RE_SEND_EMAIL_MESSAGE = "Re-Sent Verification Email.";

        public const string DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_FORGOT_EMAIL_SENT_SUCCESS_MESSAGE = "Forgot Email sent successfully!";

        public const string DATAACCESS_LOGIN_ERROR_CA_ER1 = "Unable to create account, CA_ER1";
        public const string DATAACCESS_LOGIN_ERROR_CA_ER2 = "Unable to login, CA_ER2";
        public const string DATAACCESS_VERIFY_ERROR_CA_VF1 = "Unable to login, CA_VF1";
        public const string DATAACCESS_RESET_PASSWORD_ERROR_CA_ER4 = "Unable to reset password, CA_ER4";
    }
}

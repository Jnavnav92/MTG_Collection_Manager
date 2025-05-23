using DataAccess.Data;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Shared.Models;
using Shared.Statics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Classes
{
    public class AuthDataAccessMethods : DataAccessMethods
    {
        public AuthDataAccessMethods() : base()
        {

        }

        public async Task<damReturnModel> UpdatePasswordAsync(BaseAccountModel account, string sPWHash)
        {
            damReturnModel dam = new damReturnModel();

            using (CollectionMgrContext db = new CollectionMgrContext(connectionString))
            {
                if (await db.AcctAccounts.AnyAsync(x => x.EmailAddress == account.EmailAddress) == true)
                {
                    AcctAccount acctAccount = await db.AcctAccounts.FirstAsync(x => x.EmailAddress == account.EmailAddress);

                    acctAccount.Pwhash = sPWHash;
                    await db.SaveChangesAsync();

                    dam.QueryResult = true;
                    dam.QueryMessage = AuthStaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_UPDATE_PASSWORD_SUCCESS;
                }
                else
                {
                    dam.QueryResult = false;
                    dam.QueryMessage = AuthStaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_UPDATE_PASSWORD_FAILURE;
                }
            }

            return dam;
        }

        public async Task<damReturnModel> CreateAccountAsync(BaseAccountModel account, string sPWHash, Guid? AuthTokenForTesting)
        {
            damReturnModel dam = new damReturnModel();

            using (CollectionMgrContext db = new CollectionMgrContext(connectionString))
            {
                if (await db.AcctAccounts.AnyAsync(x => x.EmailAddress == account.EmailAddress) == true)
                {
                    dam.QueryResult = false;
                    dam.QueryMessage = AuthStaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_EXISTS;
                }
                else
                {
                    Guid gAuthToken;

                    if (AuthTokenForTesting != Guid.Empty)
                    {
                        //unit test call, assign GUID
                        gAuthToken = (Guid)AuthTokenForTesting!;
                    }
                    else
                    {
                        //real call, generate new guid
                        gAuthToken = Guid.NewGuid();
                    }

                    await db.AddAsync(new AcctAccount { AccountId = Guid.NewGuid(), EmailAddress = account.EmailAddress, Pwhash = sPWHash, AuthorizationToken = gAuthToken });
                    await db.SaveChangesAsync();

                    dam.QueryResult = true;
                    dam.QueryMessage = AuthStaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_CREATED_ACCOUNT;
                    dam.AuthorizationToken = gAuthToken;
                }
            }

            return dam;
        }

        public async Task<damReturnModel> DeleteAccountAsync(BaseAccountModel account)
        {
            damReturnModel dam = new damReturnModel();

            using (CollectionMgrContext db = new CollectionMgrContext(connectionString))
            {
                AcctAccount? AccountToRemove = await db.AcctAccounts.FirstOrDefaultAsync(x => x.EmailAddress == account.EmailAddress);

                if (AccountToRemove != null)
                {
                    db.Remove(AccountToRemove);
                    await db.SaveChangesAsync();

                    dam.QueryResult = true;
                    dam.QueryMessage = AuthStaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_DELETED;
                }
                else
                {
                    dam.QueryResult = false;
                    dam.QueryMessage = AuthStaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_DOES_NOT_EXIST;
                }
            }

            return dam;
        }

        public async Task<damReturnModel> GetLoginRecordAsync(string sEmailAddress)
        {
            damReturnModel dam = new damReturnModel();

            using (CollectionMgrContext db = new CollectionMgrContext(connectionString))
            {
                if (await db.AcctAccounts.AnyAsync(x => x.EmailAddress == sEmailAddress) == true)
                {
                    AcctAccount account = await db.AcctAccounts.FirstAsync(x => x.EmailAddress == sEmailAddress);

                    dam.EmailAddress = account.EmailAddress;
                    dam.PasswordHash = account.Pwhash;

                    if (account.AccountVerified == false)
                    {
                        dam.QueryResult = false;
                        dam.QueryMessage = AuthStaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_UNVERIFIED;
                    }
                    else
                    {
                        dam.QueryResult = true;
                        dam.QueryMessage = AuthStaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_RETRIEVE_SUCCESS;
                    }
                }
                else
                {
                    dam.QueryResult = false;
                    dam.QueryMessage = AuthStaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_RETRIEVE_FAILURE;
                }
            }

            return dam;
        }

        public async Task<damReturnModel> GetAuthTokenForReVerify(string sEmailAddress)
        {
            damReturnModel dam = new damReturnModel();

            using (CollectionMgrContext db = new CollectionMgrContext(connectionString))
            {
                if (await db.AcctAccounts.AnyAsync(x => x.EmailAddress == sEmailAddress) == true)
                {
                    AcctAccount account = await db.AcctAccounts.FirstAsync(x => x.EmailAddress == sEmailAddress);

                    if (account.AccountVerified == false)
                    {
                        dam.AuthorizationToken = account.AuthorizationToken;

                        dam.QueryResult = true;
                    }
                    else
                    {
                        dam.QueryResult = true;
                        dam.QueryMessage = AuthStaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_ALREADY_VERIFIED;
                    }
                }
                else
                {
                    dam.QueryResult = false;
                    dam.QueryMessage = AuthStaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_NO_MATCH_AUTH_TOKEN;
                }
            }

            return dam;
        }

        public async Task<damReturnModel> VerifyAccountAsync(Guid AuthToken)
        {
            damReturnModel dam = new damReturnModel();

            using (CollectionMgrContext db = new CollectionMgrContext(connectionString))
            {
                if (await db.AcctAccounts.AnyAsync(x => x.AuthorizationToken == AuthToken) == true)
                {
                    AcctAccount account = await db.AcctAccounts.FirstAsync(x => x.AuthorizationToken == AuthToken);

                    account.AccountVerified = true;

                    await db.SaveChangesAsync();

                    dam.QueryResult = true;
                    dam.QueryMessage = AuthStaticStrings.DATAACCESS_RESPONSEQUERY_RESLT_ACCOUNT_VERIFY_SUCCESS;
                }
                else
                {
                    dam.QueryResult = false;
                    dam.QueryMessage = AuthStaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_NO_MATCH_AUTH_TOKEN;
                }
            }

            return dam;
        }
    }
}

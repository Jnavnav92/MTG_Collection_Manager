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

namespace DataAccess
{
    public class DataAccessMethods
    {
        public string connectionString { get; set; } = string.Empty;

        public DataAccessMethods()
        {

        }

        public async Task<damReturnModel> UpdatePasswordAsync(BaseAccountModel account, string sPWHash)
        {
            damReturnModel dam = new damReturnModel();

            using (CollectMgrContext db = new CollectMgrContext(connectionString))
            {
                if (await db.AcctAccounts.AnyAsync(x => x.EmailAddress == account.EmailAddress) == true)
                {
                    AcctAccount acctAccount = await db.AcctAccounts.FirstAsync(x => x.EmailAddress == account.EmailAddress);

                    acctAccount.Pwhash = sPWHash;
                    await db.SaveChangesAsync();

                    dam.QueryResult = true;
                    dam.QueryMessage = StaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_UPDATE_PASSWORD_SUCCESS;
                }
                else
                {
                    dam.QueryResult = false;
                    dam.QueryMessage = StaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_UPDATE_PASSWORD_FAILURE;
                }
            }

            return dam;
        }

        public async Task<damReturnModel> CreateAccountAsync(BaseAccountModel account, string sPWHash, Guid? AuthTokenForTesting)
        {
            damReturnModel dam = new damReturnModel();

            using (CollectMgrContext db = new CollectMgrContext(connectionString))
            {
                if (await db.AcctAccounts.AnyAsync(x => x.EmailAddress == account.EmailAddress) == true)
                {
                    dam.QueryResult = false;
                    dam.QueryMessage = StaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_EXISTS;
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

                    db.Add(new AcctAccount { AccountId = Guid.NewGuid(), EmailAddress = account.EmailAddress , Pwhash = sPWHash, AuthorizationToken = gAuthToken });
                    await db.SaveChangesAsync();

                    dam.QueryResult = true;
                    dam.QueryMessage = StaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_CREATED_ACCOUNT;
                    dam.AuthorizationToken = gAuthToken;
                }
            }

            return dam;
        }

        public async Task<damReturnModel> DeleteAccountAsync(BaseAccountModel account)
        {
            damReturnModel dam = new damReturnModel();

            using (CollectMgrContext db = new CollectMgrContext(connectionString))
            {
                AcctAccount? AccountToRemove = await db.AcctAccounts.FirstOrDefaultAsync(x => x.EmailAddress == account.EmailAddress);

                if (AccountToRemove != null)
                {
                    db.Remove(AccountToRemove);
                    await db.SaveChangesAsync();

                    dam.QueryResult = true;
                    dam.QueryMessage = StaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_DELETED;
                }
                else
                {
                    dam.QueryResult = false;
                    dam.QueryMessage = StaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_DOES_NOT_EXIST;
                }
            }

            return dam;
        }

        public async Task<damReturnModel> GetLoginRecordAsync(string sEmailAddress)
        {
            damReturnModel dam = new damReturnModel();

            using (CollectMgrContext db = new CollectMgrContext(connectionString))
            {
                if (await db.AcctAccounts.AnyAsync(x => x.EmailAddress == sEmailAddress) == true)
                {
                    AcctAccount account = await db.AcctAccounts.FirstAsync(x => x.EmailAddress == sEmailAddress);

                    dam.EmailAddress = account.EmailAddress;
                    dam.PasswordHash = account.Pwhash;

                    if (account.AccountVerified == false)
                    {
                        dam.QueryResult = false;
                        dam.QueryMessage = StaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_UNVERIFIED;
                    }
                    else
                    {
                        dam.QueryResult = true;
                        dam.QueryMessage = StaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_RETRIEVE_SUCCESS;
                    }
                }
                else
                {
                    dam.QueryResult = false;
                    dam.QueryMessage = StaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_RETRIEVE_FAILURE;
                }
            }

            return dam;
        }

        public async Task<damReturnModel> GetAuthTokenForReVerify(string sEmailAddress)
        {
            damReturnModel dam = new damReturnModel();

            using (CollectMgrContext db = new CollectMgrContext(connectionString))
            {
                if (await db.AcctAccounts.AnyAsync(x => x.EmailAddress == sEmailAddress) == true)
                {
                    AcctAccount account = await db.AcctAccounts.FirstAsync(x => x.EmailAddress == sEmailAddress);

                    if (account.AccountVerified ==false)
                    {
                        dam.AuthorizationToken = account.AuthorizationToken;

                        dam.QueryResult = true;
                    }
                    else
                    {
                        dam.QueryResult = true;
                        dam.QueryMessage = StaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_ALREADY_VERIFIED;
                    }
                }
                else
                {
                    dam.QueryResult = false;
                    dam.QueryMessage = StaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_NO_MATCH_AUTH_TOKEN;
                }
            }

            return dam;
        }

        public async Task<damReturnModel> VerifyAccountAsync(Guid AuthToken)
        {
            damReturnModel dam = new damReturnModel();

            using (CollectMgrContext db = new CollectMgrContext(connectionString))
            {
                if (await db.AcctAccounts.AnyAsync(x => x.AuthorizationToken == AuthToken) == true)
                {
                    AcctAccount account = await db.AcctAccounts.FirstAsync(x => x.AuthorizationToken == AuthToken);

                    account.AccountVerified = true;

                    await db.SaveChangesAsync();

                    dam.QueryResult = true;
                    dam.QueryMessage = StaticStrings.DATAACCESS_RESPONSEQUERY_RESLT_ACCOUNT_VERIFY_SUCCESS;
                }
                else
                {
                    dam.QueryResult = false;
                    dam.QueryMessage = StaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_NO_MATCH_AUTH_TOKEN;
                }
            }

            return dam;
        }


        //private List<SqlParameter> GetStandardUserLoginParams(BaseAccountModel account, string sPWHash)
        //{
        //    return new List<SqlParameter>()
        //    {
        //        new SqlParameter()
        //        {
        //            ParameterName = "@_iEmailAddress",
        //            SqlDbType = System.Data.SqlDbType.NVarChar,
        //            Direction = System.Data.ParameterDirection.Input,
        //            Size = 1000, //nvarchar is 2 bytes per character
        //            Value = account.EmailAddress
        //        },
        //        new SqlParameter()
        //        {
        //            ParameterName = "@_iPWhash",
        //            SqlDbType = System.Data.SqlDbType.Char,
        //            Size = 60, //1 byte per character 
        //            Direction = System.Data.ParameterDirection.Input,
        //            Value = sPWHash
        //        },
        //        new SqlParameter()
        //        {
        //            ParameterName = "@_oQueryResult",
        //            SqlDbType = System.Data.SqlDbType.Bit,
        //            Direction = System.Data.ParameterDirection.Output
        //        },
        //        new SqlParameter()
        //        {
        //            ParameterName = "@_oQueryMessage",
        //            SqlDbType = System.Data.SqlDbType.NVarChar,
        //            Size = 8000, //nvarchar is 2 bytes per character
        //            Direction = System.Data.ParameterDirection.Output
        //        }
        //    };
        //}

        //public async Task<damReturnModel> RunStoredProcedureAsync(string sStoredProcName, List<SqlParameter> sqlParameters, DatabaseAccessType daType)
        //{
        //    List<SqlParameter> liOutParams = new List<SqlParameter>();
        //    damReturnModel daModel = new damReturnModel();

        //    using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        //    {
        //        using (SqlConnection conn = new SqlConnection(connectionString))
        //        {
        //            await conn.OpenAsync();

        //            using (SqlCommand command = conn.CreateCommand())
        //            {
        //                command.CommandType = System.Data.CommandType.StoredProcedure;
        //                command.CommandText = sStoredProcName;

        //                command.Parameters.AddRange(sqlParameters.ToArray());

        //                switch (daType)
        //                {
        //                    case DatabaseAccessType.NonQuery:

        //                        liOutParams.AddRange([.. sqlParameters.Where(x => x.Direction == System.Data.ParameterDirection.Output)]);

        //                        await command.ExecuteNonQueryAsync();

        //                        foreach (SqlParameter outParam in liOutParams)
        //                        {
        //                            switch (outParam.ParameterName)
        //                            {
        //                                case "@_oQueryResult":
        //                                    daModel.QueryResult = bool.Parse(outParam.Value.ToString()!);
        //                                    break;

        //                                case "@_oQueryMessage":
        //                                    daModel.QueryMessage = outParam.Value.ToString()!;
        //                                    break;

        //                                case "@_oAuthorizationToken":
        //                                    daModel.AuthorizationToken = Guid.Parse(outParam.Value.ToString()!);
        //                                    break;

        //                                default:
        //                                    break;
        //                            }
        //                        }

        //                        break;

        //                    case DatabaseAccessType.Read:
        //                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
        //                        {
        //                            //always only returns 1 record
        //                            if (reader.Read())
        //                            {
        //                                if (DataAccessMethodsExtensions.HasColumn(reader, "EmailAddress") == true)
        //                                {
        //                                    daModel.EmailAddress = reader["EmailAddress"].ToString();
        //                                }

        //                                if (DataAccessMethodsExtensions.HasColumn(reader, "PasswordHash") == true)
        //                                {
        //                                    daModel.PasswordHash = reader["PasswordHash"].ToString();
        //                                }

        //                                daModel.QueryResult = bool.Parse(reader["QueryResult"].ToString()!);
        //                                daModel.QueryMessage = reader["QueryMessage"].ToString();
        //                            }
        //                        }
        //                        break;

        //                    default:
        //                        break;
        //                }
        //            }
        //        }

        //        scope.Complete();
        //    }

        //    return daModel;
        //}

        //public enum DatabaseAccessType
        //{
        //    NonQuery = 0,
        //    Read = 1
        //}
    }
}

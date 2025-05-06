using Microsoft.Data.SqlClient;
using Org.BouncyCastle.Crypto.Macs;
using Shared.Models;
using Shared.Statics;
using System.Collections.Generic;
using System.Data;
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
            List<SqlParameter> liParameters = GetStandardUserLoginParams(account, sPWHash);

            return await RunStoredProcedureAsync(StaticStrings.UPDATE_ACCOUNT_PASSWORD_PROC, liParameters, DatabaseAccessType.NonQuery);
        }

        public async Task<damReturnModel> CreateAccountAsync(BaseAccountModel account, string sPWHash)
        {
            List<SqlParameter> liParameters = GetStandardUserLoginParams(account, sPWHash);

            return await RunStoredProcedureAsync(StaticStrings.CREATE_ACCOUNT_PROC, liParameters, DatabaseAccessType.NonQuery);
        }

        private List<SqlParameter> GetStandardUserLoginParams(BaseAccountModel account, string sPWHash)
        {
            return new List<SqlParameter>()
            {
                new SqlParameter()
                {
                    ParameterName = "@_iEmailAddress",
                    SqlDbType = System.Data.SqlDbType.NVarChar,
                    Direction = System.Data.ParameterDirection.Input,
                    Size = 1000, //nvarchar is 2 bytes per character
                    Value = account.EmailAddress
                },
                new SqlParameter()
                {
                    ParameterName = "@_iPWhash",
                    SqlDbType = System.Data.SqlDbType.Char,
                    Size = 60, //1 byte per character 
                    Direction = System.Data.ParameterDirection.Input,
                    Value = sPWHash
                },
                new SqlParameter()
                {
                    ParameterName = "@_oQueryResult",
                    SqlDbType = System.Data.SqlDbType.Bit,
                    Direction = System.Data.ParameterDirection.Output
                },
                new SqlParameter()
                {
                    ParameterName = "@_oQueryMessage",
                    SqlDbType = System.Data.SqlDbType.NVarChar,
                    Size = 8000, //nvarchar is 2 bytes per character
                    Direction = System.Data.ParameterDirection.Output
                }
            };
        }

        public async Task<damReturnModel> GetLoginRecordAsync(string sEmailAddress)
        {
            List<SqlParameter> liParameters = new List<SqlParameter>()
            {
                new SqlParameter()
                {
                    ParameterName = "@_iEmailAddress",
                    SqlDbType = System.Data.SqlDbType.NVarChar,
                    Direction = System.Data.ParameterDirection.Input,
                    Size = 1000, //nvarchar is 2 bytes per character
                    Value = sEmailAddress
                }
            };

            return await RunStoredProcedureAsync(StaticStrings.LOGIN_ACCOUNT_PROC, liParameters, DatabaseAccessType.Read);
        }


        public async Task<damReturnModel> RunStoredProcedureAsync(string sStoredProcName, List<SqlParameter> sqlParameters, DatabaseAccessType daType)
        {
            List<SqlParameter> liOutParams = new List<SqlParameter>();
            damReturnModel daModel = new damReturnModel();

            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    using (SqlCommand command = conn.CreateCommand())
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.CommandText = sStoredProcName;

                        command.Parameters.AddRange(sqlParameters.ToArray());

                        switch (daType)
                        {
                            case DatabaseAccessType.NonQuery:

                                liOutParams.AddRange([.. sqlParameters.Where(x => x.Direction == System.Data.ParameterDirection.Output)]);

                                await command.ExecuteNonQueryAsync();

                                foreach (SqlParameter outParam in liOutParams)
                                {
                                    switch (outParam.ParameterName)
                                    {
                                        case "@_oQueryResult":
                                            daModel.QueryResult = bool.Parse(outParam.Value.ToString()!);
                                            break;

                                        case "@_oQueryMessage":
                                            daModel.QueryMessage = outParam.Value.ToString()!;
                                            break;

                                        case "@_oAuthorizationToken":
                                            daModel.AuthorizationToken = Guid.Parse(outParam.Value.ToString()!);
                                            break;

                                        default:
                                            break;
                                    }
                                }

                                break;

                            case DatabaseAccessType.Read:
                                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                                {
                                    //always only returns 1 record
                                    if (reader.Read())
                                    {
                                        if (DataAccessMethodsExtensions.HasColumn(reader, "EmailAddress") == true)
                                        {
                                            daModel.EmailAddress = reader["EmailAddress"].ToString();
                                        }

                                        if (DataAccessMethodsExtensions.HasColumn(reader, "PasswordHash") == true)
                                        {
                                            daModel.PasswordHash = reader["PasswordHash"].ToString();
                                        }

                                        daModel.QueryResult = bool.Parse(reader["QueryResult"].ToString()!);
                                        daModel.QueryMessage = reader["QueryMessage"].ToString();
                                    }
                                }
                                break;

                            default:
                                break;
                        }
                    }
                }

                scope.Complete();
            }

            return daModel;
        }

        public enum DatabaseAccessType
        {
            NonQuery = 0,
            Read = 1
        }
    }
}

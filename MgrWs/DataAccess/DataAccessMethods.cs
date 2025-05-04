using Microsoft.Data.SqlClient;
using Shared.Statics;
using System.Transactions;

namespace DataAccess
{
    public class DataAccessMethods
    {
        private string connectionString { get; set; }
        public DataAccessMethods(string sConnectionString)
        {
            connectionString = sConnectionString;
        }

        public async Task<(bool bQueryResult, string sQueryMessage)> CreateAccountAsync(string sEmailAddress, string sPWHash)
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

            return await RunStoredProcedureAsync(StaticStrings.CREATE_ACCOUNT_PROC, liParameters);
        }

        public async Task<(bool bQueryResult, string sQueryMessage)> RunStoredProcedureAsync(string sStoredProcName, List<SqlParameter> sqlParameters)
        {
            List<SqlParameter> liOutParams = [.. sqlParameters.Where(x => x.Direction == System.Data.ParameterDirection.Output)];

            bool bQueryResult = false;
            string sQueryMessage = string.Empty;

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

                        await command.ExecuteNonQueryAsync();

                        foreach (SqlParameter outParam in liOutParams)
                        {
                            switch (outParam.ParameterName)
                            {
                                case "@_oQueryResult":
                                    bQueryResult = bool.Parse(outParam.Value!.ToString()!);
                                    break;

                                case "@_oQueryMessage":
                                    sQueryMessage = outParam.Value.ToString()!;
                                    break;

                                default:
                                    break;
                            }
                        }
                    }
                }

                scope.Complete();
            }

            return (bQueryResult!, sQueryMessage!);
        }
    }
}

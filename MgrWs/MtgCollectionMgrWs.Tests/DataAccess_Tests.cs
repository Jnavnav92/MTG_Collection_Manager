using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataAccess;
using Microsoft.Extensions.Configuration;
using DataAccess.Models;
using Shared;
using Shared.Models;
using Shared.Statics;
using MtgCollectionMgrWs.Tests;

namespace DataAccess.Tests
{
    [TestClass()]
    public class DataAccess_Tests
    {
        IConfigurationRoot? _config;
        BaseAccountModel? newAccount;
        string? PasswordHash;

        public DataAccess_Tests()
        {
            _config = new ConfigurationBuilder()
            .AddUserSecrets<DataAccess_Tests>()
            .Build();
        }

        [TestMethod()]
        public async Task CreateAccountAsyncTest_Success()
        {
            //Arrange
            string sConnectionString = _config["MTGCollectionMgr:ConnString"];

            newAccount = new BaseAccountModel() 
            { 
                EmailAddress = $"{new Random().Next(0, 10000)}@Test.test",
            };

            //act
            string sPassword = $"{new Random().Next(0, 10000)}.TestPW";

            PasswordHash = await PWManager.HashPasswordAsync(sPassword);

            DataAccessMethods dam = new DataAccessMethods()
            {
                connectionString = sConnectionString
            };

            damReturnModel result = await dam.CreateAccountAsync(newAccount, PasswordHash);

            //assert
            Assert.IsTrue(result.QueryResult == true);
            Assert.IsTrue(result.QueryMessage == StaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_CREATED_ACCOUNT);
        }

        [TestMethod()]
        public async Task CreateAccountAsyncTest_Email_Already_Exists()
        {
            //Arrange
            string sConnectionString = _config[UnitTestingStatics.UNIT_TEST_SECRET_CONN_STRING_NAME];

            BaseAccountModel testAccount = new BaseAccountModel()
            { 
                EmailAddress = UnitTestingStatics.UNIT_TEST_EMAIL
            };

            DataAccessMethods dam = new DataAccessMethods()
            {
                connectionString = sConnectionString
            };

            //act
            damReturnModel result = await dam.CreateAccountAsync(testAccount, UnitTestingStatics.UNIT_TEST_PASSWORD_HASH);

            //assert
            Assert.IsTrue(result.QueryResult == false);
            Assert.IsTrue(result.QueryMessage == StaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_EXISTS);
        }


        [TestMethod()]
        public void UpdatePasswordAsyncTest()
        {

        }

        [TestMethod()]
        public void GetLoginRecordAsyncTest()
        {

        }

        [TestMethod()]
        public void GetAuthTokenForReVerifyTest()
        {

        }

        [TestMethod()]
        public void VerifyAccountAsyncTest()
        {

        }
    }
}

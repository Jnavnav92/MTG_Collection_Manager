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
        Guid gTestAccountAuthToken = Guid.Parse("5D24875C-7D0D-4C93-BB60-77ECF428443A");
        static object oLockerObject = new object();

        public DataAccess_Tests()
        {
            _config = new ConfigurationBuilder()
            .AddUserSecrets<DataAccess_Tests>()
            .Build();
        }

        [TestMethod()]
        [DoNotParallelize]
        public async Task A_CreateAccountAsyncTest_Success()
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
            damReturnModel result = await dam.CreateAccountAsync(testAccount, UnitTestingStatics.UNIT_TEST_PASSWORD_HASH, gTestAccountAuthToken);

            //assert
            Assert.IsTrue(result.QueryResult == true);
            Assert.IsTrue(result.QueryMessage == StaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_CREATED_ACCOUNT);
        }

        [TestMethod()]
        [DoNotParallelize]
        public async Task B_CreateAccountAsyncTest_Email_Already_Exists()
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
            damReturnModel result = await dam.CreateAccountAsync(testAccount, UnitTestingStatics.UNIT_TEST_PASSWORD_HASH, gTestAccountAuthToken);

            //assert
            Assert.IsTrue(result.QueryResult == false);
            Assert.IsTrue(result.QueryMessage == StaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_EXISTS);
        }

        [TestMethod()]
        [DoNotParallelize]
        public async Task C_GetLoginRecordAsync_Unverified_Test()
        {
            //Arrange
            string sConnectionString = _config[UnitTestingStatics.UNIT_TEST_SECRET_CONN_STRING_NAME];

            DataAccessMethods dam = new DataAccessMethods()
            {
                connectionString = sConnectionString
            };

            //act
            damReturnModel result = await dam.GetLoginRecordAsync(UnitTestingStatics.UNIT_TEST_EMAIL);

            //assert
            Assert.IsTrue(result.QueryResult == false);
            Assert.IsTrue(result.QueryMessage == StaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_UNVERIFIED);
        }

        [TestMethod()]
        [DoNotParallelize]
        public async Task D_GetAuthTokenForReVerifyTest()
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
            damReturnModel result = await dam.GetAuthTokenForReVerify(UnitTestingStatics.UNIT_TEST_EMAIL);

            //assert
            Assert.IsTrue(result.QueryResult == true);
            Assert.IsTrue(result.AuthorizationToken == gTestAccountAuthToken);
        }

        [TestMethod()]
        [DoNotParallelize]
        public async Task E_VerifyAccountAsyncTest()
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
            damReturnModel result = await dam.VerifyAccountAsync(gTestAccountAuthToken);

            //assert
            Assert.IsTrue(result.QueryResult == true);
            Assert.IsTrue(result.QueryMessage == StaticStrings.DATAACCESS_RESPONSEQUERY_RESLT_ACCOUNT_VERIFY_SUCCESS);
        }

        [TestMethod()]
        [DoNotParallelize]
        public async Task F_GetLoginRecordAsync_Verified_Test()
        {
            //Arrange
            string sConnectionString = _config[UnitTestingStatics.UNIT_TEST_SECRET_CONN_STRING_NAME];

            DataAccessMethods dam = new DataAccessMethods()
            {
                connectionString = sConnectionString
            };

            //act
            damReturnModel result = await dam.GetLoginRecordAsync(UnitTestingStatics.UNIT_TEST_EMAIL);

            //assert
            Assert.IsTrue(result.QueryResult == true);
            Assert.IsTrue(result.QueryMessage == StaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_RETRIEVE_SUCCESS);
        }


        [TestMethod()]
        [DoNotParallelize]
        public async Task G_UpdatePasswordAsync_FoundAccount_Test()
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
            damReturnModel result = await dam.UpdatePasswordAsync(testAccount, UnitTestingStatics.UNIT_TEST_PASSWORD_HASH);

            //assert
            Assert.IsTrue(result.QueryResult == true);
            Assert.IsTrue(result.QueryMessage == StaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_UPDATE_PASSWORD_SUCCESS);
        }

        [TestMethod()]
        [DoNotParallelize]
        public async Task H_UpdatePasswordAsync_NoAccount_Test()
        {
            //Arrange
            string sConnectionString = _config[UnitTestingStatics.UNIT_TEST_SECRET_CONN_STRING_NAME];

            BaseAccountModel testAccount = new BaseAccountModel()
            {
                EmailAddress = UnitTestingStatics.UNIT_TEST_NO_FOUND_EMAIL
            };

            DataAccessMethods dam = new DataAccessMethods()
            {
                connectionString = sConnectionString
            };

            //act
            damReturnModel result = await dam.UpdatePasswordAsync(testAccount, UnitTestingStatics.UNIT_TEST_PASSWORD_HASH);

            //assert
            Assert.IsTrue(result.QueryResult == false);
            Assert.IsTrue(result.QueryMessage == StaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_UPDATE_PASSWORD_FAILURE);
        }

        [TestMethod()]
        [DoNotParallelize]
        public async Task I_DeleteAccountAsync_FoundAccount_Test()
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
            damReturnModel result = await dam.DeleteAccountAsync(testAccount);

            //assert
            Assert.IsTrue(result.QueryResult == true);
            Assert.IsTrue(result.QueryMessage == StaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_DELETED);
        }

        [TestMethod()]
        [DoNotParallelize]
        public async Task J_DeleteAccountAsync_NoAccount_Test()
        {
            //Arrange
            string sConnectionString = _config[UnitTestingStatics.UNIT_TEST_SECRET_CONN_STRING_NAME];

            BaseAccountModel testAccount = new BaseAccountModel()
            {
                EmailAddress = UnitTestingStatics.UNIT_TEST_NO_FOUND_EMAIL
            };

            DataAccessMethods dam = new DataAccessMethods()
            {
                connectionString = sConnectionString
            };

            //act
            damReturnModel result = await dam.DeleteAccountAsync(testAccount);

            //assert
            Assert.IsTrue(result.QueryResult == false);
            Assert.IsTrue(result.QueryMessage == StaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_DOES_NOT_EXIST);
        }
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Configuration;
using DataAccess.Models;
using Shared;
using Shared.Models;
using Shared.Statics;
using DataAccess.Classes;
using MtgCollectionMgrWs.Tests.Shared;
using System.Text;

namespace MtgCollectionMgrWs.Tests.TestClasses
{
    [TestClass()]
    public class AuthDataAccess_Tests : BaseTestClass
    {
        private Guid gTestAccountAuthToken = Guid.Parse("5D24875C-7D0D-4C93-BB60-77ECF428443A");
        private const string UNIT_TEST_ACCOUNT_ALWAYS_EXIST = "TestAlwaysExist@test.test";
        private const string UNIT_TEST_PASSWORD_HASH_ALWAYS_EXIST = "$2a$15$poycy7JGlgzUSlrL4SBPEOoXQ8/RhInXUIZ0KTf0BrQh1X321Y9Oa";

        public AuthDataAccess_Tests() : base()
        {

        }

        /// <summary>
        /// This account should always exist in the database for unit tests outside the auth unit tests.
        /// </summary>
        /// <returns></returns>
        [TestMethod()]
        [DoNotParallelize]
        public async Task A_CreateAccountAsyncTest_CreateAlwaysExistAccount_Success()
        {
            //Arrange
            string sConnectionString = _config[UnitTestingStatics.UNIT_TEST_SECRET_CONN_STRING_NAME];

            BaseAccountModel testAccount = new BaseAccountModel()
            {
                EmailAddress = UNIT_TEST_ACCOUNT_ALWAYS_EXIST
            };

            AuthDataAccessMethods dam = new AuthDataAccessMethods()
            {
                connectionString = sConnectionString
            };

            //act
            damReturnModel result = await dam.CreateAccountAsync(testAccount, UNIT_TEST_PASSWORD_HASH_ALWAYS_EXIST, gAlwaysExistTestAccountAuthToken);

            //assert

            if (result.QueryResult == false && result.QueryMessage == AuthStaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_EXISTS)
            {
                //always exist account already exists, that's fine.
            }
            else if (result.QueryResult == true && result.QueryMessage == AuthStaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_CREATED_ACCOUNT)
            {
                //always exist test account did not exist and we created it, that's fine.
            }
            else
            {
                //something else happened that's bad, throw an exception and fail the test.
                throw new Exception(string.Format(AuthStaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_ALWAYS_EXIST_ACCOUNT_ERROR, result.QueryMessage));
            }
        }

        [TestMethod()]
        [DoNotParallelize]
        public async Task B_CreateAccountAsyncTest_Success()
        {
            //Arrange
            string sConnectionString = _config[UnitTestingStatics.UNIT_TEST_SECRET_CONN_STRING_NAME];

            BaseAccountModel testAccount = new BaseAccountModel()
            {
                EmailAddress = UnitTestingStatics.UNIT_TEST_EMAIL
            };

            AuthDataAccessMethods dam = new AuthDataAccessMethods()
            {
                connectionString = sConnectionString
            };

            //act
            damReturnModel result = await dam.CreateAccountAsync(testAccount, UnitTestingStatics.UNIT_TEST_PASSWORD_HASH, gTestAccountAuthToken);

            //assert
            Assert.IsTrue(result.QueryResult == true);
            Assert.IsTrue(result.QueryMessage == AuthStaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_CREATED_ACCOUNT);
        }

        [TestMethod()]
        [DoNotParallelize]
        public async Task C_CreateAccountAsyncTest_Email_Already_Exists()
        {
            //Arrange
            string sConnectionString = _config[UnitTestingStatics.UNIT_TEST_SECRET_CONN_STRING_NAME];

            BaseAccountModel testAccount = new BaseAccountModel()
            {
                EmailAddress = UnitTestingStatics.UNIT_TEST_EMAIL
            };

            AuthDataAccessMethods dam = new AuthDataAccessMethods()
            {
                connectionString = sConnectionString
            };

            //act
            damReturnModel result = await dam.CreateAccountAsync(testAccount, UnitTestingStatics.UNIT_TEST_PASSWORD_HASH, gTestAccountAuthToken);

            //assert
            Assert.IsTrue(result.QueryResult == false);
            Assert.IsTrue(result.QueryMessage == AuthStaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_EXISTS);
        }

        [TestMethod()]
        [DoNotParallelize]
        public async Task D_GetLoginRecordAsync_Unverified_Test()
        {
            //Arrange
            string sConnectionString = _config[UnitTestingStatics.UNIT_TEST_SECRET_CONN_STRING_NAME];

            AuthDataAccessMethods dam = new AuthDataAccessMethods()
            {
                connectionString = sConnectionString
            };

            //act
            damReturnModel result = await dam.GetLoginRecordAsync(UnitTestingStatics.UNIT_TEST_EMAIL);

            //assert
            Assert.IsTrue(result.QueryResult == false);
            Assert.IsTrue(result.QueryMessage == AuthStaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_UNVERIFIED);
        }

        [TestMethod()]
        [DoNotParallelize]
        public async Task E_GetAuthTokenForReVerifyTest()
        {
            //Arrange
            string sConnectionString = _config[UnitTestingStatics.UNIT_TEST_SECRET_CONN_STRING_NAME];

            BaseAccountModel testAccount = new BaseAccountModel()
            {
                EmailAddress = UnitTestingStatics.UNIT_TEST_EMAIL
            };

            AuthDataAccessMethods dam = new AuthDataAccessMethods()
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
        public async Task F_VerifyAccountAsyncTest()
        {
            //Arrange
            string sConnectionString = _config[UnitTestingStatics.UNIT_TEST_SECRET_CONN_STRING_NAME];

            BaseAccountModel testAccount = new BaseAccountModel()
            {
                EmailAddress = UnitTestingStatics.UNIT_TEST_EMAIL
            };

            AuthDataAccessMethods dam = new AuthDataAccessMethods()
            {
                connectionString = sConnectionString
            };

            //act
            damReturnModel result = await dam.VerifyAccountAsync(gTestAccountAuthToken);

            //assert
            Assert.IsTrue(result.QueryResult == true);
            Assert.IsTrue(result.QueryMessage == AuthStaticStrings.DATAACCESS_RESPONSEQUERY_RESLT_ACCOUNT_VERIFY_SUCCESS);
        }

        [TestMethod()]
        [DoNotParallelize]
        public async Task G_GetLoginRecordAsync_Verified_Test()
        {
            //Arrange
            string sConnectionString = _config[UnitTestingStatics.UNIT_TEST_SECRET_CONN_STRING_NAME];

            AuthDataAccessMethods dam = new AuthDataAccessMethods()
            {
                connectionString = sConnectionString
            };

            //act
            damReturnModel result = await dam.GetLoginRecordAsync(UnitTestingStatics.UNIT_TEST_EMAIL);

            //assert
            Assert.IsTrue(result.QueryResult == true);
            Assert.IsTrue(result.QueryMessage == AuthStaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_RETRIEVE_SUCCESS);
        }


        [TestMethod()]
        [DoNotParallelize]
        public async Task H_UpdatePasswordAsync_FoundAccount_Test()
        {
            //Arrange
            string sConnectionString = _config[UnitTestingStatics.UNIT_TEST_SECRET_CONN_STRING_NAME];

            BaseAccountModel testAccount = new BaseAccountModel()
            {
                EmailAddress = UnitTestingStatics.UNIT_TEST_EMAIL
            };

            AuthDataAccessMethods dam = new AuthDataAccessMethods()
            {
                connectionString = sConnectionString
            };

            //act
            damReturnModel result = await dam.UpdatePasswordAsync(testAccount, UnitTestingStatics.UNIT_TEST_PASSWORD_HASH);

            //assert
            Assert.IsTrue(result.QueryResult == true);
            Assert.IsTrue(result.QueryMessage == AuthStaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_UPDATE_PASSWORD_SUCCESS);
        }

        [TestMethod()]
        [DoNotParallelize]
        public async Task I_UpdatePasswordAsync_NoAccount_Test()
        {
            //Arrange
            string sConnectionString = _config[UnitTestingStatics.UNIT_TEST_SECRET_CONN_STRING_NAME];

            BaseAccountModel testAccount = new BaseAccountModel()
            {
                EmailAddress = UnitTestingStatics.UNIT_TEST_NO_FOUND_EMAIL
            };

            AuthDataAccessMethods dam = new AuthDataAccessMethods()
            {
                connectionString = sConnectionString
            };

            //act
            damReturnModel result = await dam.UpdatePasswordAsync(testAccount, UnitTestingStatics.UNIT_TEST_PASSWORD_HASH);

            //assert
            Assert.IsTrue(result.QueryResult == false);
            Assert.IsTrue(result.QueryMessage == AuthStaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_UPDATE_PASSWORD_FAILURE);
        }

        [TestMethod()]
        [DoNotParallelize]
        public async Task J_DeleteAccountAsync_FoundAccount_Test()
        {
            //Arrange
            string sConnectionString = _config[UnitTestingStatics.UNIT_TEST_SECRET_CONN_STRING_NAME];

            BaseAccountModel testAccount = new BaseAccountModel()
            {
                EmailAddress = UnitTestingStatics.UNIT_TEST_EMAIL
            };

            AuthDataAccessMethods dam = new AuthDataAccessMethods()
            {
                connectionString = sConnectionString
            };

            //act
            damReturnModel result = await dam.DeleteAccountAsync(testAccount);

            //assert
            Assert.IsTrue(result.QueryResult == true);
            Assert.IsTrue(result.QueryMessage == AuthStaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_DELETED);
        }

        [TestMethod()]
        [DoNotParallelize]
        public async Task K_DeleteAccountAsync_NoAccount_Test()
        {
            //Arrange
            string sConnectionString = _config[UnitTestingStatics.UNIT_TEST_SECRET_CONN_STRING_NAME];

            BaseAccountModel testAccount = new BaseAccountModel()
            {
                EmailAddress = UnitTestingStatics.UNIT_TEST_NO_FOUND_EMAIL
            };

            AuthDataAccessMethods dam = new AuthDataAccessMethods()
            {
                connectionString = sConnectionString
            };

            //act
            damReturnModel result = await dam.DeleteAccountAsync(testAccount);

            //assert
            Assert.IsTrue(result.QueryResult == false);
            Assert.IsTrue(result.QueryMessage == AuthStaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_DOES_NOT_EXIST);
        }
    }
}

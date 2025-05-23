using DataAccess.Classes;
using DataAccess.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MtgCollectionMgrWs.Tests.Shared;
using Shared.Models;
using Shared.Statics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MtgCollectionMgrWs.Tests.TestClasses
{
    [TestClass()]
    public class CollectionDataAccess_Tests : BaseTestClass
    {
        public CollectionDataAccess_Tests() : base()
        {

        }

        [TestMethod()]
        [DoNotParallelize]
        public async Task A_CreateCollectionAsync_Success_Test()
        {
            //Arrange
            string sConnectionString = _config[UnitTestingStatics.UNIT_TEST_SECRET_CONN_STRING_NAME];

            CollectionDataAccessMethods dam = new CollectionDataAccessMethods()
            {
                connectionString = sConnectionString
            };

            //act
            damReturnModel result = await dam.CreateCollectionAsync(gAlwaysExistsTestAccountAccountID, UnitTestingStatics.UNIT_TEST_COLLECTION_NAME);

            //assert
            Assert.IsTrue(result.QueryResult == true);
            Assert.IsTrue(result.QueryMessage == CollectionStaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_CREATED_COLLECTION);
        }

        [TestMethod()]
        [DoNotParallelize]
        public async Task B_CreateCollectionAsync_Failure_DuplicateCollectionName_Test()
        {
            //Arrange
            string sConnectionString = _config[UnitTestingStatics.UNIT_TEST_SECRET_CONN_STRING_NAME];

            CollectionDataAccessMethods dam = new CollectionDataAccessMethods()
            {
                connectionString = sConnectionString
            };

            //act
            damReturnModel result = await dam.CreateCollectionAsync(gAlwaysExistsTestAccountAccountID, UnitTestingStatics.UNIT_TEST_COLLECTION_NAME);

            //assert
            Assert.IsTrue(result.QueryResult == false);
            Assert.IsTrue(result.QueryMessage == CollectionStaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_COLLECTION_DUPLICATE_NAME);
        }

        [TestMethod()]
        [DoNotParallelize]
        public async Task C_CreateCollectionAsync_Failure_NoAccount_Test()
        {
            //Arrange
            string sConnectionString = _config[UnitTestingStatics.UNIT_TEST_SECRET_CONN_STRING_NAME];

            CollectionDataAccessMethods dam = new CollectionDataAccessMethods()
            {
                connectionString = sConnectionString
            };

            //act
            damReturnModel result = await dam.CreateCollectionAsync(Guid.NewGuid(), UnitTestingStatics.UNIT_TEST_COLLECTION_NAME);

            //assert
            Assert.IsTrue(result.QueryResult == false);
            Assert.IsTrue(result.QueryMessage == CollectionStaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_NOT_EXIST);
        }

        [TestMethod()]
        [DoNotParallelize]
        public async Task D_UpdateCollectionNameAsync_Success_Test()
        {
            //Arrange
            string sConnectionString = _config[UnitTestingStatics.UNIT_TEST_SECRET_CONN_STRING_NAME];

            CollectionDataAccessMethods dam = new CollectionDataAccessMethods()
            {
                connectionString = sConnectionString
            };

            //act
            damReturnModel result = await dam.UpdateCollectionNameAsync(gAlwaysExistsTestAccountAccountID, UnitTestingStatics.UNIT_TEST_COLLECTION_NAME, UnitTestingStatics.UNIT_TEST_COLLECTION_UPDATED_NAME);

            //assert
            Assert.IsTrue(result.QueryResult == true);
            Assert.IsTrue(result.QueryMessage == CollectionStaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_UPDATED_COLLECTION);
        }

        [TestMethod()]
        [DoNotParallelize]
        public async Task E_UpdateCollectionNameAsync_Failure_NoAccount_Test()
        {
            //Arrange
            string sConnectionString = _config[UnitTestingStatics.UNIT_TEST_SECRET_CONN_STRING_NAME];

            CollectionDataAccessMethods dam = new CollectionDataAccessMethods()
            {
                connectionString = sConnectionString
            };

            //act
            damReturnModel result = await dam.UpdateCollectionNameAsync(Guid.NewGuid(), UnitTestingStatics.UNIT_TEST_COLLECTION_NAME, UnitTestingStatics.UNIT_TEST_COLLECTION_UPDATED_NAME);

            //assert
            Assert.IsTrue(result.QueryResult == false);
            Assert.IsTrue(result.QueryMessage == CollectionStaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_NOT_EXIST);
        }

        [TestMethod()]
        [DoNotParallelize]
        public async Task F_UpdateCollectionNameAsync_Failure_BadOldCollectionName_Test()
        {
            //Arrange
            string sConnectionString = _config[UnitTestingStatics.UNIT_TEST_SECRET_CONN_STRING_NAME];

            CollectionDataAccessMethods dam = new CollectionDataAccessMethods()
            {
                connectionString = sConnectionString
            };

            //act
            damReturnModel result = await dam.UpdateCollectionNameAsync(gAlwaysExistsTestAccountAccountID, $"{new Random().Next(1, 1000)} BadCollectionName", UnitTestingStatics.UNIT_TEST_COLLECTION_UPDATED_NAME);

            //assert
            Assert.IsTrue(result.QueryResult == false);
            Assert.IsTrue(result.QueryMessage == CollectionStaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_DELETE_COULD_NOT_FIND_COLLECTION);
        }


        [TestMethod()]
        [DoNotParallelize]
        public async Task G_DeleteCollectionAsync_Success_Test()
        {
            //Arrange
            string sConnectionString = _config[UnitTestingStatics.UNIT_TEST_SECRET_CONN_STRING_NAME];

            CollectionDataAccessMethods dam = new CollectionDataAccessMethods()
            {
                connectionString = sConnectionString
            };

            //act
            damReturnModel result = await dam.DeleteCollectionAsync(gAlwaysExistsTestAccountAccountID, UnitTestingStatics.UNIT_TEST_COLLECTION_NAME);

            //assert
            Assert.IsTrue(result.QueryResult == true);
            Assert.IsTrue(result.QueryMessage == CollectionStaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_DELETED_COLLECTION);
        }

        [TestMethod()]
        [DoNotParallelize]
        public async Task H_DeleteCollectionAsync_Failure_NoAccount_Test()
        {
            //Arrange
            string sConnectionString = _config[UnitTestingStatics.UNIT_TEST_SECRET_CONN_STRING_NAME];

            CollectionDataAccessMethods dam = new CollectionDataAccessMethods()
            {
                connectionString = sConnectionString
            };

            //act
            damReturnModel result = await dam.DeleteCollectionAsync(Guid.NewGuid(), UnitTestingStatics.UNIT_TEST_COLLECTION_NAME);

            //assert
            Assert.IsTrue(result.QueryResult == false);
            Assert.IsTrue(result.QueryMessage == CollectionStaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_NOT_EXIST);
        }

        [TestMethod()]
        [DoNotParallelize]
        public async Task I_DeleteCollectionAsync_Failure_CollectionNameNotExist_Test()
        {
            //Arrange
            string sConnectionString = _config[UnitTestingStatics.UNIT_TEST_SECRET_CONN_STRING_NAME];

            CollectionDataAccessMethods dam = new CollectionDataAccessMethods()
            {
                connectionString = sConnectionString
            };

            //act
            damReturnModel result = await dam.DeleteCollectionAsync(gAlwaysExistsTestAccountAccountID, $"{new Random().Next(0, 100)} test collection");

            //assert
            Assert.IsTrue(result.QueryResult == false);
            Assert.IsTrue(result.QueryMessage == CollectionStaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_DELETE_COULD_NOT_FIND_COLLECTION);
        }
    }
}
using DataAccess.Data;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Macs;
using Shared.Statics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Classes
{
    public class CollectionDataAccessMethods : DataAccessMethods
    {
        public CollectionDataAccessMethods() : base()
        {

        }

        //create collection - accountID, CollectionName

        public async Task<damReturnModel> CreateCollectionAsync(Guid gAccountID, string sCollectionName)
        {
            damReturnModel dam = new damReturnModel();

            using (CollectionMgrContext db = new CollectionMgrContext(connectionString))
            {
                List<CollectCollection> liAccountCollections = await GetAccountCollectionsAsync(db, gAccountID);

                if (liAccountCollections.Count() == 0)
                {
                    //shouldn't ever happen, but if the account doesn't exist, return error.
                    dam.QueryResult = false;
                    dam.QueryMessage = CollectionStaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_NOT_EXIST;
                    return dam;
                }
                else
                {
                    //account exists, next verify collection name does not exist for this account. Collection Names are unique per account but cannot be composite key due to
                    //Collect_Cards needing a foreign key to Collect_Collections.CollectionID.

                    if (liAccountCollections.Where(x => x.CollectionName == sCollectionName).FirstOrDefault() != null)
                    { 
                        dam.QueryResult = false;
                        dam.QueryMessage = CollectionStaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_COLLECTION_DUPLICATE_NAME;
                        return dam;
                    }
                    else
                    {
                        //collection name is unique for this account, create collection tied to this account.

                        CollectCollection newCollection = liAccountCollections.First();

                        newCollection.AccountId = gAccountID;
                        newCollection.CollectionId = Guid.NewGuid();
                        newCollection.CollectionName = sCollectionName;

                        await db.CollectCollections.AddAsync(newCollection);
                        await db.SaveChangesAsync();

                        dam.QueryResult = true;
                        dam.QueryMessage = CollectionStaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_CREATED_COLLECTION;
                    }
                }
            }

            return dam;
        }

        //delete collection - Guid gAccountID, string sCollectionName

        public async Task<damReturnModel> DeleteCollectionAsync(Guid gAccountID, string sCollectionName)
        {
            damReturnModel dam = new damReturnModel();

            using (CollectionMgrContext db = new CollectionMgrContext(connectionString))
            {
                List<CollectCollection> liAccountCollections = await GetAccountCollectionsAsync(db, gAccountID);

                if (liAccountCollections.Count() == 0)
                {
                    //shouldn't ever happen, but if the account doesn't exist, return error.
                    dam.QueryResult = false;
                    dam.QueryMessage = CollectionStaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_NOT_EXIST;
                    return dam;
                }
                else
                {
                    //account exists, next find collection name for this account

                    CollectCollection? deleteCollection = liAccountCollections.Where(x => x.CollectionName == sCollectionName).FirstOrDefault();

                    if (deleteCollection != null)
                    {
                        //found collection with name, delete collection

                        db.CollectCollections.Remove(deleteCollection);
                        await db.SaveChangesAsync();

                        dam.QueryResult = true;
                        dam.QueryMessage = CollectionStaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_DELETED_COLLECTION;

                        return dam;
                    }
                    else
                    {
                        //could not find collection with the specified name, do nothing but report error.

                        dam.QueryResult = false;
                        dam.QueryMessage = CollectionStaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_DELETE_COULD_NOT_FIND_COLLECTION;
                    }
                }
            }

            return dam;
        }

        /// <summary>
        /// does a left outer join between Accounts and Collections, as collection for an account may not have any records yet.
        /// </summary>
        /// <param name="db"></param>
        /// <param name="gAccountID"></param>
        /// <returns></returns>
        private async Task<List<CollectCollection>> GetAccountCollectionsAsync(CollectionMgrContext db, Guid gAccountID)
        {
            List<CollectCollection> liCollections = await (from account in db.AcctAccounts
                                                           join collection in db.CollectCollections
                                                               on account.AccountId equals collection.AccountId into gj
                                                           from subgroup in gj.DefaultIfEmpty()
                                                           where account.AccountId == gAccountID
                                                           select new CollectCollection
                                                           {
                                                               AccountId = account.AccountId,
                                                               CollectionId = subgroup == null ? Guid.Empty : subgroup.CollectionId,
                                                               CollectionName = subgroup == null ? string.Empty : subgroup.CollectionName
                                                           }).ToListAsync();

            return liCollections;
        }

        //update collection name - collectionID, NewCollectionName

        public async Task<damReturnModel> UpdateCollectionNameAsync(Guid gAccountID, string sOldCollectionName, string sNewCollectionName)
        {
            damReturnModel dam = new damReturnModel();

            using (CollectionMgrContext db = new CollectionMgrContext(connectionString))
            {
                List<CollectCollection> liAccountCollections = await GetAccountCollectionsAsync(db, gAccountID);

                if (liAccountCollections.Count() == 0)
                {
                    //shouldn't ever happen, but if the account doesn't exist, return error.
                    dam.QueryResult = false;
                    dam.QueryMessage = CollectionStaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_ACCOUNT_NOT_EXIST;
                    return dam;
                }
                else
                {
                    //account exists, next find old collection name for this account

                    CollectCollection? updateCollection = liAccountCollections.Where(x => x.CollectionName == sOldCollectionName).FirstOrDefault();

                    if (updateCollection != null)
                    {
                        //found collection with name, change name

                        updateCollection.CollectionName = sNewCollectionName;
                        await db.SaveChangesAsync();

                        dam.QueryResult = true;
                        dam.QueryMessage = CollectionStaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_UPDATED_COLLECTION;

                        return dam;
                    }
                    else
                    {
                        //could not find collection with the specified name, do nothing but report error.

                        dam.QueryResult = false;
                        dam.QueryMessage = CollectionStaticStrings.DATAACCESS_RESPONSEQUERY_RESULT_DELETE_COULD_NOT_FIND_COLLECTION;
                    }
                }
            }

            return dam;
        }
    }
}

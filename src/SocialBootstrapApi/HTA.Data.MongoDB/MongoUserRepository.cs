using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTA.ServiceModel;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using ServiceStack.ServiceInterface.Auth;

namespace HTA.Data.MongoDB
{
    public class MongoRepository : IUserRepository, IAuthsRepository
    {
        private readonly MongoDatabase _mongoDatabase;

        public MongoRepository(MongoDatabase mongoDatabase)
        {
            _mongoDatabase = mongoDatabase;
        }

        // User collection name
        private static string User_Col
        {
            get
            {
                return typeof(User).Name;
            }
        }
        // UserAuth collection name
        private static string UserAuth_Col
        {
            get
            {
                return typeof(UserAuth).Name;
            }
        }
        // UserOAuthProvider collection name
        private static string UserOAuthProvider_Col
        {
            get
            {
                return typeof(UserOAuthProvider).Name;
            }
        }

        // Counters collection name
        private static string Counters_Col
        {
            get
            {
                return typeof(UsersCounters).Name;
            }
        }

        public void CreateMissingTables()
        {
            if (!_mongoDatabase.CollectionExists(User_Col))
                _mongoDatabase.CreateCollection(User_Col);

            if (!_mongoDatabase.CollectionExists(UserAuth_Col))
                _mongoDatabase.CreateCollection(UserAuth_Col);

            if (!_mongoDatabase.CollectionExists(UserOAuthProvider_Col))
                _mongoDatabase.CreateCollection(UserOAuthProvider_Col);

            if (!_mongoDatabase.CollectionExists(Counters_Col))
            {
                _mongoDatabase.CreateCollection(Counters_Col);

                var countersCollection = _mongoDatabase.GetCollection<UsersCounters>(Counters_Col);
                var counters = new UsersCounters();
                countersCollection.Save(counters);
            }
        }

        public void DropAndReCreateTables()
        {
            if (_mongoDatabase.CollectionExists(User_Col))
                _mongoDatabase.DropCollection(User_Col);

            if (_mongoDatabase.CollectionExists(UserAuth_Col))
                _mongoDatabase.DropCollection(UserAuth_Col);

            if (_mongoDatabase.CollectionExists(UserOAuthProvider_Col))
                _mongoDatabase.DropCollection(UserOAuthProvider_Col);

            if (_mongoDatabase.CollectionExists(Counters_Col))
                _mongoDatabase.DropCollection(Counters_Col);

            CreateMissingTables();
        }

        public void Save(User user)
        {
            if (user.Id == default(int))
                user.Id = IncUserCounter();

            var usersCollection = _mongoDatabase.GetCollection<User>(User_Col);
            usersCollection.Save(user);
        }

        public User SelectByProfile(UserProfile userProfile)
        {
            var usersCollection = _mongoDatabase.GetCollection<User>(User_Col);

            IMongoQuery query = Query.EQ("_id", userProfile.Id);

            User user = usersCollection.FindOne(query);

            return user;
        }

        public List<User> GetUsers()
        {
            var userCollection = _mongoDatabase.GetCollection<User>(User_Col);

            return userCollection.FindAll().ToList();
        }

        public List<User> GetUsersByID(int[] userIds)
        {
            var userCollection = _mongoDatabase.GetCollection<User>(User_Col);

            List<BsonValue> ids = userIds.Select(id => BsonValue.Create(id)).ToList();
            IMongoQuery query = Query.In("_id", ids);

            var users = userCollection.Find(query).ToList();
            return users;
        }

        private int IncUserCounter()
        {
            return IncCounter("UserCounter").UserCounter;
        }

        private UsersCounters IncCounter(string counterName)
        {
            var CountersCollection = _mongoDatabase.GetCollection<UsersCounters>(Counters_Col);
            var incId = Update.Inc(counterName, 1);
            var query = Query.Null;
            FindAndModifyResult counterIncResult = CountersCollection.FindAndModify(query, SortBy.Null, incId, true);
            UsersCounters updatedCounters = counterIncResult.GetModifiedDocumentAs<UsersCounters>();
            return updatedCounters;
        }

        public List<UserAuth> GetUserAuths()
        {
            var userAuthCollection = _mongoDatabase.GetCollection<UserAuth>(UserAuth_Col);

            return userAuthCollection.FindAll().ToList();
        }

        public List<UserOAuthProvider> GetUserOAuthProvider()
        {
            var userOAuthProviderCollection = _mongoDatabase.GetCollection<UserOAuthProvider>(UserOAuthProvider_Col);

            return userOAuthProviderCollection.FindAll().ToList();
        }

        // http://www.mongodb.org/display/DOCS/How+to+Make+an+Auto+Incrementing+Field
        class UsersCounters
        {
            public int Id { get; set; }
            public int UserCounter { get; set; }
        }
    }
}

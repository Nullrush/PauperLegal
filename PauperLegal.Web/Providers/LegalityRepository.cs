using System;
using System.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;

namespace PauperLegal.Web.Providers
{
    public interface ILegalityRepository
    {
        bool IsCardLegal(string cardName);
    }

    public class LegalityRepository : ILegalityRepository
    {
        private IMongoClient _client;
        private IMongoDatabase _database;

        public LegalityRepository()
        {
            _client = new MongoClient(ConfigurationManager.ConnectionStrings["LegalCardsConnectionString"].ConnectionString);
            _database = _client.GetDatabase(ConfigurationManager.AppSettings["LegalCardsDatabaseName"]);
        }


        public bool IsCardLegal(string cardName)
        {
            throw new NotImplementedException();
        }

        public string GetDatabaseName()
        {
            _database.ListCollections();
            return _database.DatabaseNamespace.DatabaseName;
        }
    }
}
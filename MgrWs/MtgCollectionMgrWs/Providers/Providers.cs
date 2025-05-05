namespace MtgCollectionMgrWs.Providers
{
    public interface IDataRepository
    {
        string GetConnectionString();
        string GetJwtSecret();
    }
    public class DataRepository : IDataRepository
    {
        public IConfiguration _config;

        public DataRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        public string GetConnectionString()
        {
            return _config.GetSection("MTGCollectionMgr:ConnString").Value;
        }

        public string GetJwtSecret()
        {
            return _config.GetSection("MTGCollectionMgr:JwtSecret").Value;
        }
    }
}

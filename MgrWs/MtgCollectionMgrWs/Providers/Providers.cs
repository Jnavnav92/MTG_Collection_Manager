namespace MtgCollectionMgrWs.Providers
{
    public interface IDataRepository
    {
        string GetConnectionString();
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
    }
}

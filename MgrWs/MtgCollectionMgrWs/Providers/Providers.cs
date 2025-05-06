namespace MtgCollectionMgrWs.Providers
{
    public interface IDataRepository
    {
        string GetConnectionString();
        string GetJwtSecret();
        string GetSMTPPassword();
        string GetSMTPEmail();
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

        public string GetSMTPEmail()
        {
            return _config.GetSection("SMTP:GmailEmailAddress").Value;
        }

        public string GetSMTPPassword()
        {
            return _config.GetSection("SMTP:GmailAppPassword").Value;
        }
    }
}

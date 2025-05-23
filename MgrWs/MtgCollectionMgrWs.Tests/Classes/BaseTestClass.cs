using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MtgCollectionMgrWs.Tests.TestClasses
{
    public class BaseTestClass
    {
        protected IConfigurationRoot? _config;
        protected Guid gAlwaysExistTestAccountAuthToken = Guid.Parse("1cba9b8c-4cab-4600-a087-5cee02c2aba4");
        protected Guid gAlwaysExistsTestAccountAccountID = Guid.Parse("8483C79D-F26E-4837-9FCD-CC1A2661FE7E");

        public BaseTestClass()
        {
            _config = new ConfigurationBuilder()
            .AddUserSecrets<BaseTestClass>()
            .Build();
        }
    }
}

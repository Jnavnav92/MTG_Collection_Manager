namespace Shared.Models
{
    public class AccountModelUserCredentialsAuth : AccountModelUserCredentials
    {
        public Guid? AuthorizationToken { get; set; }
    }

    public class AccountModelUserCredentials : BaseAccountModel
    {
        public required string UserPW { get; set; }
    }

    public class BaseAccountModel
    {
        public required string EmailAddress { get; set; }
    }
}

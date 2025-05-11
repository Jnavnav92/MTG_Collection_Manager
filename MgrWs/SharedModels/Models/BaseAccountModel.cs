namespace Shared.Models
{
    public class VerifyEmailAuthModel
    {
        public Guid? AuthorizationToken { get; set; }
    }

    public class AccountModelUserCredentialsModel : BaseAccountModel
    {
        public required string UserPW { get; set; }
    }

    public class BaseAccountModel
    {
        public required string EmailAddress { get; set; }
    }
}

namespace Wulka.Domain.Authentication
{
    public abstract class CredentialsBase 
    {
        public string UserName { get; set; }
        public abstract CredentialsTypeEnum CredentialType { get; }
    }
}

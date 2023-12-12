namespace TorunLive.WebApi.Authentication
{
    public interface IApiKeyValidation
    {
        public bool IsValid(string apiKey);
    }
}

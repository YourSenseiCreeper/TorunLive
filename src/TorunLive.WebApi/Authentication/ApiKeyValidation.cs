namespace TorunLive.WebApi.Authentication
{
    public class ApiKeyValidation(IConfiguration configuration) : IApiKeyValidation
    {
        private readonly IConfiguration _configuration = configuration;

        public bool IsValid(string userApiKey)
        {
            if (string.IsNullOrWhiteSpace(userApiKey))
                return false;

            string? apiKeyFromConfig = _configuration.GetValue<string>(Constants.ApiKeyName);
            if (apiKeyFromConfig == null || apiKeyFromConfig != userApiKey)
                return false;

            return true;
        }
    }
}

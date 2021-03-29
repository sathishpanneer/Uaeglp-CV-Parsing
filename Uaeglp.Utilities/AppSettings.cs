using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Uaeglp.Utilities
{
	public class AppSettings
	{
		public string SmtpHost { get; set; }
		public string SmtpPort { get; set; }
		public string SmtpUsername { get; set; }
		public string SmtpPassword { get; set; }
		public string ServerKey { get; set; }
		public string SenderId { get; set; }

       

        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Scope { get; set; }
        public string ApiScope { get; set; }
        public string AuthorizeUrl { get; set; }
        public string Secret { get; set; }
        public string AuthUrl { get; set; }
        public string YoutubeVideoToken { get; set; }
        public string LocationAPIKey { get; set; }
    }


    public class YoutubeClientSecret
    {
        public string client_id { get; set; }
        public string project_id { get; set; }
        public string auth_uri { get; set; }
        public string token_uri { get; set; }

        public string auth_provider_x509_cert_url { get; set; }
        public string client_secret { get; set; }
        public string[] redirect_uris { get; set; }

        public string VideoToken { get; set; }
        public string access_token { get; set; }
        public string refresh_token { get; set; }
    }

    public class EIDValidation
    {
        public string user_name { get; set; }
        public string password { get; set; }
        public string gsb_apikey { get; set; }
        public string EIDForDEV { get; set; }
        public string EndpointURL { get; set; }
    }

    public class TokenResponseValues
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string scope { get; set; }
        public string token_type { get; set; }
    }

    public class TokenRequestValues
    {
        public string client_id { get; set; }
        public string refresh_token { get; set; }
        public string client_secret { get; set; }
        public string grant_type { get; set; }
    }

	public class NlogConfig
	{
		public string client_id { get; set; }
		public string refresh_token { get; set; }
		public string client_secret { get; set; }
		public string grant_type { get; set; }
	}

    public class MinIoConfig
    {
        public string EndPoint { get; set; }
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public string BucketName { get; set; }
        public string Location { get; set; }
        public bool MinIoForDev { get; set; }
        public string FilePath { get; set; }
    }

    public class DBConnectionString
    {
        public string sqlConnection { get; set; }
        public string SqlLangConnection { get; set; }
        public string FileDBConnection { get; set; }
        public string mongoConnection { get; set; }
        public string mongoDb { get; set; }

    }

    public class CVParsingConfig
    {
        public string BaseURL { get; set; }
        public string AccountID { get; set; }
        public string ServiceKey { get; set; }
    }


    }

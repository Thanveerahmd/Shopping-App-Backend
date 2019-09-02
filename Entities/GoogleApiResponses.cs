using Newtonsoft.Json;

namespace pro.backend.Entities
{
    public class GoogleApiResponses
    {

    }
    internal class GoogleUserData
    {
        [JsonProperty("kind")]
        public string Kind { get; set; }

        [JsonProperty("etag")]
        public string ETag { get; set; }

        [JsonProperty("emails")]
        public GoogleEmail[] Emails { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("name")]
        public GoogleName Name { get; set; }

        [JsonProperty("image")]
        public GooglePicture Picture { get; set; }

        [JsonProperty("language")]
        public string language { get; set; }
    }


    internal class GooglePicture
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("isDefault")]
        public bool IsDefault { get; set; }
    }

    internal class GoogleName
    {
        [JsonProperty("familyName")]
        public string FamilyName { get; set; }

        [JsonProperty("givenName")]
        public string GivenName { get; set; }
    }

    internal class GoogleEmail
    {
        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }
    internal class GoogleUserAccessTokenData
    {
        [JsonProperty("issued_to")]
        public string IssuedTo { get; set; }

        [JsonProperty("audience")]
        public string Audience { get; set; }

        [JsonProperty("user_id")]
        public string UserId { get; set; }

        [JsonProperty("scope")]
        public string Scope { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("verified_email")]
        public bool VerifiedEmail { get; set; }

        [JsonProperty("access_type")]
        public string AccessType { get; set; }

    }

    internal class GoogleUserAccessTokenValidation
    {
        [JsonProperty("issued_to")]
        public string IssuedTo { get; set; }

        [JsonProperty("audience")]
        public string Audience { get; set; }

        [JsonProperty("user_id")]
        public string UserId { get; set; }

        [JsonProperty("scope")]
        public string Scope { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("verified_email")]
        public bool VerifiedEmail { get; set; }

        [JsonProperty("access_type")]
        public string AccessType { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("error_description")]
        public string ErrorDescription { get; set; }
    }

    internal class GoogleUserAccessTokenValidationErrors
    {
        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("error_description")]
        public string ErrorDescription { get; set; }

    }

}


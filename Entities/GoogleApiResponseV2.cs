using Newtonsoft.Json;

namespace pro.backend.Entities
{
     public class GoogleApiResponsesV2
    {

    }

    internal class GoogleUserDataV2{
        [JsonProperty("kid")]
        public string UserId { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }
        // "email": "mahdhirezvi@gmail.com",

        [JsonProperty("email_verified")]
        public string EmailVerified { get; set; }

        // "email_verified": "true",

         [JsonProperty("name")]
        public string Name { get; set; }
        //  "name": "Mahdhi Rezvi",

        [JsonProperty("picture")]
        public string PictureUrl { get; set; }
        // "picture": "https://lh3.googleusercontent.com/a-/AAuE7mB7naO786JqohcHA2HDJRDzrdSEmIGiTXbZr69Qrw=s96-c",

        [JsonProperty("given_name")]
        public string GivenName { get; set; }
        
        // "given_name": "Mahdhi",

        [JsonProperty("family_name")]
        public string FamilyName { get; set; }
        // "family_name": "Rezvi",

        [JsonProperty("locale")]
        public string Locale { get; set; }
        //  "locale": "en",
    }
}
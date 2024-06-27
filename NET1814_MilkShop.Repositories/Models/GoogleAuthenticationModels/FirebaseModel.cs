using Newtonsoft.Json;

namespace NET1814_MilkShop.Repositories.Models.GoogleAuthenticationModels;

public class FirebaseModel
{
    public Identities Identities { get; set; }
}

public class Identities
{
    [JsonProperty("google.com")] public List<string> GoogleId { get; set; }
}
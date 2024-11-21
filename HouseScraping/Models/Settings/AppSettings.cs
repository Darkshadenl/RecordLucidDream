namespace HouseScraping.Models.Settings;

public class AppSettings
{
    public required ApiKeys ApiKeys { get; set; }

}

public class ApiKeys
{
    public required string OpenAIKey { get; set; }
}

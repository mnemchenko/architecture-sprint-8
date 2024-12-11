namespace Handler.WebApi.Settings;

public class JwtSettings
{
    public required string Authority { get; set; }
    
    public required string Audience { get; set; } 
}
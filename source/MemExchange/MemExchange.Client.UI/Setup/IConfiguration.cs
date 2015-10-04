namespace MemExchange.Client.UI.Setup
{
    public interface IConfiguration
    {
        int ClientId { get; set; }
        string ServerAddress { get; set; }
        int ServerCommandPort { get; set; }
        int ServerPublishPort { get; set; }
    }
}
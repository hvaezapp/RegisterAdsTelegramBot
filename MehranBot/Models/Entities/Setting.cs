using MehranBot.Models.Common;

namespace MehranBot.Models.Entities;

public class Setting : BaseEntity<int>
{
    public string? WelcomeMessage { get; set; }
    public string? StartBotText { get; set; }
    public string? RulesMessage { get; set; }
    public long AdvertiseFee { get; set; }
    public string? BotToken { get; set; }
    public string? BotUsername { get; set; }
    public string? TargetChannelUserName { get; set; }
    public string? TargetChannelId { get; set; }
    public long AdminChatId { get; set; }
    public string? AdminUsername { get; set; }

}

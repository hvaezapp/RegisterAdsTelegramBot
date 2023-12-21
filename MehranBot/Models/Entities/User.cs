using MehranBot.Models.Common;
using MehranBot.Utility;
using System.ComponentModel.DataAnnotations.Schema;

namespace MehranBot.Models.Entities;

public class User : BaseEntity<long>
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? UserName { get; set; }
    public long ChatId { get; set; }
    public bool IsAdmin { get; set; }
    public bool IsConfirmedRules { get; set; }
    public bool IsMembership { get; set; }





    //nav property
    public ICollection<UserActivity> UserActivities { get; set; }
    public ICollection<Ads> Ads { get; set; }




}

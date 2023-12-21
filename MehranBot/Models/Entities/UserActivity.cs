using MehranBot.Models.Common;
using MehranBot.Utility;
using System.ComponentModel.DataAnnotations.Schema;

namespace MehranBot.Models.Entities;

public class UserActivity : BaseEntity<long>
{

    public long FkUserId { get; set; }

    public string Value { get; set; }

    public byte StepCount { get; set; }

    public ActivityType ActivityType { get; set; }



    //nav property

    [ForeignKey(nameof(FkUserId))]
    public User User { get; set; }

}

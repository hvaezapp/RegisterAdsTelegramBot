using MehranBot.Models.Common;
using MehranBot.Utility;
using System.ComponentModel.DataAnnotations.Schema;

namespace MehranBot.Models.Entities;

public class Ads : BaseEntity<long>
{

    public string Text { get; set; }
    public int FkCategoryId { get; set; }
    public long FkUserId { get; set; }
    public long MessageId { get; set; }
    public string Token { get; set; } 
    public bool IsPayed { get; set; }
    public bool IsAssigned { get; set; }
    public bool IsConfirmed { get; set; }

    public Ads()
    {
        Token  = AppUtility.GenerateGuid();
    }



    //nav property

    [ForeignKey(nameof(FkCategoryId))]
    public Category Category { get; set; }


    [ForeignKey(nameof(FkUserId))]
    public User User { get; set; }


}

using MehranBot.Models.Common;

namespace MehranBot.Models.Entities;

public class Category : BaseEntity<int>
{
    public string Title { get; set; }



    // nav property
    public ICollection<Ads> Ads { get; set; }

}

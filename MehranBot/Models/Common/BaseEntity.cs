using MehranBot.Utility;
namespace MehranBot.Models.Common;

public class BaseEntity<T> where T : struct
{
    public T Id { get; set; }
    public DateTime CreateDateMl { get; set; }
    public string CreateDateSh { get; set; }
    public bool IsEnable { get; set; }

    public BaseEntity()
    {
        CreateDateMl = DateTime.Now;
        CreateDateSh = DateAndTimeShamsi.DateTimeShamsi();
        IsEnable = true;
    }

}

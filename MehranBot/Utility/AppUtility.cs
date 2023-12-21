using MehranBot.Models.Entities;
using System.Text.RegularExpressions;
using Telegram.Bot.Types.ReplyMarkups;

namespace MehranBot.Utility;

public static class AppUtility
{
    
    public static bool IsNumber(this string value)
    {
        foreach (var valueItem in value)
        {
            if(!char.IsNumber(valueItem))
               return false;
            
        }
        return true;
    }


    public static string GetCategoryHashTag(string value)
    {
        return $"#{value.Replace(" ", "_")}";
    }

    public static string GenerateGuid()
    {
        return Guid.NewGuid().ToString().Replace("-", "");
    }

}

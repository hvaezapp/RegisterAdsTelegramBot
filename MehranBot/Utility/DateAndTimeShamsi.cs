using System;
using System.Globalization;

namespace MehranBot.Utility;

public static class DateAndTimeShamsi
{


    public static string DateTimeShamsi()
    {
        var currentDate = DateTime.Now;

        PersianCalendar pcCalender = new PersianCalendar();

        int year = pcCalender.GetYear(currentDate);
        int month = pcCalender.GetMonth(currentDate);
        int day = pcCalender.GetDayOfMonth(currentDate);
        int hour = pcCalender.GetHour(currentDate);
        int min = pcCalender.GetMinute(currentDate);
        int sec = pcCalender.GetSecond(currentDate);


        return String.Format("{0:yyyy/M/d HH:mm:ss}", 
                                  new DateTime(year, month, day,
                                                hour, min, sec));


    }



    public static string ToPersianDateTimeString(this DateTime dateTime)
    {
        PersianCalendar pc = new PersianCalendar();

        int year = pc.GetYear(dateTime);
        int month = pc.GetMonth(dateTime);
        int day = pc.GetDayOfMonth(dateTime);


        int hour = pc.GetHour(dateTime);
        int min = pc.GetMinute(dateTime);
        int sec = pc.GetSecond(dateTime);


        return String.Format("{0:yyyy/M/d HH:mm:ss}", new DateTime(year, month, day, hour, min, sec));
    }




    public static DateTime ToMiladiDate(this DateTime dt)
    {
        PersianCalendar pc = new PersianCalendar();
        return pc.ToDateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0, 0);
    }



}

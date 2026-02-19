namespace KickBlastStudentUI.Helpers;

public static class DateHelper
{
    public static DateTime GetSecondSaturday(int year, int month)
    {
        var date = new DateTime(year, month, 1);
        while (date.DayOfWeek != DayOfWeek.Saturday)
            date = date.AddDays(1);
        return date.AddDays(7);
    }
}

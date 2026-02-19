using System.Globalization;

namespace KickBlastStudentUI.Helpers;

public static class CurrencyHelper
{
    public static string Format(decimal value)
    {
        return $"LKR {value.ToString("N2", CultureInfo.InvariantCulture)}";
    }
}

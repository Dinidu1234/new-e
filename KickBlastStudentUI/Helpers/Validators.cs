namespace KickBlastStudentUI.Helpers;

public static class Validators
{
    public static string Required(string value, string field)
    {
        return string.IsNullOrWhiteSpace(value) ? $"{field} is required." : string.Empty;
    }

    public static string DecimalRange(string value, decimal min, decimal max, string field)
    {
        if (!decimal.TryParse(value, out var parsed))
            return $"{field} must be a number.";
        if (parsed < min || parsed > max)
            return $"{field} must be between {min} and {max}.";
        return string.Empty;
    }
}

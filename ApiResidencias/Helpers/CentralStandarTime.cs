namespace ApiResidencias.Helpers
{
    public static class CentralStandarTime
    {
        public static DateTime ToMexicoTime(this DateTime dateTime)
        {
            return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(dateTime, "Central America Standard Time");
        }
    }
}

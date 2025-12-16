namespace abfi_weighing_scale_api.Helpers
{
    public class TimeHelper
    {
        public static DateTime GetPhilippineStandardTime()
        {
            var phpTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time");
            DateTime utcNow = DateTime.UtcNow;
            DateTime phpTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, phpTimeZone);
            return phpTime;
        }
    }
}

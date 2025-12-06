namespace AspNetExample.Shared;

internal class DateTimeProvider : IDateTimeProvider
{
    public DateTime GetDateTime() => DateTime.UtcNow;
}
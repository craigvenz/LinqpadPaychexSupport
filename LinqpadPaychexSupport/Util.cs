using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinqpadPaychexSupport
{
    internal static class Util
    {
        internal static string LpDataFolder { get; } = Path.Combine(
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LINQPad"),
            "UserData"
        );

        internal static string HowLongAgo(DateTime when)
        {
            string plural(int x) => x == 1 ? string.Empty : "s";
			
            var ts = DateTime.UtcNow.Subtract(when.ToUniversalTime());
            var (secs, mins, hrs) = ((int)ts.TotalSeconds, (int)ts.TotalMinutes, (int)ts.TotalHours);
            if (secs <= 3)
                return "Just now";
            if (secs < 60)
                return $"{secs} second{plural(secs)} ago";
            if (mins < 60)
                return $"{mins} minute{plural(mins)} ago";
            if (hrs < 24)
                return $"{hrs} hour{plural(hrs)} ago";
            var local = when.ToLocalTime();
            var days = (int)ts.TotalDays;
            if (days < 2)
                return $"Yesterday at {local.ToString("h:mm tt")}";
            if (days <= 7)
                return $"{(days == 7 ? "Last " : string.Empty)}{local.ToString("dddd")} at {local.ToString("h:mm tt")}";
            return $"{local.ToLongDateString()} at {local.ToShortTimeString()}";
        }

    }
}

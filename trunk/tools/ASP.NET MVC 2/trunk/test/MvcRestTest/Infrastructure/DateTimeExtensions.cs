using System;
using System.Globalization;
using System.Text;

namespace MovieApp.Infrastructure {
    public static class DateTimeExtensions {
        public static string ToJsonString(this DateTime value) {
            DateTime time = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            long unixEpochTicks = time.Ticks;
            StringBuilder result = new StringBuilder(((value.ToUniversalTime().Ticks - unixEpochTicks) / 10000L).ToString());
            switch (value.Kind) {
                case DateTimeKind.Unspecified:
                case DateTimeKind.Local:
                    TimeSpan utcOffset = TimeZone.CurrentTimeZone.GetUtcOffset(value.ToLocalTime());
                    if (utcOffset.Ticks >= 0) {
                        result.Append("+");
                    }
                    else {
                        result.Append("-");
                    }
                    int num2 = Math.Abs(utcOffset.Hours);
                    result.Append((num2 < 10) ? ("0" + num2) : num2.ToString(CultureInfo.InvariantCulture));
                    int num3 = Math.Abs(utcOffset.Minutes);
                    result.Append((num3 < 10) ? ("0" + num3) : num3.ToString(CultureInfo.InvariantCulture));
                    break;
            }
            return result.ToString();
        }
    }
}

using System;

namespace Consumentor.ShopGun
{
    public static class TimeSpanExtensions
    {
        public static TimeSpan Days(this int time)
        {
            return new TimeSpan(time, 0, 0, 0, 0);
        }

        public static TimeSpan Hours(this int time)
        {
            return new TimeSpan(time, 0, 0);
        }

        public static TimeSpan Minutes(this int time)
        {
            return new TimeSpan(0, time, 0);
        }

        public static TimeSpan Seconds(this int time)
        {
            return new TimeSpan(0, 0, time);
        }

        public static TimeSpan Milliseconds(this int time)
        {
            return new TimeSpan(0, 0, 0, 0, time);
        }

        public static TimeSpan Half(this TimeSpan time)
        {
            return new TimeSpan(time.Ticks / 2);
        }

        public static TimeSpan Quarter(this TimeSpan time)
        {
            return new TimeSpan(time.Ticks / 4);
        }
    }
}
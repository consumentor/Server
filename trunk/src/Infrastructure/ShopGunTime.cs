using System;

namespace Consumentor.ShopGun
{
    /// <summary>
    /// Provides the time in a controllable way
    /// </summary>
    public static class ShopGunTime
    {
        private static readonly object _lockOnMe = new object();

        //private static DateTime? _nowSetTime;
        private static DateTime _nowTime = DateTime.Now;
        private static DateTime _whenNowTimeWasSet = DateTime.Now;
        private static DateTime _nowUtcTime = DateTime.UtcNow;
        private static DateTime _whenNowUtcTimeWasSet = DateTime.UtcNow;
        private static bool _isNowSet;
        private static bool _isAlwaysNowSet;


        /// <summary>
        /// Whether "Now" has been set
        /// </summary>
        public static bool IsNowSet
        {
            get { return _isNowSet; }
            private set { _isNowSet = value; }
        }


        /// <summary>
        /// Whether "Now" has been set to always return a specific value
        /// </summary>
        public static bool IsAlwaysNowSet
        {
            get { return _isAlwaysNowSet; }
            private set { _isAlwaysNowSet = value; }
        }

        /// <summary>
        /// Returns DateTime.Now, unless Now or AlwaysNow has been set
        /// </summary>
        public static DateTime Now
        {
            get
            {
                lock (_lockOnMe)
                {
                    if (IsAlwaysNowSet)
                    {
                        return _nowTime;
                    }
                    if (IsNowSet)
                    {
                        var timePassed = new TimeSpan((DateTime.Now.Ticks - _whenNowTimeWasSet.Ticks));
                        return _nowTime.Add(timePassed);
                    }
                    return DateTime.Now;
                }
            }
        }

        /// <summary>
        /// Returns DateTime.UtcNow, unless Now or AlwaysNow has been set
        /// </summary>
        public static DateTime UtcNow
        {
            get
            {
                lock (_lockOnMe)
                {
                    if (IsAlwaysNowSet)
                    {
                        return _nowUtcTime;
                    }
                    if (IsNowSet)
                    {
                        var timePassed = new TimeSpan((DateTime.UtcNow.Ticks - _whenNowUtcTimeWasSet.Ticks));
                        return _nowUtcTime.Add(timePassed);
                    }
                    return DateTime.UtcNow;
                }
            }
        }

        /// <summary>
        /// Sets a new "Now"-value. Now is then set to the specified value but will still be running
        /// </summary>
        /// <param name="hour"></param>
        /// <param name="minute"></param>
        public static void SetNow(int hour, int minute)
        {
            SetNow(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hour, minute, 0));
        }

        /// <summary>
        /// Sets a new "Now"-value. Now is then set to the specified value but will still be running
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <param name="hour"></param>
        /// <param name="minute"></param>
        /// <param name="second"></param>
        public static void SetNow(int year, int month, int day, int hour, int minute, int second)
        {
            SetNow(new DateTime(year, month, day, hour, minute, second));
        }

        /// <summary>
        /// Sets a new "Now"-value. Now is then set to the specified value but will still be running
        /// </summary>
        /// <param name="time"></param>
        public static void SetNow(DateTime time)
        {
            lock (_lockOnMe)
            {
                _nowTime = time;
                _whenNowTimeWasSet = DateTime.Now;
                _nowUtcTime = time.Add(DateTime.UtcNow.Subtract(DateTime.Now));
                _whenNowUtcTimeWasSet = DateTime.UtcNow;
                IsNowSet = true;
                IsAlwaysNowSet = false;
            }
        }

        /// <summary>
        /// Sets a new "AlwaysNow"-value. Now will return this value every time it is called
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <param name="hour"></param>
        /// <param name="minute"></param>
        /// <param name="second"></param>
        public static void SetAlwaysNow(int year, int month, int day, int hour, int minute, int second)
        {
            SetAlwaysNow(new DateTime(year, month, day, hour, minute, second));
        }

        /// <summary>
        /// Sets a new "AlwaysNow"-value. Now will return this value every time it is called
        /// </summary>
        /// <param name="time"></param>
        public static void SetAlwaysNow(DateTime time)
        {
            lock (_lockOnMe)
            {
                _nowTime = time;
                _whenNowTimeWasSet = DateTime.Now;
                _nowUtcTime = time.Add(DateTime.UtcNow.Subtract(DateTime.Now));
                _whenNowUtcTimeWasSet = DateTime.UtcNow;
                IsNowSet = true;
                IsAlwaysNowSet = true;
            }
        }
    }

}
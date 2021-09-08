using System;

namespace Lib.Utils
{
    public static class TimeUtils
    {
        public static long GetNowTime()
        {
            return DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExaminationSystem.Utils
{
    public static class TimeHelper
    {
        /// <summary>
        /// js时间戳转C#时间
        /// </summary>
        /// <param name="jsTimeStamp"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(long jsTimeStamp)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            DateTime dt = startTime.AddMilliseconds(jsTimeStamp);

            return dt;
        }


        /// <summary>
        /// C#转js时间戳
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string ToJsTime(DateTime time)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)); // 当地时区
            long timeStamp = (long)(time - startTime).TotalMilliseconds; // 相差毫秒数
            return timeStamp.ToString();
        }

        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <returns></returns>
        public static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalMilliseconds).ToString();
        }
    }
}
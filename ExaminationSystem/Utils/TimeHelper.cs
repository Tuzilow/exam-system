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
    }
}
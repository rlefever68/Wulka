using System;
using System.Diagnostics;
using System.Globalization;

namespace Wulka.Utils
{
    public static class StopwatchExtensions
    {
		/// <summary>
		/// Return the interval as a string in the format 'n.nnn Seconds'.
		/// </summary>
		/// <returns>duration as a string</returns>
		public static string ToStringInSeconds(this Stopwatch stopwatch)
		{
		    double elapsedSeconds = stopwatch.ElapsedMilliseconds/1000.0;
            return String.Format("{0} sec.", elapsedSeconds.ToString("N03", CultureInfo.CurrentCulture));
		}

		/// <summary>
		/// Return the interval as a string in the format 'm:n.nnn Seconds'.
		/// </summary>
		/// <returns>duration as a string</returns>
		public static string ToStringInMinutesAndSeconds(this Stopwatch stopwatch)
		{
            long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
		    long elapsedSeconds = (elapsedMilliseconds == 0) ? 0 : (long)Math.Truncate(elapsedMilliseconds/1000.0);
		    long elapsedMinutes = (elapsedSeconds == 0.0) ? 0 : (long)Math.Truncate(elapsedSeconds/60.0);
		    elapsedMilliseconds %= 1000;
		    elapsedSeconds %= 60;
            if (elapsedMinutes == 0)
            {
                return String.Format("{0}.{1:D3} sec.", elapsedSeconds, elapsedMilliseconds);
            }
            return String.Format("{0}:{1:D2}.{2:D3} min.", elapsedMinutes, elapsedSeconds, elapsedMilliseconds);
		}
    }
}

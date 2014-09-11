using System;

namespace Wulka.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class Convertor
    {
        /// <summary>
        /// Toes the int32.
        /// </summary>
        /// <param name="sValue">The s value.</param>
        /// <returns></returns>
        public static int ToInt32(string sValue)
        {
            return ToInt32(sValue, 0);
        }

        /// <summary>
        /// Toes the byte.
        /// </summary>
        /// <param name="sValue">The s value.</param>
        /// <returns></returns>
        public static byte ToByte(string sValue)
        {
            return (byte)ToInt32(sValue);
        }

        /// <summary>
        /// Toes the int32.
        /// </summary>
        /// <param name="sValue">The s value.</param>
        /// <param name="_default">The _default.</param>
        /// <returns></returns>
        public static int ToInt32(string sValue, int _default)
        {
            if (sValue == null || sValue == "")
                return _default;
            int result = _default;
            if (int.TryParse(sValue, out result))
                return result;
            else
                return _default;
        }

        /// <summary>
        /// Toes the date time.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="_default">The _default.</param>
        /// <returns></returns>
        public static DateTime ToDateTime(string date, DateTime _default)
        {
            if (date == null || date == "")
                return _default;
            DateTime result = DateTime.Now;
            if (DateTime.TryParse(date, out result))
                return result;
            else
                return _default;
        }

        /// <summary>
        /// Toes the double.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public static double ToDouble(string expression)
        {
            return ToDouble(expression, 0.0);
        }

        /// <summary>
        /// Toes the double.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="_default">The _default.</param>
        /// <returns></returns>
        public static double ToDouble(string expression, double _default)
        {
            if (expression == null || expression == "")
                return _default;
            double result = _default;
            if (double.TryParse(expression, out result))
                return result;
            else
                return _default;

        }

        /// <summary>
        /// Toes the bool.
        /// </summary>
        /// <param name="pValue">The p value.</param>
        /// <returns></returns>
        public static bool ToBool(string pValue)
        {
            return pValue.ToLower() == "true" ? true : false;
        }

        /// <summary>
        /// Gets the type of the expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public static Type GetExpressionType(string expression)
        {
            Type result = typeof(int);
            foreach (char chr in expression)
            {
                if (Char.IsDigit(chr))
                    continue;
                if (Char.IsPunctuation(chr))
                {
                    result = typeof(double);
                    continue;
                }
                result = typeof(string);
                break;
            }
            return result;
        }

        /// <summary>
        /// Toes the value.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public static object ToValue(string expression)
        {
            Type exp_type = GetExpressionType(expression);
            if (exp_type == typeof(int))
                return ToInt32(expression);
            if (exp_type == typeof(double))
                return ToDouble(expression);
            return expression;
        }

        /// <summary>
        /// Toes the double.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static double ToDouble(object value)
        {
            return Convert.ToDouble(value);
        }

        /// <summary>
        /// Toes the int32.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static Int32 ToInt32(object value)
        {
            return ToInt32(value.ToString(), 0);
        }


    }
}

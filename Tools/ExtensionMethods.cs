using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPConnect_Testing
{
    public static class ExtensionMethods
    {

        public static String DateStringToServerDate(this string value)
        {
            //assumes dd/mm/yyyy
            string[] split = value.Split('/');
            if (split.Length != 3) { return null; }

            if (split[2].Length == 2) { split[2] = "20" + split[2]; }
            if (split[1].Length == 1) { split[1] = "0" + split[1]; }
            if (split[0].Length == 1) { split[1] = "0" + split[0]; }

            return split[2] + "-" + split[1] + "-" + split[0];
        }

        public static String ToStringIncNull(this string value)
        {
            if (value == null) { return String.Empty; }
            return value.ToString();
        }

        public static int? StringToIntOrNull(this string value)
        {
            if (value == null || value == String.Empty || value.Equals(String.Empty)) { return null; }
            int ret = -1;

            int x;
            if (Int32.TryParse(value, out x))
            {
                ret = x;
            }

            if (ret == -1)
            {
                return null;
            }
            else
            {
                return ret;
            }

        }

        public static String IntNullToString(this Int32? value)
        {
            if (value == null) { return String.Empty; }
            return value.ToString();
        }

        public static String ToStringOrNull(this Int32? value)
        {
            if (value == null) { return String.Empty; }
            return value.ToString();
        }

        public static String ToStringOrNull(this Double? value)
        {
            if (value == null) { return String.Empty; }
            return value.ToString();
        }

        public static String ToStringOrNull(this Decimal? value)
        {
            if (value == null) { return String.Empty; }
            return value.ToString();
        }

        public static String ToTitle(this String value)
        {
            return value.Substring(0, 1).ToUpper() + value.Substring(1);
        }

        public static bool StringToBool(this string value)
        {
            bool ret = false;

            bool x;
            if (bool.TryParse(value, out x))
            {
                ret = x;
            }

            if (!ret && (value == "0" || value == "1"))
            {
                ret = value == "1" ? true : false;
            }

            return ret;
        }

        public static bool? StringToBoolOrNull(this string value)
        {
            if (value == null || value == String.Empty || value.Equals(String.Empty)) { return null; }

            bool ret = false;

            bool x;
            if (bool.TryParse(value, out x))
            {
                ret = x;
            }

            if (!ret && (value == "0" || value == "1"))
            {
                ret = value == "1" ? true : false;
            }

            return ret;
        }

        public static Decimal StringToDec(this string value)
        {
            decimal ret = -1;

            decimal x;
            if (Decimal.TryParse(value, out x))
            {
                ret = x;
            }

            return ret;
        }

        public static Double ToDouble(this string value)
        {
            Double ret = -1;

            Double x;
            if (Double.TryParse(value, out x))
            {
                ret = x;
            }

            return ret;
        }

        public static Double StringToDouble(this string value)
        {
            Double ret = -1;

            Double x;
            if (Double.TryParse(value, out x))
            {
                ret = x;
            }

            return ret;
        }

        public static Double? ToDoubleOrNull(this string value)
        {
            if (value == null || value == String.Empty || value.Equals(String.Empty)) { return null; }
            Double ret = -1;

            Double x;
            if (Double.TryParse(value, out x))
            {
                ret = x;
            }

            return ret;
        }

        public static Double? StringToDoubleOrNull(this string value)
        {
            if (value == null || value == String.Empty || value.Equals(String.Empty)) { return null; }
            Double ret = -1;

            Double x;
            if (Double.TryParse(value, out x))
            {
                ret = x;
            }

            return ret;
        }

        public static Decimal? StringToDecOrNull(this string value)
        {
            if (value == null || value == String.Empty || value.Equals(String.Empty)) { return null; }
            Decimal ret = -1;

            Decimal x;
            if (Decimal.TryParse(value, out x))
            {
                ret = x;
            }

            return ret;
        }

        public static Decimal ToDecimal(this decimal? value)
        {
            if (value == null) { return 0; }
            return (decimal)value;
        }

        public static Int32 ToInt(this Int32? value)
        {
            if (value == null) { return 0; }
            return (Int32)value;
        }

        public static Boolean isInt(this Int32? value)
        {
            if (value == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static Boolean ToBool(this Boolean? value)
        {
            if (value == null)
            {
                return false;
            }
            else
            {
                return (Boolean)value;
            }
        }

        public static Decimal? StringToDecNull(this string value)
        {

            decimal x;
            if (Decimal.TryParse(value, out x))
            {
                return x;
            }

            return null;
        }

        public static string ToMoneyFormat(this Decimal value)
        {
            return value.ToString("#,##0.00");
        }

        public static bool isDecimal(this string value)
        {
            decimal x;
            if (Decimal.TryParse(value, out x))
            {
                return true;
            }
            return false;
        }

        public static bool isInt(this string value)
        {
            Int16 x;
            if (Int16.TryParse(value, out x))
            {
                return true;
            }
            return false;
        }

        public static bool isBigInt(this string value)
        {
            Int32 x;
            if (Int32.TryParse(value, out x))
            {
                return true;
            }
            return false;
        }

        public static bool isInt32(this string value)
        {
            Int32 x;
            if (Int32.TryParse(value, out x))
            {
                return true;
            }
            return false;
        }

        public static Boolean isDecimal(this decimal? value)
        {
            return value == null ? false : true;
        }

        public static Int32 StringToInt(this string value)
        {

            int ret = -1;

            int x;
            if (Int32.TryParse(value, out x))
            {
                ret = x;
            }
            return ret;

        }

        public static Int32? StringToIntNull(this string value)
        {
            int x;
            if (Int32.TryParse(value, out x))
            {
                return x;
            }
            return null;
        }

        public static DateTime StringToDate(this string value)
        {
            DateTime ret = new DateTime();
            DateTime x;
            if (DateTime.TryParse(value, out x))
            {
                ret = x;
            }

            return x;
        }

        public static string ToUKString(this DateTime value)
        {
            return value.ToString("dd'/'MM'/'yyyy");
        }

        public static string DateStamp()
        {
            DateTime now = DateTime.Now;

            return now.Year.ToString() +
                        now.Month.ToString() +
                        now.Day.ToString() +
                        now.Hour.ToString() +
                        now.Minute.ToString() +
                        now.Second.ToString() +
                        now.Millisecond.ToString();

        }

        public static string[] InsertReturns(this string value)
        {
            return value.Split(new string[] { "^p" }, StringSplitOptions.None);
        }

        public static string removeIllegalFilenameChars(this string value)
        {
            string st = value;
            string[] c = new string[12];
            c[0] = @"\";
            c[1] = @"/";
            c[2] = @":";
            c[3] = @"*";
            c[4] = @"?";
            c[5] = @"'";
            c[6] = @"<";
            c[7] = @">";
            c[8] = @"|";
            c[9] = @","; //not an illeagal file character but chrome cannot download files with this
            c[10] = @"\\";
            c[11] = @".";
            for (int i = 0; i < c.Length; i++)
            {
                st = st.Replace(c[i], " ");
            }

            return st;

        }//removeIllegalFilenameChars

        public static string ToTitleString(this string value)
        {
            if (value.Length < 2) { return value; }
            return value.Substring(0, 1).ToUpper() + value.Substring(1).ToLower();
        }

        public static int HexToInt(this string value)
        {
            return - int.Parse(value, System.Globalization.NumberStyles.HexNumber);
        }
        public static Int64 HexToNumber(this string value)
        {
            return Int64.Parse(value, System.Globalization.NumberStyles.HexNumber);
        }



    }
}

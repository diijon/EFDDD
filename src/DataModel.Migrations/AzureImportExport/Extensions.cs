using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace EFDDD.DataModel.Migrations.AzureImportExport
{
    public static class Extensions
    {
        public static Dictionary<string, string> ToParsedAzureStorageConnection(this string connectionString)
        {
            connectionString = connectionString ?? string.Empty;
            var value = new Dictionary<string, string>();
            try
            {
                var parsedList = connectionString.ParseDelimitedList();
                foreach (var item in parsedList)
                {
                    if (item.StartsWith("defaultendpointsprotocol=", StringComparison.CurrentCultureIgnoreCase))
                    {
                        value.Add("Protocol", Regex.Replace(item, @"defaultendpointsprotocol\=(.*)", "$1", RegexOptions.IgnoreCase));
                    }
                    else if (item.StartsWith("accountname=", StringComparison.CurrentCultureIgnoreCase))
                    {
                        value.Add("AccountName", Regex.Replace(item, @"accountname\=(.*)", "$1", RegexOptions.IgnoreCase));
                    }
                    else if (item.StartsWith("accountkey=", StringComparison.CurrentCultureIgnoreCase))
                    {
                        value.Add("AccountKey", Regex.Replace(item, @"accountkey\=(.*)", "$1", RegexOptions.IgnoreCase));
                    }
                }
                return value;
            }
            catch (Exception) { return value; }
        }

        public static IEnumerable<string> ParseDelimitedList(this string _this, string delimiter = ";", string regx = null)
        {
            _this = _this ?? string.Empty;
            delimiter = string.IsNullOrEmpty(delimiter) ? ";" : delimiter;
            List<string> value = new List<string>();
            if (_this.Contains(delimiter))
            {
                _this = Regex.Replace(_this, string.Format("({0})$", delimiter), string.Empty);
                char[] splitter = { Convert.ToChar(delimiter) };
                string[] splitResult = _this.Split(splitter);
                foreach (string item in splitResult)
                {
                    if (item != null && Regex.IsMatch(item, regx ?? "(.*?)"))
                    {
                        value.Add(item.Trim());
                    }
                }
            }
            else
            {
                if (Regex.IsMatch(_this, regx ?? "(.*?)"))
                {
                    value.Add(_this.Trim());
                }
            }

            return value;
        }
        public static TValue ParseEnum<TValue>(this string _this, TValue defaultValue, bool ignoreCase)
            where TValue : struct // enum 
        {
            try
            {
                if (String.IsNullOrEmpty(_this))
                    return defaultValue;
                return (TValue)Enum.Parse(typeof(TValue), _this, ignoreCase);
            }
            catch (Exception) { return defaultValue; }
        }
        public static TValue ParseEnum<TValue>(this string _this, TValue defaultValue)
            where TValue : struct // enum 
        {
            return ParseEnum(_this, defaultValue, false);
        }
    }
}

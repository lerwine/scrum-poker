using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace ScrumPoker.StandaloneServer.DataContracts
{
    public static class StringExtensions
    {
        public static readonly Encoding DefaultEncoding = new UTF8Encoding(false, false);

        public static string EmptyIfNullOrTrimmed(this string value) { return (value == null) ? "" : value.Trim(); }

        public static string TrimmedOrNullIfEmpty(this string value) { return (value != null && (value = value.Trim()).Length > 0) ? value : null; }

        public static string ToJsonString(this DateTime value) { return value.ToString("yyyy-MM-dd"); }
        
        public static string ToJsonString(this DateTime? value) { return value.HasValue ? ToJsonString(value.Value) : null; }
        
        public static DateTime? JsonStringToDate(this string value)
        {
            DateTime result;
            if ((value = value.TrimmedOrNullIfEmpty()) != null && DateTime.TryParse(value, out result))
                return result;
            return null;
        }
        
        public static T FromJSON<T>(this string jsonText) where T : class, new()
        {
            using (MemoryStream stream = new MemoryStream(DefaultEncoding.GetBytes(jsonText)))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                return serializer.ReadObject(stream) as T;
            }
        }

        public static string ToJSON<T>(this T value) where T : class, new()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                serializer.WriteObject(stream, value);
                byte[] bytes = stream.ToArray();
                return DefaultEncoding.GetString(bytes, 0, bytes.Length);
            }
        }
    }
}

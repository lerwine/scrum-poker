using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace ScrumPoker
{
    public static class ExtensionMethods
    {
        public static string JoinAsLines(this IEnumerable<string> source)
        {
            if (source == null)
                return null;
            using (IEnumerator<string> enumerator = source.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                    return null;
                string message = enumerator.Current ?? "";
                if (enumerator.MoveNext())
                {
                    StringBuilder sb = new StringBuilder().AppendLine(message);
                    message = enumerator.Current ?? "";
                    while (enumerator.MoveNext())
                    {
                        sb.AppendLine(message);
                        message = enumerator.Current ?? "";
                    }
                    return sb.Append(message).ToString();
                }
                return message;
            }
        }
        public static Dictionary<string, Tuple<string, string[]>> ToValidationMessageDictionary(this IEnumerable<ValidationResult> validationResults, PropertyDescriptorCollection propertyDescriptors)
        {
            return (validationResults == null) ? new Dictionary<string, Tuple<string, string[]>>() : validationResults.SelectMany(r => r.MemberNames.Where(n => !string.IsNullOrWhiteSpace(n))
                .Select(n => new { Name = n, ErrorMessage = r.ErrorMessage })).GroupBy(a => a.Name)
                .Select(g => new { Name = g.Key, Messages = g.Select(a => a.ErrorMessage).Distinct().ToArray() }).ToDictionary(a => a.Name, a =>
                {
                    PropertyDescriptor pd = propertyDescriptors[a.Name];
                    return new Tuple<string, string[]>((pd == null) ? a.Name : pd.DisplayName, a.Messages);
                });
        }

        public static List<string> UpdateValidation(this Dictionary<string, Tuple<string, string[]>> validationMessageDictionary, IEnumerable<ValidationResult> validationResults, PropertyDescriptorCollection propertyDescriptors)
        {
            Dictionary<string, Tuple<string, string[]>> dict = validationResults.ToValidationMessageDictionary(propertyDescriptors);
            List<string> errorsChanged;
            if (dict.Count == 0)
            {
                errorsChanged = validationMessageDictionary.Keys.ToList();
                validationMessageDictionary.Clear();
            }
            else if (validationMessageDictionary.Count == 0)
            {
                errorsChanged = dict.Keys.ToList();
                foreach (KeyValuePair<string, Tuple<string, string[]>> kvp in dict)
                    validationMessageDictionary.Add(kvp.Key, kvp.Value);
            }
            else
            {
                string[] errorsCleared = validationMessageDictionary.Keys.Where(k => !dict.ContainsKey(k)).ToArray();
                errorsChanged = dict.Where(kvp =>
                {
                    string key = kvp.Key;
                    Tuple<string, string[]> oldMessages;
                    if (validationMessageDictionary.TryGetValue(key, out oldMessages))
                    {
                        string[] newMessages = kvp.Value.Item2;
                        if (oldMessages.Item2.Any(m => !newMessages.Contains(m)) || newMessages.Any(m => !oldMessages.Item2.Contains(m)))
                        {
                            validationMessageDictionary[key] = new Tuple<string, string[]>(oldMessages.Item1, newMessages);
                            return true;
                        }
                        return false;
                    }
                    validationMessageDictionary.Add(key, kvp.Value);
                    return true;
                }).Select(kvp => kvp.Key).ToList();
                if (errorsCleared.Length > 0)
                {
                    foreach (string k in errorsCleared)
                        validationMessageDictionary.Remove(k);
                    errorsChanged.AddRange(errorsCleared);;
                }
            }
            return errorsChanged;
        }

        public static readonly Encoding DefaultEncoding = new UTF8Encoding(false, false);

        public static bool SetIfDifferentObject<T>(this T value, object syncRoot, ref T target)
            where T : class
        {
            Monitor.Enter(syncRoot);
            try
            {
                if ((value == null) ? target == null : target != null && ReferenceEquals(value, target))
                    return false;
                target = value;
            }
            finally { Monitor.Exit(syncRoot); }
            return true;
        }

        public static readonly Regex NonNormalizedWhiteSpaceRegex = new Regex(@"( |(?! ))[\r\n\s]+", RegexOptions.Compiled);

        public static string WsNormalized(this string value) { return (value != null && (value = value.Trim()).Length > 0) ? NonNormalizedWhiteSpaceRegex.Replace(value, " ") : ""; }

        public static string EmptyIfNullOrTrimmed(this string value) { return (value == null) ? "" : value.Trim(); }

        public static bool ToEmptyIfNullOrTrimmed(this string value, object syncRoot, ref string target)
        {
            Monitor.Enter(syncRoot);
            try
            {
                if ((value = EmptyIfNullOrTrimmed(value)) == target)
                    return false;
                target = value;
            }
            finally { Monitor.Exit(syncRoot); }
            return true;
        }

        public static bool ToWsNormalized(this string value, object syncRoot, ref string target)
        {
            Monitor.Enter(syncRoot);
            try
            {
                if ((value = WsNormalized(value)) == target)
                    return false;
                target = value;
            }
            finally { Monitor.Exit(syncRoot); }
            return true;
        }

        public static string TrimmedOrNullIfEmpty(this string value) { return (value != null && (value = value.Trim()).Length > 0) ? value : null; }

        public static string NullIfEmpty(this string value) { return (value != null && value.Length > 0) ? value : null; }

        public static bool ToTrimmedOrNullIfEmpty(this string value, object syncRoot, ref string target)
        {
            Monitor.Enter(syncRoot);
            try
            {
                if ((value = TrimmedOrNullIfEmpty(value)) == target)
                    return false;
                target = value;
            }
            finally { Monitor.Exit(syncRoot); }
            return true;
        }

        public static bool ToNullIfEmpty(this string value, object syncRoot, ref string target)
        {
            Monitor.Enter(syncRoot);
            try
            {
                if ((value = NullIfEmpty(value)) == target)
                    return false;
                target = value;
            }
            finally { Monitor.Exit(syncRoot); }
            return true;
        }

        public static bool SetIfDifferent(this bool value, object syncRoot, ref bool target)
        {
            Monitor.Enter(syncRoot);
            try
            {
                if (value == target)
                    return false;
                value = target;
            }
            finally { Monitor.Exit(syncRoot); }
            return true;
        }

        public static bool SetIfDifferent(this int value, object syncRoot, ref int target)
        {
            Monitor.Enter(syncRoot);
            try
            {
                if (value == target)
                    return false;
                value = target;
            }
            finally { Monitor.Exit(syncRoot); }
            return true;
        }

        public static bool SetIfDifferent(this Guid value, object syncRoot, ref Guid target)
        {
            Monitor.Enter(syncRoot);
            try
            {
                if (value == target)
                    return false;
                value = target;
            }
            finally { Monitor.Exit(syncRoot); }
            return true;
        }

        public static Guid? NullIfEmpty(this Guid? value)
        {
            return (value.HasValue && value.Value.Equals(Guid.Empty)) ? null : value;
        }

        public static bool ToNullIfEmpty(this Guid? value, object syncRoot, ref Guid? target)
        {
            Monitor.Enter(syncRoot);
            try
            {
                if (value.HasValue && !value.Value.Equals(Guid.Empty))
                {
                    if (target.HasValue && target.Value.Equals(value.Value))
                        return false;
                }
                else if (!target.HasValue)
                    return false;
                value = target;
            }
            finally { Monitor.Exit(syncRoot); }
            return true;
        }

        public static bool SetIfDifferent(this int? value, object syncRoot, ref int? target)
        {
            Monitor.Enter(syncRoot);
            try
            {
                if (target.HasValue ? (value.HasValue && value.Value == target.Value) : !value.HasValue)
                    return false;
                value = target;
            }
            finally { Monitor.Exit(syncRoot); }
            return true;
        }

        public static DateTime ToLocalDate(this DateTime value)
        {
            switch (value.Kind)
            {
                case DateTimeKind.Utc:
                    value = value.ToLocalTime();
                    break;
                case DateTimeKind.Unspecified:
                    DateTime.SpecifyKind(value, DateTimeKind.Local);
                    break;
            }
            return (value.Date != value) ? value.Date : value;
        }

        public static bool ToLocalDate(this DateTime value, object syncRoot, ref DateTime target)
        {
            Monitor.Enter(syncRoot);
            try
            {
                DateTime x = value.ToLocalDate();
                if (x.Equals(target))
                    return false;
                target = x;
            }
            finally { Monitor.Exit(syncRoot); }
            return true;
        }

        public static DateTime? ToLocalDate(this DateTime? value)
        {
            if (value.HasValue)
                return (value.Value.Kind == DateTimeKind.Local) ? value.Value : value.Value.ToLocalTime();
            return null;
        }

        public static bool ToLocalDate(this DateTime? value, object syncRoot, ref DateTime? target)
        {
            Monitor.Enter(syncRoot);
            try
            {
                if (target.HasValue)
                {
                    if (value.HasValue)
                    {
                        DateTime x = value.Value.ToLocalDate();
                        if (x.Equals(target.Value))
                            return false;
                        target = x;
                    }
                    else
                        target = null;
                }
                else
                {
                    if (!value.HasValue)
                        return false;
                    DateTime x = value.Value.ToLocalDate();
                    target = x.Date.Equals(x) ? x : x.Date;
                }
            }
            finally { Monitor.Exit(syncRoot); }
            return true;
        }

        public static string ToJsonDateString(this DateTime value) { return value.ToString("yyyy-MM-dd"); }
        
        public static string ToJsonDateString(this DateTime? value) { return value.HasValue ? ToJsonDateString(value.Value) : null; }
        
        public static string ToJsonString(this Guid value) { return value.ToString("n"); }
        
        public static string ToJsonString(this Guid? value) { return value.HasValue ? ToJsonString(value.Value) : null; }
        
        public static Guid? JsonStringToGuid(this string value)
        {
            Guid result;
            if ((value = value.TrimmedOrNullIfEmpty()) != null && Guid.TryParse(value, out result))
                return result;
            return null;
        }
        
        public static Guid? JsonStringToGuidNotEmpty(this string value)
        {
            Guid result;
            if ((value = value.TrimmedOrNullIfEmpty()) != null && Guid.TryParse(value, out result) && !result.Equals(Guid.Empty))
                return result;
            return null;
        }
        
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

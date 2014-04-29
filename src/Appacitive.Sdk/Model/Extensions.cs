using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using Appacitive.Sdk.Services;

using System.Collections;
using Appacitive.Sdk.Internal;

namespace Appacitive.Sdk
{
    internal static class Extensions
    {
        public static string Escape(this string value)
        {
            if (value == null) return null;
            return Uri.EscapeDataString(value);
        }

        public static string ToDelimitedList(this IEnumerable<string> list, string delimiter)
        {
            StringBuilder buffer = new StringBuilder();
            list.For(item =>
                {
                    if (buffer.Length == 0)
                        buffer.Append(item);
                    else
                        buffer.Append(delimiter).Append(item);
                });
            return buffer.ToString();
        }

        public static void GetDifferences<T>(this IEnumerable<T> existing, IEnumerable<T> updated, out IEnumerable<T> added, out IEnumerable<T> removed)
        {
            var oldList = existing.ToList();
            var newList = updated.ToList();
            var common = newList.Intersect(oldList).ToList();
            added = newList.Except(common);
            removed = oldList.Except(common);
        }

        public static IEnumerable<ConstructorInfo> GetConstructors(this Type type)
        {
            #if (WINDOWS_PHONE7 || NET40)
            return type.GetConstructors() ?? new ConstructorInfo[]{};
            #else
            var typeInfo = type.GetTypeInfo();
            return typeInfo.DeclaredConstructors;
            #endif
            
        }

        public static IDictionary<string, string> FromQueryObject(this object obj)
        {
            if (obj == null)
                return null;
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            // Get list of properties and their values and convert to dictionary.
            #if (WINDOWS_PHONE7 || NET40)
            var type = obj.GetType();
            var properties = type.GetProperties().Where(p => p.CanRead == true);
            properties.For(x =>
            {
                var value = x.GetValue(obj, null);
                if (value != null)
                    result.Add(x.Name, value.ToString());
            });
            #else   
            var type = obj.GetType().GetTypeInfo();
            var properties = type.DeclaredProperties.Where(x => x.CanRead == true).ToList();
            properties.For(x =>
                {
                    var value = x.GetValue(obj);
                    if (value != null)
                        result.Add(x.Name, value.ToString());
                });
            #endif
            return result;
        }

        internal static string AsString(this object obj)
        {
            if (obj == null)
                return null;
            if (obj is IEnumerable && (obj is string == false))
            {
                var buffer = new StringBuilder();
                var enumerable = obj as IEnumerable;
                foreach (var item in enumerable)
                {
                    if (buffer.Length == 0)
                        buffer.Append(item);
                    else buffer.Append("|").Append(item);
                }
                return buffer.ToString();
            }
            else return obj.ToString();
        }

        public static IDictionary<TKey, TValue> GetModifications<TKey, TValue>(this IDictionary<TKey, TValue> current, IDictionary<TKey, TValue> old, Func<TValue, TValue, bool> isEqual)
        {
            var newClone = new Dictionary<TKey, TValue>(current);
            var oldClone = new Dictionary<TKey, TValue>(old);
            return new MapDiff().GetDifferences(newClone, oldClone, isEqual);
        }


        internal static bool RemoveAllOccurences(this List<string> list, string value)
        {   
            #if WINDOWS_PHONE7
            var initial = list.Count;
            while (list.Contains(value) == true)
                list.Remove(value);
            return list.Count != initial;
            #else
            return list.RemoveAll( x => x == value) > 0;
            #endif
        }

        internal static bool IsMultiValued(this object obj )
        {
            return
                obj != null &&
                obj.GetType().IsPrimitiveType() == false &&
                obj is IEnumerable;
        }

        public static void For<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var item in enumerable)
                action(item);
        }

        public static bool Is<T>(this Type type)
        {
            #if (WINDOWS_PHONE7 || NET40)
            return typeof(T).IsAssignableFrom(type);
            #else
            return typeof(T).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo());
            #endif
        }

        public static bool IsEnumeration(this Type type)
        {
            #if (WINDOWS_PHONE7 || NET40)
            return type.IsEnum;
            #else
            return type.GetTypeInfo().IsEnum;
            #endif
        }

        public static bool IsGeneric(this Type type)
        {
            #if (WINDOWS_PHONE7 || NET40)
            return type.IsGenericType;
            #else
            return type.GetTypeInfo().IsGenericType;
            #endif
        }

        public static bool IsPrimitiveType(this Type type)
        {
            if (type.Is<string>() || type.Is<DateTime>())
                return true;
            #if (WINDOWS_PHONE7 || NET40)
            return type.IsValueType || type.IsPrimitive;
            #else
            return type.GetTypeInfo().IsValueType || type.GetTypeInfo().IsPrimitive;
            #endif
        }

        public static PropertyInfo[] GetPropertyInfos(this Type type)
        {
            #if (WINDOWS_PHONE7 || NET40)
            var properties = type.GetProperties();
            #else
            var properties = type.GetRuntimeProperties().ToArray();
            #endif
            return properties;
        }

        public static Exception ToFault(this Status status)
        {
            if (status == null || status.IsSuccessful == true)
                return null;
            return new AppacitiveApiException(status.Message)
            {
                Code = status.Code,
                ReferenceId = status.ReferenceId,
                FaultType = status.FaultType,
                AdditionalMessages = status.AdditionalMessages == null ? null : status.AdditionalMessages.ToArray()
            };
        }

        public static HttpOperation WithApiKey(this HttpOperation client, string apiKey)
        {       
            return client.WithHeader("Appacitive-Apikey", apiKey);
        }

        public static HttpOperation WithEnvironment(this HttpOperation client, Environment env)
        {   
            return client.WithHeader("Appacitive-Environment", env.ToString());
        }

        public static HttpOperation WithUserToken(this HttpOperation client, string userToken)
        {       
            if( userToken != null )
                client.WithHeader("Appacitive-User-Auth", userToken);
            return client;
        }

    }
}

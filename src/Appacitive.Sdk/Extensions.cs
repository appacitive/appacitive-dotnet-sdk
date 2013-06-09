using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using Appacitive.Sdk.Services;
using Appacitive.Sdk.Realtime;
using System.Collections;
using Appacitive.Sdk.Internal;

namespace Appacitive.Sdk
{
    internal static class Extensions
    {
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
            #if !WINDOWS_PHONE7
            return list.RemoveAll( x => x == value ) > 0;
            #else
            {
                var found = false;
                while (list.Contains(value) == true)
                {
                    list.Remove(value);
                    found = true;
                }
                return found;
            }
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
            #if !WINDOWS_PHONE7
            return typeof(T).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo());
            #else
            return typeof(T).IsAssignableFrom(type);
            #endif
        }

        public static bool IsEnumeration(this Type type)
        {
            #if !WINDOWS_PHONE7
            return type.GetTypeInfo().IsEnum;
            #else
            return type.IsEnum;
            #endif
        }

        public static bool IsGeneric(this Type type)
        {
            #if !WINDOWS_PHONE7
            return type.GetTypeInfo().IsGenericType;
            #else
            return type.IsGenericType;
            #endif
        }

        public static bool IsPrimitiveType(this Type type)
        {
            if (type.Is<string>() || type.Is<DateTime>())
                return true;
            #if !WINDOWS_PHONE7
            return type.GetTypeInfo().IsValueType || type.GetTypeInfo().IsPrimitive;
            #else
            return type.IsValueType || type.IsPrimitive;
            #endif
        }

        public static Exception ToFault(this Status status)
        {
            if (status.IsSuccessful == true)
                return null;
            var factory = ObjectFactory.Build<IExceptionFactory>();
            return factory.CreateFault(status);
        }

        public static HttpOperation WithAppacitiveKeyOrSession(this HttpOperation client, string apiKey, string session, bool useApiSession)
        {       
            if( useApiSession == true )
                return client.WithHeader("Appacitive-Session", session);
            else
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

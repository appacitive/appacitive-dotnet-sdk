using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Appacitive.Sdk.Services;
using Appacitive.Sdk.Interfaces;

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


        public static IDictionary<string, string> GetModifications(this IDictionary<string, string> current, IDictionary<string, string> old)
        {
            var newClone = new Dictionary<string, string>(current);
            var oldClone = new Dictionary<string, string>(old);
            return new DictionaryDifference().GetDifferences(newClone, oldClone);
        }

        public static void For<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var item in enumerable)
                action(item);
        }

        public static Exception ToFault(this Status status)
        {
            if (status.IsSuccessful == true)
                return null;
            var factory = ObjectFactory.Build<IExceptionFactory>();
            return factory.CreateFault(status);
        }
                

        public static HttpOperation WithAppacitiveSession(this HttpOperation client, string session)
        {       
            return client.WithHeader("Appacitive-Session", session);
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

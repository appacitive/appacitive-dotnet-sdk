using System;
using System.Collections.Generic;
namespace Appacitive.Sdk
{
    public interface IValueCollection
    {
        void Add<T>(T item);
        System.Collections.Generic.IEnumerable<T> AsEnumerable<T>();
        int Count { get; }
        void Remove<T>(T item);
        void RemoveAll();
        void AddRange<T>(IEnumerable<T> enumerable);
    }
}

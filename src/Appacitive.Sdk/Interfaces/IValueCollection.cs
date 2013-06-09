using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Interfaces
{
    public interface IValueCollection<T> : IEnumerable<T>
    {
        void Add(T item, bool allowDuplicates = false);

        void AddRange(IEnumerable<T> items, bool allowDuplicates = false);

        bool Remove(T item);

        void RemoveAll(T item);

        void Clear();

        int Count { get; }
    }
}

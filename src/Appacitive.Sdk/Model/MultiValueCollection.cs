using Appacitive.Sdk.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    public class MultiValueCollection<T> : IValueCollection<T>
    {
        public MultiValueCollection(Entity entity, string property)
        {
            _entity = entity;
            _property = property;
        }

        private Entity _entity;
        private string _property;



        public void Add(T item, bool allowDuplicates = false)
        {
            _entity.AddItems(_property, allowDuplicates, item);
        }

        public void AddRange(IEnumerable<T> items, bool allowDuplicates = false)
        {
            _entity.AddItems(_property, allowDuplicates, items.ToArray());
        }

        public bool Remove(T item)
        {
            return _entity.RemoveItems(_property, item, true);
        }

        public void RemoveAll(T item)
        {
            _entity.RemoveItems(_property, item, false);
        }

        public void Clear()
        {
            _entity.ClearList(_property);
        }

        public int Count
        {
            get
            {
                var value = _entity[_property];
                if( value is NullValue )
                    return 0;
                if (value is SingleValue)
                    throw new Exception("Property " + _property + " is not a multi valued property.");
                return (value as MultiValue).Count;
            }
        }


        public IEnumerator<T> GetEnumerator()
        {
            return _entity.GetList<T>(_property).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}

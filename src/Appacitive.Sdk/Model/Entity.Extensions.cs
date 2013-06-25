using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using Appacitive.Sdk.Internal;
using System.Threading;
using System.Collections;

namespace Appacitive.Sdk
{
    public partial class Entity
    {
        public T Get<T>(string name)
        {
            if (typeof(T).IsPrimitiveType() == false && typeof(T).Is<IEnumerable>() == true)
                throw new ArgumentException("Cannot get multi valued properties via Get<T>().");
            return this[name].GetValue<T>();
        }

        public T Get<T>(string name, T defaultValue)
        {
            if (typeof(T).IsPrimitiveType() == false && typeof(T).Is<IEnumerable>() == true)
                throw new ArgumentException("Cannot get multi valued properties via Get<T>().");
            var value = this[name];
            if (value is NullValue)
                return defaultValue;
            else return value.GetValue<T>();

        }

        internal void Set<T>(string name, T value, bool updateLastKnown)
        {
            if (value.IsMultiValued() == true)
                throw new ArgumentException("Cannot set multi valued properties via Set<T>().");
            var propertyValue = Value.FromObject(value);
            this.SetField(name, propertyValue, updateLastKnown);
        }

        public void Set<T>(string name, T value)
        {
            Set(name, value, false);
        }

        internal void SetList<T>(string name, IEnumerable<T> enumerable, bool updateLastKnown)
        {
            if (enumerable == null)
                throw new Exception("Enumerable value cannot be null.");
            this.SetField(name, new MultiValue(enumerable), updateLastKnown);
        }

        public void SetList<T>(string name, IEnumerable<T> enumerable)
        {
            SetList(name, enumerable, false);
        }


        public IEnumerable<T> GetList<T>(string name)
        {
            var value = ReadField(name);
            if (value == null)
                return MultiValue.Empty.GetValues<T>();
            if (value is IEnumerable == false)
                throw new Exception("Value of property '" + name + "' is not multivalued.");
            var list = new MultiValue(value as IEnumerable);
            return list.GetValues<T>();
        }
    }
}

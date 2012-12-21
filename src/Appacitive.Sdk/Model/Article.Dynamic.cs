using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    public partial class Article : DynamicObject
    {
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = null;
            string outResult;
            var exists = _currentFields.TryGetValue(binder.Name, out outResult);
            if (exists == true)
                result = new Value(outResult);
            return exists;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (value != null)
                Guard.ValidateAllowedTypes(value.GetType());
            if (value is Value)
                _currentFields[binder.Name] = ((Value)value).StringValue;
            else
                _currentFields[binder.Name] = value.ToString();
            return true;
        }
    }
}

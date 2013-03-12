using System;
using System.Collections.Generic;
#if !WINDOWS_PHONE7
using System.Dynamic;
#endif
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    #if !WINDOWS_PHONE7
    public partial class Entity : DynamicObject
    {
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (base.TryGetMember(binder, out result) == true)
                return true;
            else
            {
                result = null;
                string outResult;
                var exists = _currentFields.TryGetValue(binder.Name, out outResult);
                if (exists == true)
                {
                    result = new Value(outResult);
                    return true;
                }
                else
                {
                    result = null;
                    return false;
                }
            }
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (value != null)
                Guard.ValidateAllowedTypes(value.GetType());
            if (value is Value)
                _currentFields[binder.Name] = ((Value)value).StringValue;
            else
                _currentFields[binder.Name] = value == null ? null : value.ToString();
            return true;
        }
    }
#endif
}

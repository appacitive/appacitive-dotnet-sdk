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
                result = this[binder.Name];
                return true;
            }
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (value.IsMultiValued() == true)
                throw new Exception("Dynamic properties cannot be used for multi-valued values.");
            if (value != null) 
                Guard.ValidateAllowedPrimitiveTypes(value.GetType());
            this[binder.Name] = Value.FromObject(value);
            return true;
        }
    }
#endif
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    public interface IJsonSerializer
    {
        byte[] Serialize(object o);

        T Deserialize<T>(byte[] stream);
    }
}

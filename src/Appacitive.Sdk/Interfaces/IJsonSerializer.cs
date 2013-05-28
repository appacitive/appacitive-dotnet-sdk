using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Realtime
{
    public interface IJsonSerializer
    {
        byte[] Serialize(object o);

        object Deserialize(Type type, byte[] stream);

        T Deserialize<T>(byte[] stream);
    }
}

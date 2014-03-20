using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Interfaces
{
    public interface ITypeMapping
    {
        ITypeMapping MapObjectType<T>(string name) where T : APObject;

        ITypeMapping MapConnectionType<T>(string name) where T : APConnection;

        Type GetMappedObjectType(string name);

        Type GetMappedConnectionType(string name);
    }
}

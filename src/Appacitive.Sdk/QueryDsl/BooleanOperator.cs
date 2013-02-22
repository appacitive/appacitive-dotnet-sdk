using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    public static class BooleanOperator
    {
        public static IQuery And(IEnumerable<IQuery> innerQueries)
        {
            return new AggregateQuery(BoolOperator.And, innerQueries);
        }

        public static IQuery Or(IEnumerable<IQuery> innerQueries)
        {
            return new AggregateQuery(BoolOperator.Or, innerQueries);
        }
    }
}

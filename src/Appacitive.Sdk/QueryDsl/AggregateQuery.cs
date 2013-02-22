using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    public class AggregateQuery : IQuery
    {
        public AggregateQuery(BoolOperator op, IEnumerable<IQuery> innerQueries)
        {
            this.BoolOperator = op;
            this.InnerQueries = new List<IQuery>(innerQueries);
        }

        public BoolOperator BoolOperator { get; set; }

        public List<IQuery> InnerQueries { get; private set; }

        public override string ToString()
        {
            return this.AsString();   
        }

        public string AsString()
        {
            var op = this.BoolOperator == Sdk.BoolOperator.And ? "and" : "or";
            var padding = " ";
            var buffer = new StringBuilder();
            buffer.Append("(");
            this.InnerQueries.ForEach(q =>
            {
                if (buffer.Length == 1)
                    buffer.Append(padding).Append(q.ToString());
                else buffer.Append(padding).Append(op).Append(padding).Append(q.ToString()).Append(padding);
            });
            buffer.Append(")");
            return buffer.ToString();
        }
    }
}

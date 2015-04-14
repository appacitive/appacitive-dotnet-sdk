using Appacitive.Sdk.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Internal
{
    public class AttributeExpression
    {
        internal AttributeExpression(string name)
        {
            this.Field = Field.Attribute(name);
        }

        public Field Field { get; private set; }

        public IQuery IsNull()
        {
            return new IsNullQuery(this.Field);
        }

        public IQuery IsIn(IEnumerable<string> values)
        {
            return new InQuery(this.Field, values);
        }

        public IQuery IsEqualTo(string value)
        {
            return FieldQuery.IsEqualTo(this.Field, value);
        }

        public IQuery Like(string value)
        {
            return FieldQuery.Like(this.Field, value);
        }

        public IQuery StartsWith(string value)
        {
            return FieldQuery.StartsWith(this.Field, value);
        }

        public IQuery EndsWith(string value)
        {
            return FieldQuery.EndsWith(this.Field, value);
        }

        public IQuery FreeTextMatches(string freeTextExpression)
        {
            return FieldQuery.FreeTextMatches(this.Field, StringUtils.EscapeSingleQuotes(freeTextExpression));
        }
    }
}

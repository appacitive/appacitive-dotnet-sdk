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
            this.Name = name;
        }

        public string Name { get; private set; }

        public IQuery IsNull()
        {
            return new IsNullQuery(Field.Property(this.Name));
        }

        public IQuery IsIn(IEnumerable<string> values)
        {
            return new InQuery(Field.Property(this.Name), values);
        }

        public IQuery IsEqualTo(string value)
        {
            return FieldQuery.IsEqualTo(FieldType.Attribute, this.Name, value);
        }

        public IQuery Like(string value)
        {
            return FieldQuery.Like(FieldType.Attribute, this.Name, value);
        }

        public IQuery StartsWith(string value)
        {
            return FieldQuery.StartsWith(FieldType.Attribute, this.Name, value);
        }

        public IQuery EndsWith(string value)
        {
            return FieldQuery.EndsWith(FieldType.Attribute, this.Name, value);
        }

        public IQuery FreeTextMatches(string freeTextExpression)
        {
            return FieldQuery.FreeTextMatches(FieldType.Attribute, this.Name, StringUtils.EscapeSingleQuotes(freeTextExpression));
        }
    }
}

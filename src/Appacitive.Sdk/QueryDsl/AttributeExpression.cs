using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    public class AttributeExpression
    {
        internal AttributeExpression(string name)
        {
            this.Field = name;
        }

        public string Field { get; private set; }

        public IQuery IsEqualTo(string value)
        {
            return FieldQuery.IsEqualTo(FieldType.Attribute, this.Field, value);
        }

        public IQuery Like(string value)
        {
            return FieldQuery.Like(FieldType.Attribute, this.Field, value);
        }

        public IQuery StartsWith(string value)
        {
            return FieldQuery.StartsWith(FieldType.Attribute, this.Field, value);
        }

        public IQuery EndsWith(string value)
        {
            return FieldQuery.EndsWith(FieldType.Attribute, this.Field, value);
        }

        public IQuery FreeTextMatches(string freeTextExpression)
        {
            return FieldQuery.FreeTextMatches(FieldType.Attribute, this.Field, StringUtils.EscapeSingleQuotes(freeTextExpression));
        }
    }
}

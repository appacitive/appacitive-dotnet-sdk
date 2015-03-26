using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Internal
{
    public struct Field
    {
        public static Field Property(string name)
        {
            return new Field(Sdk.FieldType.Property, name);
        }

        public static Field Attribute(string name)
        {
            return new Field(Sdk.FieldType.Attribute, name);
        }

        public static Field Aggregate(string name)
        {
            return new Field(Sdk.FieldType.Aggregate, name);
        }

        private Field(FieldType fieldType, string name) : this()
        {
            this.FieldType = fieldType;
            this.Name = name.ToLower();
        }

        public FieldType FieldType { get; private set; }

        public string Name { get; private set; }

        public override string ToString()
        {
            return string.Format("{0}{1}",
                GetPrefix(this.FieldType),
                this.Name);
        }

        private string GetPrefix(FieldType fieldType)
        {
            switch (this.FieldType)
            {
                case FieldType.Property:
                    return "*";
                case FieldType.Attribute:
                    return "@";
                case FieldType.Aggregate:
                    return "$";
                default: throw new AppacitiveRuntimeException("Unsuported field type.");
            }
        }
    }

}

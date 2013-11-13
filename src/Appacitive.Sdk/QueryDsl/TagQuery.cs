using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    internal class TagQuery : IQuery
    {
        public static TagQuery MatchAll(string[] tags)
        {
            return new TagQuery(tags, TagMatchMode.MatchAll);
        }

        public static TagQuery MatchOneOrMore(string[] tags)
        {
            return new TagQuery(tags, TagMatchMode.MatchOneOrMore );
        }

        internal TagQuery(string[] tags, TagMatchMode mode)
        {
            this.TagMatchMode = mode;
            tags = tags ?? Empty;
            this.Tags = tags;
        }

        private static readonly string[] Empty = new string[] { };

        public string[] Tags { get; private set; }

        public TagMatchMode TagMatchMode { get; set; }

        public override string ToString()
        {
            var buffer = new StringBuilder();
            if (this.TagMatchMode == TagMatchMode.MatchAll)
            {
                buffer.Append("tagged_with_all('");
            }
            else
            {
                buffer.Append("tagged_with_one_or_more('");
            }

            for (int i = 0; i < this.Tags.Length; i++)
            {
                if (i == 0)
                    buffer.Append(this.Tags[i]);
                else
                    buffer.Append(",").Append(this.Tags[i]);
            }
            buffer.Append("')");

            return buffer.ToString();
        }

        string IQuery.AsString()
        {
            return this.ToString();
        }
    }

    internal enum TagMatchMode
    {
        MatchAll,
        MatchOneOrMore
    }
}

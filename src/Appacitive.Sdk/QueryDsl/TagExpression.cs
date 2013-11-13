using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    public class TagExpression
    {
        public IQuery MatchAll(params string[] tags)
        {
            return new TagQuery(tags, TagMatchMode.MatchAll) as IQuery;
        }


        public IQuery MatchOneOrMore(params string[] tags)
        {
            return new TagQuery(tags, TagMatchMode.MatchOneOrMore) as IQuery;
        }
    }
}

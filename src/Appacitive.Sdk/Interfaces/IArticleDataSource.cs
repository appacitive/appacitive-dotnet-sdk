using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Interfaces
{
    public interface IArticleDataSource
    {
        Article Create(Article obj);

        Article Update(Article obj);

        void Delete(Article obj);

        Article Get(string id);
         
        IEnumerable<Article> Find(string queryType, object args );
    }
}

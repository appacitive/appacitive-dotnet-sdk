using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    public interface IArticles
    {
        // Create new article
        Task<Article> Create(Article article);
    }
}

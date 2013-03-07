using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Appacitive.Sdk.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Appacitive.Sdk.Tests
{
    internal static class ObjectHelper
    {
        public static async Task<Article> CreateNewAsync(Article article = null)
        {
            Console.WriteLine("Creating new article");
            var now = DateTime.Now;
            dynamic obj = article ?? new Article("object");
            if (article == null)
            {
                obj.intfield = 1;
                obj.decimalfield = 10.0m;
                obj.datefield = "2012-12-20";
                obj.datetimefield = now.ToString("o");
                obj.stringfield = "string value";
                obj.textfield = "text value";
                obj.boolfield = false;
                obj.geofield = "11.5,12.5";
                obj.listfield = "a";
                obj.SetAttribute("attr1", "value1");
                obj.SetAttribute("attr2", "value2");
            }

            var service = ObjectFactory.Build<IArticleService>();
            CreateArticleResponse response = null;

            response = await service.CreateArticleAsync(new CreateArticleRequest()
            {
                Article = obj,
                Environment = Environment.Sandbox
            });
            ApiHelper.EnsureValidResponse(response);
            Assert.IsNotNull(response.Article);
            Console.WriteLine("Created article id {0}", response.Article.Id);
            return response.Article;
        }

        public static Article NewInstance()
        {
            Console.WriteLine("Creating new article instance without saving");
            var now = DateTime.Now;
            dynamic obj = new Article("object");
            obj.intfield = 1;
            obj.decimalfield = 10.0m;
            obj.datefield = "2012-12-20";
            obj.datetimefield = now.ToString("o");
            obj.stringfield = "string value";
            obj.textfield = "text value";
            obj.boolfield = false;
            obj.geofield = "11.5,12.5";
            obj.listfield = "a";
            obj.SetAttribute("attr1", "value1");
            obj.SetAttribute("attr2", "value2");
            return obj as Article;
        }
    
    }
}

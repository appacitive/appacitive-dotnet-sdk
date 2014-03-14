using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if MONO
using NUnit.Framework;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif
using Appacitive.Sdk.Services;

namespace Appacitive.Sdk.Tests
{
    internal static class ObjectHelper
    {
        public static async Task<APObject> CreateNewAsync(APObject apObject = null)
        {
            Console.WriteLine("Creating new apObject");
            var now = DateTime.Now;
			var obj = apObject ?? new APObject("object");
            if (apObject == null)
            {
				obj.Set("intfield",1);
				obj.Set("decimalfield",10.0m);
				obj.Set("datefield","2012-12-20");
				obj.Set("datetimefield",now.ToString("o"));
				obj.Set("stringfield", "string value");
				obj.Set("textfield", "text value");
				obj.Set("boolfield", false);
				obj.Set("geofield", "11.5,12.5");
				obj.Set("listfield","a");
                obj.SetAttribute("attr1", "value1");
                obj.SetAttribute("attr2", "value2");
            }

            CreateObjectResponse response = null;

            response = await (new CreateObjectRequest()
            {
                Object = obj,
                Environment = Environment.Sandbox
            }).ExecuteAsync();
            ApiHelper.EnsureValidResponse(response);
            Assert.IsNotNull(response.Object);
            Console.WriteLine("Created apObject id {0}", response.Object.Id);
            return response.Object;
        }

        public static APObject NewInstance()
        {
            Console.WriteLine("Creating new apObject instance without saving");
            var now = DateTime.Now;
			var obj = new APObject("object");
			obj.Set("intfield",1);
			obj.Set("decimalfield",10.0m);
			obj.Set("datefield","2012-12-20");
			obj.Set("datetimefield",now.ToString("o"));
			obj.Set("stringfield","string value");
			obj.Set("textfield","text value");
			obj.Set("boolfield",false);
			obj.Set("geofield","11.5,12.5");
			obj.Set("listfield","a");
            obj.SetAttribute("attr1", "value1");
            obj.SetAttribute("attr2", "value2");
            return obj as APObject;
        }
    
    }
}

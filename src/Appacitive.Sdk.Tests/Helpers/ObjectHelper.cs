﻿using System;
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
        public static async Task<APObject> CreateNewAsync(APObject apObject = null)
        {
            Console.WriteLine("Creating new apObject");
            var now = DateTime.Now;
            dynamic obj = apObject ?? new APObject("object");
            if (apObject == null)
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
            dynamic obj = new APObject("object");
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
            return obj as APObject;
        }
    
    }
}

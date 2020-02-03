using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Lcp.Paychex.Models.Authentication;
using LINQPadPaychexSupport;

namespace LinqpadPaychexSupport.Tests
{
    [TestClass]
    public class SupportObjectTests
    {
        private class DummyUtil : ILINQPadUtil
        {
            public Dictionary<string,string> MemoryCache = 
                new Dictionary<string,string>(StringComparer.CurrentCultureIgnoreCase);
            public string LoadString(string key)
            {
                MemoryCache.TryGetValue(key, out var val);
                return val;
            }

            public void SaveString(string key, string value) => MemoryCache[key] = value;
            public DateTime GetCacheItemTime(string path) => DateTime.Now.AddMinutes(-5);
        }

        [TestMethod]
        public void TestMethod1()
        {
            var dummyCache = new DummyUtil();
            var testObject = new LINQPadStringDataCache(dummyCache);

            dummyCache.MemoryCache["paychexData_test"] = @"
{
  ""access_token"": ""91c4b0b6-1c5a-45d5-b495-9109c50e6a8f"",
  ""token_type"": ""Bearer"",
  ""expires_in"": 3600,
  ""scope"": ""oob"",
  ""TimeAuthenticated"": ""2020-01-29T17:15:08.6395871-08:00""
}";

            var result = testObject.Get<PaychexAuthToken>("test");
            result.Should().NotBeNull();
            result.access_token
                .Should()
                .Be("91c4b0b6-1c5a-45d5-b495-9109c50e6a8f");

            testObject.IgnoreCacheReads = true;

            testObject.Get<string>("test").Should().BeNullOrEmpty();
        }
    }
}

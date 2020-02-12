using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Lcp.Paychex.Models.Authentication;

namespace LINQPadPaychexSupport.Tests
{
    [TestClass]
    public class SupportObjectTests
    {
        private class DummyUtil : ILINQPadUtil
        {
            public readonly Dictionary<string,string> MemoryCache =
                new Dictionary<string,string>(StringComparer.CurrentCultureIgnoreCase);

            public string LoadString(string key)
            {
                MemoryCache.TryGetValue(key, out var val);
                return val;
            }

            public void SaveString(string key, string value) => MemoryCache[key] = value;
            public DateTime GetCacheItemTime(string path) => DateTime.Now.AddMinutes(-5);
        }

        private const string DummyJson = @"
{
  ""access_token"": ""91c4b0b6-1c5a-45d5-b495-9109c50e6a8f"",
  ""token_type"": ""Bearer"",
  ""expires_in"": 3600,
  ""scope"": ""oob"",
  ""TimeAuthenticated"": ""2020-01-29T17:15:08.6395871-08:00""
}";

        [TestMethod]
        public void CacheReturnsValue()
        {
            var apiKey = Guid.NewGuid()
                             .ToString("N");
            var dummyUtil = new DummyUtil();
            var testObject = new LINQPadStringDataCache(dummyUtil, apiKey, default);

            dummyUtil.MemoryCache[$"paychexData_{apiKey}_test"] = DummyJson;
            var result = testObject.Get<PaychexAuthToken>("test");
            result.Should().NotBeNull();
            result.access_token
                .Should()
                .Be("91c4b0b6-1c5a-45d5-b495-9109c50e6a8f");
        }

        [TestMethod]
        public void IgnoreReadsReturnsNull()
        {
            var apiKey = Guid.NewGuid()
                             .ToString("N");
            var dummyUtil = new DummyUtil();
            var testObject = new LINQPadStringDataCache(dummyUtil, apiKey, default);

            dummyUtil.MemoryCache[$"paychexData_{apiKey}_test"] = DummyJson;

            testObject.IgnoreCacheReads = true;

            testObject.Get<PaychexAuthToken>("test")
                      .Should()
                      .BeNull();
        }

        [TestMethod]
        public void TimeToLiveExpiredReturnsNull()
        {
            var apiKey = Guid.NewGuid()
                             .ToString("N");
            var dummyUtil = new DummyUtil();
            var testObject = new LINQPadStringDataCache(dummyUtil, apiKey, new TimeSpan(0,0,1), default);

            dummyUtil.MemoryCache[$"paychexData_{apiKey}_test"] = DummyJson;

            testObject.Get<PaychexAuthToken>("test")
                      .Should()
                      .BeNull();
        }

        [TestMethod]
        public void TokenCacheWorks()
        {
            var apiKey = Guid.NewGuid()
                             .ToString("N");
            var dummyUtil = new DummyUtil();
            var testObject = new LINQPadStringTokenCache(dummyUtil, apiKey);

            dummyUtil.MemoryCache[$"{apiKey}_paychexAuthToken"] = DummyJson;
            var result = testObject.Load();

            result.Should()
                  .NotBeNull();
            result.access_token
                  .Should()
                  .Be("91c4b0b6-1c5a-45d5-b495-9109c50e6a8f");
            result.isValid.Should()
                  .BeFalse();
        }

        [TestMethod]
        public void UnsetTokenReturnsNull()
        {
            var apiKey = Guid.NewGuid()
                             .ToString("N");
            var dummyUtil = new DummyUtil();
            var testObject = new LINQPadStringTokenCache(dummyUtil, apiKey);

            var result = testObject.Load();
            result.Should()
                  .BeNull();
        }
    }
}

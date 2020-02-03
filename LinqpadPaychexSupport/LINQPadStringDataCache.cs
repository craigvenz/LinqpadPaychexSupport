using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security;
using Lcp.Paychex.Api;
using Lcp.Paychex.Api.Interfaces;
using Newtonsoft.Json;
using static LinqpadPaychexSupport.Util;

namespace LINQPadPaychexSupport
{
    public class LINQPadStringDataCache : IPaychexDataCache
    {
        private readonly ILINQPadUtil _util;
        const string DefaultPrefix = "paychexData_";

        private readonly string _prefix;
        private readonly JsonSerializerSettings _settings;

        public bool IgnoreCacheReads { get; set; }

        private class DefaultLINQPadUtil : ILINQPadUtil
        {
            public string LoadString(string key) 
                => LINQPad.Util.LoadString(key);

            public void SaveString(string key, string value) 
                => LINQPad.Util.SaveString(key, value);

            public DateTime GetCacheItemTime(string r) 
                => File.GetLastWriteTime(Path.Combine(LpDataFolder, r));
        }

        public LINQPadStringDataCache() : this(DefaultPrefix) { }
        public LINQPadStringDataCache(string prefix = DefaultPrefix) : this(new DefaultLINQPadUtil(), prefix) { }
        public LINQPadStringDataCache(ILINQPadUtil util, string prefix = DefaultPrefix)
        {
            _util = util;
            _prefix = prefix;
            _settings =
                new JsonSerializerSettings()
                {
                    Formatting = Formatting.Indented,
                    MissingMemberHandling = MissingMemberHandling.Error,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore,
                    StringEscapeHandling = StringEscapeHandling.EscapeNonAscii,
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                    Converters = new List<JsonConverter>
                    {
                        new TolerantEnumConverter()
                    }
                };
        }

        public void Clear()
        {
            foreach (var fi in new DirectoryInfo(LpDataFolder).EnumerateFileSystemInfos($"{_prefix}*"))
            {
                try
                {
                    fi.Delete();
                }
                catch (Exception ex) when (ex is IOException || ex is SecurityException || ex is UnauthorizedAccessException)
                {
                }
            }
        }
        public T Get<T>(string r)
        {
            if (IgnoreCacheReads) 
                return default;

            var str = _util.LoadString(CacheKey(r));
            if (string.IsNullOrEmpty(str))
                return default;

            var f = _util.GetCacheItemTime(
                Path.Combine(
                    LpDataFolder, 
                    CacheKey(r)
                )
            );
            Trace.TraceInformation(
                $"{nameof(LINQPadStringDataCache)} - cache data is from {HowLongAgo(f)}."
            );
            return JsonConvert.DeserializeObject<T>(str, _settings);
        }
        public void Set<T>(string r, T value) => _util.SaveString(CacheKey(r), JsonConvert.SerializeObject(value, _settings));
        private string CacheKey(string key) => $"{_prefix}{key}";
    }
}

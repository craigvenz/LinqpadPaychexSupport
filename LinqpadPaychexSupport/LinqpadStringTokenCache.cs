using System;
using System.Diagnostics;
using Lcp.Paychex.Api.Interfaces;
using Lcp.Paychex.Models.Authentication;
using Newtonsoft.Json;
using static LinqpadPaychexSupport.Util;

namespace LINQPadPaychexSupport
{
    public class LINQPadStringTokenCache : IPaychexTokenCache
    {
        private const string DefaultTokenName = "paychexAuthToken";

        private readonly ILINQPadUtil _util;
        private readonly string _paychexToken;

        public bool IgnoreReads { get; set; }

        public void Invalidate() => _util.SaveString(_paychexToken, string.Empty);

        public LINQPadStringTokenCache(string apiKey, string tokenName = DefaultTokenName) :
            this(new DefaultLINQPadUtil(), apiKey, tokenName) { }

        public LINQPadStringTokenCache(ILINQPadUtil util, string apiKey, string tokenName = DefaultTokenName)
        {
            _util = util;
            _paychexToken = apiKey + "_" + tokenName;
        }

        public PaychexAuthToken Load()
        {
            if (IgnoreReads)
                return null;

            var data = _util.LoadString(_paychexToken);

            var result = string.IsNullOrEmpty(data) ? null : JsonConvert.DeserializeObject<PaychexAuthToken>(data);
            if (result == null)
                return null;

            var when = result.TimeAuthenticated.AddSeconds(result.expires_in);
            Trace.TraceInformation(
                result.isValid
                    ? $"Using cached token - expires at {when}, {(int) (when - DateTime.Now).TotalSeconds} seconds from now."
                    : $"Cached token expired at {when}, {HowLongAgo(when)}."
            );
            return result;
        }

        public void Save(PaychexAuthToken token) =>
            _util.SaveString(_paychexToken, JsonConvert.SerializeObject(token, Formatting.Indented));
    }
}

using System;
using System.Diagnostics;
using Lcp.Paychex.Api.Interfaces;
using Lcp.Paychex.Models.Authentication;
using Newtonsoft.Json;
using static LINQPad.Util;
using static LinqpadPaychexSupport.Util;

namespace LINQPadPaychexSupport
{
    public class LINQPadStringTokenCache : IPaychexTokenCache
    {
        private const string paychexToken = "paychexAuthToken";

        public bool IgnoreReads { get; set; }

        public void Invalidate()
        {
            SaveString(paychexToken, string.Empty);
        }

        public PaychexAuthToken Load()
        {
            if (IgnoreReads)
                return null;

            var data = LoadString(paychexToken);

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

        public void Save(PaychexAuthToken token)
        {
            SaveString(paychexToken, JsonConvert.SerializeObject(token, Formatting.Indented));
        }
    }
}
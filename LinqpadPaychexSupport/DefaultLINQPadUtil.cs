// LinqpadPaychexSupport.DefaultLINQPadUtil.cs
// Craig Venz - 02/11/2020 - 3:07 PM

using System;
using System.IO;
using LinqpadPaychexSupport;

namespace LINQPadPaychexSupport {
    internal class DefaultLINQPadUtil : ILINQPadUtil
    {
        public string LoadString(string key)
            => LINQPad.Util.LoadString(key);

        public void SaveString(string key, string value)
            => LINQPad.Util.SaveString(key, value);

        public DateTime GetCacheItemTime(string r)
            => File.GetLastWriteTime(Path.Combine(Util.LpDataFolder, r));
    }
}

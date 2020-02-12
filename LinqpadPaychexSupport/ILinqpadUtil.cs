using System;

namespace LINQPadPaychexSupport
{
    public interface ILINQPadUtil
    {
        string LoadString(string key);
        void SaveString(string key, string value);
        DateTime GetCacheItemTime(string path);
    }
}

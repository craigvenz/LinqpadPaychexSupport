using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LINQPadPaychexSupport
{
    public interface ILINQPadUtil
    {
        string LoadString(string key);
        void SaveString(string key, string value);
        DateTime GetCacheItemTime(string path);
    }
}

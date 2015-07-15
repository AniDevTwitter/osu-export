using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace osu_export.core
{
    public static class PathExts
    {
        public static string AsValidPath(this string pottentiallyInvalid)
        {
            string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            var regex = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
            return regex.Replace(pottentiallyInvalid, "");
        }
    }
}

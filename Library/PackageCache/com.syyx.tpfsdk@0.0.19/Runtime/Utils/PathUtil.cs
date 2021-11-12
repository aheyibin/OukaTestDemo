using System.IO;

namespace TPFSDK.Utils
{
    public class PathUtil
    {
        public static string Combine(params string[] paths)
        {
#if NET_4_6
            return Path.Combine(paths);
#else

            if (paths == null || paths.Length == 0)
            {
                return "";
            }

            string combinedPath = paths[0];
            for (int i = 1; i < paths.Length; i++)
            {
                combinedPath = Path.Combine(combinedPath, paths[i]);
            }
            return combinedPath;
#endif
        }
    }

}

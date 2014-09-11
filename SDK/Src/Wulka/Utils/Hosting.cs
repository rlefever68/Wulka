using System.Web.Hosting;

namespace Wulka.Utils
{
    public class Hosting
    {
        public static string MapPath(string path)
        {
            return HostingEnvironment.MapPath(path);
        }
    }
}

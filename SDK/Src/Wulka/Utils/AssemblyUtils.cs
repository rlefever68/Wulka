using System.Reflection;

namespace Wulka.Utils
{
    public class AssemblyUtils
    {
        public Module[] GetLoadedModules()
        {
            var asm = Assembly.GetExecutingAssembly();
            return asm.GetLoadedModules();
        }

    }
}

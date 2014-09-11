using System.ComponentModel.Composition.Hosting;

namespace Wulka.Networking.Wcf
{

    public static class ComposedServiceHosts
    {
        private static CompositionContainer _container = null;

        internal static CompositionContainer CompositionContainer
        {
            get { return _container; }
        }

        public static void SetCompositionContainer(CompositionContainer container)
        {
            _container = container;
        }
    }


}

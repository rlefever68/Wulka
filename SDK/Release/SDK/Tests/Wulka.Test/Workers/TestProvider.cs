using Wulka.Test.Interfaces;

namespace Wulka.Test.Workers
{
    public class TestProvider
    {
        public static IProviderTest Provider
        {
            get
            {
                return new TestProviderWorker();
            }
        }
    }
}

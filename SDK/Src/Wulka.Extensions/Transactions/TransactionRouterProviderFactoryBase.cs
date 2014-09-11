using Wulka.Interfaces;

namespace Wulka.Transactions
{
    public abstract class TransactionRouterProviderFactoryBase : ITransactionRouterProviderFactory
    {
        public abstract ITransactionRouter CreateRouter();
    }
}

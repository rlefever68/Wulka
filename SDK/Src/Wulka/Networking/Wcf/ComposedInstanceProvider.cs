using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace Wulka.Networking.Wcf
{

    public class ComposedInstanceProvider : IInstanceProvider
    {
        private readonly Type _serviceType;
        private readonly CompositionContainer _container;

        public ComposedInstanceProvider(Type serviceType, CompositionContainer container)
        {
            _serviceType = serviceType;
            _container = container;
        }

        public object GetInstance(InstanceContext context)
        {
            Export export = GetServiceExport();

            if (export == null)
                throw new InvalidOperationException();

            return export.Value;
        }

        public object GetInstance(InstanceContext context, Message message)
        {
            return GetInstance(context);
        }

        public void ReleaseInstance(InstanceContext context, object instance)
        {
            var disposable = instance as IDisposable;

            if (disposable != null)
                disposable.Dispose();
        }

        private Export GetServiceExport()
        {
            var importDefinition
            = new ContractBasedImportDefinition(AttributedModelServices.GetContractName(_serviceType),
                                                AttributedModelServices.GetTypeIdentity(_serviceType),
                                                null,
                                                ImportCardinality.ZeroOrMore,
                                                true,
                                                true,
                                                CreationPolicy.Any);

            return _container.GetExports(importDefinition).FirstOrDefault();
        }
    }


}

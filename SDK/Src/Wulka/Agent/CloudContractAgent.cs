using Wulka.Domain;
using Wulka.Interfaces;
using Wulka.Networking.Wcf;

namespace Wulka.Agent
{
    public class CloudContractAgent : DiscoProxy<ICloudContract>, ICloudContractAgent
    {
        public CloudContractAgent(string discoUrl) 
            : base(discoUrl)
        {
        }

        /// <summary>
        /// Gets the cloud contract.
        /// </summary>
        /// <param name="contractId">The contract identifier.</param>
        /// <returns>CloudContract[][].</returns>
        public CloudContract GetCloudContract(string contractId)
        {
            var clt = CreateClient();
            try
            {
                return clt.GetCloudContract(contractId);
            }
            finally
            {
                CloseClient(clt);
            }
        }

        /// <summary>
        /// Saves the cloud contracts.
        /// </summary>
        /// <param name="contracts">The contracts.</param>
        /// <returns>CloudContract[][].</returns>
        public CloudContract[] SaveCloudContracts(CloudContract[] contracts)
        {
            var clt = CreateClient();
            try
            {
                return clt.SaveCloudContracts(contracts);
            }
            finally
            {
                CloseClient(clt);
            }
        }


        /// <summary>
        /// Saves the cloud contract.
        /// </summary>
        /// <param name="contract">The contract.</param>
        /// <returns>CloudContract[][].</returns>
        public CloudContract SaveCloudContract(CloudContract contract)
        {
            var clt = CreateClient();
            try
            {
                return clt.SaveCloudContract(contract);
            }
            finally
            {
                CloseClient(clt);
            }
        }


        /// <summary>
        /// Gets the contract namespace.
        /// </summary>
        /// <returns>System.String.</returns>
        protected override string GetContractNamespace()
        {
            return CloudContractServiceConst.Namespace;
        }
    }
}

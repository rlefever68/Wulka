using Wulka.Interfaces;

namespace Wulka.Networking.Wcf
{
    public class ValidateSessionPortal
    {

        public static IValidateSessionAgent CreateAgent(string discoUrl)
        {
            return new ValidateSessionAgent(discoUrl);
        }




        public static IValidateSessionAgent Agent
        {
            get { return new ValidateSessionAgent(null); }
        }
    }
}

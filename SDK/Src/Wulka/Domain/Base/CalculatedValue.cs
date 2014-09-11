using System;
using System.Runtime.Serialization;

namespace Wulka.Domain.Base
{
    [DataContract]
    public class CalculatedValue : ValueItemBase
    {
        protected override FormulaBase CreateFormula()
        {
            throw new NotImplementedException();
        }
    }
}

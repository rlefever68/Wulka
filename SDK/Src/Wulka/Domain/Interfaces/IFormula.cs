using System.Collections.Generic;

namespace Wulka.Domain.Interfaces
{
    public interface IFormula
    {
        /// <summary>
        /// Gets or sets the input.
        /// </summary>
        /// <value>The input.</value>
        List<IValueItem> Input { get; set; }

        /// <summary>
        /// Gets or sets the output.
        /// </summary>
        /// <value>The output.</value>
        List<IValueItem> Output { get; set; }

    }
}
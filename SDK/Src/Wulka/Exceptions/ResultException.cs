using Wulka.Domain.Base;

namespace Wulka.Exceptions
{
    /// <summary>
    /// Exception based on a Result
    /// </summary>
    public class ResultException : WulkaException
    {
		/// <summary>
		/// Initializes a new instance of the <see cref="ResultException"/> class.
		/// </summary>
        public ResultException()
        {
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="ResultException"/> class.
		/// </summary>
		/// <param name="result">The result.</param>
        public ResultException(Result result)
        {
            Result = result;
        }

		/// <summary>
		/// Gets or sets the result.
		/// </summary>
		/// <value>The result.</value>
        public Result Result { get; set; }
    }
}
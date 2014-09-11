using System;
using System.Linq;
using Wulka.Domain.Base;

namespace Wulka.Domain
{
    /// <summary>
    /// Provides extension methods for result arrays
    /// </summary>
    public static class ResultExtensions
    {
        /// <summary>
        /// Determines whether the specified results have errors.
        /// </summary>
        /// <param name="results">The results.</param>
        /// <returns>
        ///   <c>true</c> if the specified results have errors; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasErrors(this Result[] results)
        {
            return results.Any(result => result.HasErrors);
        }

        /// <summary>
        /// Determines whether the specified results have warnings.
        /// </summary>
        /// <param name="results">The results.</param>
        /// <returns>
        ///   <c>true</c> if the specified results have warnings; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasWarnings(this Result[] results)
        {
            return results.Any(result => result.HasWarnings);
        }

        /// <summary>
        /// Determines whether the specified results have infos.
        /// </summary>
        /// <param name="results">The results.</param>
        /// <returns>
        ///   <c>true</c> if the specified results have infos; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasInfos(this Result[] results)
        {
            return results.Any(result => result.HasInfos);
        }

        /// <summary>
        /// returns newline-seperated string containing all error messages of all results in the array
        /// </summary>
        /// <param name="results">The results.</param>
        /// <returns></returns>
        public static string AllErrors(this Result[] results)
        {
            var errors = results.Select(result => result.AllErrors).ToList();
            return errors.Aggregate(string.Empty, (current, error) => current + (error + Environment.NewLine));
        }

    }


}

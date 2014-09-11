// ***********************************************************************
// Assembly         : Wulka.Fx
// Author           : ON8RL
// Created          : 12-17-2013
//
// Last Modified By : ON8RL
// Last Modified On : 12-17-2013
// ***********************************************************************
// <copyright file="FacilityInfo.cs" company="">
//     Copyright (c)2013 . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
namespace Wulka.ErrorHandling
{
    /// <summary>
    /// Class FacilityInfo.
    /// </summary>
    public class FacilityInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FacilityInfo"/> class.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="name">The name.</param>
        public FacilityInfo(FacilityCode code, string name)
        {
            Code = code;
            Name = name;
        }

        /// <summary>
        /// Gets the code.
        /// </summary>
        /// <value>The code.</value>
        public FacilityCode Code { get; private set; }
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name   { get; private set; }
    }
}

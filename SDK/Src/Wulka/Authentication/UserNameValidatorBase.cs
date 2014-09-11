#region Disclaimer
// THIS SOFTWARE COMES "AS IS", WITH NO WARRANTIES.  THIS
// MEANS NO EXPRESS, IMPLIED OR STATUTORY WARRANTY, INCLUDING
// WITHOUT LIMITATION, WARRANTIES OF MERCHANTABILITY OR FITNESS
// FOR A PARTICULAR PURPOSE OR ANY WARRANTY OF TITLE OR
// NON-INFRINGEMENT.
//
// MICROSOFT WILL NOT BE LIABLE FOR ANY DAMAGES RELATED TO
// THE SOFTWARE, INCLUDING DIRECT, INDIRECT, SPECIAL,
// CONSEQUENTIAL OR INCIDENTAL DAMAGES, TO THE MAXIMUM EXTENT
// THE LAW PERMITS, NO MATTER WHAT LEGAL THEORY IT IS
// BASED ON.
#endregion

using System;
using System.IdentityModel.Selectors;

namespace Wulka.Authentication
{
    public abstract class UsernameValidatorBase : UserNamePasswordValidator
    {
        public override void Validate(string userName, string password)
        {
            // peform
            if (null == userName || null == password)
            {
                throw new ArgumentNullException();
            }
            ValidateInternal(userName, password);
        }

        /// <summary>
        /// Validates the internal.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        protected abstract void ValidateInternal(string userName, string password);
        
       
    }
}

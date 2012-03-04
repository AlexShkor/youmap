using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace mPower.Framework.Exceptions
{
    /// <summary>
    /// Throw this exception when some user trying to access any information that does not belong to him
    /// No need to handle this exception, it will be handled automatically (redirect to 403, forbidden)
    /// </summary>
    public class HttpSecurityException : HttpException
    {
        public HttpSecurityException(string message) : base(403, message)
        {
            
        }
    }
}

using System.Web;

namespace YouMap.Framework.Exceptions
{
    /// <summary>
    /// Throw this exception when some pages not found, or actions parameters don't allow to show page correctly 
    /// </summary>
    public class HttpNotFoundException : HttpException
    {
        public HttpNotFoundException(string message)
            : base(404, message)
        {
            
        }
    }
}
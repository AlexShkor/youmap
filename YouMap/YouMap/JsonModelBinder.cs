
using System.Runtime.Serialization.Json;
using System.Web.Mvc;

namespace YouMap
{
    public class JsonModelBinder: IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            try
            {
                var serializer = new DataContractJsonSerializer(bindingContext.ModelType);
                return serializer.ReadObject(controllerContext.HttpContext.Request.InputStream);
            }
            catch
            {
                return null;
            }
        }
    }
}
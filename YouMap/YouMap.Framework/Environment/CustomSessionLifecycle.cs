using System;
using StructureMap.Pipeline;

namespace YouMap.Framework.Environment
{
    public class CustomSessionLifecycle : ILifecycle
    {
        public void EjectAll()
        {
            throw new NotImplementedException();
        }

        public IObjectCache FindCache()
        {
            throw new NotImplementedException();
        }

        public string Scope
        {
            get { return "CustomSessionLifecycle"; }
        }
    }
}

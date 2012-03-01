using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StructureMap.Pipeline;

namespace mPower.Framework.Environment
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

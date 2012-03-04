// Type: Paralect.ServiceLocator.StructureMap.StructureMapServiceLocator
// Assembly: Paralect.ServiceLocator.StructureMap, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// Assembly location: D:\install\YouMap\YouMap\libs\Paralect\Paralect.ServiceLocator.StructureMap.dll

using Microsoft.Practices.ServiceLocation;
using StructureMap;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Paralect.ServiceLocator.StructureMap
{
  public class StructureMapServiceLocator : ServiceLocatorImplBase
  {
    private IContainer container;

    public StructureMapServiceLocator(IContainer container)
    {
      base.\u002Ector();
      this.container = container;
    }

    protected virtual object DoGetInstance(Type serviceType, string key)
    {
      if (string.IsNullOrEmpty(key))
        return this.container.GetInstance(serviceType);
      else
        return this.container.GetInstance(serviceType, key);
    }

    protected virtual IEnumerable<object> DoGetAllInstances(Type serviceType)
    {
      foreach (object obj in (IEnumerable) this.container.GetAllInstances(serviceType))
        yield return obj;
    }
  }
}

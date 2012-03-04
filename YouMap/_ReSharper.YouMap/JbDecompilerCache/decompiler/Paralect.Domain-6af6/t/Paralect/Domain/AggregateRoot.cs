// Type: Paralect.Domain.AggregateRoot
// Assembly: Paralect.Domain, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// Assembly location: D:\install\YouMap\YouMapCqrsMongo\YouMap\libs\Paralect\Paralect.Domain.dll

using Microsoft.CSharp.RuntimeBinder;
using Paralect.Domain.Utilities;
using Paralect.Transitions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Paralect.Domain
{
  public abstract class AggregateRoot
  {
    private int _version = 0;
    private readonly List<IEvent> _changes = new List<IEvent>();
    protected string _id;

    public string Id
    {
      get
      {
        return this._id;
      }
    }

    public int Version
    {
      get
      {
        return this._version;
      }
      internal set
      {
        this._version = value;
      }
    }

    public Transition CreateTransition(IDataTypeRegistry dataTypeRegistry)
    {
      if (string.IsNullOrEmpty(this._id))
        throw new Exception(string.Format("ID was not specified for domain object. AggregateRoot [{0}] doesn't have correct ID. Maybe you forgot to set an _id field?", (object) this.GetType().FullName));
      List<TransitionEvent> events = new List<TransitionEvent>();
      foreach (IEvent @event in this._changes)
      {
        @event.Metadata.StoredDate = DateTime.UtcNow;
        @event.Metadata.TypeName = @event.GetType().Name;
        events.Add(new TransitionEvent(dataTypeRegistry.GetTypeId(@event.GetType()), (object) @event, (Dictionary<string, object>) null));
      }
      return new Transition(new TransitionId(this._id, this._version + 1), DateTime.UtcNow, events, (Dictionary<string, object>) null);
    }

    public void LoadFromTransitionStream(ITransitionStream stream)
    {
      foreach (Transition transition in stream.Read())
      {
        foreach (TransitionEvent transitionEvent in transition.Events)
          this.Apply((IEvent) transitionEvent.Data, false);
        this._version = transition.Id.Version;
      }
    }

    public void LoadFromEvents(IEnumerable<IEvent> events, int version = 1)
    {
      foreach (IEvent evnt in events)
        this.Apply(evnt, false);
      this._version = version;
    }

    public void Apply(IEvent evnt)
    {
      this.Apply(evnt, true);
    }

    private void Apply(IEvent evnt, bool isNew)
    {
      // ISSUE: reference to a compiler-generated field
      if (AggregateRoot.\u003CApply\u003Eo__SiteContainer0.\u003C\u003Ep__Site1 == null)
      {
        // ISSUE: reference to a compiler-generated field
        AggregateRoot.\u003CApply\u003Eo__SiteContainer0.\u003C\u003Ep__Site1 = CallSite<Action<CallSite, object, IEvent>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "On", (IEnumerable<Type>) null, typeof (AggregateRoot), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      AggregateRoot.\u003CApply\u003Eo__SiteContainer0.\u003C\u003Ep__Site1.Target((CallSite) AggregateRoot.\u003CApply\u003Eo__SiteContainer0.\u003C\u003Ep__Site1, PrivateReflectionDynamicObjectExtensions.AsDynamic((object) this), evnt);
      if (!isNew)
        return;
      this._changes.Add(evnt);
    }
  }
}

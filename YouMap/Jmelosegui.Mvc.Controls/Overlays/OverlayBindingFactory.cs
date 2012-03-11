using System;
using Telerik.Web.Mvc.Infrastructure;

namespace Jmelosegui.Mvc.Controls
{
    public class OverlayBindingFactory<TGoogleMapOverlay> where TGoogleMapOverlay : Overlay
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public OverlayBindingFactory<TGoogleMapOverlay> For<TDataItem>(Action<OverlayBindingBuilder<TGoogleMapOverlay, TDataItem>> action)
        {
            Guard.IsNotNull(action, "action");
            Binder = new OverlayBinding<TGoogleMapOverlay, TDataItem>();
            var builder = new OverlayBindingBuilder<TGoogleMapOverlay, TDataItem>((OverlayBinding<TGoogleMapOverlay, TDataItem>)Binder);
            action(builder);

            return this;
        }

        public IOverlayBinding<TGoogleMapOverlay> Binder { get; private set; }
    }
}

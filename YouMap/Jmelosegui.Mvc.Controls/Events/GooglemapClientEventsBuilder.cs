using System;
using Telerik.Web.Mvc;
using Telerik.Web.Mvc.Infrastructure;

namespace Jmelosegui.Mvc.Controls
{
    public class GoogleMapClientEventsBuilder : IHideObjectMembers
    {
        private readonly GoogleMapClientEvents clientEvents;

        public GoogleMapClientEventsBuilder(GoogleMapClientEvents clientEvents)
        {
            Guard.IsNotNull(clientEvents, "clientEvents");

            this.clientEvents = clientEvents;
        }

        public GoogleMapClientEventsBuilder OnLoad(Action onLoadInlineCode)
        {
            Guard.IsNotNull(onLoadInlineCode, "onLoadInlineCode");

            clientEvents.OnLoad.CodeBlock = onLoadInlineCode;

            return this;
        }

        public GoogleMapClientEventsBuilder OnLoad(string onLoadHandlerName)
        {
            Guard.IsNotNullOrEmpty(onLoadHandlerName, "onLoadHandlerName");

            clientEvents.OnLoad.HandlerName = onLoadHandlerName;

            return this;
        }

        public GoogleMapClientEventsBuilder OnClick(Action onClickInlineCode)
        {
            Guard.IsNotNull(onClickInlineCode, "onClickInlineCode");

            clientEvents.OnClick.CodeBlock = onClickInlineCode;

            return this;
        }

        public GoogleMapClientEventsBuilder OnClick(string onClickHandlerName)
        {
            Guard.IsNotNullOrEmpty(onClickHandlerName, "onClickHandlerName");

            clientEvents.OnClick.HandlerName = onClickHandlerName;

            return this;
        }
    }
}

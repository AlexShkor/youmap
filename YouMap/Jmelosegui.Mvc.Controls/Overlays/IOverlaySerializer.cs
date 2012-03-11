using System.Collections.Generic;

namespace Jmelosegui.Mvc.Controls.Overlays
{
    public interface IOverlaySerializer
    {
        IDictionary<string, object> Serialize();
    }
}

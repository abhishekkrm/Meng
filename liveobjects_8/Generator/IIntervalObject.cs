using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Generator
{
    [QS.Fx.Reflection.ObjectClass("22C1F947A15242D7A9A1A55FE6572349", "IIntervalObject",
        "Returns interface for intervals")]
    public interface IIntervalObject : QS.Fx.Object.Classes.IObject
    {
        [QS.Fx.Reflection.Endpoint("Generator")]
        QS.Fx.Endpoint.Classes.IExportedInterface<IIntervalInterface> Generator
        {
            get;
        }
    }
}

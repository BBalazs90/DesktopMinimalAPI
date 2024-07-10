using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DesktopMinimalAPI;
internal class WebMessageBroker
{
    private WebMessageBroker() {}

    public static WebMessageBroker Create()
    {
        return new WebMessageBroker();
    }
}

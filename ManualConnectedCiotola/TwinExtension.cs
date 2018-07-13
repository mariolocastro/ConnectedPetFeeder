using Microsoft.Azure.Devices.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManualConnectedCiotola
{
    public static class TwinExtension
    {
        private static string DesiredProperty(this Twin that, string name)
        {
            if (that == null) return string.Empty;
            if (!that.Properties.Desired.Contains(name)) return string.Empty;
            return that.Properties.Desired[name];
        }

        private static bool IsDesiredPropertyEmpty(this Twin that, string name)
        {
            return string.IsNullOrWhiteSpace(DesiredProperty(that, name));
        }
    }
}

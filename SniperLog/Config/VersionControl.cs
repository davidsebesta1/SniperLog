using SniperLog.Config.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SniperLog.Config
{
    public class VersionControl : IConfig
    {
        public static string Name => "VersionControl";

        public bool FirstLaunchEver { get; set; } = true;
    }
}

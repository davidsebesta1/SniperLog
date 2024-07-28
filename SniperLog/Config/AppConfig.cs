using SniperLog.Config.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SniperLog.Config
{
    public sealed class AppConfig : IConfig
    {
        public static string Name => "MainConfig";

        public List<Version> AppliedPatches = new List<Version>();

        public string ServerHostname { get; set; } = "dev.spsejecna.net";
        public ushort ServerPort { get; set; } = 8000;
    }
}

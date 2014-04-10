using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Deskband.AutoUpdater
{
    public class UpdateInfo
    {
        public string Version { get; set; }

        public string Title { get; set; }

        public string Url { get; set; }

        public string ChangelogUrl { get; set; }
    }
}
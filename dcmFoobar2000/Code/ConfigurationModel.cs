using Deskband.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dcmFoobar2000.Code
{
    public class ConfigurationModel : ConfigurationObjectBase
    {
        public string TestString { get; set; }
        public int TestInt { get; set; }

        public static readonly ConfigurationModel Default = new ConfigurationModel
        {
            ModuleId = Foobar2000Module.ModuleId,
            TestString = "test string default value",
            TestInt = 55
        };
    }
}

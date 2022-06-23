using System;
using System.Collections.Generic;
using System.Text;

namespace Cnp.Sdk.Utlities
{
    public class Config
    {
        public static bool IsValidConfigValueSet(string propertyName, Dictionary<string, string> config)
        {
            return config.ContainsKey(propertyName) && !string.IsNullOrEmpty(config[propertyName]);
        }
    }
}

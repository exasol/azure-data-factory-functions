using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Exasol.Utilities
{
    public class RegexUtilities
    {
        public static string GrabValueFromAzureConnectionString(string connString, string key)
        {
            string regExp = $@"(?<={key}=).[^;]*(?=;)";
            Regex mashupBlockRegex = new Regex(regExp);
            var match = mashupBlockRegex.Match(connString);
            if (match.Success)
            {
                return match.Value;
            }
            else
            {
                throw new Exception($"Identifier {key} not found");
            }
        }
    }
}

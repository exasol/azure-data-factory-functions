using Exasol.ErrorReporting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Exasol.Utilities
{
    public class RequestUtilities
    {
        public static dynamic DeserializeJsonBody(string requestBody)
        {
            dynamic data;
            try
            {
                data = JsonConvert.DeserializeObject(requestBody);
            }
            catch (Exception)
            {
                throw new Exception(ExaError.MessageBuilder("E-ADF-JSON-1").Message("The JSON does not seem well formed.").Mitigation("Please check the JSON body in the request.").ToString());
            }

            return data;
        }

    }
}

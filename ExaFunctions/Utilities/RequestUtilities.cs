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
            catch (Exception e)
            {
                throw new Exception("The JSON does not seem well formed. Please check the JSON body in the request.");
            }

            return data;
        }

        //public static string GetPropertyFromDynamic(dynamic dyn, string propertyname, string description = null)
        //{
        //    string property;
        //    try
        //    {
        //        property = dyn.;
        //    } catch (Exception e)
        //    {
        //        string errorMsg = "";
        //        if (description== null)
        //        {
        //            errorMsg = $"The request body is missing {propertyname}";
        //        } else
        //        {
        //            errorMsg = $"The request body is missing {propertyname}, { description }";
                    
        //        }
        //        throw new Exception(errorMsg);
        //    }
        //    return property;
        //}

    }
}

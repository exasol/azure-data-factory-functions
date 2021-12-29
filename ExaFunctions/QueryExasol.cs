using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data.Common;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Data;
using Exasol.Utilities;
using Exasol.ErrorReporting;

namespace Exasol
{
    public static class QueryExasol
    {
        [FunctionName("QueryExasol")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {

                log.LogInformation("Started processing request.");

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                dynamic data = RequestUtilities.DeserializeJsonBody(requestBody);

                string connString = req.Query["connectionstring"];
                string query = req.Query["query"];
                //also support post body
                connString = connString ?? data?.connectionstring;
                query = query ?? data?.query;

                CheckParameters(connString, query);

                var result = QueryUtilities.RunQuery(connString, query);

                return new OkObjectResult(result);

            }
            catch (Exception e)
            {
                var errorObject = new JObject();
                errorObject.Add("Something went wrong:", e.Message);

                // Return a 400 bad request result to the client with JSON body
                return new BadRequestObjectResult(errorObject);
            }
            finally
            {
                log.LogInformation("Finished request.");
            }
        }


        private static void CheckParameters(string connString, string query)
        {
            if (connString == null)
            {
                throw new Exception(ExaError.MessageBuilder("E-ADF-Q-1").Message("You need to provide a connection string in the request.").ToString());
            }
            if (query == null)
            {
                throw new Exception(ExaError.MessageBuilder("E-ADF-Q-2").Message("You need to provide a query in the request.").ToString());
            }
        }
    }
}

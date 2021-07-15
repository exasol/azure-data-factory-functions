using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Exasol
{
    public class QueryUtilities
    {
        public static JObject RunQuery(string connString, string query)
        {
            JObject result = new JObject();

            //https://docs.telerik.com/reporting/knowledge-base/how-to-register-db-provider-factory-in-net-core-project
            //C:\Windows\Microsoft.NET\Framework64\v4.0.30319\machine.Config -> on installation an entry gets written here + the dll gets added to the GAC
            //in .NET core there's no GAC (and no machine.config)
            //DbProviderFactories.RegisterFactory("Exasol.EXADataProvider", "Exasol.EXADataProvider");
            DbProviderFactories.RegisterFactory("Exasol.EXADataProvider", "Exasol.EXADataProvider.EXAProviderFactory, EXADataProvider, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec874333d1454516");
            DbProviderFactory factory = null;
            //try
            //{
            factory = DbProviderFactories.GetFactory("Exasol.EXADataProvider");
            Console.WriteLine("Found Exasol driver");

            //TODO: use a using here
            DbConnection connection = factory.CreateConnection();
            connection.ConnectionString =
            connString;

            connection.Open();
            Console.WriteLine("Connected to server");
            DbCommand cmd = connection.CreateCommand();
            cmd.Connection = connection;
            cmd.CommandText = query;

            DbDataReader reader = cmd.ExecuteReader();

            Console.WriteLine("Reader readout start");
            var jArray = new JArray();
            while (reader.Read())
            {
                var rowObject = new JObject();
                var nrOfFields = reader.FieldCount;
                for (var i = 0; i < nrOfFields; i++)
                {
                    var columnName = reader.GetName(i);
                    var columnValue = reader[i].ToString();

                    rowObject.Add(columnName, columnValue);

                }
                jArray.Add(rowObject);
                //result.Add(rowObject);
            }

            result.Add(new JProperty("results", jArray));
            reader.Close();
            connection.Close();

            return result;
            //}
            //catch (Exception ex)
            //{
            //	Console.WriteLine(ex.ToString());
            //	JObject error= new JObject();
            //	error.Add("error", ex.ToString());
            //	return error;
            //}
            //https://damienbod.com/2020/07/12/azure-functions-configuration-and-secrets-management/#:~:text=The%20Azure%20functions%20project%20requires%20the%20Microsoft.Extensions.Configuration.UserSecrets%20nuget,source%20code%2C%20you%20can%20add%20the%20super%20secrets.

            //https://www.bing.com/search?q=DbDatareader+to+json&cvid=d7989b1c3ebc434486ca17da562da5c4&aqs=edge..69i57.3937j0j1&pglt=2083&FORM=ANNTA1&PC=U531			
            //https://weblog.west-wind.com/posts/2009/Apr/24/JSON-Serialization-of-a-DataReader

        }

        public static JObject RunNonQuery(string connString, string query)
        {
            

            //https://docs.telerik.com/reporting/knowledge-base/how-to-register-db-provider-factory-in-net-core-project
            //C:\Windows\Microsoft.NET\Framework64\v4.0.30319\machine.Config -> on installation an entry gets written here + the dll gets added to the GAC
            //in .NET core there's no GAC (and no machine.config )
            //DbProviderFactories.RegisterFactory("Exasol.EXADataProvider", "Exasol.EXADataProvider");
            DbProviderFactories.RegisterFactory("Exasol.EXADataProvider", "Exasol.EXADataProvider.EXAProviderFactory, EXADataProvider, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec874333d1454516");
            DbProviderFactory factory = null;
            //try
            //{
            factory = DbProviderFactories.GetFactory("Exasol.EXADataProvider");
            Console.WriteLine("Found Exasol driver");

            //TODO: use a using here
            DbConnection connection = factory.CreateConnection();
            connection.ConnectionString =
            connString;

            connection.Open();
            Console.WriteLine("Connected to server");
            DbCommand cmd = connection.CreateCommand();
            cmd.Connection = connection;
            cmd.CommandText = query;

            int rowsAffected = cmd.ExecuteNonQuery();

            JObject result = new JObject();
            result.Add("rows affected", rowsAffected);

            connection.Close();

            return result;

        }
    }
}

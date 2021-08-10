using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Exasol
{
    public class QueryUtilities
    {
        public enum QueryType
        {
            Query,
            NonQuery
        }
        public static JObject Run(string connString, string query, QueryType queryType)
        {
            JObject result = new JObject();
            DbProviderFactory factory = GetFactory();

            using (DbConnection connection = factory.CreateConnection())
            {
                connection.ConnectionString = connString;
                connection.Open();
                DbCommand cmd = CreateCommand(query, connection);
                if (queryType == QueryType.Query)
                {
                    ReadOutReader(result, cmd);
                }
                else
                {
                    ExecuteAndReturnNrOfAffectedRows(result, cmd);
                }
            }
            return result;
        }
        public static JObject RunQuery(string connString, string query)
        {
            return Run(connString, query, QueryType.Query);
        }
        public static JObject RunNonQuery(string connString, string query)
        {
            return Run(connString, query, QueryType.NonQuery);
        }
        private static void ReadOutReader(JObject result, DbCommand cmd)
        {
            DbDataReader reader = cmd.ExecuteReader();

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
            }

            result.Add(new JProperty("results", jArray));
            reader.Close();
        }

        private static DbCommand CreateCommand(string query, DbConnection connection)
        {
            DbCommand cmd = connection.CreateCommand();
            cmd.Connection = connection;
            cmd.CommandText = query;
            return cmd;
        }

        private static DbProviderFactory GetFactory()
        {
            DbProviderFactories.RegisterFactory("Exasol.EXADataProvider", "Exasol.EXADataProvider.EXAProviderFactory, EXADataProvider, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec874333d1454516");
            DbProviderFactory factory = null;

            factory = DbProviderFactories.GetFactory("Exasol.EXADataProvider");
            return factory;
        }

        private static void ExecuteAndReturnNrOfAffectedRows(JObject result, DbCommand cmd)
        {
            int rowsAffected = cmd.ExecuteNonQuery();

            result.Add("rows affected", rowsAffected);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AnalysisServices.Tabular;
using Microsoft.AnalysisServices.AdomdClient;
using Newtonsoft.Json;
using JsonSerializer = Microsoft.AnalysisServices.Tabular.JsonSerializer;

namespace PbixSerializer
{
    class Program
    {
        static void Main(string[] args)
        {
            string server_port = args[0];
            string database_name = args[1];
            Console.WriteLine($"Server port: {server_port}, Database: {database_name}");
            string ConnectionString = "DataSource=" + server_port;
            
            using (Server server = new Server())
            {
                try
                {
                    server.Connect(ConnectionString);
                    var db = server.Databases[database_name];
                    Pb_table t = new Pb_table(db.Model.Tables.First());
                    string fileName = "WeatherForecast.json";
                    string j = JsonConvert.SerializeObject(t, Formatting.Indented);
                    File.WriteAllText(fileName, j);
                }
                catch (Exception ex)
                {
                    //log exceptions somewhere
                    Console.WriteLine(ex.ToString());
                }
            }
            Console.WriteLine("Press Enter to close this console window.");
            Console.ReadLine();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AnalysisServices.Tabular;
using Newtonsoft.Json;
using JsonSerializer = Microsoft.AnalysisServices.Tabular.JsonSerializer;

namespace PbixSerializer
{
    class Program
    {
        static char[] invalid_chars = System.IO.Path.GetInvalidFileNameChars();
        static string sanitize_name(string name)
        {
            return String.Join("_", name.Split(Program.invalid_chars, StringSplitOptions.RemoveEmptyEntries));
        }
        static void serialize_measure(string path, Measure m)
        {
            Pb_measure item = new Pb_measure(m);
            string out_path = Path.Combine(path, sanitize_name(item.name+".json"));
            string j = JsonConvert.SerializeObject(item, Formatting.Indented);
            File.WriteAllText(out_path, j);
        }
        static void serialize_column(string path, Column c)
        {
            Pb_column item = new Pb_column(c);
            string out_path = Path.Combine(path, sanitize_name(item.name + ".json"));
            string j = JsonConvert.SerializeObject(item, Formatting.Indented);
            File.WriteAllText(out_path, j);
        }
        static void serialize_table(string path, Table t, List<Relationship> relations)
        {
            Console.WriteLine("Serializing table: "+ t.Name);
            Pb_table item = new Pb_table(t, relations);
            string table_path = Path.Combine(path, sanitize_name(item.name));
            DirectoryInfo table_dir = System.IO.Directory.CreateDirectory(table_path);
            string j = JsonConvert.SerializeObject(item, Formatting.Indented);
            File.WriteAllText(Path.Combine(table_dir.FullName, sanitize_name(item.name + ".json")), j);
            string columns_path = Path.Combine(table_dir.FullName, "Columns");
            DirectoryInfo columns_dir = System.IO.Directory.CreateDirectory(columns_path);
            foreach (Column c in t.Columns)
                if (!c.IsHidden)
                    serialize_column(columns_dir.FullName, c);
            string measures_path = Path.Combine(table_dir.FullName, "Measures");
            DirectoryInfo measures_dir = System.IO.Directory.CreateDirectory(measures_path);
            foreach (Measure c in t.Measures)
                serialize_measure(measures_dir.FullName, c);
        }
        static void serialize_database(string path, Database db, Dictionary<string, List<Relationship>> relations)
        {
            Console.WriteLine("Serializing database");
            Pb_database item = new Pb_database(db);
            string j = JsonConvert.SerializeObject(item, Formatting.Indented);
            File.WriteAllText(Path.Combine(path, "database.json"), j);
            string tables_path = Path.Combine(path, "tables");
            DirectoryInfo tables_dir = System.IO.Directory.CreateDirectory(tables_path);
            foreach (Table t in db.Model.Tables)
            {
                if (!t.IsPrivate)
                {
                    List<Relationship> r;
                    relations.TryGetValue(t.Name, out r);
                    serialize_table(tables_dir.FullName, t, r);
                }
            }
        }

        static void Main(string[] args)
        {
            string server_port = args[0];
            string database_name = args[1];
            Console.WriteLine($"Server port: {server_port}, Database: {database_name}");
            string ConnectionString = "DataSource=" + server_port;
            Server server = new Server();
            Console.WriteLine("Connecting to server");
            try
            {
                server.Connect(ConnectionString);
                Database db = server.Databases[database_name];
                Console.WriteLine("Connection Succeeded");
                Console.WriteLine("Output path: " + Directory.GetCurrentDirectory());
                if (Directory.Exists("Versioning"))
                {
                    Console.WriteLine("Deleting current 'Versioning' folder");
                    Directory.Delete("Versioning", true);
                }
                DirectoryInfo base_dir = System.IO.Directory.CreateDirectory("Versioning");
                Console.WriteLine("Processing Relationships");
                Dictionary<string, List<Relationship>> relations = new Dictionary<string, List<Relationship>>();
                foreach (Relationship r in db.Model.Relationships)
                {
                    string from = r.FromTable.Name;
                    if (!relations.ContainsKey(from))
                        relations[from] = new List<Relationship>();
                    relations[from].Add(r);
                }
                serialize_database(base_dir.FullName, db, relations);

            }
            catch (Exception ex)
            {
                //generic exception
                Console.WriteLine(ex.ToString());
            }
            
            Console.WriteLine("Press Enter to close this console window.");
            Console.ReadLine();
        }
    }
}
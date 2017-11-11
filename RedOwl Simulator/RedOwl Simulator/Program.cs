using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Newtonsoft.Json;
using System.IO;
using KafkaNet;
using KafkaNet.Model;
using KafkaNet.Protocol;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace RedOwl_Simulator
{
    public class Program
    {
        public static Random rnd;
        static string FileLocation = String.Format(@"{0}\RO_UserRiskLevel.txt", System.IO.Directory.GetCurrentDirectory());  //@"C:\Users\natan.radostin\Source\Repos\Project for testers\KafkaDBImporter\KafkaDBImporter\bin\Debug\RO_UserRiskLevel.txt";

        static void Main(string[] args)
        {
            Console.ReadLine();

            if (args[0] == "-?")                          //      arg[0]    arg[1]       arg[2]                arg[3]                     arg[4]         arg[5]        arg[6]               
                Console.WriteLine("\n\nUsing:KafkaDBImporter.exe [SQL IP] [SQL USER] [ [Password] [EXTERNAL/INTERNAL/All users to scan] [Kafka IP]  [Kafka Topic] [Number of users should be downloaded from SQL/Default all users will be pulled] \n\n ");
            else
            {
                string connectionTypeToSql = String.Format(@"Data Source={0},1433;Network Library=DBMSSOCN;Initial Catalog=wbsn-data-security;User ID={1};Password={2}", (args[0]), args[1], args[2]);
                SqlConnection connection1 = new SqlConnection(connectionTypeToSql);
                connection1.Open();
                SqlCommand cmd = new SqlCommand(@"select * from pa_repo_users", connection1);
                SqlDataReader reader = cmd.ExecuteReader();
                StreamWriter writer = new StreamWriter(FileLocation);
                List<DataJson> testData = new List<DataJson>();
                List<RiskScore> riscore = new List<RiskScore>();
                riscore.Add(new RiskScore("_global_", 0.81));
                int user_limit = Convert.ToInt32(args[6]);
                

                switch (args[3])
                {
                    case "EXTERNAL":
                        DBImporterHelper.ScanOnlyExternalUsers(testData, reader, riscore, user_limit);
                        break;

                    case "INTERNAL":
                        DBImporterHelper.ScanOnlyInternalUsers(testData, reader, riscore, user_limit);
                        break;

                    default:
                        DBImporterHelper.ScanAllUsers(testData, reader, riscore, user_limit);
                        break;

                }



                Console.WriteLine("\n\n\n\n\n\n\t\t\t\t\t\t****************************WAIT FOR TRANFERING COMLETE****************************\n\n\n\n\n\n\t\t\t\t\t\t");
                reader.Close();
                connection1.Close();
                writeToJsonFile(testData, writer);



                writer.Close();
                string ip_Kafka = String.Format("http://{0}", args[4]);
                var configKafka = new KafkaOptions(new Uri(ip_Kafka));
                var route = new BrokerRouter(configKafka);

                var client = new Producer(route);
                string jsonFile = File.ReadAllText(FileLocation);

                JArray jsonArray = JArray.Parse(jsonFile);
                string topic = args[5];
                dynamic data;
                for (int i = 0; i < jsonArray.Count; i++)
                {

                    data = JObject.Parse(jsonArray[i].ToString());
                    client.SendMessageAsync(topic, new[] { new Message(Convert.ToString(data)) });
                    Thread.Sleep(300);
                }
                using (client) { };
                Console.WriteLine("\n\n\n\n\n\n\t\t\t\t\t\t****************************DONE!!!****************************\n\n\n\n\n\n\t\t\t\t\t\t");
            }

        }

        static void writeToJsonFile(List<DataJson> TestData, StreamWriter writer)

        {
            writer.Write(JsonConvert.SerializeObject(TestData, Newtonsoft.Json.Formatting.Indented));
        }


    }
}

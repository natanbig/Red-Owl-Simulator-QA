using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedOwl_Simulator
{
    class Program
    {
        public static Random rnd;
        static string FileLocation = String.Format(@"{0}\RO_UserRiskLevel.txt", System.IO.Directory.GetCurrentDirectory());  //@"C:\Users\natan.radostin\Source\Repos\Project for testers\KafkaDBImporter\KafkaDBImporter\bin\Debug\RO_UserRiskLevel.txt";

        static void Main(string[] args)
        {
            Console.ReadLine();

            if (args[0] == "-?")
                Console.WriteLine("\n\nUsing:KafkaDBImporter.exe [SQL IP] [SQL USER] [ [Password] [Kafka IP] [Kafka Topic] [Number of users should be downloaded from SQL] \n\n ");
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
                int user_limit = Convert.ToInt32(args[5]);
                rnd = new Random();
                while (reader.Read())
                {

                    Console.WriteLine("{0}", reader.GetString(0));
                    testData.Add(new DataJson(reader.GetString(0),
                    DateTime.Now.ToString("MM-dd-yyyyThh:mm:ssZ"),
                    Convert.ToInt32(rnd.NextDouble() * 5), riscore));
                    user_limit--;
                    if (user_limit == 0)
                    {
                        break;
                    }
                }
                Console.WriteLine("\n\n\n\n\n\n\t\t\t\t\t\t****************************WAIT FOR TRANFERING COMLETE****************************\n\n\n\n\n\n\t\t\t\t\t\t");
                reader.Close();
                connection1.Close();
                writeToJsonFile(testData, writer);



                writer.Close();
                string ip_Kafka = String.Format("http://{0}", args[3]);
                var configKafka = new KafkaOptions(new Uri(ip_Kafka));
                var route = new BrokerRouter(configKafka);

                var client = new Producer(route);
                string jsonFile = File.ReadAllText(FileLocation);

                JArray jsonArray = JArray.Parse(jsonFile);
                string topic = args[4];
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

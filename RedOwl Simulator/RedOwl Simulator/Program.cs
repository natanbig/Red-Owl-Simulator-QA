﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;


namespace RedOwl_Simulator
{
    
    public class Program
    {
        public static Random rnd;
        static string FileLocation = String.Format(@"{0}\RO_UserRiskLevel.txt", System.IO.Directory.GetCurrentDirectory());  //@"C:\Users\natan.radostin\Source\Repos\Project for testers\KafkaDBImporter\KafkaDBImporter\bin\Debug\RO_UserRiskLevel.txt";

        static SqlConnection connection1;
        static SqlCommand cmd;
        static SqlDataReader reader;
        private static string connectionTypeToSql;


 
        static void Main(string[] args)
        {
            StreamWriter writer;
            List<DataJson> testData;
            List<RiskScore> riscore;
            riscore = new List<RiskScore>();
            writer = new StreamWriter(FileLocation);
            testData = new List<DataJson>();
            riscore.Add(new RiskScore("_global_", 0.81));
            if (args[0] == "-?")                          //      arg[0]    arg[1]       arg[2]                arg[3]                     arg[4]                    arg[5]               
                Console.WriteLine("\n\nUsing:RedOwl Simulator.exe [SQL IP] [SQL USER] [ [Password] [EXTERNAL/INTERNAL/All users to scan] [Kafka IP:port]  [Number of users should be downloaded from SQL] \n\n or Using:RedOwl Simulator.exe [manual]      - for edditing risk level from cmd \n\n or Using:RedOwl Simulator.exe [automation] [SQL IP] [SQL USER] [Kafka IP:port] [email][new Risk level] ");
            else if (args[0] == "manual")

            {

                Console.WriteLine("\n\n\n\nEnter [SQL IP] [SQL USER] [Password] [Kafka IP:port]");
                string conectionSQL = Console.ReadLine();
                string[] array = conectionSQL.Split(' ');

                Console.Write("Enter number of user IDs you want to send to Kafka  ");
                int count = Convert.ToInt16(Console.ReadLine());
                string[] userData;
                userData = new string[count];
                connectionTypeToSql = String.Format(@"Data Source={0},1433;Network Library=DBMSSOCN;Initial Catalog=wbsn-data-security;User ID={1};Password={2}", (array[0]), array[1], array[2]);

                for (int i = 0; i < count; i++)
                {
                    Console.WriteLine("Input [User ID] [New Risk Level] for" + i + "users you want to change\n\n");
                    userData[i] = Console.ReadLine();
                    if (userData[i].Length > 40)
                    {
                        Console.WriteLine("Error: Too many digits pressed. Please retry");
                        i--;
                    }

                }
                StartSQLConnection();
                DBImporterHelper.FilterOnlyExistedUsers(userData, testData, reader, riscore);
                FinalizeAndSendToKafka(writer, testData, array[3]);

            }
            else if(args[0]=="automation")
            {
                connectionTypeToSql = String.Format(@"Data Source={0},1433;Network Library=DBMSSOCN;Initial Catalog=wbsn-data-security;User ID={1};Password={2}", (args[1]), args[2], args[3]);
                string userEmail = args[5];
                int new_RiskLevel = Convert.ToInt16(args[6]);
                StartSQLConnection();
                DBImporterHelper.ValidateIfEmailExistInDB(userEmail, new_RiskLevel, testData, reader, riscore);
                FinalizeAndSendToKafka(writer, testData, args[4]);
            }
            else

            {
                connectionTypeToSql = String.Format(@"Data Source={0},1433;Network Library=DBMSSOCN;Initial Catalog=wbsn-data-security;User ID={1};Password={2}", (args[0]), args[1], args[2]);
                StartSQLConnection();

                int user_limit = Convert.ToInt32(args[5]);


                switch (args[3])
                {
                    case "EXTERNAL":
                        DBImporterHelper.ScanOnlyExternalUsers(testData, reader, user_limit, riscore);
                        break;

                    case "INTERNAL":
                        DBImporterHelper.ScanOnlyInternalUsers(testData, reader, user_limit, riscore);
                        break;

                    default:
                        DBImporterHelper.ScanAllUsers(testData, reader, user_limit, riscore);
                        break;

                }
                FinalizeAndSendToKafka(writer, testData, args[4]);


            }


        }

        private static void FinalizeAndSendToKafka(StreamWriter writer, List<DataJson> testData, string ip)
        {
            CloseSQLConnection();
            WriteToFile(writer, testData);
            CopyFromFileAndSendToKafka(ip);
        }

        private static void WriteToFile(StreamWriter writer, List<DataJson> testData)
        {
            writeToJsonFile(testData, writer);
            writer.Close();
        }

        private static void CopyFromFileAndSendToKafka(string ip)
        {
            Console.WriteLine("\n\n\n\n\n\n\t\t\t\t\t\t****************************WAIT FOR TRANFERING COMLETE****************************\n\n\n\n\n\n\t\t\t\t\t\t");

            
            KafkaClientHelper producer = new KafkaClientHelper();

            producer.ConfigKafkaProducer(ip); //args[4] 

            string jsonFile = File.ReadAllText(FileLocation);
            JArray jsonArray = JArray.Parse(jsonFile);
            producer.SendDataToKafka("ENTITY_RISK_LEVEL", jsonArray);
            Console.WriteLine("\n\n\n\n\n\n\t\t\t\t\t\t\t****************************DONE!!!****************************\n\n\n\n\n\n\t\t\t\t\t\t");
        }

        private static void CloseSQLConnection()
        {
            reader.Close();
            connection1.Close();
        }

        private static void StartSQLConnection()
        {
            connection1 = new SqlConnection(connectionTypeToSql);
            connection1.Open();
            cmd = new SqlCommand(@"select * from pa_repo_users", connection1);
            reader = cmd.ExecuteReader();
        }

        static void writeToJsonFile(List<DataJson> TestData, StreamWriter writer)

        {
            writer.Write(JsonConvert.SerializeObject(TestData, Newtonsoft.Json.Formatting.Indented));
        }

        


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace RedOwl_Simulator
{
    public static class DBImporterHelper
    {
       

        public static void ScanOnlyExternalUsers(List<DataJson> testData, SqlDataReader reader, List<RiskScore> riscore, int user_limit)
        {
            Random rnd = new Random();
            
            while (reader.Read())
            {
                if (reader.GetString(1) == "EXTERNAL")
                {
                    ReadDB(testData, reader, riscore, rnd);
                    user_limit--;
                    if (user_limit == 0)
                    {
                        break;
                    }
                }
            }
        }



        public static void ScanOnlyInternalUsers(List<DataJson> testData, SqlDataReader reader, List<RiskScore> riscore, int user_limit)
        {
            Random rnd = new Random();

            while (reader.Read())
            {

                if (reader.GetString(1) == "INTERNAL")
                {
                    ReadDB(testData, reader, riscore, rnd);
                    user_limit--;
                    if (user_limit == 0)
                    {
                        break;
                    }
                }
            }
        }

        public static void  ScanAllUsers(List<DataJson> testData, SqlDataReader reader, List<RiskScore> riscore,  int user_limit)
        {
            Random rnd = new Random();

            while (reader.Read())
            {

                ReadDB(testData, reader, riscore, rnd);
                user_limit--;
                if (user_limit == 0)
                {
                    break;
                }
            }
            
        }

        private static void ReadDB(List<DataJson> testData, SqlDataReader reader, List<RiskScore> riscore, Random rnd)
        {
            Console.WriteLine("{0}", reader.GetString(0));
            testData.Add(new DataJson(reader.GetString(0),
            DateTime.Now.ToString("MM-dd-yyyyThh:mm:ssZ"),
            Convert.ToInt32(1+rnd.NextDouble() * 4), riscore));
            

        }


    }
}

using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace RedOwl_Simulator
{
    public static class DBImporterHelper
    {


        public static void FilterOnlyExistedUsers(string[] userData, List<DataJson> testData, SqlDataReader reader, List<RiskScore> riscore)
        {
            int remainingCount = userData.Length;
            while (reader.Read() && remainingCount!=0)
            {
                foreach(string element in userData)
                {
                    if (reader.GetString(0) == element.Substring(0, element.Length - 2))
                    {
                        int user_Defined_RL = Convert.ToInt32(element.Substring(element.Length-1,1));
                        CreateJsonObjectFromUserInputedData(testData, reader, riscore, user_Defined_RL);
                        remainingCount--;
                    }

                        
                }
            }
        }



        public static void ScanOnlyExternalUsers(List<DataJson> testData, SqlDataReader reader, List<RiskScore> riscore, int user_limit)
        {
            Random rnd = new Random();
            
            while (reader.Read())
            {
                if ((reader.GetString(1) == "EXTERNAL")&&(reader.GetString(3)!= "DELETED"))
                {
                    CreateJsonObjectFromScanningDB(testData, reader, riscore, rnd);
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

                if ((reader.GetString(1) == "INTERNAL") && (reader.GetString(3) != "DELETED"))
                {
                    CreateJsonObjectFromScanningDB(testData, reader, riscore, rnd);
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
                if (reader.GetString(3) != "DELETED")
                {
                    CreateJsonObjectFromScanningDB(testData, reader, riscore, rnd);
                    user_limit--;
                    if (user_limit == 0)
                    {
                        break;
                    }
                }
            }
            
        }

        private static void CreateJsonObjectFromScanningDB(List<DataJson> testData, SqlDataReader reader, List<RiskScore> riscore, Random rnd)
        {
            Console.WriteLine("{0}", reader.GetString(0));
            testData.Add(new DataJson(reader.GetString(0),
            DateTime.Now.ToString("MM-dd-yyyyThh:mm:ssZ"),
            Convert.ToInt32(1+rnd.NextDouble() * 4), riscore));
            

        }

        private static void CreateJsonObjectFromUserInputedData(List<DataJson> testData, SqlDataReader reader, List<RiskScore> riscore, int user_Defined_RL)
        {
            Console.WriteLine("{0}", reader.GetString(0));
            testData.Add(new DataJson(reader.GetString(0),
            DateTime.Now.ToString("MM-dd-yyyyThh:mm:ssZ"),
            user_Defined_RL, riscore));

        }


    }
}

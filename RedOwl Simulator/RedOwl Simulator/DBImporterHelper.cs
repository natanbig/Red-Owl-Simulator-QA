﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;

namespace RedOwl_Simulator
{
    public static class DBImporterHelper
    {

        internal static void ValidateIfEmailExistInDB(string userEmail, int new_RiskLevel, List<DataJson> testData, SqlDataReader reader, List<RiskScore> riscore)
        {
            bool found = false;
            while (reader.Read()&&!found)
            {
                if (reader.GetString(5) == userEmail && reader.GetInt32(12) != 0)
                {
                    CreateJsonObjectFromUserInputdData(testData, reader, new_RiskLevel,riscore);
                    found = true;
                }

            }
            if (!found)
                Console.WriteLine("\n\n\n\n\\t\t\t\t\t++++++++++++++The email: " + userEmail + "  doesn't exist or user is not a RAP user!++++++++++++++");
        }


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
                        CreateJsonObjectFromUserInputdData(testData, reader, user_Defined_RL,riscore);
                        remainingCount--;
                    }

                        
                }
            }
        }



        public static void ScanOnlyExternalUsers(List<DataJson> testData, SqlDataReader reader,  int user_limit, List<RiskScore> riscore)
        {
            Random rnd = new Random();
            
            while (reader.Read())
            {
                if ((reader.GetString(1) == "EXTERNAL")&&(reader.GetString(3)!= "DELETED")&&(reader.GetInt32(12) != 0))
                {
                    CreateJsonObjectFromScanningDB(testData, reader, rnd,riscore);
                    user_limit--;
                    if (user_limit == 0)
                    {
                        break;
                    }
                }
            }
        }



        public static void ScanOnlyInternalUsers(List<DataJson> testData, SqlDataReader reader, int user_limit, List<RiskScore> riscore)
        {
             Random rnd = new Random();

            while (reader.Read())
            {

                if ((reader.GetString(1) == "INTERNAL") && (reader.GetString(3) != "DELETED")&&(reader.GetInt32(12) != 0))
                {
                    CreateJsonObjectFromScanningDB(testData, reader, rnd, riscore);
                    user_limit--;
                    if (user_limit == 0)
                    {
                        break;
                    }
                }
            }
        }

        internal static void ValidateFileUsersExistsInDB(List<string> usersAndRL, List<DataJson> testData, SqlDataReader reader, List<RiskScore> riscore, string option)
        {
            for (int index = 0; index < usersAndRL.Count - 1; index = index + 2)
            {
                while (reader.Read())
                 {
                    if (reader.GetString(3) != "DELETED" && reader.GetInt32(12) != 0 && reader.GetString(0) == usersAndRL[index] && ((option=="fromfile")||(option== "sendandchangerl")))
                    {
                        CreateJsonObjectFromUserInputdData(testData, reader, Convert.ToInt32(usersAndRL[index + 1]), riscore);
                        Console.WriteLine("\nThe UserID = " + usersAndRL[index] + " FOUND!!!" + "\tuserName = "+reader.GetString(9) + "\tRiskLevel = " + reader.GetInt32(12));
                        break;
                    }
                    else if (option == "notvalid")
                    {
                        CreateJsonObjectFromUserInputdData(testData, reader, Convert.ToInt32(usersAndRL[index + 1]), riscore);
                        Console.WriteLine("\nThe UserID = " + usersAndRL[index] + " FOUND!!!" + "\tuserName = " + reader.GetString(9) + "\tRiskLevel = " + reader.GetInt32(12));
                        break;
                    }
                    else
                    {
                        Console.WriteLine("\n\n\t\t+++++++++++++++The UserID = " + usersAndRL[index] + " does not exist in Data Base");
                    }

                 }

            }
            usersAndRL.Clear();
            
        }

        public static void ImportDBToCSVFile(SqlDataReader reader, StreamWriter copyFromDB)
        {
            while (reader.Read())
            {
                if (reader.GetString(3) != "DELETED" && reader.GetInt32(12) != 0)
                {
                    copyFromDB.WriteLine(reader.GetString(0));
                }

            }
            copyFromDB.Flush();
            copyFromDB.Dispose();
        }

        public static void  ScanAllUsers(List<DataJson> testData, SqlDataReader reader,  int user_limit, List<RiskScore> riscore)
        {
            Random rnd = new Random();

            while (reader.Read())
            {
                if (reader.GetString(3) != "DELETED" && reader.GetInt32(12) != 0)
                {
                    CreateJsonObjectFromScanningDB(testData, reader, rnd, riscore);
                    user_limit--;
                    if (user_limit == 0)
                    {
                        break;
                    }
                }
            }
            
        }

        private static void CreateJsonObjectFromScanningDB(List<DataJson> testData, SqlDataReader reader, Random rnd, List<RiskScore> riscore)
        {
            Console.WriteLine("{0}", reader.GetString(0));
            testData.Add(new DataJson(reader.GetString(0),
            DateTime.Now.ToString("MM-dd-yyyyThh:mm:ssZ"),
            Convert.ToInt32(1+rnd.NextDouble() * 4),riscore));
            

        }

        private static void CreateJsonObjectFromUserInputdData(List<DataJson> testData, SqlDataReader reader, int user_Defined_RL, List<RiskScore> riscore)
        {
            
            testData.Add(new DataJson(reader.GetString(0),
            DateTime.Now.ToString("MM-dd-yyyyThh:mm:ssZ"),
            user_Defined_RL, riscore));

        }

        public static List<DataJson> CompareBetweenSentEntitiesAndDbEntities(List<DataJson> testData, SqlDataReader reader)
        {
            
            List<DataJson> notUpdatedUsers = new List<DataJson>();
            List<DataJson> copiedFromDb = new List<DataJson>();
            while (reader.Read())
            {
                copiedFromDb.Add((new DataJson(reader.GetString(0), reader.GetInt32(12))));

            }
            if (testData.Count != copiedFromDb.Count)
            {
                for (int i = 0; i < testData.Count; i++)
                {
                    for (int loop = 0; loop < copiedFromDb.Count; loop++)
                    {
                        if (testData[i].User_id == copiedFromDb[loop].User_id)
                            break;
                        if ((testData[i].User_id != copiedFromDb[loop].User_id) && (loop == copiedFromDb.Count - 1))
                        {
                            notUpdatedUsers.Add(new DataJson(testData[i].User_id, testData[i].Risk_level));
                        }

                    }
                }
            }
            return notUpdatedUsers;

            }
            
         }
        }
        
    



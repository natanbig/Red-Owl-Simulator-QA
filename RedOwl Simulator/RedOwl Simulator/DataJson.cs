using Newtonsoft.Json;
using System;
using System.Collections.Generic;
namespace RedOwl_Simulator
{
    public class DataJson
    {


        private string user_id;
        private string timestamp;
        private int risk_level;



        [JsonProperty("user_id")]
        public string User_id
        {
            get
            {
                return user_id;
            }

            set
            {
                user_id = value;
            }
        }

        [JsonProperty("timestamp")]
        public string Timestamp
        {
            get
            {
                return timestamp;
            }

            set
            {
                timestamp = value;
            }
        }

        [JsonProperty("risk_level")]
        public int Risk_level
        {
            get
            {
                return risk_level;
            }

            set
            {
                risk_level = value;
            }
        }
        public List<RiskScore> RiskScores { get; set; }

        public DataJson(string userId, string timestamp, int riskLevel)
        {
            this.user_id = userId;
            this.risk_level = riskLevel;
            this.timestamp = timestamp;
        }
        public DataJson(string userId, string timestamp, int riskLevel, List<RiskScore> RiskScores)
        {
            this.user_id = userId;
            this.risk_level = riskLevel;
            this.timestamp = timestamp;
            this.RiskScores = RiskScores;

        }

        public DataJson(string user_id, int risk_level)
        {
            this.user_id = user_id;
            this.risk_level = risk_level;
        }
    }



    }

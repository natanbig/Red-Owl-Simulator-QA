using Newtonsoft.Json;
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

        public DataJson(string userId, string timestamp, int riskLevel)
        {
            this.user_id = userId;
            this.risk_level = riskLevel;
            this.timestamp = timestamp;
        }
    }
}

using Newtonsoft.Json;
using System;
namespace RedOwl_Simulator
{
    public class RiskScore
    {
      public RiskScore(string scenario_id, double risk_score)
       {		
          ScenarioId = scenario_id;		
          PurpleRiskScore = (new Random().NextDouble() * risk_score).ToString("0.00");
        }

         [JsonProperty("risk_score"),]		
         public string PurpleRiskScore { get; set; }		
		
         [JsonProperty("scenario_id")]		
         public string ScenarioId { get; set; }

    }
}

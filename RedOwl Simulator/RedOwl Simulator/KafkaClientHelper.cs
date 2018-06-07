using System;
using KafkaNet.Model;
using KafkaNet;
using Newtonsoft.Json.Linq;
using System.Threading;
using KafkaNet.Protocol;
using System.Collections.Generic;

namespace RedOwl_Simulator
{
    public class KafkaClientHelper
    {
        
        private Producer client;
        public void ConfigKafkaProducer(string ip)
        {
            string ip_Kafka = String.Format("http://{0}", ip);
            var configKafka = new KafkaOptions(new Uri(ip_Kafka));
            var route = new BrokerRouter(configKafka);
            this.client = new Producer(route);
        }

        public void SendDataToKafka(string kafkaTopic, JArray jsonArray)
        {
            
            string topic = kafkaTopic;
            List<string> data = new List<string>();
            for (int i = 0; i < jsonArray.Count; i++)
            {

               
                data.Add(Convert.ToString(JObject.Parse(jsonArray[i].ToString())));
                
                {
                    
                    client.SendMessageAsync(topic, new[] { new Message(data[i].ToString()) });
                    if (i % 150 == 0 || jsonArray.Count-1==i)
                        Thread.Sleep(500);                       
                }
            }
            using (client) { };
        }

        ~KafkaClientHelper()
        {



        }
    }
}
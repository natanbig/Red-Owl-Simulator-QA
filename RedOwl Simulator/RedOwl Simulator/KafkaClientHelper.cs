using System;
using KafkaNet.Model;
using KafkaNet;
using Newtonsoft.Json.Linq;
using System.Threading;
using KafkaNet.Protocol;

namespace RedOwl_Simulator
{
    public class KafkaClientHelper
    {
        private  Producer client;
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
            dynamic data;
            for (int i = 0; i < jsonArray.Count; i++)
            {

                data = JObject.Parse(jsonArray[i].ToString());
                client.SendMessageAsync(topic, new[] { new Message(Convert.ToString(data)) }).Wait();
                //Thread.Sleep(300);
            }
            using (client) { };
        }

        ~KafkaClientHelper()
        {

            
            
       }
    }
}

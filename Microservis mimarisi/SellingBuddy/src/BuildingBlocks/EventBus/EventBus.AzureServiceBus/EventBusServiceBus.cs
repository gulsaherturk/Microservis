using EventBus.Base;
using EventBus.Base.Events;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.AzureServiceBus
{
    public class EventBusServiceBus : BaseEventBus// BaseEventBus'tan  türettik
    {
        private ITopicClient topicClient;
        private ManagementClient managementClient;
        public EventBusServiceBus(EventBusConfig config, IServiceProvider serviceProvider) : base(config, serviceProvider)
        {
            managementClient = new ManagementClient(config.EventBusConnectionString);//bizden bir tane connecitonstring istiyor confi içinde eventbus vardi 
        }
        private ITopicClient createTopicClient() 
        {
            if (topicClient==null|| topicClient.IsClosedOrClosing) //eğer topicClient boş ise veya topicClient kapalıysa
            {
                topicClient = new TopicClient(EventBusConfig.EventBusConnectionString,EventBusConfig.DefaultTopicName,RetryPolicy.Default);


            }
            if (!managementClient.TopicExistsAsync(EventBusConfig.DefaultTopicName).GetAwaiter().GetResult())//daha önce böyle bir topik var mı yok mu yoksa 
                managementClient.CreateTopicAsync
)
            {

            }
        }
        public override void Publish(IntegrationEvent @event)
        {
            throw new NotImplementedException();
        }

        public override void Subscribe<T, TH>()
        {
            throw new NotImplementedException();
        } 

        public override void UnSubscribe<T, TH>()
        {
            throw new NotImplementedException();
        }
    }

using EventBus.Base;
using EventBus.Base.Events;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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
        private ILogger logger;










        public EventBusServiceBus(EventBusConfig config, IServiceProvider serviceProvider) : base(config, serviceProvider)
        {

            logger = serviceProvider.GetService(typeof(ILogger<EventBusServiceBus>)) as ILogger<EventBusServiceBus>;

            managementClient = new ManagementClient(config.EventBusConnectionString);//bizden bir tane connecitonstring istiyor confi içinde EventBusConnectionString vardi 
            topicClient = createTopicClient();
        }

        //topic create etme 
        private ITopicClient createTopicClient()
        {
            if (topicClient == null || topicClient.IsClosedOrClosing) //eğer topicClient boş ise veya topicClient anlık olarak kapalıysa
            {
                topicClient = new TopicClient(EventBusConfig.EventBusConnectionString, EventBusConfig.DefaultTopicName, RetryPolicy.Default);//topicclient yarattık


            }
            if (!managementClient.TopicExistsAsync(EventBusConfig.DefaultTopicName).GetAwaiter().GetResult())//daha önce böyle bir topik var mı yok mu yoksa eğer burdan bir şey gelmiyorsa 
                managementClient.CreateTopicAsync(EventBusConfig.DefaultTopicName).GetAwaiter().GetResult(); // benim için bir tgane topic create etsin
            return topicClient;
            {

            }
        }
        public override void Publish(IntegrationEvent @event)
        {

            var eventName = @event.GetType().Name; //example: OrderCreatedIntegrationEvent
            eventName = ProcessEventName(eventName);//example: ordercreate

            var eventStr = JsonConvert.SerializeObject(@event);
            var bodyArr = Encoding.UTF8.GetBytes(eventStr);



            var message = new Message()
            {
                MessageId = Guid.NewGuid().ToString(),
                Body = null,
                Label = ""

            };
            topicClient.SendAsync(message).GetAwaiter().GetResult();
        }

        public override void Subscribe<T, TH>()
        {

            var eventName = typeof(T).Name;
            eventName = ProcessEventName(eventName);
            if (!SubsManager.HasSubscriptionsForEvent(eventName))

            {
                var subscriptionClient = CreateSubscriptionClientIfNotExists(eventName);

                RegisterSubscriptionClientMessageHandler(subscriptionClient);
            }
            logger.LogInformation("Subscribing to event {EventName} with{EventHandler}", eventName, typeof(TH).Name);

            SubsManager.AddSubscription<T, TH>();
        }

        public override void UnSubscribe<T, TH>()
       {
            var eventName = typeof(T).Name;
            try
            {
                var subscriptionClient = CreateSubscriptionClient(eventName);
                subscriptionClient
                    .RemoveRuleAsync(eventName)
                    .GetAwaiter()
                    .GetResult();

            }
            catch (MessagingEntityNotFoundException)
            {

                logger.LogWarning("The messaging entity{eventName} Could not be found.", eventName);

            }
            logger.LogInformation("Unsubscribing from event {EventName}", eventName);
            SubsManager.RemoveSubscription<T, TH>();
        }


        private void RegisterSubscriptionClientMessageHandler(ISubscriptionClient subscriptionClient)


        {
            subscriptionClient.RegisterMessageHandler(
                async (message, token) =>

                {
                    var eventName = $"{message.Label}";
                    var messageData = Encoding.UTF8.GetString(message.Body);

                    if (await ProcessEvent(ProcessEventName(eventName), messageData))
                    {
                        await subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);


                    }
                },
                new MessageHandlerOptions(exceptionReceivedHandler) { MaxConcurrentCalls=10,AutoComplete=false});
        } 
        private Task exceptionReceivedHandler(ExceptionReceivedEventArgs  exceptionReceivedEventArgs)
        {
            var ex = exceptionReceivedEventArgs.Exception;
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            logger.LogError(ex, "error handling message:{ExceptionMessage}-Context:{@ExceptionContext}", ex.Message, context);
            return Task.CompletedTask;
        }











        private ISubscriptionClient CreateSubscriptionClientIfNotExists(string eventName)
        {
            var subClient = CreateSubscriptionClient(eventName);

            var exists = managementClient.SubscriptionExistsAsync(EventBusConfig.DefaultTopicName, GetSubName(eventName)).GetAwaiter().GetResult();

            if (!exists)

            {

                managementClient.CreateSubscriptionAsync(EventBusConfig.DefaultTopicName, GetSubName(eventName)).GetAwaiter().GetResult();

                RemoveDefaultRule(subClient);
            }
            CreateRuleIfNotExists(ProcessEventName(eventName), subClient);

            return subClient;

        }

        private void CreateRuleIfNotExists(string eventName, ISubscriptionClient subscriptionClient)
        {
            bool ruleExists;
            try
            {
                var rule = managementClient.GetRuleAsync(EventBusConfig.DefaultTopicName, eventName, eventName).GetAwaiter().GetResult();
                ruleExists = rule != null;
            }
            catch (MessagingEntityNotFoundException)
            {
                //azure Management Client doesnt have RuleExists method

                ruleExists = false;
            }
            if (!ruleExists)
            {
                subscriptionClient.AddRuleAsync(new RuleDescription

                {
                    Filter = new CorrelationFilter { Label = eventName },
                    Name = eventName
                }).GetAwaiter().GetResult();
            }
        }
         










        private void RemoveDefaultRule(SubscriptionClient subscriptionClient)
        {
            try
            {
                subscriptionClient
                    .RemoveRuleAsync(RuleDescription.DefaultRuleName)
                    .GetAwaiter()
                    .GetResult();

            }
            catch (MessagingEntityNotFoundException)
            {

                logger.LogWarning("The messaging entity{DefaultRuleName} could not be found.", RuleDescription.DefaultRuleName);
            }
        }
        public override void Dispose()
        {
            base.Dispose();
            topicClient.CloseAsync().GetAwaiter().GetResult();
            managementClient.CloseAsync().GetAwaiter().GetResult();
            topicClient = null;
            managementClient = null;
        }

        private SubscriptionClient CreateSubscriptionClient(string eventName)
        {
            return new SubscriptionClient(EventBusConfig.EventBusConnectionString, EventBusConfig.DefaultTopicName, GetSubName(eventName));
        }
    }
}
using EventBus.Base.Abstraction;
using EventBus.Base.SubManagers;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Base.Events
{
    public abstract class BaseEventBus : IEventBus
    {
        public readonly IServiceProvider ServiceProvider;
        public readonly IEventBusSubscriptionManager SubsManager;
        public EventBusConfig EventBusConfig { get; set; }


        public BaseEventBus(EventBusConfig config, IServiceProvider serviceProvider)
        {
            EventBusConfig = config;
            ServiceProvider = serviceProvider;
            SubsManager = new InMemoryEventBusSubscriptionManager(ProcessEventName);

        }

        public virtual string ProcessEventName(string eventName)
        {
            if (EventBusConfig.DeleteEventPrefix) // eğer config tarafında deleteeventprefix seçilmiş ise başından bir şey silinmesi  seçilmiş ise  
                eventName = eventName.TrimStart(EventBusConfig.EventNamePrefix.ToArray());//String TrimStart yöntemi ile string ifadesinde bulunan ilk
                                                                                          //karakterleri kaldırmak için kullanılır. Geriye eşleşen karakterler
                                                                                          //silindikten sonraki string ifade döner.

            if (EventBusConfig.DeleteEventSuffix)
                eventName = eventName.TrimEnd(EventBusConfig.EventNameSuffix.ToArray());//String TrimEnd yöntemi string ifadesinin son karakterinden itibaren
                                                                                        //başlayarak eşleştiği ilk karakterleri siler. String ifade içerisinde
                                                                                        //sondan karakter temizlenmesi için kullanılır.

            return eventName;

        }
        public virtual string GetSubName(string eventName)
        {
            return $"{EventBusConfig.SubscriberClientAppName}.{ProcessEventName(eventName)}";
        }
        public virtual void Dispose()//Dispose metodunda gerekli kodlamaları yaparak ilgili nesnenin anında bellekten atılmasını sağlayabiliriz. 
        {
            EventBusConfig = null; // bir dispose işlei gerçekleştiğinde eventbuss null ypamak istiyoruz ma buna ramen ezilebilsin diye de virtual olarak tanımladık
        }


        public async Task<bool> ProcessEvent(string eventName,string message) // dışardan bir tane eventName gönderilicek  bir tane
                                                                              // message gönderilecek bu rabbbitmQ ve azureden bize ulaştırılmış bir mesaj
        {
            eventName = ProcessEventName(eventName);
            var processed = false;
            //eğer biz handle motdunu dinliyorsak bu çalışcak 
            if (SubsManager.HasSubscriptionsForEvent(eventName))
            {
                var subscriptions = SubsManager.GetHandlersForEvent(eventName);
                using (var scope = ServiceProvider.CreateScope())
                {
                    foreach (var subscription in subscriptions)
                    {
                        var handler = ServiceProvider.GetService(subscription.HandlerType); //handlertype ile service provide dan birtane service get ediyoruz

                        if (handler == null) continue;
                        var eventType = SubsManager.GetEventTypeByName($"{EventBusConfig.EventNamePrefix}{eventName}{EventBusConfig.EventNameSuffix}");// benim elimde bitane
                                                                                                                                                       // prefix var suufix de
                                                                                                                                                       // var eğer eklnmiş ise
                                                                                                                                                       // veya silinmiş ise onları
                                                                                                                                                       // başına aynı şekilde ekilyotuz
                                                                                                                                                       // ki biligilerimz eşleşsin  
                        var integrationEvent = JsonConvert.DeserializeObject(message, eventType);


                        var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
                        await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { integrationEvent });


                    }
                }
                processed = true;


            }
            return processed;
        }

        public abstract void Publish(IntegrationEvent @event);


        public abstract void Subscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>;

        public abstract void UnSubscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>;
          
    }
}

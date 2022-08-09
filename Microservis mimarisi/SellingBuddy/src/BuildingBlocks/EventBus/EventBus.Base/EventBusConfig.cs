using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Base
{
    //azure servise kaç kez bağlanabir gibi bir sürü ayarımız olacak  bu ayarlarıda dışardan alabilmek için eventBusConfig isimli classımızı oluşturyoruz
    public class EventBusConfig
    {
        public int ConnectionRetryCount { get; set; } = 5;//rabbitMQ bağlanırken 5 kez dene 
        public string DefaultTopicName { get; set; } = "SellingBuddyEventBus"; //hata almayalım diye default bir değer girdik eğer boş gelirse nu değeri göstericek
        public string EventBusConnectionString { get; set; } = String.Empty;
        public string SubscriberClientAppName { get; set; } = String.Empty; 
        public string EventNamePrefix { get; set; } = String.Empty;
        public string EventNameSuffix { get; set; } = "IntegrationEvent";
        public EventBusType EventBusType { get; set; } = EventBusType.RabbitMQ;//RabbitMQ default olarak gleicek yani biz kullan demedek bile default olarak bunu kullanıcak 
        public object Connection { get; set; }//obje tipinde bir cconnectionmız var

        public bool DeleteEventPrefix => !String.IsNullOrEmpty(EventNamePrefix);

        public bool DeleteEventSuffix => !String.IsNullOrEmpty(EventNameSuffix);


    }
    public enum EventBusType
    {
        RabbitMQ=0,
        AzureServiceBus=1

    }
}

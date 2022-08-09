using EventBus.Base.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Base.Abstraction
{
    //
     public interface IEventBusSubscriptionManager
    {
        bool IsEmpty { get; } //her hamgi bir eventi dinliyor muyuz 
      event  EventHandler<string> OnEventRemoved; // bu event remove edildiği zaman içerde bir event oluşturcaz ve dışardan bize gelen unsubscript metodu çaalıştığında bu eventı de tetikliyor olucaz.

        void AddSubscription<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>; //subscripte eklicez
        void RemoveSubscription<T, TH>() where TH : IIntegrationEventHandler<T> where T : IntegrationEvent;//subscripte silicez
        bool HasSubscriptionsForEvent<T>() where T : IntegrationEvent; //dışardan bize bir tane event gönderilidğinde  bizim zaten onu dinleyip dinlemediğimizi anlayan metot  
        bool HasSubscriptionsForEvent(string eventName);//burada da eventin adı gönderildiğind3 yukarıdakiyle aynı işlemi yapıcak 
        Type GetEventTypeByName(string eventName);//bir tane event name gönderildiğind  onun tipini göndericek
        void Clear();//listeyi silebilicez
        IEnumerable<SubscriptionInfo> GetHandlersForEvent<T>() where T : IntegrationEvent; // GetHandlersForEvent bunu için bize dışardan gönderilen bir eventin bütün subscriptionlarını bütün handlerlerini geriye döndürecğeiimz metot

        IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName);//dışardan event isminin parametre olarak alıyor
        string GetEventKey<T>();// eventlerimizin bir ismi olacak o isimleri UNİQ olarak kullanıcaz ve onlsarı kullanıarak projeye sürdürecğeimiz bu kod ise integration eventler içi kllanılan bir key 
    }
}

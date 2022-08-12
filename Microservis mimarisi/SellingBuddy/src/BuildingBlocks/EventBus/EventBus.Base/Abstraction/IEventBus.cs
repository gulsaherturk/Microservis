using EventBus.Base.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Base.Abstraction
{
    // bizim için uygulmalarımız microservislerimizin subscription işlemlerini hangi evente subscripton edicğini söyledikleri bir eventBus olacaktır
    public interface IEventBus :IDisposable
    {

        void Publish(IntegrationEvent @event);//servisimiz bir event fırlatacağı zmaan bu publish metodunu kullanıcak ve buna bir ıntegraiton event göndermek zorunda kalıcak o yuzden parametre olarak alalım

        //bize bir integratıon event bir de integratıonHandler vericek biz de onu subscirbe ediceğiz RabbitMQ veya Azure service Bus da ilgili kanalları oluşturcaz
        void Subscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>;// T integration event olma zorunluluğu ve TH IIntegtaitonEventHandler olma zorunluluğu <T> Tipinden

        void UnSubscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>;
    }
}
using EventBus.Base.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Base.Abstraction
{
    //oluşturudğumuz eventler handler edilmesi için kullanıcaz
 public interface IIntegrationEventHandler<TIntegrationEvent> : IntegrationEventHandler where TIntegrationEvent: IntegrationEvent
     // aşağıda oluşturduğumuz interface den implemente edilecek
     // dinamik olacak dışardan bir tip alcak <TIntegrationEvent>. buradaki şartımız
     // where ile birlikte bu TIntegrationEvent Integration handler classından türetilmiş olma zorunluluğu

    {
        Task Handle(TIntegrationEvent @event); //handle metodu 
    }
    public  interface IntegrationEventHandler
    {

    }
}

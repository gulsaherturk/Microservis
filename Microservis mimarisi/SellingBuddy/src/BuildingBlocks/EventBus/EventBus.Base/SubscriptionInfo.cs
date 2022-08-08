using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Base
{
    //dışardan bize gönderilen verilerin içerde tutulması için kullanıcaz
    public class SubscriptionInfo
    {
        public Type HandlerType { get; } // bize gönderilen integration eventin  tipini burda tutucaz bu tip üzerinden handle metoduna ulaşarak ilgili metodu çaırıcaz

        public SubscriptionInfo(Type handlerType)
        {
            HandlerType = handlerType ?? throw new ArgumentNullException(nameof(handlerType));
        }
        public static SubscriptionInfo Typed(Type handlerType)
        {
            return new SubscriptionInfo(handlerType);
        }
    }
}

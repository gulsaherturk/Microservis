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

        public SubscriptionInfo(Type handlerType)// dışardan controctor olaraqk aldığım için ( yani buradaki işlem )yukarıdaki set'i sildik
        {
            HandlerType = handlerType ?? throw new ArgumentNullException(nameof(handlerType));
        }
        public static SubscriptionInfo Typed(Type handlerType)// SubscriptionInfo statik oalrak dışardan da gönderilebilsin 
        {
            return new SubscriptionInfo(handlerType);
        }
    }
}

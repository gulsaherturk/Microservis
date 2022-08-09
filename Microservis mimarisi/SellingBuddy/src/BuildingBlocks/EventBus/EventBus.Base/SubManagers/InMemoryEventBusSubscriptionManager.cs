using EventBus.Base.Abstraction;
using EventBus.Base.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Base.SubManagers
{
  public  class InMemoryEventBusSubscriptionManager : IEventBusSubscriptionManager//interfaceden türedi
    {
        private readonly Dictionary<string, List<SubscriptionInfo>> _handlers;//handlerları tutabilmek için bir tane dictionary yaptık  SubscriptionInfo listesi var
        private readonly List<Type> _eventTypes;

   

        public event EventHandler<string> OnEventRemoved;
        public Func<string, string> eventNameGetter;



        // bir tane contructor var eventNameGetter isminde bir function dışardan parametre alıyor  
        public InMemoryEventBusSubscriptionManager(Func<string, string> eventNameGetter)
        {
            _handlers= new Dictionary<string, List<SubscriptionInfo>>();    
            _eventTypes = new List<Type>();
            this.eventNameGetter = eventNameGetter;//içerde eventNameGetter metotr var ama onu dışardan parametre göndericez
        }   


        public bool IsEmpty => !_handlers.Keys.Any();//handlerımızda key olup olmadığına bakıyoruz
        public void Clear()=> _handlers.Clear();//handlerı temizliyoruz

        

         


        public void AddSubscription<T,TH>() where T : IntegrationEvent where TH :IIntegrationEventHandler<T>
        {
            var eventName = GetEventKey<T>();//getevent key  metodunu çağırarak bunun eventinin ismini alıyoruz ve localde Subscription işeleimini gerçekleştiyoruz
            AddSubscription(typeof(TH), eventName);
            if (!_eventTypes.Contains(typeof(T)))
            {
                _eventTypes.Add(typeof(T));
            }
        }
        //(listeye ekle kısmı varsa hata ver yoksa listeye ekle
        private void AddSubscription(Type handlerType, string eventName)
        {
            if (!HasSubscriptionsForEvent(eventName)) //,dictionary de bunun için bir tane key var mı ona bakıyoruz eğer subscripte edilmediyse 
            {
            _handlers.Add(eventName, new List<SubscriptionInfo>()); //bunu gidip listemize ekliyoruz ve list kısmına yeni bir tane liste oluştuyoruz
            }
            if (_handlers[eventName].Any(s=> s.HandlerType==handlerType)) // integrationevent gelicekbir tana de tip gelecek bu tip ile daha önce başka bir hander var ise 
            {
                throw new ArgumentException($"Handler Type{handlerType.Name}already registered for '{eventName}'", nameof(handlerType));//burada hata göstericez(bu eventi daha önce göndermişssiniz)
            }
            _handlers[eventName].Add(SubscriptionInfo.Typed(handlerType));   //böyle b ir işlem gerçekleşmediyse SubscriptionInfo dan yeni bir tip yaratıp bunun tipinide verebiliyoruz
        }

        //var mı varsa git içerisindeki bul evetkeyı onu remove et
        public void RemoveSubscription<T,TH>() where TH : IIntegrationEventHandler<T> where T : IntegrationEvent
        {
            var handlerToRemove = FindSubscriptionToRemove<T, TH>();
            var eventName = GetEventKey<T>();
            RemoveHandler(eventName, handlerToRemove);
        }

       

        private void RemoveHandler(string eventName, SubscriptionInfo subsToRemove)
        {
            if (subsToRemove!=null)
            {
                _handlers[eventName].Remove(subsToRemove);
                if (!_handlers[eventName].Any())
                {
                    _handlers.Remove(eventName);    
                    var eventType= _eventTypes.SingleOrDefault(e =>e.Name==eventName);
                    if (eventType !=null)
                    {
                        _eventTypes.Remove(eventType);  

                    }
                    RaiseOnEventRemoved(eventName);

                }
            }
        }

        //handlerların listrsi tipini geri döndürdüğümüz bir metot
        public IEnumerable<SubscriptionInfo> GetHandlersForEvent<T>() where T : IntegrationEvent
        {
           var key = GetEventKey<T>();
            return GetHandlersForEvent(key);
        }


        public IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName) => _handlers[eventName];//keyın bütn dğerlerini geriye döndüror

        private void RaiseOnEventRemoved(string eventName)//eğer bir şey silindiyse kullanıcılara haber viercek
        {
            var handler = OnEventRemoved;
            handler?.Invoke(this, eventName);   
        }


       private SubscriptionInfo FindSubscriptionToRemove<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>
        {
            var eventName=GetEventKey<T>();
            return FindSubscriptionToRemove(eventName, typeof(TH));
        }


        private SubscriptionInfo FindSubscriptionToRemove(string eventName,Type HandlerType)
        {
            if (!HasSubscriptionsForEvent(eventName))
            {
                return null;

            }
            return _handlers[eventName].SingleOrDefault(s=>s.HandlerType== HandlerType);  // bu eventName ile handlertype olan bu event var mı diye bakıyoruz  
        }

     
        

        public bool HasSubscriptionsForEvent<T>() where T : IntegrationEvent
        {
        var key =GetEventKey<T>();
            return HasSubscriptionsForEvent(key);
        }

        public bool HasSubscriptionsForEvent(string eventName)=> _handlers.ContainsKey(eventName); //bu event ile key varmı ona bakıyoruz ve o bilgiyi geri dönüyoruz



        public Type GetEventTypeByName(string eventName)=>_eventTypes.SingleOrDefault(t=>t.Name== eventName);

        public string GetEventKey<T>()
        {
            string eventName=typeof(T).Name;
            return eventNameGetter(eventName);

        }



    }
}

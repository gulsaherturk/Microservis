using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Base.Events
{
    //Burası AzureServiceBus aracılığıyla diğer servislere haber ulaştılaran wventler anlamına gelecek (mimaride xervisler arası iletişimde kullanılan objelerimiz classlarımız olarak düşüenebiliriz
   public class IntegrationEvent
    {
        [JsonProperty]
        public Guid Id { get;  private set; }//ve Her crete edilen evetın id'si
        [JsonProperty]
        public DateTime CreatedDate { get; private set; } //bu event ne zmaan create edildi 


        public IntegrationEvent() //dışardan parametre gelmezse ne olacağını belirler
        {
            //GUID (Globally Unique IDentifier), bilgisayar yazılımlarında tanımlayıcı olarak kullanılan benzersiz bir referans numarasıdır.

            Id = Guid.NewGuid();
            CreatedDate = DateTime.Now;
        }
        [JsonConstructor]

        public IntegrationEvent(Guid id, DateTime createdDate) //dışardan parametre geliyor gibi düşünelim
        {
            Id = id;
            CreatedDate = createdDate;
        }
    }
}

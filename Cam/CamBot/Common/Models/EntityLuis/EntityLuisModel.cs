using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CamBot.Common.Models.EntityLuis
{
    public class EntityLuisModel
    {
        public List<DatetimeEntity>datetime { get; set; }
    }
    public class DatetimeEntity
    {
        public List <string>timex { get;  set; }
        public string type { get; set; }
    }
}

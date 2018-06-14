using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RxSubstitution.Models
{
    public class DrugModel
    {
        public string id { get; set; }
        public string ndc { get; set; }
        public string rxcui { get; set; }
        public string description { get; set; }
        public bool generic { get; set; }
        public bool active { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }
}

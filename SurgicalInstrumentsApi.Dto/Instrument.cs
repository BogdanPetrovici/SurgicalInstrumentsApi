using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgicalInstrumentsApi.Dto
{
    public class Instrument
    {
        public int InstrumentID { get; set; }
        public string Name { get; set; }
        public string Sku { get; set; }
        public string ImageUrl { get; set; }
    }
}

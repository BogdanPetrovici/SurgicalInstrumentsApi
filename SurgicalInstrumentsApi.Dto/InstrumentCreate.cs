using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgicalInstrumentsApi.Dto
{
    public class InstrumentCreate
    {
        public string Name { get; set; }
        public string Sku { get; set; }
        public string? ImageUrl { get; set; }

        public int SubcategoryID { get; set; }
    }
}

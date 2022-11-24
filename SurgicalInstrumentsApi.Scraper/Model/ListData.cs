using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgicalInstrumentsApi.Scraper.Model
{
    public class ListData<T>
    {
        public T Entity { get; set; }
        public string Url { get; set; }
    }
}

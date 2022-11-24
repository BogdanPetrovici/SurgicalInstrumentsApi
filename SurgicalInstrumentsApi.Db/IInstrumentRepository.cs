using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgicalInstrumentsApi.Db
{
    public interface IInstrumentRepository
    {
        public Dto.Instrument GetById(int id);
        public IEnumerable<Dto.Instrument> FindByName(string name);
        public Dto.Instrument CreateInstrument(Dto.InstrumentCreate instrument);

        public Dto.Instrument UpdateInstrument(Dto.InstrumentUpdate instrument);
        public Dto.Instrument DeleteInstrument(int instrumentId);
    }
}

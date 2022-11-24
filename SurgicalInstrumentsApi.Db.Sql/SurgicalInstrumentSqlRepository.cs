using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SurgicalInstrumentsApi.Db.Exceptions;

namespace SurgicalInstrumentsApi.Db.Sql
{
    public class SurgicalInstrumentSqlRepository : IInstrumentRepository
    {
        private readonly ILogger<SurgicalInstrumentSqlRepository> _logger;
        private readonly SurgicalInstrumentsDbContext _db;

        public SurgicalInstrumentSqlRepository(ILogger<SurgicalInstrumentSqlRepository> logger, SurgicalInstrumentsDbContext dbContext)
        {
            _logger = logger;
            _db = dbContext;
        }

        public IEnumerable<Dto.Instrument> FindByName(string name)
        {
            if (name.Length < 3) { return Enumerable.Empty<Dto.Instrument>(); }
            var instruments = _db.Instruments
                                .Where(i => i.Name.ToLower().Contains(name.ToLower()))
                                .ToList()
                                .Select(i => new Dto.Instrument { InstrumentID = i.InstrumentID, Name = i.Name, ImageUrl = i.ImageUrl });
            return instruments;
        }

        public Dto.Instrument GetById(int id)
        {
            var instrument = _db.Instruments
                                .Where(i => i.InstrumentID == id)
                                            .ToList()
                                            .Select(i => new Dto.Instrument { InstrumentID = i.InstrumentID, Name = i.Name, ImageUrl = i.ImageUrl })
                                            .FirstOrDefault();
            return instrument;
        }

        public Dto.Instrument CreateInstrument(Dto.InstrumentCreate instrument)
        {
            var subcategory = _db.Subcategories.Where(s => s.SubcategoryID == instrument.SubcategoryID).ToList().FirstOrDefault();
            if (subcategory == null) { throw new SubcategoryNotFoundException(); }
            var dbInstrument = new Model.Instrument { ImageUrl = instrument.ImageUrl, Sku = instrument.Sku, Name = instrument.Name, Subcategory = subcategory };
            _db.Instruments.Add(dbInstrument);

            try
            {
                _db.SaveChanges();
                return new Dto.Instrument { ImageUrl = dbInstrument.ImageUrl, Name = dbInstrument.Name, Sku = dbInstrument.Sku, InstrumentID = dbInstrument.InstrumentID };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while trying to commit changes to db");
                throw new DatabaseOperationException();
            }
        }

        public Dto.Instrument UpdateInstrument(Dto.InstrumentUpdate instrument)
        {
            var existingInstrument = _db.Instruments.Include(i => i.Subcategory).Where(i => i.InstrumentID == instrument.InstrumentID).ToList().FirstOrDefault();
            if (existingInstrument == null) { throw new InstrumentNotFoundException(); }
            if (instrument.SubcategoryID.HasValue && instrument.SubcategoryID != existingInstrument.Subcategory.SubcategoryID)
            {
                var subcategory = _db.Subcategories.Find(instrument.SubcategoryID);
                if (subcategory == null) { throw new SubcategoryNotFoundException(); }
                existingInstrument.Subcategory = subcategory;
            }

            if (!string.IsNullOrEmpty(instrument.Sku)) { existingInstrument.Sku = instrument.Sku; }
            if (!string.IsNullOrEmpty(instrument.Name)) { existingInstrument.Name = instrument.Name; }
            if (!string.IsNullOrEmpty(instrument.ImageUrl)) { existingInstrument.ImageUrl = instrument.ImageUrl; }

            try
            {
                _db.SaveChanges();
                return new Dto.Instrument { ImageUrl = existingInstrument.ImageUrl, Name = existingInstrument.Name, Sku = existingInstrument.Sku, InstrumentID = existingInstrument.InstrumentID };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while trying to commit changes to db");
                throw new DatabaseOperationException();
            }
        }

        public Dto.Instrument DeleteInstrument(int instrumentId)
        {
            var existingInstrument = _db.Instruments.Find(instrumentId);
            if (existingInstrument == null) { throw new InstrumentNotFoundException(); }
            _db.Instruments.Remove(existingInstrument);

            try
            {
                _db.SaveChanges();
                return new Dto.Instrument { ImageUrl = existingInstrument.ImageUrl, Name = existingInstrument.Name, Sku = existingInstrument.Sku, InstrumentID = existingInstrument.InstrumentID };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while trying to commit changes to db");
                throw new DatabaseOperationException();
            }
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using SurgicalInstrumentsApi.Db;
using SurgicalInstrumentsApi.Db.Exceptions;
using SurgicalInstrumentsApi.Db.Sql;
using SurgicalInstrumentsApi.Dto;

namespace SurgicalInstrumentsApi.Web.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class InstrumentController : ControllerBase
    {
        private readonly ILogger<InstrumentController> _logger;
        private IInstrumentRepository _instrumentRepository;

        public InstrumentController(ILogger<InstrumentController> logger, IInstrumentRepository instrumentRepository)
        {
            _logger = logger;
            _instrumentRepository = instrumentRepository;
        }

        [HttpGet("{id}", Name = "GetInstrument")]
        public ActionResult Get(int id)
        {
            var instrument = _instrumentRepository.GetById(id);
            if (instrument == null)
            {
                return NotFound();
            }

            return Ok(instrument);
        }

        [HttpGet(Name = "FindInstruments")]
        public IEnumerable<Instrument> Find(string q)
        {
            var instruments = _instrumentRepository.FindByName(q);
            return instruments;
        }

        [HttpPost(Name = "CreateInstrument")]
        public ActionResult Post([FromBody] InstrumentCreate instrument)
        {
            try
            {
                var newInstrument = _instrumentRepository.CreateInstrument(instrument);
                return Created($"/instruments/{newInstrument.InstrumentID}", newInstrument);
            }
            catch (SubcategoryNotFoundException ex)
            {
                return BadRequest("Invalid subcategory");
            }
            catch (DatabaseOperationException ex)
            {
                _logger.LogWarning("Could not save instrument to db", ex);
                return StatusCode(500);
            }
        }

        [HttpPut(Name = "UpdateInstrument")]
        public ActionResult Put([FromBody] InstrumentUpdate instrument)
        {
            try
            {
                var updatedInstrument = _instrumentRepository.UpdateInstrument(instrument);
                return Ok(updatedInstrument);
            }
            catch (InstrumentNotFoundException ex)
            {
                return NotFound("Instrument not found");
            }
            catch (SubcategoryNotFoundException ex)
            {
                return BadRequest("Invalid subcategory");
            }
            catch (DatabaseOperationException ex)
            {
                _logger.LogWarning("Error occurred while saving data.", ex);
                return StatusCode(500);
            }
        }

        [HttpDelete(Name = "DeleteInstrument")]
        public ActionResult Delete(int id)
        {
            try
            {
                var removedInstrument = _instrumentRepository.DeleteInstrument(id);
                return Ok(removedInstrument);
            }
            catch (InstrumentNotFoundException ex)
            {
                return NotFound("Instrument not found");
            }
            catch (DatabaseOperationException ex)
            {
                _logger.LogWarning("Could not delete instrument from db.", ex);
                return StatusCode(500);
            }
        }
    }
}

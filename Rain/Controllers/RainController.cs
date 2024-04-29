using Microsoft.AspNetCore.Mvc;
using Rain.Models;
using Rain.Services;

namespace Rain.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RainController : ControllerBase
    {
        private readonly RainService _rainService;


        public RainController(RainService rainService)
        {
            _rainService = rainService;       
        }

        // api/rains
        [HttpGet]
        public async Task<List<RainModel>> GetAll() => await _rainService.GetAllEntries();

        // api/rains/{id}
        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<RainModel>> GetById(string id)
        {
            var rain = await _rainService.GetEntryById(id);

            if (rain == null)
            {
                return NotFound();
            }
            return rain;
        }

        // api/jobs/filter?location={location}
        [HttpGet("filter")]
        public async Task<ActionResult<List<RainModel>>> GetByDate(string date)
        {
            var rainList = await _rainService.GetEntryByDate(date);
            if (rainList.Count == 0)
            {
                return NotFound();
            }
            return rainList;
        }

        // api/rains
        [HttpPost]
        public async Task<IActionResult> Post(RainModel newrain)
        {
            await _rainService.CreateEntry(newrain);
            return CreatedAtAction(nameof(GetById), new { id = newrain.Id }, newrain);
        }

        // api/rains/{id}
        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Put(string id, RainModel updatedrain)
        {
            var rain = await _rainService.GetEntryById(id);

            if (rain is null)
            {
                return NotFound();
            }
            updatedrain.Id = rain.Id;
            await _rainService.UpdateEntry(id, updatedrain);

            return NoContent();
        }

        // api/rains/{id}
        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var rain = await _rainService.GetEntryById(id);

            if (rain is null)
            {
                return NotFound();
            }
            await _rainService.RemoveEntry(id);
            return NoContent();
        }

    }
}

using Device.Models;
using Device.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Device.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeviceController : ControllerBase
    {
        private readonly DeviceService _deviceService;

        public DeviceController(DeviceService deviceService)
        {
            _deviceService = deviceService;
        }

        [HttpGet("GetByDateTimeRange")]
        public async Task<IActionResult> GetByDateTimeRange([FromQuery] string date, [FromQuery] string startTime, [FromQuery] string endTime)
        {
            if (string.IsNullOrEmpty(date) || string.IsNullOrEmpty(startTime) || string.IsNullOrEmpty(endTime))
            {
                return BadRequest("Date, start time, and end time parameters are required.");
            }

            var devices = await _deviceService.GetDevicesByDateTimeRange(date, startTime, endTime);

            if (devices == null || devices.Count == 0)
            {
                return NotFound();
            }

            return Ok(devices);
        }

        // api/Device/GetByDate?date={date}
        [HttpGet("GetByDate")]
        public async Task<ActionResult<List<DeviceModel>>> GetByDate(string date)
        {
            if (string.IsNullOrEmpty(date))
            {
                return BadRequest("Date is required.");
            }

            var rainList = await _deviceService.GetEntryByDate(date);
            if (rainList.Count == 0)
            {
                return NotFound();
            }
            return Ok(rainList);
        }

        // api/Device/GetAll
        [HttpGet("GetAll")]
        public async Task<List<DeviceModel>> GetAll() => await _deviceService.GetAllEntries();

        // api/Device/{id:length(24)}
        [HttpGet("GetById/{id:length(24)}")]
        public async Task<ActionResult<DeviceModel>> GetById(string id)
        {
            var rain = await _deviceService.GetEntryById(id);

            if (rain == null)
            {
                return NotFound();
            }
            return rain;
        }

        // api/Device/Create
        [HttpPost("Create")]
        public async Task<IActionResult> Post(DeviceModel newrain)
        {
            newrain.Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
            await _deviceService.CreateEntry(newrain);
            return CreatedAtAction(nameof(GetById), new { id = newrain.Id }, newrain);
        }

        // api/Device/Update/{id}
        [HttpPut("Update/{id:length(24)}")]
        public async Task<IActionResult> Put(string id, DeviceModel updatedrain)
        {
            var rain = await _deviceService.GetEntryById(id);

            if (rain is null)
            {
                return NotFound();
            }
            updatedrain.Id = rain.Id;
            await _deviceService.UpdateEntry(id, updatedrain);

            return NoContent();
        }

        // api/Device/Delete/{id}
        [HttpDelete("Delete/{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var rain = await _deviceService.GetEntryById(id);

            if (rain is null)
            {
                return NotFound();
            }
            await _deviceService.RemoveEntry(id);
            return NoContent();
        }
    }
}

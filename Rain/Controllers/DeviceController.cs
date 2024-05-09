using Device.Models;
using Device.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
//using Rain.Models;
//using Rain.Services;

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

        //public async Task<IActionResult> DisplayDeviceImage(string deviceId)
        //{
        //    DeviceModel device = await _deviceService.GetDeviceById(deviceId);
        //    if (device != null && device.value != null && device.value.Length > 0)
        //    {
        //        // Convert binary data to base64 string
        //        string base64Image = System.Convert.ToBase64String(device.value);

        //        // Create a ViewData dictionary to pass data to the view
        //        ViewData["Base64Image"] = base64Image;

        //        // Return the view with the ViewData
        //        return View("Views/Home/RainView.cshtml", device);
        //    }
        //    return NotFound();
        //}

        // api/Device
        [HttpGet]
        public async Task<List<DeviceModel>> GetAll() => await _deviceService.GetAllEntries();

        // api/Device/{id}
        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<DeviceModel>> GetById(string id)
        {
            var rain = await _deviceService.GetEntryById(id);

            if (rain == null)
            {
                return NotFound();
            }
            return rain;
        }

        // api/Device/filter?date={date}
        [HttpGet("filter")]
        public async Task<ActionResult<List<DeviceModel>>> GetByDate(string date)
        {
            var rainList = await _deviceService.GetEntryByDate(date);
            if (rainList.Count == 0)
            {
                return NotFound();
            }
            return rainList;
        }

        // api/rains
        [HttpPost]
        public async Task<IActionResult> Post(DeviceModel newrain)
        {
            newrain.Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
            await _deviceService.CreateEntry(newrain);
            return CreatedAtAction(nameof(GetById), new { id = newrain.Id }, newrain);
        }

        // api/Device/{id}
        [HttpPut("{id:length(24)}")]
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

        // api/Device/{id}
        [HttpDelete("{id:length(24)}")]
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


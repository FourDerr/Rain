using Device.Services;
using Microsoft.AspNetCore.Mvc;
using Rain.Models;
using RainFinal.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly DeviceService _deviceService;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult SignIn()
    {
        return View();
    }

    public IActionResult SignUp()
    {
        return View();
    }
    [Authorize]
    public IActionResult RainView(DateTime? datepicker)
    {
        if (datepicker.HasValue)
        {
            // Fetch data based on the selected date
            var data = _deviceService.GetEntryByDate(datepicker.Value.ToString("yyyy-MM-dd")).Result;
            ViewBag.Data = data;
            ViewBag.SelectedDate = datepicker.Value;
        }
        else
        {
            // Fetch all data if no date is selected
            //var data = _deviceService.GetAllEntries().Result;
            //ViewBag.Data = data;
        }

        return View();
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

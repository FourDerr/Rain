﻿using Device.Services;
using Microsoft.AspNetCore.Mvc;
using Rain.Models;
using RainFinal.Models;
using System.Diagnostics;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

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
    public IActionResult RainView(DateTime? datepicker)
    {
        if (datepicker.HasValue)
        {
            // Process the selected date
            // Example: ViewBag.SelectedDate = datepicker.Value;
        }

        return View();
    }

    

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

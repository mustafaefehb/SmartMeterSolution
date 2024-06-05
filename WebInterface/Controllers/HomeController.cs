using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebInterface.Services;
using WebInterface.Models;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

public class HomeController : Controller
{
    private readonly MeterServiceClient _meterServiceClient;
    private readonly ReportServiceClient _reportServiceClient;
    private readonly IConfiguration _configuration;

    public HomeController(
        MeterServiceClient meterServiceClient, 
        ReportServiceClient reportServiceClient,
        IConfiguration configuration)
    {
        _meterServiceClient = meterServiceClient;
        _reportServiceClient = reportServiceClient;
        _configuration = configuration;
    }

    public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10, string meterSerialNumber = null)
    {
        var pagedResult = await _meterServiceClient.GetMeterDataAsync(pageNumber, pageSize, meterSerialNumber);
        ViewBag.PageNumber = pagedResult.PageNumber;
        ViewBag.PageSize = pagedResult.PageSize;
        ViewBag.TotalPages = pagedResult.TotalPages;
        ViewBag.MeterSerialNumber = meterSerialNumber;
        return View(pagedResult.Data ?? new List<MeterData>());
    }

    public async Task<IActionResult> Reports(int pageNumber = 1, int pageSize = 10)
    {
        var reportServiceUrl = _configuration["Services:ReportService"];

        var pagedResult = await _reportServiceClient.GetReportsAsync(pageNumber, pageSize);
        ViewBag.PageNumber = pagedResult.PageNumber;
        ViewBag.PageSize = pagedResult.PageSize;
        ViewBag.TotalPages = pagedResult.TotalPages;
        ViewBag.ReportServiceUrl = reportServiceUrl;
        return View(pagedResult.Data ?? new List<Report>());
    }

    public async Task<IActionResult> ReportDetails(Guid id)
    {
        var report = await _reportServiceClient.GetReportByIdAsync(id);
        return View(report);
    }

    public IActionResult CreateMeterData()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateMeterData(MeterData meterData)
    {
        if (ModelState.IsValid)
        {
            await _meterServiceClient.CreateMeterDataAsync(meterData);
            return RedirectToAction(nameof(Index));
        }
        return View(meterData);
    }

    public IActionResult CreateReport()
    {
        return View(new CreateReportViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> CreateReport(CreateReportViewModel model)
    {
        if (ModelState.IsValid)
        {
            await _reportServiceClient.CreateReportAsync(model.MeterSerialNumber);
            return RedirectToAction(nameof(Reports));
        }
        return View(model);
    }
}

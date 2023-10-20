using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using POS.Models;
using POS.Services;

namespace POS.Controllers;

public class SaleController : Controller
{
    private readonly ILogger<SaleController> _logger;
    private readonly SaleService _service;

    public SaleController(ILogger<SaleController> logger, SaleService service)
    {
        _logger = logger;
        _service = service;
    }
    
    // GET
    public IActionResult Index()
    {
        var sales = _service.GetSales();
        return View(sales);
    }

    public IActionResult Details(string id)
    {
        var sale = _service.GetSale(id);
        return View(sale);
    }
    
    public IActionResult Create()
    {
        return View();
    }
    
    // HTTP METHODS
    

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
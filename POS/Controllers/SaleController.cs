using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using POS.Models;
using POS.Services;

namespace POS.Controllers;

public class SaleController : Controller
{
    private readonly ILogger<SaleController> _logger;
    private readonly SaleService _saleService;
    private readonly SupervisorService _supervisorService;

    public SaleController(ILogger<SaleController> logger, SaleService saleService, SupervisorService supervisorService)
    {
        _logger = logger;
        _saleService = saleService;
        _supervisorService = supervisorService;
    }
    
    // GET
    public IActionResult Index()
    {
        var sales = _saleService.GetAllSales();
        return View(sales);
    }
    
    public IActionResult Canceled()
    {
        var sales = _saleService.GetCanceledSales();
        return View(sales);
    }

    public IActionResult Details(string id)
    {
        var sale = _saleService.GetSale(id);
        return View(sale);
    }
    
    public IActionResult Create()
    {
        return View();
    }
    
    // HTTP METHODS
    
    
    [HttpPost("/Sale/Register")]
    public async Task<IActionResult> Register()
    {
        try
        {
            using var reader = new StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync();

            if (string.IsNullOrEmpty(body))
            {
                return BadRequest("Body is null or empty");
            }

            var sale = JsonSerializer.Deserialize<SaleModel>(body);
            Console.WriteLine(sale.ToJson());

            if (!ModelState.IsValid) return BadRequest();
            _saleService.CreateSale(sale);
            return Ok();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return BadRequest(ex.Message);
        }
    }
    
    [HttpPost]
    public IActionResult CancelSale(SaleModel sale)
    {
        Console.WriteLine(sale.ToJson());
        if(sale.Id == null || sale.Id.Equals("")) 
        {
            return BadRequest("Id de venta inválido");
        }

        _saleService.CancelSale(sale.Id);
        return RedirectToAction("Index"); // Redirige a la página principal o donde desees después de cancelar
    }

    [HttpPost("/VerifySupervisorToken")]
    public IActionResult VerifySupervisorToken(string token)
    {
        Console.WriteLine($"Token: {token}");
        var supervisor = _supervisorService.GetSupervisorByToken(token);
        if (supervisor == null)
        {
            return BadRequest(new { isValid = false, message = "Token del supervisor inválido." });
        }
    
        return Ok(new { isValid = true });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
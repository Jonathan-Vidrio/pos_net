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
    private readonly ProductService _productService;
    private readonly CanceledSaleService _canceledSaleService;

    public SaleController
    (
        ILogger<SaleController> logger,
        SaleService saleService, 
        SupervisorService supervisorService,
        ProductService productService,
        CanceledSaleService canceledSaleService
        )
    {
        _logger = logger;
        _saleService = saleService;
        _supervisorService = supervisorService;
        _productService = productService;
        _canceledSaleService = canceledSaleService;
    }
    
    // GET
    public IActionResult Index()
    {
        var sales = _saleService.GetAllSales();
        return View(sales);
    }
    
    public IActionResult CanceledIndex()
    {
        var canceledSales = _canceledSaleService.GetAll();
        var sales = _saleService.GetCanceledSales();
    
        var viewModelList = new List<CanceledSaleViewModel>();
        foreach(var canceledSale in canceledSales)
        {
            var sale = sales.FirstOrDefault(s => s.Id == canceledSale.SaleId.ToString());

            if(sale != null)
            {
                viewModelList.Add(new CanceledSaleViewModel 
                {
                    Sale = sale,
                    CanceledSale = canceledSale
                });
            }
            else
            {
                Console.WriteLine("Sale not found");
            }
        }

        return View(viewModelList);
    }

    public IActionResult Details(string id)
    {
        SaleModel sale = _saleService.GetSale(id);

        var productCodes = sale.SaleDetails.Select(s => s.Code).Distinct().ToList();
        var relatedProducts = _productService.GetProductsByCodes(productCodes);
        var productMap = relatedProducts.ToDictionary(p => p.Code, p => p);
        ViewBag.ProductMap = productMap;

        return View(sale);
    }
    
    public IActionResult Create()
    {
        return View();
    }
    
    // HTTP METHODS
    /*
    [HttpGet("/Product/GetByCode/${productCode}")]
    public IActionResult GetProductByCode(string productCode)
    {
        var product = _productService.GetProductByCode(productCode);
        if(product == null)
        {
            return BadRequest("Producto no encontrado");
        }
        return Ok(product);
    }
    */
    
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
            _productService.SubstractStock(sale.SaleDetails);
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
        return RedirectToAction("Index");
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
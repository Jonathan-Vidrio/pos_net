using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using POS.Models;
using POS.Services;

namespace POS.Controllers;

public class ProductController : Controller
{
    private readonly ProductService _service;
    
    public ProductController(ProductService productService)
    {
        _service = productService;
    }
    
    // GET
    public IActionResult Index()
    {
        var products = _service.GetProducts();
        return View(products);
    }
    
    public IActionResult Create()
    {
        return View();
    }
    
    public IActionResult Edit(string id)
    {
        var product = _service.GetProductById(id);
        return View(product);
    }
    
    // HTTP METHODS
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(ProductModel product)
    {
        if (!ModelState.IsValid) return View(product);
        _service.CreateProduct(product);
        return RedirectToAction(nameof(Index));
    }
    
    [HttpGet("Product/GetAll")]
    public IActionResult GetAll()
    {
        var products = _service.GetProducts();
        return Json(products);
    }
    
    [HttpGet("Product/GetByCode/{code}")]
    public IActionResult GetByCode(string code)
    {
        var product = _service.GetProductByCode(code);
        return Json(product);
    }
    
    [HttpGet("Product/Search/{term}")]
    public IActionResult Search(string term)
    {
        var product = _service.SearchProducts(term);
        return Json(product);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(string id, ProductModel product)
    {
        if (!ModelState.IsValid) return View(product);
        _service.UpdateProduct(id, product);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(string id)
    {
        _service.DeleteProduct(id);
        return RedirectToAction(nameof(Index));
    }
}
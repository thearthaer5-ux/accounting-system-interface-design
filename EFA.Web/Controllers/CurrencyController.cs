using EFA.Application.DTOs;
using EFA.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EFA.Web.Controllers;

[Authorize]
public class CurrencyController : Controller
{
    private readonly ICurrencyService _currencyService;
    private readonly ILogger<CurrencyController> _logger;

    public CurrencyController(ICurrencyService currencyService, ILogger<CurrencyController> logger)
    {
        _currencyService = currencyService;
        _logger = logger;
    }

    public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10)
    {
        var currencies = await _currencyService.GetAllCurrenciesAsync(pageNumber, pageSize);
        return View(currencies);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CurrencyDto model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _currencyService.CreateCurrencyAsync(model);

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        TempData["SuccessMessage"] = "تم إنشاء العملة بنجاح";
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Edit(int id)
    {
        var currency = await _currencyService.GetCurrencyByIdAsync(id);
        if (currency == null)
            return NotFound();

        return View(currency);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(CurrencyDto model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _currencyService.UpdateCurrencyAsync(model);

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        TempData["SuccessMessage"] = "تم تحديث العملة بنجاح";
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Details(int id)
    {
        var currency = await _currencyService.GetCurrencyByIdAsync(id);
        if (currency == null)
            return NotFound();

        return View(currency);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _currencyService.DeleteCurrencyAsync(id);

        if (result.Success)
            TempData["SuccessMessage"] = "تم حذف العملة بنجاح";
        else
            TempData["ErrorMessage"] = result.Message;

        return RedirectToAction("Index");
    }
}

using Microsoft.AspNetCore.Mvc;
using AndtrWeb.Models;

namespace AndtrWeb.Controllers;

public class EmployeeController : Controller
{
    private const int PageSize = 15;

    public IActionResult Search(string? q, int? recallClass, string? status, int page = 1)
    {
        var hits = RecallRepository.Search(q, recallClass, status);
        var totalPages = Math.Max(1, (int)Math.Ceiling(hits.Count / (double)PageSize));
        page = Math.Clamp(page, 1, totalPages);

        var vm = new EmployeeSearchViewModel
        {
            Query = q,
            RecallClass = recallClass,
            Status = status,
            Page = page,
            TotalPages = totalPages,
            TotalCount = hits.Count,
            Items = hits.Skip((page - 1) * PageSize).Take(PageSize).ToList()
        };
        return View(vm);
    }

    public IActionResult Notice(int id)
    {
        var notice = RecallRepository.Find(id);
        if (notice is null) return NotFound();
        return View(notice);
    }
}
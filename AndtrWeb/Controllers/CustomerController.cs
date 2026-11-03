using Microsoft.AspNetCore.Mvc;
using AndtrWeb.Models;

namespace AndtrWeb.Controllers;

public class CustomerController : Controller
{
    public IActionResult Dashboard(string? search)
    {
        // Temporary: pretend customer1 is logged in (replace with real login later)
        var mine = SampleData.Reviews.Where(r => r.CustomerUsername == "customer1");

        if (!string.IsNullOrWhiteSpace(search))
            mine = mine.Where(r =>
                r.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                r.DeviceName.Contains(search, StringComparison.OrdinalIgnoreCase));

        var vm = new CustomerDashboardViewModel
        {
            CustomerName = "Customer One",
            Search = search,
            Reviews = mine.OrderByDescending(r => r.SharedDate).ToList()
        };
        return View(vm);
    }

    // Placeholder so "View Details" works; we'll build the real page next
    public IActionResult Details(int id)
    {
        var review = SampleData.Reviews.FirstOrDefault(r => r.Id == id && r.CustomerUsername == "customer1");
        if (review is null || review.Status != "Available") return NotFound();
        return Content($"Details for: {review.Name}");
    }
}
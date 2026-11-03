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

    public IActionResult Details(int id)
    {
        // Temporary: pretend customer1 is logged in
        var review = SampleData.Reviews.FirstOrDefault(r => r.Id == id && r.CustomerUsername == "customer1");
        if (review is null) return NotFound();                       // not shared with this customer
        if (review.Status != "Available") return RedirectToAction("Dashboard");
        return View(review);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AddComment(int id, string text)
    {
        var review = SampleData.Reviews.FirstOrDefault(r => r.Id == id && r.CustomerUsername == "customer1");
        if (review is null) return NotFound();

        if (!string.IsNullOrWhiteSpace(text))
        {
            review.Comments.Add(new ReviewComment
            {
                Author = "Customer One",
                Role = "Customer",
                Text = text.Trim(),
                Date = DateTime.Now
            });
        }
        return RedirectToAction("Details", "Customer", new { id }, "comments");
    }
}

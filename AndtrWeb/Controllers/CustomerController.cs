using Microsoft.AspNetCore.Mvc;
using AndtrWeb.Models;

namespace AndtrWeb.Controllers;

public class CustomerController : Controller
{
    // Temporary: pretend customer1 is logged in (replace with real login later)
    private const string CurrentUser = "customer1";

    private SharedReview? FindMine(int id) =>
        SampleData.Reviews.FirstOrDefault(r => r.Id == id && r.CustomerUsername == CurrentUser);

    public IActionResult Dashboard(string? search)
    {
        var mine = SampleData.Reviews.Where(r => r.CustomerUsername == CurrentUser);

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
        var review = FindMine(id);
        if (review is null) return NotFound();
        if (review.Status != "Available") return RedirectToAction("Dashboard");
        return View(review);
    }

    public IActionResult Comments(int id)
    {
        var review = FindMine(id);
        if (review is null) return NotFound();          // not shared with this customer
        return View(review);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AddComment(int id, string text)
    {
        var review = FindMine(id);
        if (review is null) return NotFound();

        if (!review.CanComment)
        {
            TempData["Error"] = "Commenting is unavailable because this review has expired or access has been withdrawn.";
            return RedirectToAction("Comments", new { id });
        }

        if (string.IsNullOrWhiteSpace(text))
        {
            TempData["Error"] = "Please type a comment before sending.";
            return RedirectToAction("Comments", new { id });
        }

        review.Comments.Add(new ReviewComment
        {
            Author = "Customer One",
            Role = "Customer",
            Text = text.Trim().Length > 1000 ? text.Trim()[..1000] : text.Trim(),
            Date = DateTime.Now
        });

        TempData["Message"] = "Your comment has been sent.";
        return RedirectToAction("Comments", new { id });
    }
}
namespace AndtrWeb.Models;

public class SharedReview
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string DeviceName { get; set; } = "";
    public DateTime SharedDate { get; set; }
    public string Status { get; set; } = "Available";   // Available | Expired | Unavailable
    public string CustomerUsername { get; set; } = "";
}

public class CustomerDashboardViewModel
{
    public string CustomerName { get; set; } = "";
    public string? Search { get; set; }
    public List<SharedReview> Reviews { get; set; } = new();
}

public static class SampleData
{
    public static readonly List<SharedReview> Reviews = new()
    {
        new() { Id = 1, Name = "Infusion pump risk review", DeviceName = "InfuSafe 300",  SharedDate = new DateTime(2026, 10, 12), Status = "Available", CustomerUsername = "customer1" },
        new() { Id = 2, Name = "Pacemaker battery review",  DeviceName = "CardioPace X2", SharedDate = new DateTime(2026, 9, 3),   Status = "Expired",   CustomerUsername = "customer1" },
        new() { Id = 3, Name = "Ventilator alarm review",   DeviceName = "AirAssist V5",  SharedDate = new DateTime(2026, 10, 28), Status = "Available", CustomerUsername = "customer2" },
    };
}
namespace AndtrWeb.Models;

public class ReviewComment
{
    public string Author { get; set; } = "";
    public string Role { get; set; } = "";      // Customer | Employee
    public string Text { get; set; } = "";
    public DateTime Date { get; set; }
}

public class SharedReview
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string DeviceName { get; set; } = "";
    public DateTime SharedDate { get; set; }
    public string Status { get; set; } = "Available";   // Available | Expired | Unavailable
    public string CustomerUsername { get; set; } = "";

    // Wireframe 2 fields
    public string SharedBy { get; set; } = "";
    public DateTime? AccessExpires { get; set; }
    public string RiskLevel { get; set; } = "Low";       // Low | Medium | High
    public string RecallReference { get; set; } = "";
    public string ModelNumber { get; set; } = "";
    public string LotCode { get; set; } = "";
    public string Summary { get; set; } = "";
    public string PotentialRisks { get; set; } = "";
    public string RecommendedActions { get; set; } = "";
    public List<ReviewComment> Comments { get; set; } = new();
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
        new()
        {
            Id = 1, Name = "Infusion pump risk review", DeviceName = "InfuSafe 300",
            SharedDate = new DateTime(2026, 10, 12), Status = "Available", CustomerUsername = "customer1",
            SharedBy = "ANDtr - Alex (Risk Analyst)", AccessExpires = new DateTime(2027, 1, 12),
            RiskLevel = "High", RecallReference = "RC-2026-0412", ModelNumber = "IS-300-B", LotCode = "LOT-77421",
            Summary = "A software fault may cause the pump to deliver an incorrect flow rate after a battery change.",
            PotentialRisks = "Over- or under-infusion of medication, which could lead to patient harm.",
            RecommendedActions = "Quarantine affected units, apply firmware 2.4.1 and report any incidents to ANDtr.",
            Comments = new()
            {
                new() { Author = "Alex (ANDtr)", Role = "Employee", Text = "Please confirm how many units you hold from this lot.", Date = new DateTime(2026, 10, 13, 9, 30, 0) }
            }
        },
        new()
        {
            Id = 2, Name = "Pacemaker battery review", DeviceName = "CardioPace X2",
            SharedDate = new DateTime(2026, 9, 3), Status = "Expired", CustomerUsername = "customer1",
            SharedBy = "ANDtr - Alex (Risk Analyst)", AccessExpires = new DateTime(2026, 10, 3),
            RiskLevel = "Medium", RecallReference = "RC-2026-0377", ModelNumber = "CP-X2", LotCode = "LOT-55120",
            Summary = "Batteries in one lot may deplete earlier than expected.",
            PotentialRisks = "Loss of pacing if the battery is not replaced in time.",
            RecommendedActions = "Increase follow-up frequency for implanted patients."
        },
        new()
        {
            Id = 3, Name = "Ventilator alarm review", DeviceName = "AirAssist V5",
            SharedDate = new DateTime(2026, 10, 28), Status = "Available", CustomerUsername = "customer2",
            SharedBy = "ANDtr - Alex (Risk Analyst)", AccessExpires = null,
            RiskLevel = "High", RecallReference = "RC-2026-0450", ModelNumber = "AV-5", LotCode = "LOT-90311",
            Summary = "The low-pressure alarm may not sound when a circuit disconnects.",
            PotentialRisks = "Unnoticed disconnection leading to inadequate ventilation.",
            RecommendedActions = "Perform a manual alarm check before each use until repaired."
        },
    };
}

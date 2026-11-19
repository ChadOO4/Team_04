namespace AndtrWeb.Models;

public class RecallNotice
{
    public int Id { get; set; }
    public string ProductCode { get; set; } = "";
    public string ProductCodeName { get; set; } = "";
    public string RegulationNumber { get; set; } = "";
    public string RecallNumber { get; set; } = "";
    public string ProductDescription { get; set; } = "";
    public string TradeName { get; set; } = "";
    public int RecallClass { get; set; }                 // 1, 2 or 3 (FDA recall class)
    public DateTime? ClassifiedDate { get; set; }
    public DateTime? PostedDate { get; set; }
    public DateTime? TerminationDate { get; set; }
    public string FirmName { get; set; } = "";
    public string RecallReason { get; set; } = "";

    // Name to show in lists (some rows have no trade name)
    public string DisplayName => string.IsNullOrWhiteSpace(TradeName) ? ProductCodeName : TradeName;

    // FDA Class I = most serious. Mapped to the same High/Medium/Low labels the customer pages use.
    public string RiskLevel => RecallClass switch { 1 => "High", 2 => "Medium", _ => "Low" };

    // No termination date means the recall is still open
    public bool Ongoing => TerminationDate == null;
}

public record SearchHit(RecallNotice Notice, int Score);

public class EmployeeSearchViewModel
{
    public string? Query { get; set; }
    public int? RecallClass { get; set; }
    public string? Status { get; set; }                  // "ongoing" | "terminated" | null
    public int Page { get; set; } = 1;
    public int TotalPages { get; set; } = 1;
    public int TotalCount { get; set; }
    public List<SearchHit> Items { get; set; } = new();
}
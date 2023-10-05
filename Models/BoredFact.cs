using DemoProject.Data;

namespace DemoProject.Models;

public class BoredFact{
    public string? Activity { get; set; }
    public string? type { get; set; }
    public int Participants { get; set; }
    public decimal Price { get; set; }
    public string? link { get; set; }
    public string? key { get; set; }
    public decimal Accessibility { get; set; }
}
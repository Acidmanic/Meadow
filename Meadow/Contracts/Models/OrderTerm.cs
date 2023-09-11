namespace Meadow.Contracts.Models;

public class OrderTerm
{
    public string Key { get; set; } = "Id";

    public OrderSort Sort { get; set; } = OrderSort.Ascending;
}
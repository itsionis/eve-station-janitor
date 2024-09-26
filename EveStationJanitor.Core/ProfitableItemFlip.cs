namespace EveStationJanitor.Core;

public record class ProfitableItemFlip(string ItemName, int Quantity, decimal BuyPrice, decimal Profit)
{
    public decimal Margin => Profit / BuyPrice;
}

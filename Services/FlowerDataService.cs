using Practice4.Models;

namespace Practice4.Services;

public class FlowerDataService : IFlowerDataService
{
    public IReadOnlyList<FlowerDto> PrepareForShowcase(IEnumerable<Flower> flowers, int maxItems)
    {
        return flowers
            .OrderBy(f => f.Name)
            .Take(maxItems)
            .Select(f => new FlowerDto(
                f.Id,
                f.Name,
                f.Price,
                f.Stock,
                f.Category,
                f.Stock > 0 ? "Да" : "Нет"))
            .ToList();
    }
}

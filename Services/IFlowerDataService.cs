using System.Text.Json.Serialization;
using Practice4.Models;

namespace Practice4.Services;

public interface IFlowerDataService
{
    IReadOnlyList<FlowerDto> PrepareForShowcase(IEnumerable<Flower> flowers, int maxItems);
}

public record FlowerDto(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("price")] decimal Price,
    [property: JsonPropertyName("stock")] int Stock,
    [property: JsonPropertyName("category")] string Category,
    [property: JsonPropertyName("inStock")] string InStock);

using PortfolioLab.Domain.Trades;
using System.ComponentModel.DataAnnotations;

namespace PortfolioLab.Api.Dtos
{
    public class TradeRequest : IValidatableObject
    {
        [Required]
        public required string InstrumentId { get; init; }
        [Required]
        public required TradeSide Side { get; init; }
        public required decimal Quantity { get; init; }
        public required decimal UnitPrice { get; init; }
        public required DateTimeOffset ExecutedAt { get; init; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Quantity <= 0)
            {
                yield return new ValidationResult(
                    "Quantity must be greater than zero.",
                    new[] { nameof(Quantity) });
            }

            if (UnitPrice <= 0)
            {
                yield return new ValidationResult(
                    "UnitPrice must be greater than zero.",
                    new[] { nameof(UnitPrice) });
            }
        }
    }
}

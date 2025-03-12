using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using StockTradingApplication.Entities;

namespace StockTradingApplication.Entities;

[Table("portfolio_transaction")]
public class PortfolioTransaction : BaseEntity
{
    [Required]
    [Column("stock_price_at_transaction")]
    public decimal stockPriceAtTransaction { get; set; }

    [Required]
    [ForeignKey("Stock")]
    [Column("stock_id")]
    public long StockId { get; set; }

    public Stock Stock { get; set; }

    [Required]
    [ForeignKey("Portfolio")]
    [Column("portfolio_id")]
    public long PortfolioId { get; set; }

    public Portfolio Portfolio { get; set; }
}
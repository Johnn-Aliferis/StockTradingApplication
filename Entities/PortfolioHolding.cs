using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using StockTradingApplication.Entities;

namespace StockTradingApplication.Entities;

[Table("portfolio_holding")]
public class PortfolioHolding : BaseEntity
{
    [Column("stock_quantity")]
    public decimal Quantity { get; set; }
    
    [Required]
    [ForeignKey("Stock")]
    [Column("stock_id")]
    public long StockId { get; set; }

    public virtual Stock Stock { get; set; }

    [Required]
    [ForeignKey("Portfolio")]
    [Column("portfolio_id")]
    public long PortfolioId { get; set; }

    public virtual Portfolio Portfolio { get; set; }
}
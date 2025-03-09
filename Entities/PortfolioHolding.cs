using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockTradingApplication.Entities;

[Table("portfolio_holding")]
public class PortfolioHolding
{
    [Key]
    [Column("portfolio_holding_id")]
    public long Id { get; set; }
    
    [Required]
    [Column("created_at")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    [Column("stock_quantity")]
    public decimal Quantity { get; set; }
    
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
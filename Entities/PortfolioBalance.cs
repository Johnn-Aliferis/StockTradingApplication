using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockTradingApplication.Entities;

[Table("portfolio_balance")]
public class PortfolioBalance : BaseEntity
{

    [Column("balance")]
    public decimal Balance { get; set; }

    [Required]
    [ForeignKey("Portfolio")]
    [Column("portfolio_id")]
    public long PortfolioId { get; set; }

    public Portfolio Portfolio { get; set; }
}
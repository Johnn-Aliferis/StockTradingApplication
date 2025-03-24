using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockTradingApplication.Entities;

[Table("portfolio")]
public class Portfolio : ConcurrentParentEntity
{
    [Required]
    [ForeignKey("AppUser")]
    [Column("user_id")]
    public long UserId { get; set; }
    
    [Required]
    [Column("cash_balance")]
    public decimal CashBalance { get; set; }
    
    public AppUser AppUser { get; set; } 
    
    public ICollection<PortfolioTransaction> Transactions { get; set; }
    
    public ICollection<PortfolioHolding> Holdings { get; set; }
}
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
    
    public virtual AppUser AppUser { get; set; } 
    
    public virtual ICollection<PortfolioTransaction> Transactions { get; set; }
    
    public virtual ICollection<PortfolioHolding> Holdings { get; set; }
}
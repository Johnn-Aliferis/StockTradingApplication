using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using StockTradingApplication.Entities;

namespace StockTradingApplication.Entities;

[Table("portfolio")]
public class Portfolio : BaseEntity
{
    [Required]
    [ForeignKey("AppUser")]
    [Column("user_id")]
    public long UserId { get; set; }
    
    public AppUser AppUser { get; set; } 
    
    public PortfolioBalance Balance { get; set; } 
    
    public ICollection<PortfolioTransaction> Transactions { get; set; }
    
    public ICollection<PortfolioHolding> Holdings { get; set; }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockTradingApplication.Entities;

[Table("portfolio")]
public class Portfolio
{
    [Key]
    [Column("portfolio_id")]
    public long Id { get; set; }
    
    [Required]
    [Column("created_at")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    [Required]
    [ForeignKey("AppUser")]
    [Column("user_id")]
    public long UserId { get; set; }
    
    public AppUser AppUser { get; set; } 
    
    public ICollection<PortfolioTransaction> Transactions { get; set; }
    
    public ICollection<PortfolioHolding> Holdings { get; set; }
}
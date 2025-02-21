using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockTradingApplication.Entities;

[Table("stock")]
public class Stock
{
    [Key]
    [Column("stock_id")]
    public long Id { get; set; }
    
    [Required]
    [Column("stock_symbol")]
    public string Symbol { get; set; }
    
    [Required]
    [Column("stock_name")]
    public string Name { get; set; }
    
    [Required]
    [Column("stock_price")]
    public decimal Price { get; set; }
    
    [Required]
    [Column("stock_currency")]
    public string Currency { get; set; }
    
    [Required]
    [Column("updated_at")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    [Required]
    [Column("created_at")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public virtual ICollection<StockHistory> StockHistories { get; set; }
}
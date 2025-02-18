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
    
    [Column("stock_price")]
    public decimal Price { get; set; }
    
    [Column("stock_currency")]
    public string Currency { get; set; }
    
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
    
    [Column("created_At")]
    public DateTime CreatedAt { get; set; }
    
    public virtual ICollection<StockHistory> StockHistories { get; set; }
}
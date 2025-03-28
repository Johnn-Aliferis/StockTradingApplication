using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockTradingApplication.Entities;

[Table("stock")]
public class Stock : BaseEntity
{
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
    
    public virtual ICollection<StockHistory> StockHistories { get; set; }
}
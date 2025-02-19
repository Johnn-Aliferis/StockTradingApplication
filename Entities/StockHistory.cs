using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockTradingApplication.Entities;

[Table("stock_history")]
public class StockHistory
{
    [Key] 
    [Column("stock_history_id")]
    public long Id { get; set; }
    
    [Required]
    [Column("stock_price")]
    public decimal Price { get; set; }
    
    [Required]
    [Column("created_At")]
    public DateTime CreatedAt { get; set; }
    
    [Required]
    [Column("stock_id")]
    public long StockId { get; set; }

    [ForeignKey("StockId")]
    public virtual Stock Stock { get; set; }
}
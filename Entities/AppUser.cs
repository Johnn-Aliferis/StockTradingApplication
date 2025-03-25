using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockTradingApplication.Entities;

[Table("app_user")]
public class AppUser : BaseEntity
{
    [Required]
    [Column("username")]
    public string Username { get; set; }
    
    public virtual Portfolio Portfolio { get; set; }
}
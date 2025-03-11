using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockTradingApplication.Entities;

[Table("app_user")]
public class AppUser
{
    [Key]
    [Column("user_id")]
    public long Id { get; set; }
    
    [Required]
    [Column("username")]
    public string Username { get; set; }
    
    [Required]
    [Column("created_at")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public Portfolio Portfolio { get; set; }
}
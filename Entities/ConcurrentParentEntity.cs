using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockTradingApplication.Entities;

public abstract class ConcurrentParentEntity : BaseEntity
{
    [Timestamp]
    [Column("xmin")]
    public uint RowVersion { get; set; }
}
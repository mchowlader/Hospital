using System;

namespace Hospital.Domain.Common;

public abstract class BaseEntity
{
    public long Id { get; protected set; }
    
    public DateTime CreatedAt { get; set; }
    
    public long? CreatedBy { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
    
    public long? UpdatedBy { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public bool IsDeleted { get; set; } = false;
}

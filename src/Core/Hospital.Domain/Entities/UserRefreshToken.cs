using System;
using Hospital.Domain.Common;

namespace Hospital.Domain.Entities;

public class UserRefreshToken : BaseEntity
{
    public long UserId { get; set; }
    public User User { get; set; } = default!;

    public string Token { get; set; } = default!;
    public DateTime ExpiryTime { get; set; }
    public bool IsRevoked { get; set; }
    public string? CreatedByIp { get; set; }
}

namespace Hospital.Domain.Common;

public interface ICurrentUserProvider
{
    string? GetCurrentUsername();
    long? GetCurrentUserId();
}

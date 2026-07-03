namespace Hospital.Shared.Common;

public enum ErrorType
{
    Failure = 0,
    Validation = 1,
    NotFound = 2,
    Conflict = 3,
    Unauthorized = 4
}

public record Error(string Code, string Description, ErrorType Type)
{
    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.Failure);

    public static Error Validation(string description) => new("Validation", description, ErrorType.Validation);
    public static Error Conflict(string description) => new("Conflict", description, ErrorType.Conflict);
    public static Error NotFound(string description) => new("NotFound", description, ErrorType.NotFound);
    public static Error Failure(string description) => new("Failure", description, ErrorType.Failure);
    public static Error Unauthorized(string description) => new("Unauthorized", description, ErrorType.Unauthorized);
    public static Error Unexpected(string description) => new("Unexpected", description, ErrorType.Failure);
}

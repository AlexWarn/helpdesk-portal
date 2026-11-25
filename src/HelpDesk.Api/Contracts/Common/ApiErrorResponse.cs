namespace HelpDesk.Api.Contracts.Common;

public sealed record ApiErrorResponse(
    bool Success,
    object? Data,
    IReadOnlyList<ApiError> Errors);
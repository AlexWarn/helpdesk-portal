namespace HelpDesk.Api.Contracts.Common;

public sealed record ApiError(
    string Code,
    string Message);
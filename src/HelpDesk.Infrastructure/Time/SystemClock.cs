using HelpDesk.Application.Abstractions;

namespace HelpDesk.Infrastructure.Time;

public sealed class SystemClock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}
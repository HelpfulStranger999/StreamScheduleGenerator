using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace ScheduleGenerator;

public class Schedule
{
    public bool IsEmpty => Streams.Count <= 0;

    // Adding one includes the end date into the count
    public int Count => (EndDate?.DayNumber - StartDate?.DayNumber + 1) ?? 0;

    [MemberNotNullWhen(false, nameof(IsEmpty))]
    public DateOnly? StartDate { get; private set; } = null;

    [MemberNotNullWhen(false, nameof(IsEmpty))]
    public DateOnly? EndDate { get; private set; } = null;

    private Dictionary<DateOnly, ScheduledStream> Streams { get; } = [];

    public ScheduledStream this[DateOnly date]
    {
        get => Streams[date];
    }

    public void Add(ScheduledStream stream)
    {
        Streams.Add(stream.Date, stream);

        if ((StartDate is null) || (stream.Date < StartDate))
        {
            StartDate = stream.Date;
        }

        if ((EndDate is null) || (stream.Date > EndDate))
        {
            EndDate = stream.Date;
        }
    }

    public ImmutableList<ScheduledStream> ToList()
    {
        if (StartDate is null)
        {
            return [];
        }

        var streams = new List<ScheduledStream>();

        for (var date = (DateOnly)StartDate; date <= EndDate; date = date.AddDays(1))
        {
            if (Streams.TryGetValue(date, out var stream))
            {
                streams.Add(stream);
            }
            else
            {
                var newStream = new ScheduledStream(date);
                Streams.Add(date, newStream);
                streams.Add(newStream);
            }
        }

        return streams.ToImmutableList();
    }
}

public class ScheduledStream
{
    public bool DoesStream => Time != null;

    [MemberNotNullWhen(true, nameof(DoesStream))]
    public string? Title { get; set; } = null;

    [MemberNotNullWhen(true, nameof(DoesStream))]
    public TimeOnly? Time { get; set; }

    public DateOnly Date { get; }

    public ScheduledStream(DateOnly date)
    {
        Date = date;
    }
}

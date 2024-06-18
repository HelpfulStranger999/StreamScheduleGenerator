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

    public void Remove(ScheduledStream stream)
    {
        if (stream.Date != StartDate && stream.Date != EndDate)
        {
            Streams.Remove(stream.Date);
        }
        else
        {
            stream.Time = null;
            stream.Title = null;
            return;
        }

        if (Streams.Count == 1)
        {
            StartDate = null;
            EndDate = null;
            return;
        }

        if (stream.Date == StartDate)
        {
            var date = StartDate.Value.AddDays(1);
            ScheduledStream? newStream;

            while (!Streams.TryGetValue(date, out newStream))
            {
                date = date.AddDays(1);
            }

            StartDate = newStream.Date;
        }

        if (stream.Date == EndDate)
        {
            var date = EndDate.Value.AddDays(-1);
            ScheduledStream? newStream;

            while (!Streams.TryGetValue(date, out newStream))
            {
                date = date.AddDays(-1);
            }

            EndDate = newStream.Date;
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

public class ScheduledStream(DateOnly date)
{
    public bool DoesStream => Time != null;

    [MemberNotNullWhen(true, nameof(DoesStream))]
    public string? Title { get; set; } = null;

    [MemberNotNullWhen(true, nameof(DoesStream))]
    public TimeOnly? Time { get; set; }

    public DateOnly Date { get; } = date;

    public override string ToString()
    {
        if (DoesStream)
        {
            return $"streaming \"{Title}\" on {Date} at {Time}";
        }
        else
        {
            return $"not streaming on {Date}";
        }
    }
}

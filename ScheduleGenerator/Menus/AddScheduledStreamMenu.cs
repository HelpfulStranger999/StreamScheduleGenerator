namespace ScheduleGenerator.Menus;

using Sharprompt;

public class AddScheduledStreamMenu(Schedule schedule) : IMenu
{
    public virtual string MenuTitle => "Add a new scheduled stream";
    public Schedule Schedule { get; } = schedule;

    protected ScheduledStream GatherStreamDetails(DateOnly date)
    {
        var willStream = Prompt.Confirm($"Will you be streaming on {date.ToLongDateString()}?", defaultValue: true);
        if (!willStream)
        {
            return new ScheduledStream(date);
        }
        else
        {
            var title = Prompt.Input<string>("What will you stream?");
            while (title.Length > 15 && Prompt.Confirm("Warning: This title may run out of bounds. Do you wish to edit?"))
            {
                title = Prompt.Input<string>("What will you stream?");
            }

            var time = Prompt.Input<TimeOnly>("When will you stream?");
            return new ScheduledStream(date)
            {
                Title = title,
                Time = time
            };
        }
    }

    public virtual void Execute()
    {
        var date = Prompt.Input<DateOnly>("What day do you wish to add to the schedule?");
        Schedule.Add(GatherStreamDetails(date));
    }
}

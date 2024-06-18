namespace ScheduleGenerator.Menus;

using Sharprompt;

public class EditTimeMenu(ScheduledStream stream) : IMenu
{
    public string MenuTitle => "Edit Time";

    private ScheduledStream Stream { get; } = stream;

    public void Execute()
    {
        Stream.Time = Prompt.Input<TimeOnly>("When will you stream?", placeholder: Stream.Time?.ToString());

        if (string.IsNullOrWhiteSpace(Stream.Title))
        {
            Stream.Title = Prompt.Input<string>("Since this is a new stream, what will you stream?", placeholder: Stream.Title);
        }
    }
}

namespace ScheduleGenerator.Menus;

using Sharprompt;

public class EditTitleMenu(ScheduledStream stream) : IMenu
{
    public string MenuTitle => "Edit Title";

    private ScheduledStream Stream { get; } = stream;

    public void Execute()
    {
        Stream.Title = Prompt.Input<string>("What will you stream?", placeholder: Stream.Title);
        Stream.Time ??= Prompt.Input<TimeOnly>("Since this is a new stream, when will you stream?");
    }
}

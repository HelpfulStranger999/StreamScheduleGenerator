namespace ScheduleGenerator.Menus;

using Sharprompt;

public class RemoveStreamMenu(Schedule schedule, ScheduledStream stream) : IMenu
{
    public string MenuTitle => "Remove stream";

    private Schedule Schedule { get; } = schedule;
    private ScheduledStream Stream { get; } = stream;

    public void Execute()
    {
        if (Prompt.Confirm("Are you sure you wish to remove this stream?"))
        {
            Stream.Time = null;
            Stream.Title = null;

            if (Stream.Date == Schedule.EndDate || Stream.Date == Schedule.StartDate)
            {
                Schedule.Remove(Stream);
            }
        }
    }
}

namespace ScheduleGenerator.Menus;

using Sharprompt;

public class StartScheduleMenu(Schedule schedule) : AddScheduledStreamMenu(schedule)
{
    public override void Execute()
    {
        var date = Prompt.Input<DateOnly>("What is the first date of the stream schedule?", defaultValue: DateOnly.FromDateTime(DateTime.Now).AddDays(1), placeholder: "Tomorrow");
        Schedule.Add(GatherStreamDetails(date));
    }
}

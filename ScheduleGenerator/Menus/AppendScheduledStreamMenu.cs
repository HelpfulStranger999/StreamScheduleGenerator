namespace ScheduleGenerator.Menus;

public class AppendScheduledStreamMenu(Schedule schedule) : AddScheduledStreamMenu(schedule)
{
    public override string MenuTitle => "Append a stream to the schedule";

    public override void Execute()
    {
        if (Schedule.EndDate is null)
        {
            base.Execute();
        }
        else
        {
            var date = Schedule.EndDate.Value.AddDays(1);
            Schedule.Add(GatherStreamDetails(date));
        }
    }
}

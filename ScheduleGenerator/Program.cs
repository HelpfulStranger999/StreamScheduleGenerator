using Sharprompt;
using ScheduleGenerator;
using ScheduleGenerator.Menus;

var schedule = new Schedule();
IMenu menu = new StartScheduleMenu(schedule);
menu.Execute();

while (Prompt.Confirm("Add another day to the schedule?", defaultValue: true))
{
    menu = new AppendScheduledStreamMenu(schedule);
    menu.Execute();
}

while (true)
{
    menu = new ReviewScheduleMenu(schedule);
    menu.Execute();
}



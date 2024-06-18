namespace ScheduleGenerator.Menus;

using Sharprompt;

public class ReviewScheduleMenu(Schedule schedule) : IMenu
{
    public string MenuTitle => "Review Schedule";

    public void Execute()
    {
        Console.WriteLine();
        Console.WriteLine("Here is your stream schedule:");

        foreach (var stream in schedule.ToList())
        {
            Console.Write("\t");
            Console.WriteLine(stream.ToString());
        }

        var submenus = new List<IMenu>()
        {
            new AddScheduledStreamMenu(schedule),
            new EditScheduleMenu(schedule),
            new GenerateScheduleMenu(schedule)
        };

        var menu = Prompt.Select("What do you want to do?", submenus, defaultValue: submenus[^1], textSelector: submenu => submenu.MenuTitle);
        menu.Execute();
    }
}

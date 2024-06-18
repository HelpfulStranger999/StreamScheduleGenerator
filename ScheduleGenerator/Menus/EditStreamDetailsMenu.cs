namespace ScheduleGenerator.Menus;

using Sharprompt;

public class EditStreamDetailsMenu(Schedule schedule, ScheduledStream stream) : IMenu
{
    public string MenuTitle => Stream.ToString();

    private Schedule Schedule { get; } = schedule;
    private ScheduledStream Stream { get; } = stream;

    public void Execute()
    {
        var submenus = new List<IMenu>() {
            new EditTimeMenu(Stream),
            new EditTitleMenu(Stream),
            new RemoveStreamMenu(Schedule, Stream),
            new CancelMenu()
        };

        var submenu = Prompt.Select("What do you want to edit?", submenus, textSelector: submenu => submenu.MenuTitle);
        submenu.Execute();
    }
}

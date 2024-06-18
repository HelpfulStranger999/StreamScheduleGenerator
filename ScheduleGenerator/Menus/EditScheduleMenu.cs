namespace ScheduleGenerator.Menus;

using Sharprompt;

public class EditScheduleMenu(Schedule schedule) : IMenu
{
    public string MenuTitle => "Edit Schedule";
    private Schedule Schedule { get; } = schedule;

    public void Execute()
    {
        var submenus = Schedule.ToList().Select(stream => new EditStreamDetailsMenu(Schedule, stream)).ToList<IMenu>();
        submenus.Add(new CancelMenu());

        var submenu = Prompt.Select("Which stream do you wish to edit?", submenus, textSelector: submenu => submenu.MenuTitle);
        submenu.Execute();
    }
}
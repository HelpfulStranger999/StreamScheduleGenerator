namespace ScheduleGenerator.Menus;

public interface IMenu
{
    string MenuTitle { get; }

    void Execute();
}

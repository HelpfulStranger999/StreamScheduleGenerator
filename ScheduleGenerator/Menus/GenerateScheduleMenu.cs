using Sharprompt;
using SixLabors.ImageSharp;

namespace ScheduleGenerator.Menus;

public class GenerateScheduleMenu(Schedule schedule) : IMenu
{
    public string MenuTitle => "Generate Image";

    public Schedule Schedule { get; } = schedule;

    public void Execute()
    {
        var imageGenerator = new ImageGenerator(TimeZoneInfo.FindSystemTimeZoneById("America/Boise"));
        var image = imageGenerator.GenerateScheduleImage(Schedule);
        Console.WriteLine("Schedule successfully generated.");

        var defaultFileLocation = $"Schedule_{Schedule.StartDate?.ToString("MM-dd-yy")}_{Schedule.EndDate?.ToString("MM-dd-yy")}";
        string fileLocation;

        do
        {
            fileLocation = Prompt.Input<string>("Where would you like the schedule saved?", defaultValue: defaultFileLocation);
        } while (File.Exists($"{fileLocation}.png") && !Prompt.Confirm("A file already exists at this location. Do you wish to override?"));

        image.SaveAsPng($"{fileLocation}.png");
        Environment.Exit(0);
    }
}

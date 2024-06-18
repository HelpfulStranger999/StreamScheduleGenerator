using Sharprompt;
using SixLabors.ImageSharp;
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

menu = new ReviewScheduleMenu(schedule);
menu.Execute();

var imageGenerator = new ImageGenerator(TimeZoneInfo.FindSystemTimeZoneById("America/Boise"));
var image = imageGenerator.GenerateScheduleImage(schedule);
Console.WriteLine("Schedule successfully generated.");

var defaultFileLocation = $"Schedule_{schedule.StartDate?.ToString("MM-dd-yy")}_{schedule.EndDate?.ToString("MM-dd-yy")}";
string fileLocation;

do
{
    fileLocation = Prompt.Input<string>("Where would you like the schedule saved?", defaultValue: defaultFileLocation);
} while (File.Exists($"{fileLocation}.png") && !Prompt.Confirm("A file already exists at this location. Do you wish to override?"));

await image.SaveAsPngAsync($"{fileLocation}.png");


using System.Reflection;
using System.Text;
using Sharprompt;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

using ScheduleGenerator;
using ScheduleGenerator.Menus;

FontFamily GetFontFamily()
{
    var collection = new FontCollection();

    var assembly = Assembly.GetExecutingAssembly();
    var name = assembly.GetName().Name;

    using var stream = assembly.GetManifestResourceStream($"{name}.font.DancingScript-VariableFont_wght.ttf");
    return collection.Add(stream!);
}

Image<Rgba32> GenerateScheduleImage(Schedule schedule, FontFamily fontFamily)
{
    if (schedule.IsEmpty) throw new InvalidOperationException("Cannot generate a schedule without streams");
    var image = new Image<Rgba32>(400, 140 + schedule.Count * 110, Rgba32.ParseHex("#F4EEDA"));

    var largeFont = fontFamily.CreateFont(50, FontStyle.Regular);
    var smallFont = fontFamily.CreateFont(20, FontStyle.Regular);
    var regularFont = fontFamily.CreateFont(30, FontStyle.Regular);
    var medFont = fontFamily.CreateFont(35, FontStyle.Regular);

    var textColor = new Color(Rgba32.ParseHex("#000000"));
    var pen = new SolidPen(textColor, 10);

    var firstDate = (DateOnly)schedule.StartDate!;
    var lastDate = (DateOnly)schedule.EndDate!;
    var sameMonth = firstDate.Month == lastDate.Month;

    var dateRange = new StringBuilder();

    if (sameMonth)
    {
        dateRange.Append(firstDate.ToString("MMMM d"));
        dateRange.Append(" - ");
        dateRange.Append(lastDate.Day);
    }
    else
    {
        dateRange.Append(firstDate.ToString("MMM d"));
        dateRange.Append(" - ");
        dateRange.Append(lastDate.ToString("MMM d"));
    }

    var streams = schedule.ToList();
    image.Mutate(context =>
    {
        context.DrawText($"Weekly Schedule\nfor {dateRange}", largeFont, textColor,
            new PointF(x: 20, y: 30));

        context.DrawLine(pen, new PointF(160, 140), new PointF(160, image.Height));

        for (int i = 0; i < streams.Count; i++)
        {
            int xPos = 140 + i * 110;
            var schedule = streams[i];

            context.DrawLine(pen, new PointF(0, xPos), new PointF(400, xPos));

            context.DrawText(schedule.Date.ToString("dddd"), smallFont, textColor, new PointF(20, xPos + 20));
            context.DrawText(schedule.Date.ToString("MMM d"), regularFont, textColor, new PointF(20, xPos + 50));

            if (schedule.Time == null)
            {
                context.DrawText("No Stream", medFont, textColor, new PointF(180, xPos + 40));
            }
            else
            {
                string timezone = TimeZoneInfo.Local.IsDaylightSavingTime(schedule.Date.ToDateTime((TimeOnly)schedule.Time)) ? "MDT" : "MST";
                context.DrawText($"{schedule.Title}\nat {schedule.Time?.ToString("t")} {timezone}", regularFont, textColor, new PointF(180, xPos + 30));
            }
        }

    });

    return image;
}

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

var image = GenerateScheduleImage(schedule, GetFontFamily());
Console.WriteLine("Schedule successfully generated.");

var defaultFileLocation = $"Schedule_{schedule.StartDate?.ToString("MM-dd-yy")}_{schedule.EndDate?.ToString("MM-dd-yy")}";
string fileLocation;

do
{
    fileLocation = Prompt.Input<string>("Where would you like the schedule saved?", defaultValue: defaultFileLocation);
} while (File.Exists($"{fileLocation}.png") && !Prompt.Confirm("A file already exists at this location. Do you wish to override?"));

await image.SaveAsPngAsync($"{fileLocation}.png");

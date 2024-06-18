
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

ScheduledStream CreateScheduleDay(DateOnly currentDate)
{
    var willStream = Prompt.Confirm($"Will you be streaming on {currentDate.ToLongDateString()}?", defaultValue: true);
    if (!willStream)
    {
        return new ScheduledStream(currentDate);
    }
    else
    {
        var title = Prompt.Input<string>("What will you stream?");
        while (title.Length > 15 && Prompt.Confirm("Warning: This title may run out of bounds. Do you wish to edit?"))
        {
            title = Prompt.Input<string>("What will you stream?");
        }

        var time = Prompt.Input<TimeOnly>("When will you stream?");
        return new ScheduledStream(currentDate)
        {
            Title = title,
            Time = time
        };
    }
}


var firstDate = Prompt.Input<DateOnly>("What is the first date of the stream schedule?", defaultValue: DateOnly.FromDateTime(DateTime.Now).AddDays(1), placeholder: "Tomorrow");
var schedule = new Schedule();

DateOnly currentDate = firstDate;
while (true)
{
    schedule.Add(CreateScheduleDay(currentDate));
    if (Prompt.Confirm("Add another day to the schedule?", defaultValue: true))
    {
        currentDate = currentDate.AddDays(1);
    }
    else
    {
        break;
    }
}

while (true)
{
    Console.WriteLine();
    Console.WriteLine("Here is your stream schedule:");

    foreach (var stream in schedule.ToList())
    {
        Console.Write("\t");
        Console.WriteLine(stream.ToString());
    }

    if (!Prompt.Confirm("Do you wish to edit the schedule before generating it?"))
    {
        break;
    }

    var editMenu = new EditScheduleMenu(schedule);
    editMenu.Execute();
}

var image = GenerateScheduleImage(schedule, GetFontFamily());

var fileLocation = Prompt.Input<string>("Schedule generated. Where would you like the schedule saved?");

await image.SaveAsPngAsync($"{fileLocation}.png");

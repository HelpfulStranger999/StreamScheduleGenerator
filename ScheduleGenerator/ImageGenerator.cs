using System.Reflection;
using System.Text;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace ScheduleGenerator;

public class ImageGenerator
{
    private Font SmallFont { get; }
    private Font RegularFont { get; }
    private Font MediumFont { get; }
    private Font LargeFont { get; }

    private Color TextColor { get; } = new Color(Rgba32.ParseHex("#000000"));
    private SolidPen Pen { get; }

    private TimeZoneInfo TimeZone { get; }

    public ImageGenerator(TimeZoneInfo timezone)
    {
        TimeZone = timezone;

        var fontFamily = GetFontFamily();
        LargeFont = fontFamily.CreateFont(50, FontStyle.Regular);
        SmallFont = fontFamily.CreateFont(20, FontStyle.Regular);
        RegularFont = fontFamily.CreateFont(30, FontStyle.Regular);
        MediumFont = fontFamily.CreateFont(35, FontStyle.Regular);

        Pen = new SolidPen(TextColor, 10);
    }

    private string GetTimeZoneAbbreviation(DateTime dateTime)
    {
        var timezoneName = TimeZone.IsDaylightSavingTime(dateTime) ? TimeZone.DaylightName : TimeZone.StandardName;
        return string.Join("", timezoneName.Split(' ').Select(s => s[0]));
    }

    private FontFamily GetFontFamily()
    {
        var collection = new FontCollection();

        var assembly = Assembly.GetExecutingAssembly();
        var name = assembly.GetName().Name;

        using var stream = assembly.GetManifestResourceStream($"{name}.font.DancingScript-VariableFont_wght.ttf");
        return collection.Add(stream!);
    }

    public Image<Rgba32> GenerateScheduleImage(Schedule schedule)
    {
        if (schedule.IsEmpty) throw new InvalidOperationException("Cannot generate a schedule without streams");
        var image = new Image<Rgba32>(400, 140 + schedule.Count * 110, Rgba32.ParseHex("#F4EEDA"));

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
            context.DrawText($"Weekly Schedule\nfor {dateRange}", LargeFont, TextColor,
                new PointF(x: 20, y: 30));

            context.DrawLine(Pen, new PointF(160, 140), new PointF(160, image.Height));

            for (int i = 0; i < streams.Count; i++)
            {
                int xPos = 140 + i * 110;
                var schedule = streams[i];

                context.DrawLine(Pen, new PointF(0, xPos), new PointF(400, xPos));

                context.DrawText(schedule.Date.ToString("dddd"), SmallFont, TextColor, new PointF(20, xPos + 20));
                context.DrawText(schedule.Date.ToString("MMM d"), RegularFont, TextColor, new PointF(20, xPos + 50));

                if (schedule.Time == null)
                {
                    context.DrawText("No Stream", MediumFont, TextColor, new PointF(180, xPos + 40));
                }
                else
                {
                    string timezone = GetTimeZoneAbbreviation(schedule.Date.ToDateTime((TimeOnly)schedule.Time));
                    context.DrawText($"{schedule.Title}\nat {schedule.Time?.ToString("t")} {timezone}", RegularFont, TextColor, new PointF(180, xPos + 30));
                }
            }

        });

        return image;
    }
}
using System.Globalization;

namespace ChatAppWithDeafStudents.Client.Converters
{

    public class MessagePositionConverter : IValueConverter
    {
        public static Guid CurrentUserId { get; set; }

        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is Guid senderId)
            {

                if (senderId == CurrentUserId)
                    return LayoutOptions.End;

                else
                    return LayoutOptions.Start;
            }
            return LayoutOptions.Start;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return null;
        }
    }
}

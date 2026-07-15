using System.Globalization;

namespace ChatAppWithDeafStudents.Client.Converters
{
    public class SenderNameVisibilityConverter : IValueConverter
    {
        public static Guid CurrentUserId { get; set; }

        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is Guid senderId)
            {

                return senderId != CurrentUserId;
            }
            return false;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {

            return null;
        }
    }
}

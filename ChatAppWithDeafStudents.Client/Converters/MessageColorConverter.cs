using System.Globalization;

namespace ChatAppWithDeafStudents.Client.Converters
{

    public class MessageColorConverter : IValueConverter
    {
        
        public static Guid CurrentUserId { get; set; }

        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is Guid senderId)
            {

                if (senderId == CurrentUserId)
                    return Color.FromArgb("#007AFF"); 

                else
                    return Color.FromArgb("#2a3f4d"); 
            }
            return Color.FromArgb("#2a3f4d");
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {

            return null;
        }
    }
}

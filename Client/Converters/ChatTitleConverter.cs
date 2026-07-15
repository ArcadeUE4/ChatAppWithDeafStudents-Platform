using System.Globalization;

namespace ChatAppWithDeafStudents.Client.Converters
{

    public class ChatTitleConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2)
                return "Chats";

            string chatTitle = values[0]?.ToString() ?? "Chat";
            string? friendName = values.Length > 1 ? values[1]?.ToString() : null;

            
            if (!string.IsNullOrEmpty(friendName))
            {
                return friendName;
            }

            
            return !string.IsNullOrEmpty(chatTitle) ? chatTitle : "Chat";
        }

        public object[]? ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            
            return null;
        }
    }
}

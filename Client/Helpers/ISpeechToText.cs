using System.Globalization;

namespace ChatAppWithDeafStudents.Client.Helpers
{
    public interface ISpeechToText
    {
        Task<string> Listen(CultureInfo culture,
            IProgress<string> recogntionResult,
            CancellationToken cancellationToken);

        Task<bool> RequestPermissions();
    }
}

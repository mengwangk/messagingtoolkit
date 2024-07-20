using MessagingToolkit.Barcode.WindowsPhone8.App.Resources;

namespace MessagingToolkit.Barcode.WindowsPhone8.App
{
    /// <summary>
    /// Provides access to string resources.
    /// </summary>
    public class LocalizedStrings
    {
        private static AppResources _localizedResources = new AppResources();

        public AppResources LocalizedResources { get { return _localizedResources; } }
    }
}
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Handlers;

#if ANDROID
using Android.Widget;
#endif

#if IOS
using UIKit;
#endif


namespace CulturalVenues
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                //.UseMauiMaps()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if ANDROID
            EntryHandler.Mapper.AppendToMapping("NoUnderline", (handler, view) =>
            {
                if (handler.PlatformView is Android.Widget.EditText editText)
                {
                    editText.Background = null;
                }
            });
#endif


#if IOS
            EntryHandler.Mapper.AppendToMapping("NoBorder", (handler, view) =>
            {
                if (handler.PlatformView is UITextField textField)
                {
                    textField.BorderStyle = UITextBorderStyle.None;
                }
            });        
#endif

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}

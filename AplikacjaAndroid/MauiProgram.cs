
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Core;
using Microsoft.Extensions.Logging;
using Mopups.Hosting;

namespace AplikacjaAndroid
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {


            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureMopups()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("MaterialIcons-Regular.ttf", "MaterialIcons");
                });

            
            // Dodaj HttpClient do usług
            builder.Services.AddHttpClient("api", client =>
            {
                client.BaseAddress = new Uri("http://10.0.2.2:5000/api/");
            });

            // Rejestracja Views i Contexts
            builder.Services.AddTransient<LoginView>();
            builder.Services.AddTransient<LoginContext>();

            builder.Services.AddTransient<BooksView>();
            builder.Services.AddTransient<BooksContext>();

            builder.Services.AddTransient<UsersView>();
            builder.Services.AddTransient<UsersContext>();

            builder.Services.AddTransient<DetailsBookView>();
            builder.Services.AddTransient<DetailsBookContext>();

            builder.Services.AddTransient<AppShell>();
            builder.Services.AddTransient<AppShellContext>(); 

            builder.Services.AddTransient<ToReadBookView>();
            builder.Services.AddTransient<ToReadBookContext>();

            builder.Services.AddTransient<ReadedBookView>();
            builder.Services.AddTransient<ReadedBookContext>();

            builder.Services.AddTransient<BookMenuPopup>();
            builder.Services.AddTransient<BookMenuPopupContext>();
            // Serwisy
            builder.Services.AddSingleton<IUsersService, UsersService>();
            builder.Services.AddSingleton<IBooksService, BooksService>();

            //Storage
            builder.Services.AddSingleton<UserStorage>();
            builder.Services.AddSingleton<ReadedBookStorage>();
            builder.Services.AddSingleton<ToReadBookStorage>();


#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}

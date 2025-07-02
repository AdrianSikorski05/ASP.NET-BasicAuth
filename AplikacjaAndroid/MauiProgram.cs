
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Mopups.Hosting;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace AplikacjaAndroid
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {


            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseSkiaSharp()
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
                client.BaseAddress = new Uri("http://192.168.33.104:5000/api/");
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

            builder.Services.AddTransient<UserConfigView>();
            builder.Services.AddTransient<UserConfigContext>();

            builder.Services.AddTransientPopup<CommentPopupView>();
            builder.Services.AddTransient<CommentPopupContext>();

            builder.Services.AddTransientPopup<RefreshTokenPopupView>();
            builder.Services.AddTransient<RefreshTokenPopupContext>();

            builder.Services.AddTransientPopup<ConfirmeDeleteAccountView>();
            builder.Services.AddTransient<ConfirmeDeleteAccountContext>();

            builder.Services.AddTransientPopup<SuccessPopupView>();

            // Serwisy
            builder.Services.AddSingleton<IBooksService, BooksService>();
            builder.Services.AddSingleton<ICommentService, CommentService>();
            builder.Services.AddSingleton<IUsersService, UsersService>();
            builder.Services.AddSingleton<NavigationService>();

            //Storage
            builder.Services.AddSingleton<UserStorage>();
            builder.Services.AddSingleton<ReadedBookStorage>();
            builder.Services.AddSingleton<ToReadBookStorage>();


#if DEBUG
            builder.Logging.AddDebug();
#endif

            var app = builder.Build();

            return app;
        }
    }
}

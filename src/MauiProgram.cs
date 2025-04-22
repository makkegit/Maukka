using CommunityToolkit.Maui;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Toolkit.Hosting;

namespace Maukka
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureSyncfusionToolkit()
                .ConfigureMauiHandlers(handlers =>
                {
                })
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("SegueUI-Semibold.ttf", "SegueSemibold");
                    fonts.AddFont("FluentSystemIcons-Regular.ttf", FluentUI.FontFamily);
                    fonts.AddFont("MaterialSymbolsSharp-VariableFont_FILL.ttf", MaterialIcons.FontFamily);
                    fonts.AddFont("Clothes.ttf", ClothesUI.FontFamily);
                });

#if DEBUG
            builder.Logging.AddDebug();
            builder.Services.AddLogging(configure => configure.AddDebug());
#endif
            builder.Services.AddSingleton<SqliteConnection>(_ =>
                new SqliteConnection(Constants.DatabaseConnectionString));

            builder.Services.AddSingleton<WardrobeRepository>();
            // builder.Services.AddSingleton<TaskRepository>();
            // builder.Services.AddSingleton<CategoryRepository>();
            // builder.Services.AddSingleton<TagRepository>();
            builder.Services.AddSingleton<SeedDataService>();
            builder.Services.AddSingleton<ModalErrorHandler>();
            builder.Services.AddSingleton<MainPageModel>();
            builder.Services.AddSingleton<WardrobeListPageModel>();
            builder.Services.AddSingleton<ManageMetaPageModel>();

            builder.Services.AddTransientWithShellRoute<WardrobeDetailPage, WardrobeDetailPageModel>("wardrobe");
            builder.Services.AddTransientWithShellRoute<TaskDetailPage, TaskDetailPageModel>("task");

            return builder.Build();
        }
    }
}
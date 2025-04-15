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
                    fonts.AddFont("SegoeUI-Semibold.ttf", "SegoeSemibold");
                    fonts.AddFont("FluentSystemIcons-Regular.ttf", FluentUI.FontFamily);
                });

#if DEBUG
            builder.Logging.AddDebug();
            builder.Services.AddLogging(configure => configure.AddDebug());
#endif
            var databasePath = Path.Combine(AppContext.BaseDirectory, Constants.DatabasePath);
            builder.Services.AddSingleton<SqliteConnection>(_ =>
                new SqliteConnection($"Data Source = {databasePath}"));

            builder.Services.AddSingleton<WardrobeRepository>();
            // builder.Services.AddSingleton<TaskRepository>();
            // builder.Services.AddSingleton<CategoryRepository>();
            // builder.Services.AddSingleton<TagRepository>();
            builder.Services.AddSingleton<SeedDataService>();
            builder.Services.AddSingleton<ModalErrorHandler>();
            builder.Services.AddSingleton<MainPageModel>();
            builder.Services.AddSingleton<ProjectListPageModel>();
            builder.Services.AddSingleton<ManageMetaPageModel>();

            builder.Services.AddTransientWithShellRoute<WardrobeDetailPage, WardrobeDetailPageModel>("project");
            builder.Services.AddTransientWithShellRoute<TaskDetailPage, TaskDetailPageModel>("task");

            return builder.Build();
        }
    }
}
namespace SpreadsheetGUI;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
        var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");

                fonts.AddFont("Canterbury.ttf", "OpenCanterbury");
                fonts.AddFont("ScaryHalloweenFont.ttf", "OpenScaryHalloweenFont");
                fonts.AddFont("FutureWest.ttf", "OpenFutureWest");
                fonts.AddFont("Futuren0tFoundRegular.ttf", "OpenFuturen0tFoundRegular");
            });

		return builder.Build();
	}
}


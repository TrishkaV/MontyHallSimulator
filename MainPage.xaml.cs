using Microsoft.Maui.Platform;
using MontyHall.Core;
using MontyHall.Extensions;

namespace MontyHall;

public partial class MainPage : ContentPage
{
    private int gamesPlayed = 0;
    private int gamesWon = 0;
    private int gamesLost = 0;

    internal readonly List<string> winners = new()
    {
        "porsche.png", "treasure.webp", "holiday.jpg"
    };
    private readonly List<List<string>> combinations = new()
    {
        new List<string> { "porsche.png", "goat.jpg", "goat.jpg" },
        new List<string> { "treasure.webp", "lama.jpg", "lama.jpg" },
        new List<string> { "holiday.jpg", "sloth.jpg", "sloth.jpg" },
    };

	public MainPage()
	{
		InitializeComponent();
	}

    private async void MainPage_Loaded(object sender, EventArgs e)
    {
        await SplashScreen.FadeTo(0, 500);
        SplashScreen.IsVisible = false;

        StartPage.IsVisible = true;
        await StartPage.FadeTo(1, 500);        
    }
    
    private async void Welcome_Loaded(object sender, EventArgs e)
    {
        await WelcomeScreen.FadeTo(0, 500);
        WelcomeScreen.IsVisible = false;

        SplashScreen.IsVisible = true;
        await SplashScreen.FadeTo(1, 500);        
    }

    /* NOTE: contest logic could have been moved in a whole new class if it were
     * significantly bigger, here for simplicity
     */
    internal List<string> RunContest(int nDoors = 3)
    {
        combinations.ShuffleCrypto();
        var combinationsShuffled = ExtRandomness.ShuffleCryptoRO(in combinations);
        var combo = combinationsShuffled.First();
        
        if (combinationsShuffled.First().Count < nDoors)
        {
            var comboOriginal = new List<string>(combinationsShuffled.First());

            for (var i = combo.Count; i < nDoors; i++)
                combo.Add(string.Empty);

            combinationsShuffled[0] = comboOriginal;
        }
        combo.ShuffleCrypto();

        return combo;
    }

    private void OnNSimulationsChange(object sender, EventArgs e)
    {
        ValidateNSimulations();
    }

    private void OnNDoorsChange(object sender, EventArgs e)
    {
        ValidateNDoors();
    }

    private bool ValidateNDoors()
    {
        if (NDoorsLabel == null || NDoorsLabel.Text == null)
            return false;

        if (!int.TryParse(NDoors.Text.Trim(), out _) || int.Parse(NDoors.Text.Trim()) < 1)
        {
            NDoorsLabel.Text = "Please insert a whole number greater than 0.";
            NDoorsLabel.BackgroundColor = new Color(250, 0, 0);
            return false;
        }

        NDoorsLabel.Text = "How many doors you want to use?";
        NDoorsLabel.BackgroundColor = new Color(0, 0, 0);
        SimulationStartBtn.Text = "Run simulation";
        SimulationStartBtn.BackgroundColor = new Color(255, 255, 255);
        SimulationStartBtn.TextColor = new Color(0, 0, 250);

        return true;
    }

    private bool ValidateNSimulations()
    {
        if (NSimulations == null || NSimulations.Text == null)
            return false;
        if (!int.TryParse(NSimulations.Text.Trim(), out _) || int.Parse(NSimulations.Text.Trim()) < 1)
        {
            NSimulationsLabel.Text = "\tPlease insert a whole number greater than 0.\t";
            NSimulationsLabel.BackgroundColor = new Color(250, 0, 0);
            return false;
        }

        NSimulationsLabel.Text = "Number of simulated games? ( > 1000000 is advised )";
        NSimulationsLabel.BackgroundColor = new Color(0, 0, 0);
        SimulationStartBtn.Text = "Run simulation";
        SimulationStartBtn.BackgroundColor = new Color(255, 255, 255);
        SimulationStartBtn.TextColor = new Color(0, 0, 250);

        return true;
    }

    async void OnSimulationStartClicked(object sender, EventArgs e)
    {
        if (!ValidateNSimulations() || !ValidateNDoors())
        {
            SimulationStartBtn.Text = "Please resolve errors marked in red first.";
            SimulationStartBtn.BackgroundColor = new Color(250, 0, 0);
            SimulationStartBtn.TextColor = new Color(255, 255, 255);
            return;
        }
        
        var nSimulations = int.Parse(NSimulations.Text.Trim());
        var isDoorChange = IsDoorChange.IsChecked;
        var nDoors = int.Parse(NDoors.Text.Trim());
        /* NOTE: maximise efficiency by running the simulations during the animations in a separate thread
         */
        var simulationRun = Task.Factory.StartNew(() => new CoreSimulations().RunSimulations(nSimulations, isDoorChange, nDoors));

        /* NOTE: test optimal number of parallel simulations
         */
        //var optimalNParallelSimulations = new CoreSimulations().OptimizeNParallelSim(nSimulations, isDoorChange);

        await StartPage.FadeTo(0, 1_000);
        StartPage.IsVisible = false;

        LoadingPage.IsVisible = true;
        await LoadingPage.FadeTo(1, 1_000);
        /* NOTE: framework workaround to keep the main thread running
         * (for animations and responsivity) while simulations are run in 
         * another thread
         */
        while (!simulationRun.IsCompleted)
            await LoadingPage.FadeTo(1, 1_000);
        
        var (simulationsWon, simulationsLost) = simulationRun.Result;

        var ratioWin = (0d + simulationsWon) / nSimulations;
        var ratioLost = (0d + simulationsLost) / nSimulations;

        WinRate.Text = ratioWin * 100 >= 1 ? $"{ratioWin * 100:##.#####}%" : $"0{(ratioWin * 100):##.#####}%";
        LoseRate.Text = ratioLost * 100 >= 1 ? $"{ratioLost * 100:##.#####}%" : $"0{(ratioLost * 100):##.#####}%";
        SimulationsRan.Text = $"{nSimulations}";
        WasDoorOpened.Text = isDoorChange ? "YES" : "NO";

        await LoadingPage.FadeTo(0, 1_000);
        LoadingPage.IsVisible = false;

        SimulationPage.IsVisible = true;
        await SimulationPage.FadeTo(1, 1_000);
        Thread.Sleep(3_000);

        SimPageToStarPage();
    }


    private async void SimPageToStarPage()
    {
        await SimulationPage.FadeTo(0, 1000);
        SimulationPage.Opacity = 0;
        SimulationPage.IsVisible = false;

        StartPage.IsVisible = true;
        await StartPage.FadeTo(1, 1_000);
    }

    async void OnDoor1Clicked(object sender, EventArgs e)
    {
        /* FUTURE: implementation for when support for better property getters
         * is introduced in MAUI's future releases
         */
        //var doorChosenName = ((ImageButton)sender).StyleId;
        //var doorChosenItem = NameScopeExtensions.FindByName<ImageButton>(this, doorChosenName);
        //var otherDoors = ((HorizontalStackLayout)MHGame.Children);
                         //.[PropertyChanged-name].Where(x => x.StyleId != doorChosenName).ToList();
        
        var results = RunContest();

        Door1.RotationY = 50;

        await Door1.ScaleTo(0.7, 100);

        await Task.WhenAll(
            Door1.FadeTo(0, 500),
            Door2.FadeTo(0, 500),
            Door3.FadeTo(0, 500),
            Door2.ScaleTo(0.7, 100),
            Door3.ScaleTo(0.7, 100)
        );

        Door1.Source = results[0];
        Door2.Source = results[1];
        Door3.Source = results[2];

        Door1.RotationY = -1;

        await Task.WhenAll(
            Door1.FadeTo(1, 300),
            Door2.FadeTo(0.7, 300),
            Door3.FadeTo(0.7, 300)
        );

        await Door1.ScaleTo(1.1, 300);

        UpdateScore(results[0]);

        Thread.Sleep(1_500);
        RestoreDoors(new List<ImageButton> { Door1, Door2, Door3 });
    }

    private void UpdateScore(string chosenDoor)
    {
        if (winners.Contains(chosenDoor))
            gamesWon++;
        else
            gamesLost++;

        LabelGamesPlayed.Text = $"Games played = {++gamesPlayed}";
        LabelGamesWon.Text = $"Games won = {gamesWon}";
        LabelGamesLost.Text = $"Games lost = {gamesLost}";
    }

    private async void RestoreDoors(List<ImageButton> doors)
    {
        var doorsTasks = new List<Task>();
        doorsTasks.AddRange(doors.ConvertAll(x => x.FadeTo(0, 500)));
        
        await Task.WhenAll(doorsTasks);

        Door1.Source = "door1.png";
        Door2.Source = "door2.png";
        Door3.Source = "door3.png";

        doorsTasks.Clear();
        doorsTasks.AddRange(doors.ConvertAll(x => x.FadeTo(1, 100)));
        await Task.WhenAll(doorsTasks);
        
        doorsTasks.Clear();
        doorsTasks.AddRange(doors.ConvertAll(x => x.ScaleTo(1, 300)));
        await Task.WhenAll(doorsTasks);
    }

	async void OnDoor2Clicked(object sender, EventArgs e)
	{
        var results = RunContest();

        Door2.RotationY = 50;

        await Door2.ScaleTo(0.7, 100);

        await Task.WhenAll(
            Door2.FadeTo(0, 500),
            Door1.FadeTo(0, 500),
            Door3.FadeTo(0, 500),
            Door1.ScaleTo(0.7, 100),
            Door3.ScaleTo(0.7, 100)
        );

        Door2.Source = results[0];
        Door1.Source = results[1];
        Door3.Source = results[2];

        Door2.RotationY = -1;

        await Task.WhenAll(
            Door2.FadeTo(1, 300),
            Door1.FadeTo(0.7, 300),
            Door3.FadeTo(0.7, 300)
        );

        await Door2.ScaleTo(1.1, 300);

        UpdateScore(results[0]);

        Thread.Sleep(1_500);
        RestoreDoors(new List<ImageButton> { Door1, Door2, Door3 });
    }

    async void OnDoor3Clicked(object sender, EventArgs e)
    {
        var results = RunContest();

        Door3.RotationY = 50;

        await Door3.ScaleTo(0.7, 100);

        await Task.WhenAll(
            Door3.FadeTo(0, 500),
            Door1.FadeTo(0, 500),
            Door2.FadeTo(0, 500),
            Door1.ScaleTo(0.7, 100),
            Door2.ScaleTo(0.7, 100)
        );

        Door3.Source = results[0];
        Door1.Source = results[1];
        Door2.Source = results[2];

        Door3.RotationY = -1;

        await Task.WhenAll(
            Door3.FadeTo(1, 300),
            Door1.FadeTo(0.7, 300),
            Door2.FadeTo(0.7, 300)
        );

        await Door3.ScaleTo(1.1, 300);

        UpdateScore(results[0]);

        Thread.Sleep(1_500);
        RestoreDoors(new List<ImageButton> { Door1, Door2, Door3 });
    }
}


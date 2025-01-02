namespace Archive.Views
{
    public partial class MoviesPage : ContentPage
    {
        public MoviesPage()
        {
            InitializeComponent();
        }
        private async void GoToTVSeriesButton_Clicked(object sender, EventArgs e)
        {
            // Przejście na stronę TVSeriesPage
            await Navigation.PushAsync(new TVSeriesPage());
        }

    }
}

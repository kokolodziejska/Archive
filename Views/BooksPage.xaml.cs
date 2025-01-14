using Archive.ViewModels;

namespace Archive.Views
{
    public partial class BooksPage : ContentPage
    {
        public BooksPage(BooksViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        private async void GoToTVSeriesButton_Clicked(object sender, EventArgs e)
        {
            var viewModel = App.serviceProvider.GetService<TVSeriesViewModel>();
            await Navigation.PushAsync(new NavigationPage(new TVSeriesPage(viewModel)));
        }

        private async void GoToMoviesButton_Clicked(object sender, EventArgs e)
        {
            var viewModel = App.serviceProvider.GetService<MoviesViewModel>();
            await Navigation.PushAsync(new NavigationPage(new MoviesPage(viewModel)));
        }
    }
}

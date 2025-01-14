using Archive.ViewModels;

namespace Archive.Views
{
    public partial class MoviesPage : ContentPage
    {
        public MoviesPage(MoviesViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        private async void GoToBooksButton_Clicked(object sender, EventArgs e)
        {
            var viewModel = App.serviceProvider.GetService<BooksViewModel>();
            await Navigation.PushAsync(new NavigationPage(new BooksPage(viewModel)));
        }

        private async void GoToTVSeriesButton_Clicked(object sender, EventArgs e)
        {
            var viewModel = App.serviceProvider.GetService<TVSeriesViewModel>();
            await Navigation.PushAsync(new NavigationPage(new TVSeriesPage(viewModel)));
        }
    }
}

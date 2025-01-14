using Archive.ViewModels;

namespace Archive.Views
{
    public partial class TVSeriesPage : ContentPage
    {
        public TVSeriesPage(TVSeriesViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        private async void GoToMoviesButton_Clicked(object sender, EventArgs e)
        {
            var viewModel = App.serviceProvider.GetService<MoviesViewModel>();
            await Navigation.PushAsync(new NavigationPage(new MoviesPage(viewModel)));
        }

        private async void GoToBooksButton_Clicked(object sender, EventArgs e)
        {
            var viewModel = App.serviceProvider.GetService<BooksViewModel>();
            await Navigation.PushAsync(new NavigationPage(new BooksPage(viewModel)));
        }
    }
}

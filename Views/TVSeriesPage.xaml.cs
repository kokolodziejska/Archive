using System;
using System.Collections.Generic;
using Archive.ViewModels;
using Microsoft.Maui.Controls;

namespace Archive.Views
{
    public partial class TVSeriesPage : ContentPage
    {
        public TVSeriesPage()
        {
            InitializeComponent();
            BindingContext = new TVSeriesViewModel();
        }
        private async void GoToBooksButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new BooksPage());
        }
        private async void GoToMoviesButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MoviesPage());
        }
    }
}

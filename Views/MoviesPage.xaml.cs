using System;
using System.Collections.Generic;
using Archive.ViewModels;
using Microsoft.Maui.Controls;

namespace Archive.Views
{
    public partial class MoviesPage : ContentPage
    {
        public MoviesPage()
        {
            InitializeComponent();
            BindingContext = new MoviesViewModel();
        }

        private async void GoToTVSeriesButton_Clicked(object sender, EventArgs e)
        {
            // Przejście na stronę TVSeriesPage
            await Navigation.PushAsync(new TVSeriesPage());
        }
    }
}

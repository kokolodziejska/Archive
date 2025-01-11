using System;
using Archive.ViewModels;
using Microsoft.Maui.Controls;

namespace Archive.Views
{
    public partial class BooksPage : ContentPage
    {
        public BooksPage()
        {
            InitializeComponent();
            BindingContext = new BooksViewModel(); // Bind the BooksViewModel to the page
        }
        private async void GoToTVSeriesButton_Clicked(object sender, EventArgs e)
        {
            // Przejœcie na stronê TVSeriesPage
            await Navigation.PushAsync(new TVSeriesPage());
        }
    }
}

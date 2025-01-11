using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Archive.Models;
using System.Windows.Input;


namespace Archive.ViewModels
{
    public partial class MoviesViewModel : ObservableObject
    {
        private const string ApiBaseUrl = "https://10.0.2.2:7219/api/movies";
        public ObservableCollection<Movie> Movies { get; set; } = new();

        private readonly HttpClient _httpClient;

        [ObservableProperty]
        private string newMovieTitle;

        [ObservableProperty]
        private string newMovieCategory;

        [ObservableProperty]
        private string newMovieRating;

        public int ParsedRating => int.TryParse(NewMovieRating, out var rating) ? rating : 0;

        [ObservableProperty]
        private DateTime newMovieDate = DateTime.Now;


        // Komenda do ładowania filmów
        public IAsyncRelayCommand LoadMoviesCommand { get; }
        // Komenda do usuwania filmu
        public ICommand DeleteCommand { get; }
        // Komenda do doania filmu
        public IAsyncRelayCommand AddMovieCommand { get; }

        public MoviesViewModel()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };

            _httpClient = new HttpClient(handler);
            LoadMoviesCommand = new AsyncRelayCommand(LoadMoviesAsync);
            DeleteCommand = new Command<Movie>(DeleteMovie);
            AddMovieCommand = new AsyncRelayCommand(AddMovieAsync);
            LoadMoviesAsync();
        }

        private async Task LoadMoviesAsync()
        {
            try
            {
                Console.WriteLine("Rozpoczynam pobieranie filmów...");
                var response = await _httpClient.GetAsync(ApiBaseUrl);
                if (response.IsSuccessStatusCode)
                {
                    var movies = await response.Content.ReadFromJsonAsync<List<Movie>>();
                    if (movies != null && movies.Any())
                    {
                        Console.WriteLine($"Pobrano {movies.Count} filmów.");
                        Movies.Clear();
                        foreach (var movie in movies)
                        {
                            Movies.Add(movie);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Brak filmów do wyświetlenia.");
                    }
                }
                else
                {
                    Console.WriteLine($"Błąd połączenia z API: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd podczas pobierania filmów: {ex.Message}");
            }
        }

        public void DeleteMovie(Movie movie)
        {
            if (Movies.Contains(movie))
            {
                Movies.Remove(movie);
                // Wywołaj API, aby usunąć film z bazy danych (jeśli korzystasz z API)
                _httpClient.DeleteAsync($"{ApiBaseUrl}/{movie.Id}");
            }
        }

        private async Task AddMovieAsync()
        {
            try
            {
                var newMovie = new Movie
                {
                    Title = NewMovieTitle,
                    Category = NewMovieCategory,
                    Rating = ParsedRating,
                    Date = NewMovieDate,
                };

                var json = JsonContent.Create(newMovie);
                var response = await _httpClient.PostAsync(ApiBaseUrl, json);

                if (response.IsSuccessStatusCode)
                {
                    // Clear the input fields
                    NewMovieTitle = string.Empty;
                    NewMovieCategory = string.Empty;
                    NewMovieRating = string.Empty;
                    NewMovieDate = DateTime.Now;

                    // Reload the movie list from the backend
                    await LoadMoviesAsync();
                }
                else
                {
                    Console.WriteLine($"Failed to add movie: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while adding a movie: {ex.Message}");
            }
        }
    }
}

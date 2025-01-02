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

namespace Archive.ViewModels
{
    public partial class MoviesViewModel : ObservableObject
    {
        private const string ApiBaseUrl = "https://10.0.2.2:7219/api/movies";
        public ObservableCollection<Movie> Movies { get; set; } = new();

        private readonly HttpClient _httpClient;

        // Komenda do ładowania filmów
        public IAsyncRelayCommand LoadMoviesCommand { get; }
        // Komenda do usuwania filmu
        public IRelayCommand<Movie> DeleteMovieCommand { get; }

        public MoviesViewModel()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };

            _httpClient = new HttpClient(handler);
            LoadMoviesCommand = new AsyncRelayCommand(LoadMoviesAsync);
            DeleteMovieCommand = new RelayCommand<Movie>(DeleteMovieAsync);
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

        private async void DeleteMovieAsync(Movie movie)
        {
            if (movie == null) return;

            try
            {
                var response = await _httpClient.DeleteAsync($"{ApiBaseUrl}/{movie.Id}");
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Usunięto film: {movie.Title}");
                    Movies.Remove(movie);
                }
                else
                {
                    Console.WriteLine($"Błąd podczas usuwania filmu: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd podczas usuwania filmu: {ex.Message}");
            }
        }
    }
}

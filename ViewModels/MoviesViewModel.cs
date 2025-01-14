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
using System.Diagnostics;

namespace Archive.ViewModels
{
    public partial class MoviesViewModel : ObservableObject
    {
        private const string ApiBaseUrl = "http://localhost:5109/api/movies";
        public ObservableCollection<Movie> Movies { get; set; } = new();

        private readonly HttpClient _httpClient;

        public IAsyncRelayCommand LoadMoviesCommand { get; }
        public IRelayCommand<Movie> DeleteMovieCommand { get; }
        public IRelayCommand<Movie> EditMovieCommand { get; }
        public IRelayCommand SaveMovieCommand { get; }

        [ObservableProperty]
        private Movie selectedMovie;

        [ObservableProperty]
        private bool isEditing;

        [ObservableProperty]
        private string saveButtonText;

        public MoviesViewModel()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };

            _httpClient = new HttpClient(handler);
            LoadMoviesCommand = new AsyncRelayCommand(LoadMoviesAsync);
            DeleteMovieCommand = new RelayCommand<Movie>(DeleteMovieAsync);
            EditMovieCommand = new RelayCommand<Movie>(EditMovie);
            SaveMovieCommand = new RelayCommand(SaveMovie);
            LoadMoviesAsync();
            IsEditing = false;
            selectedMovie = new Movie();
            saveButtonText = "Add Movie";
        }

        private async Task LoadMoviesAsync()
        {
            try
            {
                Debug.WriteLine("Rozpoczynam pobieranie filmów...");
                var response = await _httpClient.GetAsync(ApiBaseUrl);
                if (response.IsSuccessStatusCode)
                {
                    var movies = await response.Content.ReadFromJsonAsync<List<Movie>>();
                    if (movies != null && movies.Any())
                    {
                        Debug.WriteLine($"Pobrano {movies.Count} filmów.");
                        Movies.Clear();
                        foreach (var movie in movies)
                        {
                            Movies.Add(movie);
                        }
                    }
                    else
                    {
                        Debug.WriteLine("Brak filmów do wyświetlenia.");
                    }
                }
                else
                {
                    Debug.WriteLine($"Błąd połączenia z API: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Błąd podczas pobierania filmów: {ex.Message}");
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
                    Debug.WriteLine($"Usunięto film: {movie.Title}");
                    Movies.Remove(movie);
                }
                else
                {
                    Debug.WriteLine($"Błąd podczas usuwania filmu: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Błąd podczas usuwania filmu: {ex.Message}");
            }
        }

        private void EditMovie(Movie movie)
        {
            if (movie == null) return;

            SelectedMovie = movie;
            IsEditing = true;
            SaveButtonText = "Save Changes";
        }

        private async void SaveMovie()
        {
            if (SelectedMovie == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(SelectedMovie.Title) || string.IsNullOrEmpty(SelectedMovie.Category) || SelectedMovie.Rating < 1 || SelectedMovie.Rating > 10)
            {
                Debug.WriteLine("Niepoprawne dane filmu.");
                return;
            }

            try
            {
                if (IsEditing)
                {
                    SaveEdit();
                }
                else
                {
                    AddMovie();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Błąd podczas zapisywania filmu: {ex.Message}");
            }
        }

        private async void AddMovie()
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"{ApiBaseUrl}", SelectedMovie);
            if (response.IsSuccessStatusCode)
            {
                var movie = await response.Content.ReadFromJsonAsync<Movie>();
                Movies.Add(movie);
                SelectedMovie = new Movie();
                IsEditing = false;
                SaveButtonText = "Add Movie";
                Debug.WriteLine($"Dodano film: {movie.Title}");
            }
        }

        private async void SaveEdit()
        {
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"{ApiBaseUrl}/{SelectedMovie.Id}", SelectedMovie);
            if (response.IsSuccessStatusCode)
            {
                var index = Movies.IndexOf(Movies.First(m => m.Id == SelectedMovie.Id));
                Movies[index] = SelectedMovie;
                SelectedMovie = new Movie();
                IsEditing = false;
                SaveButtonText = "Add Movie";
                Debug.WriteLine($"Zapisano film: {SelectedMovie.Title}");
            }
            else
            {
                Debug.WriteLine($"Błąd podczas zapisywania filmu: {response.StatusCode}");
            }
        }
    }
}

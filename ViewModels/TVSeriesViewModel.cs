using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Archive.Models;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Archive.ViewModels
{
    public partial class TVSeriesViewModel : ObservableObject
    {
        private const string ApiBaseUrl = "https://10.0.2.2:7219/api/tvseries";
        public ObservableCollection<TVSeries> Series { get; set; } = new();

        private readonly HttpClient _httpClient;

        [ObservableProperty]
        private string newSeriesTitle;

        [ObservableProperty]
        private string newSeriesCategory;

        [ObservableProperty]
        private string newSeriesRating;

        public int ParsedRating => int.TryParse(NewSeriesRating, out var rating) ? rating : 0;

        [ObservableProperty]
        private DateTime newSeriesDate = DateTime.Now;

        public IAsyncRelayCommand LoadSeriesCommand { get; }
        public ICommand DeleteCommand { get; }
        public IAsyncRelayCommand AddSeriesCommand { get; }

        public TVSeriesViewModel()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };

            _httpClient = new HttpClient(handler);
            LoadSeriesCommand = new AsyncRelayCommand(LoadSeriesAsync);
            DeleteCommand = new Command<TVSeries>(DeleteSeries);
            AddSeriesCommand = new AsyncRelayCommand(AddSeriesAsync);
            LoadSeriesAsync();
        }

        private async Task LoadSeriesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(ApiBaseUrl);
                if (response.IsSuccessStatusCode)
                {
                    var seriesList = await response.Content.ReadFromJsonAsync<List<TVSeries>>();
                    if (seriesList != null)
                    {
                        Series.Clear();
                        foreach (var series in seriesList)
                        {
                            Series.Add(series);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading TV series: {ex.Message}");
            }
        }

        private void DeleteSeries(TVSeries series)
        {
            if (Series.Contains(series))
            {
                Series.Remove(series);
                _httpClient.DeleteAsync($"{ApiBaseUrl}/{series.Id}");
            }
        }

        private async Task AddSeriesAsync()
        {
            try
            {
                var newSeries = new TVSeries
                {
                    Title = NewSeriesTitle,
                    Category = NewSeriesCategory,
                    Rating = ParsedRating,
                    Date = NewSeriesDate,
                };

                var json = JsonContent.Create(newSeries);
                var response = await _httpClient.PostAsync(ApiBaseUrl, json);

                if (response.IsSuccessStatusCode)
                {
                    NewSeriesTitle = string.Empty;
                    NewSeriesCategory = string.Empty;
                    NewSeriesRating = string.Empty;
                    NewSeriesDate = DateTime.Now;

                    await LoadSeriesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding TV series: {ex.Message}");
            }
        }
    }
}

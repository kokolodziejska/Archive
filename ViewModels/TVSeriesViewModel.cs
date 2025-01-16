using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Archive.Models;

namespace Archive.ViewModels
{
    public partial class TVSeriesViewModel : ObservableObject
    {
        private const string ApiBaseUrl = "http://localhost:5109/api/tvseries";
        public ObservableCollection<TVSeries> TVSeries { get; set; } = new();

        private readonly HttpClient _httpClient;

        public IAsyncRelayCommand LoadTVSeriesCommand { get; }
        public IRelayCommand<TVSeries> DeleteTVSeriesCommand { get; }
        public IRelayCommand<TVSeries> EditTVSeriesCommand { get; }
        public IRelayCommand SaveTVSeriesCommand { get; }

        [ObservableProperty]
        private TVSeries selectedTVSeries;

        [ObservableProperty]
        private bool isEditing;

        [ObservableProperty]
        private string saveButtonText;

        [ObservableProperty]
        private string centerText;

        [ObservableProperty]
        private string color;

        public TVSeriesViewModel()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };

            _httpClient = new HttpClient(handler);
            LoadTVSeriesCommand = new AsyncRelayCommand(LoadTVSeriesAsync);
            DeleteTVSeriesCommand = new RelayCommand<TVSeries>(DeleteTVSeriesAsync);
            EditTVSeriesCommand = new RelayCommand<TVSeries>(EditTVSeries);
            SaveTVSeriesCommand = new RelayCommand(SaveTVSeries);
            LoadTVSeriesAsync();
            IsEditing = false;
            selectedTVSeries = new TVSeries();
            saveButtonText = "Add TV Series";
            CenterText = "Add New TV Series";
            Color = "#67f5f3";
        }

        private async Task LoadTVSeriesAsync()
        {
            try
            {
                Debug.WriteLine("Rozpoczynam pobieranie seriali...");
                var response = await _httpClient.GetAsync(ApiBaseUrl);
                if (response.IsSuccessStatusCode)
                {
                    var seriesList = await response.Content.ReadFromJsonAsync<List<TVSeries>>();
                    if (seriesList != null && seriesList.Any())
                    {
                        Debug.WriteLine($"Pobrano {seriesList.Count} seriali.");
                        TVSeries.Clear();
                        foreach (var series in seriesList)
                        {
                            TVSeries.Add(series);
                        }
                    }
                    else
                    {
                        Debug.WriteLine("Brak seriali do wyświetlenia.");
                    }
                }
                else
                {
                    Debug.WriteLine($"Błąd połączenia z API: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Błąd podczas pobierania seriali: {ex.Message}");
            }
        }

        private async void DeleteTVSeriesAsync(TVSeries series)
        {
            if (series == null) return;

            try
            {
                var response = await _httpClient.DeleteAsync($"{ApiBaseUrl}/{series.Id}");
                if (response.IsSuccessStatusCode)
                {
                    Debug.WriteLine($"Usunięto serial: {series.Title}");
                    TVSeries.Remove(series);
                }
                else
                {
                    Debug.WriteLine($"Błąd podczas usuwania serialu: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Błąd podczas usuwania serialu: {ex.Message}");
            }
        }

        private void EditTVSeries(TVSeries series)
        {
            if (series == null) return;

            SelectedTVSeries = series;
            IsEditing = true;
            SaveButtonText = "Save Changes";
            CenterText = "Edit TV Series";
            Color = "#ffae36";
        }

        private async void SaveTVSeries()
        {
            if (SelectedTVSeries == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(SelectedTVSeries.Title) || string.IsNullOrEmpty(SelectedTVSeries.Category) || SelectedTVSeries.Rating < 1 || SelectedTVSeries.Rating > 10)
            {
                Debug.WriteLine("Niepoprawne dane serialu.");
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
                    AddTVSeries();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Błąd podczas zapisywania serialu: {ex.Message}");
            }
        }

        private async void AddTVSeries()
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"{ApiBaseUrl}", SelectedTVSeries);
            if (response.IsSuccessStatusCode)
            {
                var series = await response.Content.ReadFromJsonAsync<TVSeries>();
                TVSeries.Add(series);
                SelectedTVSeries = new TVSeries();
                IsEditing = false;
                SaveButtonText = "Add TV Series";
                CenterText = "Add New TV Series";
                Color = "#67f5f3";
                Debug.WriteLine($"Dodano serial: {series.Title}");
            }
        }

        private async void SaveEdit()
        {
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"{ApiBaseUrl}/{SelectedTVSeries.Id}", SelectedTVSeries);
            if (response.IsSuccessStatusCode)
            {
                var index = TVSeries.IndexOf(TVSeries.First(m => m.Id == SelectedTVSeries.Id));
                TVSeries[index] = SelectedTVSeries;
                SelectedTVSeries = new TVSeries();
                IsEditing = false;
                SaveButtonText = "Add TV Series";
                CenterText = "Add New TV Series";
                Color = "#67f5f3";
                Debug.WriteLine($"Zapisano serial: {SelectedTVSeries.Title}");
            }
            else
            {
                Debug.WriteLine($"Błąd podczas zapisywania serialu: {response.StatusCode}");
            }
        }
    }
}

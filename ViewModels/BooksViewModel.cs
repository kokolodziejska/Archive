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
    public partial class BooksViewModel : ObservableObject
    {
        private const string ApiBaseUrl = "http://localhost:5109/api/books";
        public ObservableCollection<Book> Books { get; set; } = [];

        private readonly HttpClient _httpClient;

        public IAsyncRelayCommand LoadBooksCommand { get; }
        public IRelayCommand<Book> DeleteBookCommand { get; }
        public IRelayCommand<Book> EditBookCommand { get; }
        public IRelayCommand SaveBookCommand { get; }

        [ObservableProperty]
        private Book selectedBook;

        [ObservableProperty]
        private bool isEditing;

        [ObservableProperty]
        private string saveButtonText;

        [ObservableProperty]
        private string centerText;

        [ObservableProperty]
        private string color;

        public BooksViewModel()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };

            _httpClient = new HttpClient(handler);
            LoadBooksCommand = new AsyncRelayCommand(LoadBooksAsync);
            DeleteBookCommand = new RelayCommand<Book>(DeleteBookAsync);
            EditBookCommand = new RelayCommand<Book>(EditBook);
            SaveBookCommand = new RelayCommand(SaveBook);
            LoadBooksAsync();
            IsEditing = false;
            SelectedBook = new Book();
            SaveButtonText = "Add Book";
            CenterText = "Add New Book";
            Color = "#67f5f3";
        }

        private async Task LoadBooksAsync()
        {
            try
            {
                Debug.WriteLine("Rozpoczynam pobieranie książek...");
                var response = await _httpClient.GetAsync(ApiBaseUrl);
                if (response.IsSuccessStatusCode)
                {
                    var booksList = await response.Content.ReadFromJsonAsync<List<Book>>();
                    if (booksList != null && booksList.Any())
                    {
                        Debug.WriteLine($"Pobrano {booksList.Count} książek.");
                        Books.Clear();
                        foreach (var book in booksList)
                        {
                            Books.Add(book);
                        }
                    }
                    else
                    {
                        Debug.WriteLine("Brak książek do wyświetlenia.");
                    }
                }
                else
                {
                    Debug.WriteLine($"Błąd połączenia z API: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Błąd podczas pobierania książek: {ex.Message}");
            }
        }

        private async void DeleteBookAsync(Book book)
        {
            if (book == null) return;

            try
            {
                var response = await _httpClient.DeleteAsync($"{ApiBaseUrl}/{book.Id}");
                if (response.IsSuccessStatusCode)
                {
                    Debug.WriteLine($"Usunięto książkę: {book.Title}");
                    Books.Remove(book);
                }
                else
                {
                    Debug.WriteLine($"Błąd podczas usuwania książki: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Błąd podczas usuwania książki: {ex.Message}");
            }
        }

        private void EditBook(Book book)
        {
            if (book == null) return;

            SelectedBook = book;
            IsEditing = true;
            SaveButtonText = "Save Changes";
            CenterText = "Edit Book";
            Color = "#ffae36";
        }

        private async void SaveBook()
        {
            if (SelectedBook == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(SelectedBook.Title) || string.IsNullOrEmpty(SelectedBook.Category) || SelectedBook.Rating < 1 || SelectedBook.Rating > 10)
            {
                Debug.WriteLine("Niepoprawne dane książki.");
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
                    AddBook();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Błąd podczas zapisywania książki: {ex.Message}");
            }
        }

        private async void AddBook()
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"{ApiBaseUrl}", SelectedBook);
            if (response.IsSuccessStatusCode)
            {
                var book = await response.Content.ReadFromJsonAsync<Book>();
                Books.Add(book);
                SelectedBook = new Book();
                IsEditing = false;
                SaveButtonText = "Add Book";
                CenterText = "Add New Book";
                Color = "#67f5f3";
                Debug.WriteLine($"Dodano książkę: {book.Title}");
            }
        }

        private async void SaveEdit()
        {
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"{ApiBaseUrl}/{SelectedBook.Id}", SelectedBook);
            if (response.IsSuccessStatusCode)
            {
                var index = Books.IndexOf(Books.First(m => m.Id == SelectedBook.Id));
                Books[index] = SelectedBook;
                SelectedBook = new Book();
                IsEditing = false;
                SaveButtonText = "Add Book";
                CenterText = "Add New Book";
                Color = "#67f5f3";
                Debug.WriteLine($"Zapisano książkę: {SelectedBook.Title}");
            }
            else
            {
                Debug.WriteLine($"Błąd podczas zapisywania książki: {response.StatusCode}");
            }
        }
    }
}

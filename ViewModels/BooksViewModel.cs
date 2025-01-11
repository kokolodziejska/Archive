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
    public partial class BooksViewModel : ObservableObject
    {
        private const string ApiBaseUrl = "https://10.0.2.2:7219/api/books";
        public ObservableCollection<Book> Books { get; set; } = new();

        private readonly HttpClient _httpClient;

        [ObservableProperty]
        private string newBookTitle;

        [ObservableProperty]
        private string newBookCategory;

        [ObservableProperty]
        private string newBookRating;

        public int ParsedRating => int.TryParse(NewBookRating, out var rating) ? rating : 0;

        [ObservableProperty]
        private DateTime newBookDate = DateTime.Now;

        public IAsyncRelayCommand LoadBooksCommand { get; }
        public ICommand DeleteCommand { get; }
        public IAsyncRelayCommand AddBookCommand { get; }

        public BooksViewModel()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };

            _httpClient = new HttpClient(handler);
            LoadBooksCommand = new AsyncRelayCommand(LoadBooksAsync);
            DeleteCommand = new Command<Book>(DeleteBook);
            AddBookCommand = new AsyncRelayCommand(AddBookAsync);
            LoadBooksAsync();
        }

        private async Task LoadBooksAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(ApiBaseUrl);
                if (response.IsSuccessStatusCode)
                {
                    var bookList = await response.Content.ReadFromJsonAsync<List<Book>>();
                    if (bookList != null)
                    {
                        Books.Clear();
                        foreach (var book in bookList)
                        {
                            Books.Add(book);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading books: {ex.Message}");
            }
        }

        private void DeleteBook(Book book)
        {
            if (Books.Contains(book))
            {
                Books.Remove(book);
                _httpClient.DeleteAsync($"{ApiBaseUrl}/{book.Id}");
            }
        }

        private async Task AddBookAsync()
        {
            try
            {
                var newBook = new Book
                {
                    Title = NewBookTitle,
                    Category = NewBookCategory,
                    Rating = ParsedRating,
                    Date = NewBookDate,
                };

                var json = JsonContent.Create(newBook);
                var response = await _httpClient.PostAsync(ApiBaseUrl, json);

                if (response.IsSuccessStatusCode)
                {
                    NewBookTitle = string.Empty;
                    NewBookCategory = string.Empty;
                    NewBookRating = string.Empty;
                    NewBookDate = DateTime.Now;

                    await LoadBooksAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding book: {ex.Message}");
            }
        }
    }
}

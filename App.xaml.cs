namespace Archive
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new Archive.Views.MoviesPage();

        }
    }
}

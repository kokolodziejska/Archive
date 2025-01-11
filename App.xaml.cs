namespace Archive
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new Archive.Views.MoviesPage());

        }
    }
}

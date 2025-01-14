namespace Archive
{
    public partial class App : Application
    {
        public static IServiceProvider serviceProvider;

        public App(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            App.serviceProvider = serviceProvider;
            MainPage = new NavigationPage(serviceProvider.GetService<Views.MoviesPage>());
        }
    }
}

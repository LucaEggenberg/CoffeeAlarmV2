namespace CoffeeAlarmV2
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainPageVM viewModel)
        {
            BindingContext = viewModel;
            InitializeComponent();
        }
    }

}

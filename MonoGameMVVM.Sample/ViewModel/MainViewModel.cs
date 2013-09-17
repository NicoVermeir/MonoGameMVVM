namespace MonoGameMVVM.Sample
{
    public class MainViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private int _counter;

        private string _test;

        public string Test
        {
            get { return _test; }
            set
            {
                if (_test == value) return;

                _test = value;

                RaisePropertyChanged();
            }
        }

        public int Counter
        {
            get
            {
                return _counter;
            }
            set
            {
                if (_counter == value) return;

                _counter = value;
                RaisePropertyChanged(() => Counter);
            }
        }

        public MainViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public void NextPageCommand()
        {
            _navigationService.Navigate(typeof (SecondView));
        }

        public void CounterUpCommand()
        {
            Counter = Counter + 1;
        }
    }
}

using CoffeeAlarmV2.Service;
using CoffeeAlarmV2.Model;
using System.ComponentModel;

namespace CoffeeAlarmV2
{
    public class MainPageVM : INotifyPropertyChanged
    {
        private readonly ICoffeeService _coffeeService;
        private readonly IStorageService _storageService;

        private Command? turnOnCommand;
        private Command? turnOffCommand;
        private Command? initializeCommand;
        private Command? labelTappedCommand;
        private Command? resetCommand;
        private Command? coffeeTappedCommand;
        private Command? espressoTappedCommand;

        private Command? setTimerCommand;

        private bool isAdmin;
        private TimeSpan selectedTime;
        private DateTime activeTimerTime;
        private bool isSet;
        private bool isCoffee;
        private bool isEspresso;
        private string errorMessage = string.Empty;

        private System.Timers.Timer? timer = null;
        private int tapCount = 0;

        public MainPageVM(ICoffeeService coffeeService, IStorageService storageService)
        {
            _coffeeService = coffeeService;
            _storageService = storageService;
            LoadData();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public Command LabelTappedCommand => labelTappedCommand ??= new Command(() => HeaderTapped());
        public Command ResetCommand => resetCommand ??= new Command(() => IsAdmin = false);

        public Command TurnOnCommand => turnOnCommand ??= new Command(async () => await _coffeeService.TurnOn(), () => CanExecuteApiCalls);
        public Command TurnOffCommand => turnOffCommand ??= new Command(async () => await _coffeeService.TurnOff(), () => CanExecuteApiCalls);
        public Command InitializeCommand => initializeCommand ??= new Command(async () => await _coffeeService.InitCoffeeMaker(), () => CanExecuteApiCalls);

        public Command SetTimerCommand => setTimerCommand ??= new Command(async () => await ToggleTimer(), () => CanExecuteApiCalls);
        public Command CoffeeTappedCommand => coffeeTappedCommand ??= new Command(() => IsCoffee = true);
        public Command EspressoTappedCommand => espressoTappedCommand ??= new Command(() => IsEspresso = true);


        public bool IsAdmin
        {
            get => isAdmin;
            set
            {
                if (isAdmin != value)
                {
                    isAdmin = value;
                    OnPropertyChanged(nameof(IsAdmin));
                }
            }
        }

        public TimeSpan SelectedTime
        {
            get => selectedTime;
            set
            {
                if (selectedTime != value)
                {
                    selectedTime = value;
                    OnPropertyChanged(nameof(SelectedTime));
                }
            }
        }

        public bool IsSet
        {
            get => isSet;
            set
            {
                if (isSet != value)
                {
                    isSet = value;
                    OnPropertyChanged(nameof(IsSet));
                }
            }
        }

        public DateTime ActiveTimerTime
        {
            get => activeTimerTime;
            set
            {
                if (activeTimerTime != value)
                {
                    activeTimerTime = value;
                    OnPropertyChanged(nameof(ActiveTimerTime));
                }
            }
        }

        public bool IsCoffee
        {
            get => isCoffee;
            set
            {
                if (isCoffee != value)
                {
                    isCoffee = value;
                    isEspresso = !isCoffee;
                    OnPropertyChanged(nameof(IsCoffee));
                    OnPropertyChanged(nameof(IsEspresso));
                    OnPropertyChanged(nameof(IsSetCommandEnabled));
                }
            }
        }

        public bool IsEspresso
        {
            get => isEspresso;
            set
            {
                if (isEspresso != value)
                {
                    isEspresso = value;
                    isCoffee = !isEspresso;
                    OnPropertyChanged(nameof(IsCoffee));
                    OnPropertyChanged(nameof(IsEspresso));
                    OnPropertyChanged(nameof(IsSetCommandEnabled));
                }
            }
        }

        public string ErrorMessage
        {
            get => errorMessage;
            set
            {
                if (errorMessage != value)
                {
                    errorMessage = value;
                    OnPropertyChanged(nameof(ErrorMessage));
                    OnPropertyChanged(nameof(CanExecuteApiCalls));
                    OnPropertyChanged(nameof(TurnOnCommand));
                    OnPropertyChanged(nameof(TurnOffCommand));
                    OnPropertyChanged(nameof(InitializeCommand));
                    OnPropertyChanged(nameof(SetTimerCommand));
                    OnPropertyChanged(nameof(IsErrorMessageVisible));
                }
            }
        }

        public bool IsErrorMessageVisible => !string.IsNullOrEmpty(errorMessage);

        public bool CanExecuteApiCalls => string.IsNullOrEmpty(errorMessage);

        public bool IsSetCommandEnabled => IsCoffee || IsEspresso;

        public string SetTimerText => IsSet ? "Lösche" : "Setze";

        public string ActiveTimerText => !IsSet ? string.Empty : $"Timer: {ActiveTimerTime:hh:mm} {(IsCoffee ? "Kafi" : "Espresso")}";

        private async Task ToggleTimer()
        {
            if (IsSet)
            {
                await _coffeeService.DeleteTimer();
                IsSet = false;
                LoadData();
                return;
            }

            var now = DateTime.Now;

            var alarmTime = new DateTime(now.Year, now.Month, now.Day, SelectedTime.Hours, SelectedTime.Minutes, SelectedTime.Seconds);
            if (now > alarmTime)
            {
                alarmTime = alarmTime.AddDays(1);
            }

            if (IsCoffee)
            {
                await _coffeeService.SetCoffeeTimer(alarmTime);
            }

            else if (IsEspresso)
            {
                await _coffeeService.SetEspressoTimer(alarmTime);
            }

            await _storageService.SaveValue(Constants.IsCoffeeStorageKey, IsCoffee.ToString());
            await _storageService.SaveValue(Constants.SelectedTimeStorageKey, SelectedTime.ToString());

            LoadData();
        }

        private void HeaderTapped()
        {
            tapCount++;
            DisposeTimer();
            timer = new System.Timers.Timer(Constants.TapDebounce);
            timer.Elapsed += TimerElapsed;
            timer.Enabled = true;
            timer.Start();

            if (tapCount >= Constants.AdminTapCount)
            {
                IsAdmin = true;
            }
        }

        private void DisposeTimer()
        {
            if (timer != null)
            {
                timer.Enabled = false;
                timer.Elapsed -= TimerElapsed;
                timer.Dispose();
                timer = null;
            }
        }

        private void TimerElapsed(object? sender, EventArgs e)
        {
            DisposeTimer();
            tapCount = 0;
        }

        private async void LoadData()
        {
            try
            {
                var res = await _coffeeService.GetCurrentTimer();

                if (res != null && res.Time != null && res.Coffee != null)
                {
                    IsSet = true;
                    ActiveTimerTime = res.Time.Value;

                    switch (res.Coffee)
                    {
                        case "coffee":
                            IsCoffee = true;
                            break;
                        case "espresso":
                            IsEspresso = true;
                            break;
                        default:
                            throw new NotImplementedException($"Coffee '{res.Coffee}' is not implemented");
                    }
                }
            }
            catch
            {
                ErrorMessage = "Connection could not be established";
            }
            
            if (TimeSpan.TryParse(await _storageService.GetValue(Constants.SelectedTimeStorageKey), out TimeSpan time) && time != SelectedTime)
            {
                SelectedTime = time;
            }

            if (bool.TryParse(await _storageService.GetValue(Constants.IsCoffeeStorageKey), out var isCoffee))
            {
                if (isCoffee)
                {
                    IsCoffee = true;
                }
                else
                {
                    IsEspresso = true;
                }
            }
            else
            {
                IsCoffee = true;
            }

            OnPropertyChanged(nameof(ActiveTimerText));
            OnPropertyChanged(nameof(SelectedTime));
            OnPropertyChanged(nameof(SetTimerText));
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

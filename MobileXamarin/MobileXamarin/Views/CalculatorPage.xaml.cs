using MobileXamarin.ViewModels; // Add this line at the top
using Xamarin.Forms.Xaml;
using Xamarin.Forms;
using System;

namespace MobileXamarin.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CalculatorPage : ContentPage
    {
        CalculatorViewModel _viewModel;

        void FocusAndPressString(string arg)
        {
            var button = this.FindByName<Button>(arg);
            if (button != null)
            {
                FocusAndPress(button);
            }
        }
        void FocusOnlyString(string arg)
        {
            var button = this.FindByName<Button>(arg);
            if (button != null)
            {
                FocusButton(button);
            }
        }

        public CalculatorPage()
        {
            InitializeComponent();

            _viewModel = new CalculatorViewModel();
            BindingContext = _viewModel;




            MessagingCenter.Subscribe<CalculatorViewModel, string>(this, "FocusAndPress", (sender, arg) =>
            {
                FocusAndPressString(arg);
            });
            MessagingCenter.Subscribe<CalculatorViewModel, string>(this, "FocusOnly", (sender, arg) =>
            {
                var button = this.FindByName<Button>(arg);
                if (button != null)
                {
                    FocusButton(button);
                }
            });


            var keyboardService = DependencyService.Get<IKeyboardService>();

            _viewModel.SaveOperationHistory("Init completed!");
            FocusAndPressString("0");

            return;

            keyboardService.RegisterKeyPress((key) =>
            {

                _viewModel.SaveOperationHistory("Pressed keyboard shortcut: " + key);
                
                
                switch (key)
                {
                    case "Enter":
                        FocusAndPressString("ResultButton");
                        break;
                    case "Backspace":
                        FocusAndPressString("Backspace");
                        break;
                    case "Delete":
                        FocusAndPressString("ClearEntryButton");
                        break;
                    case "Escape":
                        FocusAndPressString("ClearButton");
                        break;
                }

                switch (key.ToLower())
                {
                    case "-":
                        FocusAndPressString("MinusButton");
                        break;
                    case "+":
                        FocusAndPressString("PlusButton");
                        break;
                    case "*":
                        FocusAndPressString("MultButton");
                        break;
                    case "/":
                        FocusAndPressString("DivButton");
                        break;
                    case "=":
                        FocusAndPressString("ResultButton");
                        break;

                    case "@":
                        FocusAndPressString("SqrtButton");
                        break;
                    case "%":
                        FocusAndPressString("PercentButton");
                        break;
                    case "\\":
                        FocusAndPressString("FractionButton");
                        break;
                    case "0":
                        FocusAndPressString("Digit0Button");
                        break;
                    case "1":
                        FocusAndPressString("Digit1Button");
                        break;
                    case "2":
                        FocusAndPressString("Digit2Button");
                        break;
                    case "3":
                        FocusAndPressString("Digit3Button");
                        break;
                    case "4":
                        FocusAndPressString("Digit4Button");
                        break;
                    case "5":
                        FocusAndPressString("Digit5Button");
                        break;
                    case "6":
                        FocusAndPressString("Digit6Button");
                        break;
                    case "7":
                        FocusAndPressString("Digit7Button");
                        break;
                    case "8":
                        FocusAndPressString("Digit8Button");
                        break;
                    case "9":
                        FocusAndPressString("Digit9Button");
                        break;
                    case ".":
                        FocusAndPressString("ComaButton");
                        break;
                    case ",":
                        FocusAndPressString("ComaButton");
                        break;
                    case "b":
                        FocusAndPressString("Backspace");
                        break;
                    case "e":
                        FocusAndPressString("ClearButton");
                        break;
                    case "f":
                        FocusAndPressString("PlusMinusButton");
                        break;
                }

            });


        }
        private void FocusAndPress(Button button)
        {
            FocusButton(button);
            PressButton(button);
        }
        private void FocusButton(Button button)
        {
            button.Focus();
        }
        private void PressButton(Button button)
        {
            if (button.Command != null && button.Command.CanExecute(button.CommandParameter))
            {
                button.Command.Execute(button.CommandParameter);
            } else
            {
                button.SendClicked();
            }
        }

        private void ClearAllButton_Clicked(object sender, EventArgs e)
        {
            _viewModel.ClearAll();
        }

        private void ClearEntryButton_Clicked(object sender, EventArgs e)
        {
            _viewModel.ClearEntry();
        }



        //Dealing with 0 buttpon problem - shitcode - not using
        private void Digit0ButtonAutoAutoFocusProblem(string key)
        {
            if (key == "Enter")
            {
                FocusOnlyString("ResultButton");
            }
            if (key == "Escape")
            {
                FocusOnlyString("ClearButton");
            }
            if (key == "Delete")            {
                FocusAndPressString("ClearEntryButton");
            }
        }
    }
}

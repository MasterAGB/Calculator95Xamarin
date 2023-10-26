using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

namespace MobileXamarin.ViewModels
{
    public class CalculatorViewModel : BaseViewModel
    {

        private ICommand _keyPressCommand;
        public ICommand KeyPressCommand => _keyPressCommand ?? (_keyPressCommand = new Command<string>(ProcessButtonAction));

        

        private string _prevNumberVisualText;
        public string PrevNumberVisualText
        {
            get => _prevNumberVisualText;
            set
            {
                _prevNumberVisualText = value;
                OnPropertyChanged(nameof(PrevNumberVisualText));
            }
        }

        private string _mainInputText = "0";
        public string MainInputText
        {
            get => _mainInputText;
            set
            {
                _mainInputText = value;
                OnPropertyChanged(nameof(MainInputText));
            }
        }



        //List of entered chars.
        //TODO: can be replaced by chars, some refactoring required then..
        public List<string> ResultValueSymbolsList;

        //When result field can be deleted with a single Backspace 
        public bool ResultValueIsTemporary;

        //When we are on final step, after pressing = sign (Showing also full math sample on the top: 2+2=...)
        public bool IsInFinalStep;

        public double FirstOperandForFinalStep;
        public string OperationCode = "=";
        public double SecondOperandForFinalStep;




      
      




        //Clear ALL
        public void ClearAll()
        {
            ResultValueIsTemporary = false;
            IsInFinalStep = false;
            SecondOperandForFinalStep = 0;
            MessagingCenter.Send(this, "UpdateHistory", string.Empty);
            ResultValueIsTemporary = false;
            SetNewPendingOperation("=");
            SetPrevValue(0);
            ClearResultValueSymbolsList();
            SetVisualOperationCode();
        }


        //CLEAR ENTRY
        public void ClearEntry()
        {
            ClearResultValueSymbolsList();
            ResultValueIsTemporary = false;
        }

        void SetFullyValueAndSync(double newValue, bool temporary = false)
        {
            ClearResultValueSymbolsList();

            ResultValueIsTemporary = temporary;

            newValue = FixValueBeforeUsing(newValue);
            string value = newValue.ToString(CultureInfo.CurrentCulture);
            //value = value.Replace(",", ".");
            char[] charArr = value.ToCharArray();
            foreach (char ch in charArr)
            {
                ResultValueSymbolsList.Add(ch.ToString());
            }

            SyncResultValue();
        }

        void ClearResultValueSymbolsList()
        {
            ResultValueSymbolsList.Clear();
            SyncResultValue();
        }

        void SyncResultValue(bool keepZeroes = false)
        {
            if (keepZeroes)
            {
                string tempFilledText = GetTmpInputValueText();
                tempFilledText = tempFilledText.Replace(".", NumberFormatInfo.InvariantInfo.NumberDecimalSeparator);
                MainInputText = tempFilledText;
            }
            else
            {
                double doubleNumber = GetResultInputValue();
                MainInputText = OptimizeDoubleForVisuals(doubleNumber);
            }

            MessagingCenter.Send(this, "UpdateTempList", String.Join(",", ResultValueSymbolsList));
        }


        void SaveValueHistory(double oldValue)
        {
            Debug.WriteLine("Update hisotry from Calculator View Model:" + oldValue);
            string newHistoryText1 = (oldValue.ToString(CultureInfo.CurrentCulture) + Environment.NewLine);
            MessagingCenter.Send(this, "UpdateHistory", newHistoryText1);
        }

        public void SaveOperationHistory(string oldValue)
        {
            Debug.WriteLine("Update Operation hisotry from Calculator View Model:" + oldValue);
            string newHistoryText2 = (oldValue + Environment.NewLine);
            MessagingCenter.Send(this, "UpdateHistory", newHistoryText2);
        }


        void SetVisualOperationCode()
        {
            SyncHistoryLabel();
        }

        void SetPrevValue(double newValue)
        {
            FirstOperandForFinalStep = newValue;
            SyncHistoryLabel();
        }

        void SyncHistoryLabel()
        {
            PrevNumberVisualText = "";
            if (OperationCode != "=")
            {
                PrevNumberVisualText = String.Format("{0} {1}", OptimizeDoubleForVisuals(FirstOperandForFinalStep),
                    OperationCode);
                ;
            }

            if (IsInFinalStep)
            {
                PrevNumberVisualText += String.Format(" {0} = ", SecondOperandForFinalStep);
            }
        }

        void SetNewPendingOperation(string newOperationCode)
        {
            OperationCode = newOperationCode;
        }

        double GetPrevValue()
        {
            return FirstOperandForFinalStep;
        }



        public CalculatorViewModel()
        {
            Title = "Calculator 95";
            ResultValueSymbolsList = new List<string>();
            // Set default values
            MainInputText = "0";
            PrevNumberVisualText = "";
        }


        double TryToCompilePreviousOperation()
        {
            double currentValue = GetResultInputValue();
            if (ResultValueIsTemporary)
            {
                //time to remember last used value
                currentValue = SecondOperandForFinalStep;
            }


            double prevValue = GetPrevValue();


            double newValue = currentValue;
            try
            {
                switch (OperationCode)
                {
                    case "+":
                        newValue = prevValue + currentValue;
                        break;
                    case "-":
                        newValue = prevValue - currentValue;
                        break;
                    case "*":
                        newValue = prevValue * currentValue;
                        break;
                    case "/":
                        newValue = prevValue / currentValue;
                        break;
                    case "=":
                        newValue = currentValue;
                        break;
                }
            }
            catch
            {
                // ignored - we just use the current value then
            }


            SaveValueHistory(prevValue);
            SaveOperationHistory(OperationCode);
            SaveValueHistory(currentValue);
            SaveOperationHistory("=");
            SaveValueHistory(newValue);

            return newValue;
        }


        public void ProcessButtonAction(string x)
        {
            if (x != "=")
            {
                IsInFinalStep = false;
            }

            switch (x)
            {
                //simple modification of temporary entered values
                case "0":
                case "1":
                case "2":
                case "3":
                case "4":
                case "5":
                case "6":
                case "7":
                case "8":
                case "9":
                    if (ResultValueIsTemporary)
                    {
                        ClearResultValueSymbolsList();
                    }

                    ResultValueSymbolsList.Add(x);
                    ResultValueIsTemporary = false;
                    SyncResultValue(true);
                    break;
                case ".":
                    if (ResultValueSymbolsList.Count == 0)
                    {
                        ResultValueSymbolsList.Add("0");
                    }

                    if (ResultValueSymbolsList.Count == 1 && ResultValueSymbolsList[0] == "-")
                    {
                        ResultValueSymbolsList.Add("0");
                    }

                    if (!ResultValueSymbolsList.Contains(".") && !ResultValueSymbolsList.Contains(","))
                    {
                        ResultValueSymbolsList.Add(x);
                        ResultValueIsTemporary = false;
                    }


                    SaveOperationHistory("=aaa" + x);
                    SyncResultValue(true);
                    break;
                case "Backspace":
                    SaveOperationHistory("=vvv" + x);
                    if (ResultValueIsTemporary)
                    {
                        ClearResultValueSymbolsList();
                        ResultValueIsTemporary = false;
                    }
                    else
                    {
                        if (ResultValueSymbolsList.Count > 0)
                        {
                            ResultValueSymbolsList.RemoveAt(ResultValueSymbolsList.Count - 1);
                        }
                    }

                    SyncResultValue(true);
                    break;
                case "Flip":
                    if (ResultValueSymbolsList.Count > 0)
                    {
                        if (ResultValueSymbolsList[0] == "-")
                        {
                            ResultValueSymbolsList.RemoveAt(0);
                        }
                        else
                        {
                            ResultValueSymbolsList.Insert(0, "-");
                        }

                        ResultValueIsTemporary = false;
                    }

                    SyncResultValue(true);
                    break;

                //single functions

                case "@": //sqrt
                    {
                        double currentValue = GetResultInputValue();
                        double newValue = 0;
                        if (currentValue > 0)
                        {
                            newValue = FixValueBeforeUsing(Math.Sqrt(currentValue));
                        }

                        SaveOperationHistory(x);
                        SaveValueHistory(currentValue);
                        SaveOperationHistory("=");
                        SaveValueHistory(newValue);


                        //SetPrevValue(currentValue);
                        SetVisualOperationCode();
                        SetFullyValueAndSync(newValue);
                    }
                    break;

                case "%":
                    {
                        double currentValue = GetResultInputValue();
                        double prevValue = GetPrevValue();

                        double newValue = FixValueBeforeUsing(prevValue / 100 * currentValue);


                        SaveOperationHistory(x);
                        SaveValueHistory(currentValue);
                        SaveOperationHistory("=");
                        SaveValueHistory(newValue);


                        //SetPrevValue(currentValue);
                        SetVisualOperationCode();
                        SetFullyValueAndSync(newValue);
                    }
                    break;
                case "Fraction":
                    {
                        double currentValue = GetResultInputValue();

                        double newValue = FixValueBeforeUsing(1 / currentValue);


                        SaveOperationHistory(x);
                        SaveValueHistory(currentValue);
                        SaveOperationHistory("=");
                        SaveValueHistory(newValue);


                        //SetPrevValue(currentValue);
                        SetVisualOperationCode();
                        SetFullyValueAndSync(newValue, true);
                    }
                    break;

                //2 values operations

                case "+":
                    {
                        double newPrevValue = TryToCompilePreviousOperation();
                        SetNewPendingOperation(x);
                        SetPrevValue(newPrevValue);
                        SetVisualOperationCode();
                        SetFullyValueAndSync(newPrevValue, true);
                    }
                    break;
                case "-":
                    {
                        double newPrevValue = TryToCompilePreviousOperation();
                        SetNewPendingOperation(x);
                        SetPrevValue(newPrevValue);
                        SetVisualOperationCode();
                        SetFullyValueAndSync(newPrevValue, true);
                    }
                    break;
                case "*":
                    {
                        double newPrevValue = TryToCompilePreviousOperation();
                        SetNewPendingOperation(x);
                        SetPrevValue(newPrevValue);
                        SetVisualOperationCode();
                        SetFullyValueAndSync(newPrevValue, true);
                    }
                    break;
                case "/":
                    {
                        double newPrevValue = TryToCompilePreviousOperation();
                        SetNewPendingOperation(x);
                        SetPrevValue(newPrevValue);
                        SetVisualOperationCode();
                        SetFullyValueAndSync(newPrevValue, true);
                    }
                    break;
                case "=":
                    {
                        double currentValue = GetResultInputValue();
                        if (!IsInFinalStep)
                        {
                            IsInFinalStep = true;
                            SecondOperandForFinalStep = currentValue;
                        }
                        else
                        {
                            SetPrevValue(currentValue);
                        }

                        //Saving the last Value for reusing it, when pressing = again and again
                        //SetFullyValueAndSync(currentValue, true);
                        double newPrevValue = TryToCompilePreviousOperation();
                        SetVisualOperationCode();
                        SetFullyValueAndSync(newPrevValue, true);
                        //SetPrevValue(newPrevValue);
                    }
                    break;
            }
        }

        string GetTmpInputValueText()
        {
            string text = String.Join(String.Empty, ResultValueSymbolsList);
            text = text.Replace(',', '.');
            return text;
        }

        double GetResultInputValue()
        {
            string text = GetTmpInputValueText();
            if (double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out var value))
            {
                value = FixValueBeforeUsing(value);
                return value;
            }
            else
            {
                return 0;
            }
        }

        double FixValueBeforeUsing(double value)
        {
            //value = Math.Round(value, 12, MidpointRounding.AwayFromZero);
            return value;
        }


        string OptimizeDoubleForVisuals(double doubleNumber)
        {
            if (double.IsInfinity(doubleNumber))
            {
                return "∞";
            }


            //Just not showing too big values
            if (doubleNumber > 999999999999999)
            {
                doubleNumber = 999999999999999;
            }

            string text = String.Format("{0:F15}", doubleNumber).TrimEnd('0').TrimEnd(',').TrimEnd('.');

            // Gets the InvariantInfo.

            text = text.Replace(".", NumberFormatInfo.InvariantInfo.NumberDecimalSeparator);

            return text;
        }


        void FocusAndPress(string buttonName)
        {
            //if we would decide to make any focuses from viewModel, code moved to Page
            MessagingCenter.Send(this, "FocusAndPress", buttonName);
        }

        void FocusOnly(string buttonName)
        {
            //if we would decide to make any focuses from viewModel, code moved to Page
            MessagingCenter.Send(this, "FocusOnly", buttonName);
        }




    }
}
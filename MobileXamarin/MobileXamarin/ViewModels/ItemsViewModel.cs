using MobileXamarin.Views;
using System;
using Xamarin.Forms;
using System.Diagnostics;


namespace MobileXamarin.ViewModels
{
    public class ItemsViewModel : BaseViewModel
    {
        public ItemsViewModel()
        {
            Title = "History";
           
        }

        private string _historyText;
        public string HistoryText
        {
            get => _historyText;
            set
            {
                Debug.WriteLine("Update hisotry from setter:" + value);
                if (_historyText == value)
                    return;

                _historyText = value;
                OnPropertyChanged(nameof(HistoryText));
            }
        }

        private string _tempListTextboxText;
        public string TempListTextboxText
        {
            get => _tempListTextboxText;
            set
            {
                if (_tempListTextboxText == value)
                    return;

                _tempListTextboxText = value;
                OnPropertyChanged(nameof(TempListTextboxText));

            }
        }
        
    }
}
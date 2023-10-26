using MobileXamarin.ViewModels;
using MobileXamarin.Views;
using System;
using System.Diagnostics;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileXamarin
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();
                    MainPage = new AppShell();

            // Create an instance of ItemsViewModel
            ItemsViewModel itemsViewModel = new ItemsViewModel();

            MessagingCenter.Subscribe<ItemsViewModel, string>(this, "UpdateHistory", (sender, arg) => {
                Debug.WriteLine("Update hisotry from subscribtion:" + arg);

                if (arg == string.Empty)
                {
                    itemsViewModel.HistoryText = "";
                }
                else
                {
                    itemsViewModel.HistoryText = arg + itemsViewModel.HistoryText;
                }
            });
            MessagingCenter.Subscribe<ItemsViewModel, string>(this, "UpdateTempList", (sender, arg) => {
                itemsViewModel.TempListTextboxText = arg;
            });
            Debug.WriteLine("Update hisotry on start: Subscribed");
            MessagingCenter.Send(this, "UpdateHistory", "Subscribed!");
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}


using MobileXamarin.ViewModels;


using Xamarin.Forms;

namespace MobileXamarin.Views
{
    public partial class ItemsPage : ContentPage
    {

        public ItemsPage()
        {
            InitializeComponent();
            BindingContext = new ItemsViewModel();
        }

    }
}
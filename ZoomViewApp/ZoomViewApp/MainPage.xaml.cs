using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ZoomViewApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void ZoomContainer_Swipe(object sender, SwipedEventArgs e)
        {
            DisplayAlert("", $"You swiped: {e.Direction.ToString()}", "OK");
        }
    }
}

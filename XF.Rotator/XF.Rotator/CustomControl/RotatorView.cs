using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace XF.Rotator.CustomControl
{
    public class RotatorView : ContentView
    {
        public RotatorView()
        {
           
        }

        public RotatorView(ContentView view)
        {
            Content = view.Content;
        }
        public string ID { get; set; }
    }
}

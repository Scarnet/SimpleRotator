using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using XF.Rotator.CustomControl;

namespace XF.Rotator.Models
{
    public static class Pages
    {
        public static List<RotatorView> ThosePages = new List<RotatorView>()
        {
            new RotatorView()
            {
                BackgroundColor = Color.Yellow
            },
            new RotatorView()
            {
                BackgroundColor = Color.Black
            },
            new RotatorView()
            {
                BackgroundColor = Color.Blue
            }


        };
    }
}

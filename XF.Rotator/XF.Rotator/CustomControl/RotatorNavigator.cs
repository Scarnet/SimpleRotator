using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace XF.Rotator.CustomControl
{
    public class RotatorNavigator : Label
    {
        public Color ActiveColor { get; private set; }
        public Color InActiveColor { get; private set; }

        private bool _isActive;
        public bool IsActive
        {
            get => _isActive;
            set
            {
                _isActive = value;
                TextColor = value ? ActiveColor : InActiveColor;
            }
        }

        public int Index { get; set; }
        
        public RotatorNavigator(Color activeColor, Color inActiveColor)
        {
            ActiveColor = activeColor;
            InActiveColor = inActiveColor;
            Text = "⬤";
            FontSize = Device.GetNamedSize(NamedSize.Large, GetType());
        }

        public RotatorNavigator()
        {
            ActiveColor = Color.Black;
            InActiveColor = Color.Gray;
            Text = "⬤";
            FontSize = Device.GetNamedSize(NamedSize.Large, GetType());
        }
    }
}

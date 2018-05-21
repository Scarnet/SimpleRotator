using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XF.Rotator.CustomControl;

namespace XF.Rotator
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Main : ContentPage
	{
	    public bool SwipeEnabled { get; set; } = true;
		public Main ()
		{
			InitializeComponent ();
		}

        protected override void OnAppearing()
        {
            base.OnAppearing();

            //rotator.Init();
            
            try
            {
                rt.Pages.Clear();
                rt.Pages.Add(new RotatorView()
                {
                    BackgroundColor = Color.Blue
                });
                rt.Pages.Add(new RotatorView()
                {
                    BackgroundColor = Color.Bisque
                });

            }
            catch (Exception exception)
            {

            }

        }

        private void Button_OnClicked(object sender, EventArgs e)
	    {
	        
	    }

	    private void Button_OnClicked1(object sender, EventArgs e)
	    {
	        

	    }

	    private void Button_OnClicked2(object sender, EventArgs e)
	    {
	        
	    }


	    private void TapGestureRecognizer_OnTapped(object sender, EventArgs e)
	    {
	        
	    }

	    private void TapGestureRecognizer_OnTapped1(object sender, EventArgs e)
	    {
	        
	    }

	    private void TapGestureRecognizer_OnTapped2(object sender, EventArgs e)
	    {
	        
	    }
	}

}
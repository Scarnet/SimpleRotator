using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

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
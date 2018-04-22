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
	    public bool SwipeEnabled { get; set; } = false;
	    public List<RotatorView> Pages => Models.Pages.ThosePages;
		public Main ()
		{
			InitializeComponent ();

		}
	}
}
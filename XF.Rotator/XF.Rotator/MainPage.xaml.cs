using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using XF.Rotator.CustomControl;

namespace XF.Rotator
{
    public partial class MainPage : ContentPage
    {
        private const string FirstPageId = "FirstView";
        private Stack<RotatorView> _leftStack;
        private Stack<RotatorView> _rightStack;
        public RotatorView CurrentPage { get; set; }
        public List<RotatorView> Pages { get; set; }
        private bool _swipeRun;
        public MainPage()
        {
            InitializeComponent();
            _swipeRun = false;
            Pages = Models.Pages.ThosePages;
            InitPages();
            InitStacks();
        }
        private void InitStacks()
        {
            _leftStack = new Stack<RotatorView>();
            _rightStack = new Stack<RotatorView>();

            Pages.ForEach(page => _rightStack.Push(page));
        }


        private void InitPages()
        {
            var firstPage = Pages[0];
            firstPage.ID = FirstPageId;
            CurrentPage = firstPage;
            relativeParent.Children.Add(firstPage, Constraint.RelativeToParent(parent => parent.X),
                Constraint.RelativeToParent(parent => parent.Y), Constraint.RelativeToParent(parent => parent.Width),
                Constraint.RelativeToParent(parent => parent.Height));

            Pages.Where(p => p.ID != FirstPageId).ForEach(page =>
                {
                    relativeParent.Children.Add(page, Constraint.RelativeToParent(parent => parent.Width),
                        Constraint.RelativeToParent(parent => parent.Y),
                        Constraint.RelativeToParent(parent => parent.Width),
                        Constraint.RelativeToParent(parent => parent.Height));
                });
        }

        private void PanGestureRecognizer_OnPanUpdated(object sender, PanUpdatedEventArgs e)
        {
            if (e.StatusType == GestureStatus.Started)
                _swipeRun = false;

            if (Math.Abs(e.TotalX) <= Math.Abs(e.TotalY) && e.StatusType != GestureStatus.Running)
                return;

            if (_swipeRun)
                return;


            if (e.TotalX > 0)
                SwipeRight();
            else if (e.TotalX < 0)
                SwipeLeft();
            _swipeRun = true;
        }

        private void SwipeLeft()
        {
            if (!_rightStack.Any())
                return;

            if (CurrentPage != null)
            {
                double movingValue = CurrentPage.ID == FirstPageId ? CurrentPage.Width * -1 : CurrentPage.Width * -2;
                CurrentPage.TranslateTo(movingValue, 0, 250, Easing.Linear);
                _leftStack.Push(CurrentPage);
            }


            var newVisibleView = _rightStack.Pop();
            double newMovingValue = newVisibleView.ID == FirstPageId ? 0 : newVisibleView.Width * -1;

            newVisibleView.TranslateTo(newMovingValue, 0, 250, Easing.Linear);
            CurrentPage = newVisibleView;
        }

        private void SwipeRight()
        {
            if (!_leftStack.Any())
                return;

            if (CurrentPage != null)
            {
                double movingValue = CurrentPage.ID == FirstPageId ? CurrentPage.Width * 1 : 0;
                CurrentPage.TranslateTo(movingValue, 0, 250, Easing.Linear);
                _rightStack.Push(CurrentPage);
            }


            var newVisibleView = _leftStack.Pop();
            double newMovingValue = newVisibleView.ID == FirstPageId ? 0 : newVisibleView.Width * -1;
            newVisibleView.TranslateTo(newMovingValue, 0, 250, Easing.Linear);
            CurrentPage = newVisibleView;
        }
    }
}

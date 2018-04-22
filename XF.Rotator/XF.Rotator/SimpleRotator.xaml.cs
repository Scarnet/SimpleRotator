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
    [ContentProperty("Pages")]
    public partial class SimpleRotator : ContentView
    {
        public static readonly BindableProperty PagesProperty = BindableProperty.Create(

            nameof(Pages),
            typeof(List<RotatorView>),
            typeof(SimpleRotator),
            new List<RotatorView>());

        public static readonly BindableProperty SwipeEnabledProperty = BindableProperty.Create(

            nameof(SwipeEnabled),
            typeof(bool),
            typeof(SimpleRotator),
            true);


        private const string FirstPageId = "FirstView";
        private Stack<RotatorView> _leftStack;
        private Stack<RotatorView> _rightStack;
        public RotatorView CurrentPage { get; private set; }
        
        public int CurrentIndex { get; private set; }
        private RotatorNavigator _currentNavigator;
        private List<RotatorNavigator> _navigators;

        private List<RotatorView> _pages;
        public List<RotatorView> Pages
        {
            get => _pages;
            set
            {
                if(!value.Any())
                    return;

                _pages = value;
                InitPages();
            } 
        }

        public bool SwipeEnabled { get; set; }
        private List<RotatorView> _outBoundPages;
        private bool _swipeRun;

        public SimpleRotator()
        {
            InitializeComponent();
            _swipeRun = false;
            Pages = new List<RotatorView>();
            //InitPages();
            //InitNavigators();
            //InitStacks();
        }

        
        private void InitPages()
        {
            var firstPage = Pages[0];
            firstPage.ID = FirstPageId;
            CurrentPage = firstPage;
            relativeParent.Children.Add(firstPage, Constraint.RelativeToParent(parent => parent.X),
                Constraint.RelativeToParent(parent => parent.Y), Constraint.RelativeToParent(parent => parent.Width),
                Constraint.RelativeToParent(parent => parent.Height));

            _outBoundPages = Pages.Where(page => page.ID != FirstPageId).ToList();
            _outBoundPages.ForEach(page =>
            {
                relativeParent.Children.Add(page, Constraint.RelativeToParent(parent => parent.Width),
                    Constraint.RelativeToParent(parent => parent.Y),
                    Constraint.RelativeToParent(parent => parent.Width),
                    Constraint.RelativeToParent(parent => parent.Height));
            });
        }

        private void InitStacks()
        {
            _leftStack = new Stack<RotatorView>();
            _rightStack = new Stack<RotatorView>();

            _outBoundPages?.ForEach(page => _rightStack.Push(page));
        }

        private void InitNavigators()
        {
            CurrentIndex = 0;
            _navigators = new List<RotatorNavigator>();
            for (int index = 0; index < Pages.Count; index++)
            {
                var navigator = new RotatorNavigator
                {
                    Index = index,
                    IsActive = index == 0
                };
                var gestureRecognizer = new TapGestureRecognizer();
                gestureRecognizer.Tapped += HandleNavigate;

                navigator.GestureRecognizers.Add(gestureRecognizer);
                _navigators.Add(navigator);
                if (navigator.Index == 0)
                    _currentNavigator = navigator;
            }
            _navigators.ForEach(nav => NavigationBar.Children.Add(nav));
        }

        private void HandleNavigate(object sender, EventArgs eventArgs)
        {
            var navigator = (RotatorNavigator) sender;
            int steps = navigator.Index - CurrentIndex;
            if(steps == 0)
                return;

            Action swipeAction = (steps > 0) ? (Action)SwipeLeft : (Action)SwipeRight;
            for (int step = 0; step < Math.Abs(steps); step++)
                swipeAction();

        }



        private void PanGestureRecognizer_OnPanUpdated(object sender, PanUpdatedEventArgs e)
        {
            if(!SwipeEnabled)
                return;

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

        private void UpdateNavStatus(bool rtl)
        {
            int newIndex = (rtl)? CurrentIndex - 1 : CurrentIndex + 1;

            if(newIndex < 0 || newIndex > _navigators.Count -1)
                return;

            var newNav = _navigators[newIndex];
            CurrentIndex = newNav.Index;
            newNav.IsActive = true;
            _currentNavigator.IsActive = false;
            _currentNavigator = newNav;

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
            UpdateNavStatus(rtl: false);
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
            UpdateNavStatus(rtl: true);
        }
    }
}

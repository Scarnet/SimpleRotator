using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
        delegate Task SwipeAction();


        public static readonly BindableProperty PagesProperty = BindableProperty.Create(

            nameof(Pages),
            typeof(ObservableCollection<RotatorView>),
            typeof(SimpleRotator),
            new ObservableCollection<RotatorView>());

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

        public event EventHandler SwipeCompleted;
        public event EventHandler SwipeStarted;

        private RotatorNavigator _currentNavigator;
        private List<RotatorNavigator> _navigators;

        private ObservableCollection<RotatorView> _pages = new ObservableCollection<RotatorView>();
        public ObservableCollection<RotatorView> Pages
        {
            get => _pages;
            set
            {
                _pages = value;
            }
        }

        public bool SwipeEnabled { get { return (bool)GetValue(SwipeEnabledProperty); } set { SetValue(SwipeEnabledProperty, value); } }
        private List<RotatorView> _outBoundPages;
        private bool _swipeRun;

        public SimpleRotator()
        {
            //Pages = new List<RotatorView>();
            InitializeComponent();
            _swipeRun = false;

            Pages.CollectionChanged += Pages_CollectionChanged;

            //Device.StartTimer(TimeSpan.FromSeconds(0.1), () =>
            //{
            //    InitPages();
            //    InitNavigators();
            //    InitStacks();
            //    return false;
            //});
        }

        private void Pages_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Init();
        }

        public void Init()
        {
            InitPages();
            InitNavigators();
            InitStacks();
        }

        private void InitPages()
        {
            var firstPage = Pages[0];
            firstPage.ID = FirstPageId;
            var parentPanGestureRecognizer = new PanGestureRecognizer();
            parentPanGestureRecognizer.PanUpdated += PanGestureRecognizer_OnPanUpdated;
            firstPage.GestureRecognizers.Add(parentPanGestureRecognizer);
            CurrentPage = firstPage;
           
            relativeParent.Children.Clear();

            relativeParent.Children.Add(firstPage, Constraint.RelativeToParent(parent => parent.X),
                Constraint.RelativeToParent(parent => parent.Y), Constraint.RelativeToParent(parent => parent.Width),
                Constraint.RelativeToParent(parent => parent.Height));

            _outBoundPages = Pages.Where(page => page.ID != FirstPageId).ToList();
            _outBoundPages.ForEach(page =>
            {
                var panGestureRecognizer = new PanGestureRecognizer();
                panGestureRecognizer.PanUpdated += PanGestureRecognizer_OnPanUpdated;
                page.GestureRecognizers.Add(panGestureRecognizer);
                page.IsVisible = false;
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

            _outBoundPages.Reverse();
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

            NavigationBar.Children.Clear();

            _navigators.ForEach(nav => NavigationBar.Children.Add(nav));
        }

        private void HandleNavigate(object sender, EventArgs eventArgs)
        {
            var navigator = (RotatorNavigator)sender;
            int steps = navigator.Index - CurrentIndex;
            if (steps == 0)
                return;

            var swipeAction = (steps > 0) ? (SwipeAction)SwipeLeft : (SwipeAction)SwipeRight;
            Device.BeginInvokeOnMainThread(async () =>
            {
                for (int step = 0; step < Math.Abs(steps); step++)
                    await swipeAction();
            });
        }



        private void PanGestureRecognizer_OnPanUpdated(object sender, PanUpdatedEventArgs e)
        {
            if (!SwipeEnabled)
                return;

            if (e.StatusType == GestureStatus.Started)
                _swipeRun = false;

            if (Math.Abs(e.TotalX) <= Math.Abs(e.TotalY) && e.StatusType != GestureStatus.Running)
                return;

            if (_swipeRun)
                return;


            if (e.TotalX > 0)
                Device.BeginInvokeOnMainThread(async () => { await SwipeRight(); });
            else if (e.TotalX < 0)
                Device.BeginInvokeOnMainThread(async () => { await SwipeLeft(); });
            _swipeRun = true;
        }

        private void UpdateNavStatus(bool rtl)
        {
            int newIndex = (rtl) ? CurrentIndex - 1 : CurrentIndex + 1;

            if (newIndex < 0 || newIndex > _navigators.Count - 1)
                return;

            var newNav = _navigators[newIndex];
            CurrentIndex = newNav.Index;
            newNav.IsActive = true;
            _currentNavigator.IsActive = false;
            _currentNavigator = newNav;
        }

        public async Task SwipeLeft()
        {
            if (!_rightStack.Any())
                return;

            SwipeStarted?.Invoke(this, EventArgs.Empty);
            var newVisibleView = _rightStack.Pop();
            newVisibleView.IsVisible = true;

            Task taskOne = null;
            Task taskTwo;

            if (CurrentPage != null)
            {
                double movingValue = CurrentPage.ID == FirstPageId ? CurrentPage.Width * -1 : CurrentPage.Width * -2;
                taskOne = CurrentPage.TranslateTo(movingValue, 0, 250, Easing.Linear);
                _leftStack.Push(CurrentPage);
            }


            double newMovingValue = newVisibleView.ID == FirstPageId ? 0 : newVisibleView.Width * -1;

            taskTwo = newVisibleView.TranslateTo(newMovingValue, 0, 250, Easing.Linear);
            await Task.WhenAll(taskOne, taskTwo);

            if (CurrentPage != null)
                CurrentPage.IsVisible = false;

            CurrentPage = newVisibleView;
            UpdateNavStatus(rtl: false);
            SwipeCompleted?.Invoke(this, EventArgs.Empty);
        }

        public async Task SwipeRight()
        {
            if (!_leftStack.Any())
                return;

            SwipeStarted?.Invoke(this, EventArgs.Empty);
            var newVisibleView = _leftStack.Pop();
            newVisibleView.IsVisible = true;
            Task taskOne = null;
            Task taskTwo;

            if (CurrentPage != null)
            {
                double movingValue = CurrentPage.ID == FirstPageId ? CurrentPage.Width * 1 : 0;
                taskOne = CurrentPage.TranslateTo(movingValue, 0, 250, Easing.Linear);
                _rightStack.Push(CurrentPage);
            }


            double newMovingValue = newVisibleView.ID == FirstPageId ? 0 : newVisibleView.Width * -1;
            taskTwo = newVisibleView.TranslateTo(newMovingValue, 0, 250, Easing.Linear);
            await Task.WhenAll(taskOne, taskTwo);

            if (CurrentPage != null)
                CurrentPage.IsVisible = false;

            CurrentPage = newVisibleView;
            UpdateNavStatus(rtl: true);
            SwipeCompleted?.Invoke(this, EventArgs.Empty);
        }
    }
}

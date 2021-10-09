using L2Data2CodeWPF.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace L2Data2CodeWPF.ViewModel
{
    public class MessagesViewModel : BaseViewModel, IMessagesViewModel
    {
        private bool runningPurger;
        private readonly Dispatcher dispatcher;

        public ObservableCollection<MessageViewModel> AllMessages { get; }

        public MessagesViewModel()
        {
            dispatcher = Application.Current.Dispatcher;
            AllMessages = new ObservableCollection<MessageViewModel>();
            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(250)
            };
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        public void Add(string text, bool viewStatus = false, string code = null)
        {
            dispatcher?.BeginInvoke(() =>
            {
                lock(AllMessages)
                {
                    var previous = AllMessages.FirstOrDefault(m => m.Text == text);
                    if (previous == null)
                    {
                        AllMessages.Add(new MessageViewModel(text, viewStatus, code));
                    }
                    else
                    {
                        previous.EnlargeLife(viewStatus);
                    }
                }
            });
        }

        public void ViewAll(bool view)
        {
            if (!view) return;

            dispatcher?.BeginInvoke(() =>
            {
                foreach (var item in AllMessages)
                {
                    item.EnlargeLife(true);
                }

            });

        }

        public void ClearPinned(string code)
        {
            WaitForPurgeFinished();

            dispatcher?.BeginInvoke(() =>
            {
                runningPurger = true;

                List<MessageViewModel> toRemove;
                lock (AllMessages)
                {
                    toRemove = AllMessages.Where(m => m.Code == code).ToList();
                }
                foreach (var item in toRemove)
                {
                    item.ClearPinned();
                }
                runningPurger = false;
            });
        }

        private void WaitForPurgeFinished()
        {
            if (!runningPurger) return;

            dispatcher?.BeginInvoke(() =>
            {
                var start = DateTime.Now;
                while (runningPurger)
                {
                    var dif = DateTime.Now - start;
                    if (dif > TimeSpan.FromMinutes(1))
                    {
                        runningPurger = false;
                        throw new Exception("To many time waitting to Purge");
                    }
                }
            });
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            ExecuteActionWhenNotRunningPurge(Purge);
        }

        private void Purge()
        {
            List<MessageViewModel> toRemove;
            toRemove = AllMessages.Where(m => m.IsGoingToDie).ToList();
            if (toRemove.Any())
            {
                lock (AllMessages)
                {
                    toRemove.ForEach(item => AllMessages.Remove(item));
                }
            }
        }

        private void ExecuteActionWhenNotRunningPurge(Action action)
        {
            if (runningPurger) return;
            runningPurger = true;
            action.Invoke();
            runningPurger = false;
        }
    }
}
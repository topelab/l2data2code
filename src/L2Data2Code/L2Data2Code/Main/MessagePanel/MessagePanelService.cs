using L2Data2Code.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Threading;

namespace L2Data2Code.Main.MessagePanel
{
    public class MessagePanelService : IMessagePanelService
    {
        private bool runningPurger;
        private readonly IDispatcherWrapper dispatcher;

        public ObservableCollection<MessageVM> AllMessages { get; }

        public MessagePanelService(IDispatcherWrapper dispatcher)
        {
            this.dispatcher = dispatcher;
            AllMessages = new ObservableCollection<MessageVM>();
            DispatcherTimer timer = new()
            {
                Interval = TimeSpan.FromMilliseconds(250)
            };
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        public void Add(string text, bool viewStatus = false, string code = null)
        {
            dispatcher.Invoke(() =>
            {
                lock (AllMessages)
                {
                    var previous = AllMessages.FirstOrDefault(m => m.Text == text);
                    if (previous == null)
                    {
                        AllMessages.Add(new MessageVM(text, viewStatus, code));
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
            if (!view)
            {
                return;
            }

            dispatcher.Invoke(() =>
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

            dispatcher.Invoke(() =>
            {
                runningPurger = true;

                List<MessageVM> toRemove;
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
            if (!runningPurger)
            {
                return;
            }

            dispatcher.Invoke(() =>
            {
                var start = DateTime.Now;
                while (runningPurger)
                {
                    var dif = DateTime.Now - start;
                    if (dif > TimeSpan.FromMinutes(1))
                    {
                        runningPurger = false;
                        throw new Exception("To many time waiting to Purge");
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
            List<MessageVM> toRemove;
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
            if (runningPurger)
            {
                return;
            }

            runningPurger = true;
            action.Invoke();
            runningPurger = false;
        }
    }
}
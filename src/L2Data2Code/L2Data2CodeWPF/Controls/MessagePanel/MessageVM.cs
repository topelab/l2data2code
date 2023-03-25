using L2Data2CodeWPF.Base;
using System;
using System.Windows.Media;

namespace L2Data2CodeWPF.Controls.MessagePanel
{
    public class MessageVM : BaseVM
    {
        public const double GlobalTimeOfLife = 10.0;
        public const double GlobalTimeToDie = 1.0;

        private string _text;
        private bool _viewed;
        private DateTime _created;
        private TimeSpan _timeOfLife;
        private bool _pinned;
        private string _code;

        public string Text { get => _text; set => SetProperty(ref _text, value); }
        public bool Viewed
        {
            get => _viewed;
            set => SetProperty(ref _viewed, value, () =>
            {
                OnPropertyChanged(nameof(IsGoingToDie));
                OnPropertyChanged(nameof(Color));
            });
        }
        public DateTime Created { get => _created; set => SetProperty(ref _created, value); }
        public TimeSpan TimeOfLife
        {
            get => _timeOfLife;
            set => SetProperty(ref _timeOfLife, value, () =>
            {
                OnPropertyChanged(nameof(IsGoingToDie));
                OnPropertyChanged(nameof(Color));
            });
        }
        public bool Pinned
        {
            get => _pinned;
            set => SetProperty(ref _pinned, value, () =>
            {
                OnPropertyChanged(nameof(IsGoingToDie));
                OnPropertyChanged(nameof(Color));
            });
        }
        public string Code { get => _code; set => SetProperty(ref _code, value, () => Pinned = !string.IsNullOrWhiteSpace(value)); }
        public bool IsGoingToDie { get => !_pinned && _viewed && DateTime.Now > _created + _timeOfLife; }

        private readonly SolidColorBrush ColorBlack = new(Colors.Black);
        private readonly SolidColorBrush ColorRed = new(Colors.Red);

        public SolidColorBrush Color => _pinned ? ColorRed : ColorBlack;

        public MessageVM(string text, bool viewed = false, string code = null, double timeOfLifeInSeconds = GlobalTimeOfLife)
        {
            Text = text;
            Viewed = viewed;
            Created = DateTime.Now;
            TimeOfLife = TimeSpan.FromSeconds(timeOfLifeInSeconds);
            Code = code;
        }

        public void EnlargeLife(bool viewed)
        {
            Viewed = viewed;
            Created = DateTime.Now;
        }

        public void ClearPinned()
        {
            Created = DateTime.Now;
            TimeOfLife = TimeSpan.FromSeconds(GlobalTimeToDie);
            Code = null;
        }

        public void Forever()
        {
            Pinned = true;
        }
    }
}
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace SportNutritionShop.UserControls
{

    public partial class RatingStars : UserControl
    {
        public static readonly DependencyProperty RatingProperty =
            DependencyProperty.Register(
                nameof(Rating),
                typeof(double),
                typeof(RatingStars),
                new FrameworkPropertyMetadata(0.0, OnRatingChanged, CoerceRating),
                ValidateRating);

        public double Rating
        {
            get => (double)GetValue(RatingProperty);
            set => SetValue(RatingProperty, value);
        }

        public static readonly RoutedEvent RatingChangedEvent =
            EventManager.RegisterRoutedEvent(
                "RatingChanged",
                RoutingStrategy.Direct,
                typeof(RoutedEventHandler),
                typeof(RatingStars));

        public event RoutedEventHandler RatingChanged
        {
            add => AddHandler(RatingChangedEvent, value);
            remove => RemoveHandler(RatingChangedEvent, value);
        }

        private static readonly Brush FilledBrush = new SolidColorBrush(Color.FromRgb(0xFF, 0xB8, 0x00));
        private static readonly Brush EmptyBrush = new SolidColorBrush(Color.FromRgb(0xD8, 0xE0, 0xEA));

        public RatingStars()
        {
            InitializeComponent();
            BuildStars();
        }

        
        private static bool ValidateRating(object value) =>
            value is double d && d >= 0 && d <= 5;


        private static object CoerceRating(DependencyObject d, object baseValue)
        {
            double v = (double)baseValue;
            if (v < 0) return 0.0;
            if (v > 5) return 5.0;
            return v;
        }

        private static void OnRatingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RatingStars control)
            {
                control.BuildStars();
                control.RaiseEvent(new RoutedEventArgs(RatingChangedEvent));
            }
        }

        private void BuildStars()
        {
            StarsPanel.Children.Clear();
            int filledCount = (int)System.Math.Round(Rating);

            for (int i = 1; i <= 5; i++)
            {
                var star = new System.Windows.Shapes.Path
                {
                    Style = (Style)Resources["StarPathStyle"],
                    Fill = i <= filledCount ? FilledBrush : EmptyBrush
                };

                if (i <= filledCount)
                {
                    star.Effect = new DropShadowEffect
                    {
                        Color = Colors.Orange,
                        BlurRadius = 6,
                        ShadowDepth = 0,
                        Opacity = 0.5
                    };
                }

                StarsPanel.Children.Add(star);
            }
        }
    }
}

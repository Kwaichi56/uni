using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace SportNutritionShop.UserControls
{
    /// <summary>
    /// Пользовательский элемент управления: отображает рейтинг товара в виде звёзд.
    /// Использует графику (Path-геометрия звезды), стили (StarPathStyle), анимацию
    /// (увеличение звезды при наведении через EventTrigger) и визуальный эффект (DropShadowEffect).
    /// </summary>
    public partial class RatingStars : UserControl
    {
        public static readonly DependencyProperty RatingProperty =
            DependencyProperty.Register(
                nameof(Rating),
                typeof(double),
                typeof(RatingStars),
                new PropertyMetadata(0.0, OnRatingChanged));

        public double Rating
        {
            get => (double)GetValue(RatingProperty);
            set => SetValue(RatingProperty, value);
        }

        private static readonly Brush FilledBrush = new SolidColorBrush(Color.FromRgb(0xFF, 0xB8, 0x00));
        private static readonly Brush EmptyBrush = new SolidColorBrush(Color.FromRgb(0xD8, 0xE0, 0xEA));

        public RatingStars()
        {
            InitializeComponent();
            BuildStars();
        }

        private static void OnRatingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RatingStars control) control.BuildStars();
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
                    // Визуальный эффект для закрашенных звёзд
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

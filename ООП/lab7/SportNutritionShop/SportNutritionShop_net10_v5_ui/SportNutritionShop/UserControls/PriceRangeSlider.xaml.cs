using System.Windows;
using System.Windows.Controls;

namespace SportNutritionShop.UserControls
{
    public partial class PriceRangeSlider : UserControl
    {
        public static readonly DependencyProperty MinPriceProperty =
            DependencyProperty.Register(
                nameof(MinPrice),
                typeof(string),
                typeof(PriceRangeSlider),
                new FrameworkPropertyMetadata("0", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnRangeChanged, CoerceMinPrice),
                ValidateNumber);

        public static readonly DependencyProperty MaxPriceProperty =
            DependencyProperty.Register(
                nameof(MaxPrice),
                typeof(string),
                typeof(PriceRangeSlider),
                new FrameworkPropertyMetadata("200", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnRangeChanged, CoerceMaxPrice),
                ValidateNumber);

        public string MinPrice { get => (string)GetValue(MinPriceProperty); set => SetValue(MinPriceProperty, value); }
        public string MaxPrice { get => (string)GetValue(MaxPriceProperty); set => SetValue(MaxPriceProperty, value); }

        public static readonly RoutedEvent PreviewRangeChangedEvent =
            EventManager.RegisterRoutedEvent(
                "PreviewRangeChanged",
                RoutingStrategy.Tunnel,
                typeof(RoutedEventHandler),
                typeof(PriceRangeSlider));

        public static readonly RoutedEvent RangeChangedEvent =
            EventManager.RegisterRoutedEvent(
                "RangeChanged",
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(PriceRangeSlider));

        public event RoutedEventHandler PreviewRangeChanged
        {
            add => AddHandler(PreviewRangeChangedEvent, value);
            remove => RemoveHandler(PreviewRangeChangedEvent, value);
        }

        public event RoutedEventHandler RangeChanged
        {
            add => AddHandler(RangeChangedEvent, value);
            remove => RemoveHandler(RangeChangedEvent, value);
        }

        private bool _isSyncing;

        public PriceRangeSlider()
        {
            InitializeComponent();
            MinSlider.Value = 0;
            MaxSlider.Value = 200;
        }

        
        private static bool ValidateNumber(object value) =>
            value is string s && (string.IsNullOrEmpty(s) || double.TryParse(s, out _));

      
        private static object CoerceMinPrice(DependencyObject d, object baseValue)
        {
            if (d is PriceRangeSlider c &&
                double.TryParse((string)baseValue, out var min) &&
                double.TryParse(c.MaxPrice, out var max) && min > max)
                return c.MaxPrice;
            return baseValue;
        }

        
        private static object CoerceMaxPrice(DependencyObject d, object baseValue)
        {
            if (d is PriceRangeSlider c &&
                double.TryParse((string)baseValue, out var max) &&
                double.TryParse(c.MinPrice, out var min) && max < min)
                return c.MinPrice;
            return baseValue;
        }

        private static void OnRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PriceRangeSlider c)
            {
                // Синхронизируем ползунки со значениями DP (без зацикливания)
                c._isSyncing = true;
                if (double.TryParse(c.MinPrice, out var min)) c.MinSlider.Value = min;
                if (double.TryParse(c.MaxPrice, out var max)) c.MaxSlider.Value = max;
                c._isSyncing = false;

                // Сначала Tunnel (родитель получает событие первым, идёт сверху вниз)
                c.RaiseEvent(new RoutedEventArgs(PreviewRangeChangedEvent));
                // Затем Bubble (идёт снизу вверх от источника к родителю)
                c.RaiseEvent(new RoutedEventArgs(RangeChangedEvent));
            }
        }

        private void MinSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_isSyncing) return;
            MinPrice = e.NewValue.ToString("0");
        }

        private void MaxSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_isSyncing) return;
            MaxPrice = e.NewValue.ToString("0");
        }
    }
}

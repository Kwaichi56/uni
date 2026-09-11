using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using SportNutritionShop.Helpers;
using SportNutritionShop.Models;
using SportNutritionShop.Services;

namespace SportNutritionShop.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public ObservableCollection<Product> Products { get; set; }
        public ICollectionView ProductsView { get; set; }
        public ObservableCollection<CartItem> CartItems { get; set; } = new();
        public ObservableCollection<string> Categories { get; set; } = new();
        public ObservableCollection<string> Brands { get; set; } = new();
        private Product? _selectedProduct;
        private string _searchText = "";
        private string _selectedCategory = "All";
        private string _selectedBrand = "All";
        private string _selectedSort = "Name";
        private string _minPrice = "";
        private string _maxPrice = "";
        public UserRole CurrentRole { get; }
        public string CurrentUserName { get; }
        public bool IsAdmin => CurrentRole == UserRole.Admin;
        public decimal CartTotal => CartItems.Sum(x => x.Total);
        public Product? SelectedProduct { get => _selectedProduct; set { _selectedProduct = value; OnPropertyChanged(); } }
        public string SearchText { get => _searchText; set { _searchText = value; OnPropertyChanged(); ProductsView.Refresh(); } }
        public string SelectedCategory { get => _selectedCategory; set { _selectedCategory = value; OnPropertyChanged(); ProductsView.Refresh(); } }
        public string SelectedBrand { get => _selectedBrand; set { _selectedBrand = value; OnPropertyChanged(); ProductsView.Refresh(); } }
        public string SelectedSort { get => _selectedSort; set { _selectedSort = value; OnPropertyChanged(); ApplySorting(); } }
        public string MinPrice { get => _minPrice; set { _minPrice = value; OnPropertyChanged(); ProductsView.Refresh(); } }
        public string MaxPrice { get => _maxPrice; set { _maxPrice = value; OnPropertyChanged(); ProductsView.Refresh(); } }
        public ICommand AddToCartCommand { get; }
        public ICommand RemoveFromCartCommand { get; }
        public ICommand IncreaseCartCommand { get; }
        public ICommand DecreaseCartCommand { get; }
        public ICommand PlaceOrderCommand { get; }
        public ICommand AddProductCommand { get; }
        public ICommand DeleteProductCommand { get; }
        public ICommand SaveProductsCommand { get; }
        public ICommand ResetFiltersCommand { get; }
        public ICommand ChangeLanguageRuCommand { get; }
        public ICommand ChangeLanguageEnCommand { get; }
        public RelayCommand UndoCommand { get; }
        public RelayCommand RedoCommand { get; }
        private readonly UndoRedoManager _undoRedo = new();

        public MainViewModel(UserRole role, string userName)
        {
            CurrentRole = role;
            CurrentUserName = userName;
            Products = DataService.LoadProducts();
            ProductsView = CollectionViewSource.GetDefaultView(Products);
            ProductsView.Filter = FilterProducts;
            LoadFilterCollections();
            ApplySorting();
            SelectedProduct = Products.FirstOrDefault();
            AddToCartCommand = new RelayCommand(p => AddToCart(p as Product), p => p is Product product && !product.IsOutOfStock);
            RemoveFromCartCommand = new RelayCommand(p => RemoveFromCart(p as CartItem));
            IncreaseCartCommand = new RelayCommand(p => IncreaseCart(p as CartItem));
            DecreaseCartCommand = new RelayCommand(p => DecreaseCart(p as CartItem));
            PlaceOrderCommand = new RelayCommand(_ => PlaceOrder(), _ => CartItems.Any());
            AddProductCommand = new RelayCommand(_ => AddProduct(), _ => IsAdmin);
            DeleteProductCommand = new RelayCommand(_ => DeleteSelectedProduct(), _ => IsAdmin && SelectedProduct != null);
            SaveProductsCommand = new RelayCommand(_ => SaveProducts(), _ => IsAdmin);
            ResetFiltersCommand = new RelayCommand(_ => ResetFilters());
            ChangeLanguageRuCommand = new RelayCommand(_ => { LocalizationService.ChangeLanguage("ru-RU"); ResetFilters(); LoadFilterCollections(); });
            ChangeLanguageEnCommand = new RelayCommand(_ => { LocalizationService.ChangeLanguage("en-US"); ResetFilters(); LoadFilterCollections(); });
            UndoCommand = new RelayCommand(_ => _undoRedo.Undo(), _ => _undoRedo.CanUndo);
            RedoCommand = new RelayCommand(_ => _undoRedo.Redo(), _ => _undoRedo.CanRedo);
            _undoRedo.HistoryChanged += (_, _) =>
            {
                UndoCommand.RaiseCanExecuteChanged();
                RedoCommand.RaiseCanExecuteChanged();
            };
        }

        private string T(string key) => Application.Current.TryFindResource(key)?.ToString() ?? key;
        private void LoadFilterCollections()
        {
            Categories.Clear();
            Categories.Add(T("AllValue"));
            foreach (var category in Products.Select(p => p.Category).Distinct().OrderBy(x => x)) Categories.Add(category);
            Brands.Clear();
            Brands.Add(T("AllValue"));
            foreach (var brand in Products.Select(p => p.Brand).Distinct().OrderBy(x => x)) Brands.Add(brand);
            if (!Categories.Contains(SelectedCategory)) SelectedCategory = T("AllValue");
            if (!Brands.Contains(SelectedBrand)) SelectedBrand = T("AllValue");
            OnPropertyChanged(nameof(CartTotal));
        }
        private bool FilterProducts(object obj)
        {
            if (obj is not Product product) return false;
            bool matchesSearch = string.IsNullOrWhiteSpace(SearchText) || product.ShortName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) || product.FullName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) || product.Brand.Contains(SearchText, StringComparison.OrdinalIgnoreCase);
            bool matchesCategory = SelectedCategory == T("AllValue") || product.Category == SelectedCategory;
            bool matchesBrand = SelectedBrand == T("AllValue") || product.Brand == SelectedBrand;
            bool minOk = true; bool maxOk = true;
            if (decimal.TryParse(MinPrice, NumberStyles.Any, CultureInfo.InvariantCulture, out var min1) || decimal.TryParse(MinPrice, NumberStyles.Any, CultureInfo.CurrentCulture, out min1)) minOk = product.FinalPrice >= min1;
            if (decimal.TryParse(MaxPrice, NumberStyles.Any, CultureInfo.InvariantCulture, out var max1) || decimal.TryParse(MaxPrice, NumberStyles.Any, CultureInfo.CurrentCulture, out max1)) maxOk = product.FinalPrice <= max1;
            return matchesSearch && matchesCategory && matchesBrand && minOk && maxOk;
        }
        private void ApplySorting()
        {
            ProductsView.SortDescriptions.Clear();
            switch (SelectedSort)
            {
                case "PriceAsc": ProductsView.SortDescriptions.Add(new SortDescription(nameof(Product.FinalPrice), ListSortDirection.Ascending)); break;
                case "PriceDesc": ProductsView.SortDescriptions.Add(new SortDescription(nameof(Product.FinalPrice), ListSortDirection.Descending)); break;
                case "RatingDesc": ProductsView.SortDescriptions.Add(new SortDescription(nameof(Product.Rating), ListSortDirection.Descending)); break;
                default: ProductsView.SortDescriptions.Add(new SortDescription(nameof(Product.ShortName), ListSortDirection.Ascending)); break;
            }
        }
        private void AddToCart(Product? product)
        {
            if (product == null || product.Quantity <= 0) return;
            var existing = CartItems.FirstOrDefault(c => c.Product.Id == product.Id);
            bool wasNew = existing == null;
            _undoRedo.ExecuteAction(new UndoableAction(
                "AddToCart",
                doAction: () =>
                {
                    if (wasNew) CartItems.Add(new CartItem { Product = product, Quantity = 1 });
                    else
                    {
                        var item = CartItems.First(c => c.Product.Id == product.Id);
                        if (item.Quantity < product.Quantity) item.Quantity++;
                    }
                    OnPropertyChanged(nameof(CartTotal));
                },
                undoAction: () =>
                {
                    var item = CartItems.FirstOrDefault(c => c.Product.Id == product.Id);
                    if (item == null) return;
                    if (wasNew) CartItems.Remove(item);
                    else if (item.Quantity > 1) item.Quantity--;
                    OnPropertyChanged(nameof(CartTotal));
                }));
        }
        private void RemoveFromCart(CartItem? item)
        {
            if (item == null) return;
            var removedProduct = item.Product;
            var removedQuantity = item.Quantity;
            _undoRedo.ExecuteAction(new UndoableAction(
                "RemoveFromCart",
                doAction: () =>
                {
                    var current = CartItems.FirstOrDefault(c => c.Product.Id == removedProduct.Id);
                    if (current != null) CartItems.Remove(current);
                    OnPropertyChanged(nameof(CartTotal));
                },
                undoAction: () =>
                {
                    CartItems.Add(new CartItem { Product = removedProduct, Quantity = removedQuantity });
                    OnPropertyChanged(nameof(CartTotal));
                }));
        }
        private void IncreaseCart(CartItem? item)
        {
            if (item == null) return;
            if (item.Quantity < item.Product.Quantity) { item.Quantity++; OnPropertyChanged(nameof(CartTotal)); }
        }
        private void DecreaseCart(CartItem? item)
        {
            if (item == null) return;
            item.Quantity--;
            if (item.Quantity <= 0) CartItems.Remove(item);
            OnPropertyChanged(nameof(CartTotal));
        }
        private void PlaceOrder()
        {
            foreach (var cartItem in CartItems.ToList())
            {
                cartItem.Product.Quantity -= cartItem.Quantity;
                cartItem.Product.SoldCount += cartItem.Quantity;
            }
            CartItems.Clear();
            OnPropertyChanged(nameof(CartTotal));
            ProductsView.Refresh();
            DataService.SaveProducts(Products);
            MessageBox.Show(T("OrderSuccess"), T("InfoCaption"), MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void AddProduct()
        {
            int nextId = Products.Any() ? Products.Max(p => p.Id) + 1 : 1;
            var product = new Product { Id = nextId, ShortName = "New Product", FullName = "New Product Full Name", Description = "Description", Category = "Протеин", Brand = "My Brand", Country = "Беларусь", Flavor = "Neutral", Weight = "500 г", Price = 50, DiscountPercent = 0, Quantity = 5, Rating = 4.0, SoldCount = 0, ImagePath = "Assets/Products/creatine.jpg" };
            _undoRedo.ExecuteAction(new UndoableAction(
                "AddProduct",
                doAction: () =>
                {
                    Products.Add(product);
                    SelectedProduct = product;
                    LoadFilterCollections();
                    ProductsView.Refresh();
                },
                undoAction: () =>
                {
                    Products.Remove(product);
                    SelectedProduct = Products.FirstOrDefault();
                    LoadFilterCollections();
                    ProductsView.Refresh();
                }));
        }
        private void DeleteSelectedProduct()
        {
            if (SelectedProduct == null) return;
            var product = SelectedProduct;
            int index = Products.IndexOf(product);
            _undoRedo.ExecuteAction(new UndoableAction(
                "DeleteProduct",
                doAction: () =>
                {
                    Products.Remove(product);
                    SelectedProduct = Products.FirstOrDefault();
                    LoadFilterCollections();
                    ProductsView.Refresh();
                },
                undoAction: () =>
                {
                    int insertAt = System.Math.Min(index, Products.Count);
                    if (insertAt < 0) insertAt = Products.Count;
                    Products.Insert(insertAt, product);
                    SelectedProduct = product;
                    LoadFilterCollections();
                    ProductsView.Refresh();
                }));
        }
        private void SaveProducts()
        {
            DataService.SaveProducts(Products);
            MessageBox.Show(T("SaveSuccess"), T("InfoCaption"), MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void ResetFilters()
        {
            SearchText = "";
            SelectedCategory = T("AllValue");
            SelectedBrand = T("AllValue");
            SelectedSort = "Name";
            MinPrice = "";
            MaxPrice = "";
            ProductsView.Refresh();
        }
    }
}

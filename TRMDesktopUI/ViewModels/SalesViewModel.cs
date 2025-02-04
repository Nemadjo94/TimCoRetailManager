﻿using AutoMapper;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TRMDesktopUI.Library.Api;
using TRMDesktopUI.Library.Helpers;
using TRMDesktopUI.Library.Models;
using TRMDesktopUI.Models;

namespace TRMDesktopUI.ViewModels
{
    public class SalesViewModel : Screen
    {
        private BindingList<ProductDisplayModel> _products;
        private int _itemQuantity = 1;
        private BindingList<CartItemDisplayModel> _cart = new BindingList<CartItemDisplayModel>();
        private ProductDisplayModel _selectedProduct;
        private CartItemDisplayModel _selectedCartItem;
        private readonly StatusInfoViewModel _status;
        private readonly IWindowManager _window;
        private IProductEndpoint _productEndpoint;
        private IConfigHelper _configHelper;
        private ISaleEndpoint _saleEndpoint;
        private IMapper _mapper;

        public SalesViewModel(IProductEndpoint productEndpoint, IConfigHelper configHelper, ISaleEndpoint saleEndpoint, IMapper mapper, StatusInfoViewModel status, IWindowManager window)
        {
            _productEndpoint = productEndpoint;
            _configHelper = configHelper;
            _saleEndpoint = saleEndpoint;
            _mapper = mapper;
            _status = status;
            _window = window;
        }

        /// <summary>
        /// Fires an event after page is loaded and await loads a list of products
        /// </summary>
        /// <param name="view"></param>
        protected override async void OnViewLoaded(object view)
        { 
            base.OnViewLoaded(view);
            try
            {       
                await LoadProducts();
            }
            catch (Exception)
            {
                dynamic settings = new ExpandoObject();
                settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                settings.ResizeMode = ResizeMode.NoResize;
                settings.Title = "System Message";

                _status.UpdateMessage("Unauthorized Access", "You do not have permission to interact with the Sales Form");
                _window.ShowDialog(_status, null, settings);
                TryClose();
            }
        }

        /// <summary>
        /// Load products from api
        /// </summary>
        /// <returns></returns>
        private async Task LoadProducts()
        {
            var productsList = await _productEndpoint.GetAll();
            var products = _mapper.Map<List<ProductDisplayModel>>(productsList); // Map ProductDisplayModel <-> ProductModel
            Products = new BindingList<ProductDisplayModel>(products);
        }

        // Since binding list auto binds, notification should not be a problem
        public BindingList<ProductDisplayModel> Products
        {
            get { return _products; }
            set 
            {
                _products = value;
                NotifyOfPropertyChange(() => Products);
            }
        }

        public int ItemQuantity
        {
            get { return _itemQuantity; }
            set
            {
                _itemQuantity = value;
                NotifyOfPropertyChange(() => ItemQuantity);
                NotifyOfPropertyChange(() => CanAddToCart);
            }
        }

        public BindingList<CartItemDisplayModel> Cart
        {
            get { return _cart; }
            set
            {
                _cart = value;
                NotifyOfPropertyChange(() => Cart);
            }
        }

        public ProductDisplayModel SelectedProduct
        {
            get { return _selectedProduct; }
            set
            {
                _selectedProduct = value;
                NotifyOfPropertyChange(() => SelectedProduct);
                NotifyOfPropertyChange(() => CanAddToCart);
            }
        }

        public CartItemDisplayModel SelectedCartItem
        {
            get { return _selectedCartItem; }
            set
            {
                _selectedCartItem = value;
                NotifyOfPropertyChange(() => SelectedCartItem);
                NotifyOfPropertyChange(() => CanRemoveFromCart);
            }
        }

        public string SubTotal
        {
            
            get
            {
                return CalculateSubTotal().ToString("C"); 
            }
        }

        private decimal CalculateSubTotal()
        {
            decimal subTotal = 0;

            foreach (var item in Cart)
            {
                subTotal += item.Product.RetailPrice * item.QuantityInCart;
            }

            return subTotal;
        }

        public string Total
        {
            get { return (CalculateSubTotal() + CalculateTax()).ToString("C"); }
        }

        public string Tax
        {
            get
            {
                return CalculateTax().ToString("C");
            }
        }

        private decimal CalculateTax()
        {
            decimal taxAmount = 0;
            decimal taxRate = _configHelper.GetTaxRate() / 100; // Transform from percent to decimal

            taxAmount = Cart
                .Where(x => x.Product.IsTaxable) // Only taxable products
                .Sum(x => x.Product.RetailPrice * x.QuantityInCart * taxRate); 

            return taxAmount;
        }

        public bool CanAddToCart
        {
            get
            {
                bool output = false;

                // Make sure something is selected 
                // Make sure there is an item quantity
                if(ItemQuantity > 0 && SelectedProduct?.QuantityInStock >= ItemQuantity)
                {
                    output = true;
                }

                return output;
            }
        }

        public bool CanRemoveFromCart
        {
            get
            {
                bool output = false;

                // Make sure something is selected 
                // Make sure there is an item quantity
                if(SelectedCartItem != null && SelectedCartItem?.Product.QuantityInStock >= 0)
                {
                    output = true;
                }

                return output;
            }
        }

        public bool CanCheckOut
        {
            get
            {
                bool output = false;

                // Make sure there is something in the cart
                if(Cart.Count > 0)
                {
                    output = true;
                }

                return output;
            }
        }

        public void AddToCart()
        {
            CartItemDisplayModel existingItem = Cart.FirstOrDefault(x => x.Product == SelectedProduct);

            if(existingItem != null)
            {
                // Update an existing item quantity so we dont have duplicate products in cart
                existingItem.QuantityInCart += ItemQuantity;

                //// Triggers quantity refresh - could be done better
                //Cart.Remove(existingItem);
                //Cart.Add(existingItem);
            }
            else
            {
                CartItemDisplayModel item = new CartItemDisplayModel
                {
                    Product = SelectedProduct,
                    QuantityInCart = ItemQuantity
                };

                Cart.Add(item);
            }
            
            SelectedProduct.QuantityInStock -= ItemQuantity; // Subtrack from item quantity
            ItemQuantity = 1; // Reset item quantity back to 1
            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => CanCheckOut);

        }

        public void RemoveFromCart()
        {
            // BUG: when whole quantity is selected into cart, remove from cart button greys out
            SelectedCartItem.Product.QuantityInStock += 1;
            if (SelectedCartItem.QuantityInCart > 1)
            {
                SelectedCartItem.QuantityInCart -= 1;
            }
            else
            {           
                Cart.Remove(SelectedCartItem);
                NotifyOfPropertyChange(() => CanAddToCart);
            }

            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => CanCheckOut);
        }

        public async Task CheckOut()
        {
            // Create a SaleModel and post to the API
            SaleModel sale = new SaleModel();

            foreach(var item in Cart) // Adding the parallel foreach would speed up the process
            {
                sale.SaleDetails.Add(new SaleDetailModel
                {
                    ProductId = item.Product.Id,
                    Quantity = item.QuantityInCart
                });
            }
            // Add exception handlers
            // call api
            await _saleEndpoint.PostSale(sale);
            // reset the page so the products are updated after sale
            await ResetSalesViewModel();
        }

        /// <summary>
        /// After check out reset the whole view
        /// </summary>
        private async Task ResetSalesViewModel()
        {
            Cart = new BindingList<CartItemDisplayModel>(); // Reset the cart
            // TODO: Add clearing the selectedCartItem 
            await LoadProducts(); // Load products again 

            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => CanCheckOut);
        }
    }
}

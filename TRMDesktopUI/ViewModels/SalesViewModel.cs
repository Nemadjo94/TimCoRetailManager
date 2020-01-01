using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRMDesktopUI.Library.Api;
using TRMDesktopUI.Library.Models;

namespace TRMDesktopUI.ViewModels
{
    public class SalesViewModel : Screen
    {
        private BindingList<ProductModel> _products;
        private int _itemQuantity;
        private BindingList<ProductModel> _cart;
        private IProductEndpoint _productEndpoint;

        public SalesViewModel(IProductEndpoint productEndpoint)
        {
            _productEndpoint = productEndpoint;
        }

        /// <summary>
        /// Fires an event after page is loaded and await loads a list of products
        /// </summary>
        /// <param name="view"></param>
        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            await LoadProducts();
        }

        /// <summary>
        /// Load products from api
        /// </summary>
        /// <returns></returns>
        private async Task LoadProducts()
        {
            var productsList = await _productEndpoint.GetAll();
            Products = new BindingList<ProductModel>(productsList);
        }

        // Since binding list auto binds, notification should not be a problem
        public BindingList<ProductModel> Products
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
            }
        }

        public BindingList<ProductModel> Cart
        {
            get { return _cart; }
            set
            {
                _cart = value;
                NotifyOfPropertyChange(() => Cart);
            }
        }

        public string SubTotal
        {
            // TODO - Replace with calculation
            get { return "$0.00";  }
        }

        public string Total
        {
            // TODO - Replace with calculation
            get { return "$0.00"; }
        }

        public string Tax
        {
            // TODO - Replace with calculation
            get { return "$0.00"; }
        }

        public bool CanAddToCart
        {
            get
            {
                bool output = false;

                // Make sure something is selected 
                // Make sure there is an item quantity

                return output;
            }
        }

        public bool CanRemoveToCart
        {
            get
            {
                bool output = false;

                // Make sure something is selected 
                // Make sure there is an item quantity

                return output;
            }
        }

        public bool CanCheckOut
        {
            get
            {
                bool output = false;

                // Make sure there is something in the cart

                return output;
            }
        }

        public void AddToCart()
        {

        }

        public void RemoveFromCart()
        {

        }

        public void CheckOut()
        {

        }
    }
}

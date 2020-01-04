using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRMDesktopUI.Library.Api;
using TRMDesktopUI.Library.Helpers;
using TRMDesktopUI.Library.Models;

namespace TRMDesktopUI.ViewModels
{
    public class SalesViewModel : Screen
    {
        private BindingList<ProductModel> _products;
        private int _itemQuantity = 1;
        private BindingList<CartItemModel> _cart = new BindingList<CartItemModel>();
        private ProductModel _selectedProduct; 
        private IProductEndpoint _productEndpoint;
        private IConfigHelper _configHelper;
        private ISaleEndpoint _saleEndpoint;

        public SalesViewModel(IProductEndpoint productEndpoint, IConfigHelper configHelper, ISaleEndpoint saleEndpoint)
        {
            _productEndpoint = productEndpoint;
            _configHelper = configHelper;
            _saleEndpoint = saleEndpoint;
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
                NotifyOfPropertyChange(() => CanAddToCart);
            }
        }

        public BindingList<CartItemModel> Cart
        {
            get { return _cart; }
            set
            {
                _cart = value;
                NotifyOfPropertyChange(() => Cart);
            }
        }

        public ProductModel SelectedProduct
        {
            get { return _selectedProduct; }
            set
            {
                _selectedProduct = value;
                NotifyOfPropertyChange(() => SelectedProduct);
                NotifyOfPropertyChange(() => CanAddToCart);
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
                if(Cart.Count > 0)
                {
                    output = true;
                }

                return output;
            }
        }

        public void AddToCart()
        {
            CartItemModel existingItem = Cart.FirstOrDefault(x => x.Product == SelectedProduct);

            if(existingItem != null)
            {
                // Update an existing item quantity so we dont have duplicate products in cart
                existingItem.QuantityInCart += ItemQuantity;

                // Triggers quantity refresh - could be done better
                Cart.Remove(existingItem);
                Cart.Add(existingItem);
            }
            else
            {
                CartItemModel item = new CartItemModel
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

            // call api
            await _saleEndpoint.PostSale(sale);
            Console.WriteLine(sale);
        }
    }
}

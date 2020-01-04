using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRMDesktopUI.Library.Models;

namespace TRMDesktopUI.Models
{
    // This is all about display
    public class CartItemDisplayModel : INotifyPropertyChanged
    {
        public ProductDisplayModel Product { get; set; }

        private int _quantityInCart;
        public int QuantityInCart
        {
            get { return _quantityInCart; }
            set
            {
                _quantityInCart = value;
                CallPropertyChanged(nameof(QuantityInCart)); // Refresh quantity
                CallPropertyChanged(nameof(DisplayText)); // Refresh display text
            }
        }
        public string DisplayText
        {
            get
            {
                return $"{ Product.ProductName } ({ QuantityInCart })";
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        // Invoke property changed event to trigger refresh in UI
        public void CallPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

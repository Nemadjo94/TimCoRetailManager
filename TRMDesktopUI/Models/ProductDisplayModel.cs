using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRMDesktopUI.Models
{
    // This is all about display
    public class ProductDisplayModel : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public decimal RetailPrice { get; set; }

        private int _quantityInStock;
        public int QuantityInStock 
        {
            get { return _quantityInStock; }
            set
            {
                _quantityInStock = value;
                CallPropertyChanged(nameof(QuantityInStock)); // Refresh QuantityInStock in the UI
            }
        }
        public bool IsTaxable { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        // Invoke property changed event to trigger the refresh on UI
        public void CallPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

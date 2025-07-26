using System.ComponentModel;
using MauiApp1.Models;

namespace MauiApp1.Models
{
    public class DetailLivraisonViewModel : INotifyPropertyChanged
    {
        private Livraison _livraison;
        public Livraison Livraison
        {
            get => _livraison;
            set
            {
                if (_livraison != value)
                {
                    _livraison = value;
                    OnPropertyChanged(nameof(Livraison));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
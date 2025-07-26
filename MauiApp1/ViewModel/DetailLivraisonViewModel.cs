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
                    OnPropertyChanged(nameof(PeutLivrer));
                }
            }
        }


        public bool PeutLivrer
        {
            get => Livraison?.Statut == "EN_COURS";
            set
            {
                OnPropertyChanged(nameof(PeutLivrer));
            }
        }

        public bool PeutTerminer => Livraison?.Statut == "EN_COURS";

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
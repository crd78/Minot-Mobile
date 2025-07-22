using System;
using System.Windows.Input;
using Microsoft.Maui.Graphics;

namespace MauiApp1.Models
{
    public class Livraison
    {
        public int IdLivraison { get; set; }
        public int IdCommande { get; set; }
        public int IdTransport { get; set; }
        public string Statut { get; set; } = string.Empty;
        public int IdEntrepot { get; set; }
        public int IdVehicule { get; set; }
        public DateTime DatePrevue { get; set; }
        public DateTime? DateLivraison { get; set; }
        public string Commentaire { get; set; } = string.Empty;
        public DateTime DateCreation { get; set; }
        public DateTime DateMiseAJour { get; set; }

        // Propriétés pour l'affichage
        public string Initiales => "A"; // À adapter si tu as le nom de l'entreprise ou du client
        public string Numero => $"Livraison #{IdLivraison}";
        public string NomEntreprise { get; set; } = string.Empty;
        public string Entreprise => $"{NomEntreprise} | {DatePrevue:dd/MM/yyyy}";
        public string StatutText => Statut switch
        {
            "LIVREE" => "Livrée",
            "ANNULEE" => "Annulée",
            "EN_COURS" => "En cours",
            _ => Statut
        };
        public Color StatutColor => Statut switch
        {
            "LIVREE" => Colors.Green,
            "ANNULEE" => Colors.Brown,
            "EN_COURS" => Colors.Orange,
            _ => Colors.Gray
        };
    }
}
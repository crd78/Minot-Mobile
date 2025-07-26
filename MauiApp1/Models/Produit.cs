using System;
using System.Text.Json.Serialization;

namespace MauiApp1.Models
{
    public class ProduitDetail
    {
        [JsonPropertyName("IdProduit")]
        public int IdProduit { get; set; }

        [JsonPropertyName("NomProduit")]
        public string NomProduit { get; set; } = string.Empty;

        [JsonPropertyName("TypeProduit")]
        public string TypeProduit { get; set; } = string.Empty;

        [JsonPropertyName("PrixHT")]
        public string PrixHT { get; set; } // <-- string au lieu de decimal

        [JsonPropertyName("PrixTTC")]
        public string PrixTTC { get; set; } // <-- string au lieu de decimal

        [JsonPropertyName("IdMouvement")]
        public int IdMouvement { get; set; }

        [JsonPropertyName("DateCreation")]
        public DateTime DateCreation { get; set; }

        [JsonPropertyName("DateMiseAJour")]
        public DateTime DateMiseAJour { get; set; }
    }

    public class ProduitCommande
    {
        [JsonPropertyName("IdProduit")]
        public int IdProduit { get; set; }

        [JsonPropertyName("produit")]
        public ProduitDetail Produit { get; set; } = new();

        [JsonPropertyName("Quantite")]
        public int Quantite { get; set; }
    }
}
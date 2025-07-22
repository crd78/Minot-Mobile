using System.Collections.ObjectModel;
using System.Threading.Tasks;
using MinotMobile.Services;

namespace MauiApp1.Models
{
    public class ListLivraisonViewModel
    {
        public ObservableCollection<Livraison> Livraisons { get; set; } = new();

        public async Task LoadLivraisonsAsync(HttpClientService httpClient)
        {
            var result = await httpClient.GetAsync<List<Livraison>>("api/livraisons");
            Livraisons.Clear();
            if (result != null)
            {
                foreach (var l in result)
                    Livraisons.Add(l);
            }
        }
    }
}
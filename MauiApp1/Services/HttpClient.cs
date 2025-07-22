using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace MinotMobile.Services
{
    public class HttpClientService
    {
        private readonly HttpClient _client;

        // Ajout de la propriété statique pour stocker le dernier JSON reçu
        public static string? LastJsonRecu;

        public HttpClientService(string baseUrl)
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };
        }

        // GET : récupère et désérialise la réponse
        public async Task<T?> GetAsync<T>(string endpoint)
        {
            return await _client.GetFromJsonAsync<T>(endpoint);
        }

        // POST : envoie un objet et récupère la réponse désérialisée avec logs
        public async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
        {
            var jsonEnvoye = JsonSerializer.Serialize(data);
            Console.WriteLine($"[HTTP POST] JSON envoyé vers {endpoint} : {jsonEnvoye}");

            var response = await _client.PostAsJsonAsync(endpoint, data);

            var jsonRecu = await response.Content.ReadAsStringAsync();
            LastJsonRecu = jsonRecu; // Stocke le JSON reçu

            Console.WriteLine($"[HTTP POST] JSON reçu : {jsonRecu}");

            if (response.IsSuccessStatusCode)
            {
                return JsonSerializer.Deserialize<TResponse>(jsonRecu);
            }
            return default;
        }

        // POST sans attente de réponse
        public async Task<bool> PostAsync<TRequest>(string endpoint, TRequest data)
        {
            var response = await _client.PostAsJsonAsync(endpoint, data);
            return response.IsSuccessStatusCode;
        }
    }
}
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using good_tests.Infra;
using Newtonsoft.Json;

namespace good_tests.TestInfra.Storage
{
    public class InMemoryDocumentStore : IDocumentStore
    {
        // No, not going to implement a real database
        private Dictionary<string, string> _data = new Dictionary<string, string>();

        public async Task Save<T>(string key, T document, CancellationToken ct = default) where T:class
        {
            _data[key] = JsonConvert.SerializeObject(document);
        }

        public async Task<T?> Get<T>(string key, CancellationToken ct = default) where T : class
        {
            if (!_data.TryGetValue(key, out var document))
            {
                return null;
            }

            return JsonConvert.DeserializeObject<T>(document);
        }


    }
}
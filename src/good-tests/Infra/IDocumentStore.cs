using System.Threading;
using System.Threading.Tasks;

namespace good_tests.Infra
{
    public interface IDocumentStore
    {
        Task Save<T>(string key, T document, CancellationToken ct = default) where T:class;
        Task<T?> Get<T>(string key, CancellationToken ct = default) where T:class;
    }
}
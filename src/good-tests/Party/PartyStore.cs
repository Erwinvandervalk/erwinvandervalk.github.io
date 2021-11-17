using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using good_tests.Infra;
using Microsoft.Extensions.Logging;

namespace good_tests.Party
{
    public class PartyStore
    {
        private readonly IDocumentStore _documentStore;
        private readonly ILogger<PartyStore> _logger;

        public PartyStore(IDocumentStore documentStore, ILogger<PartyStore> logger)
        {
            _documentStore = documentStore;
            _logger = logger;
        }

        public async Task Save(Party party, CancellationToken ct = default)
        {
            var key = party.Id.ToString();

            var dso = new PartyDocument(
                Id: party.Id.Value,
                Name: party.Name.Value,
                Location: party.Location.Value.ToString(),
                Date: party.Date.Value,
                Attendees: party.Attendees
                    .Select(x => x.Name.ToString())
                    .ToArray());

            _logger.LogInformation("Saving party with id {id} under key {key}", party.Id, key);
            await _documentStore.Save(key, dso, ct);
        }

        public async Task<Party?> Get(PartyId id, CancellationToken ct = default)
        {
            var key = id.ToString();


            var dso = await _documentStore.Get<PartyDocument>(key, ct);

            if (dso == null)
            {
                return null;
            }

            return new Party(
                Id: new PartyId(dso.Id) ,
                Name: new PartyName(dso.Name),
                Location: new PartyLocation(dso.Location),
                Date: new PartyDate(dso.Date),
                Attendees: dso.Attendees
                    .Select(x => new Attendee(x))
                    .ToArray());
        }
    }
}
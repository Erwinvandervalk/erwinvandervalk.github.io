using good_tests.Party;
using good_tests.TestInfra.Logging;
using good_tests.TestInfra.Storage;
using Xunit.Abstractions;

namespace good_tests.TestInfra.Data
{
    public class PartyFixture
    {

        private readonly TestLoggerProvider LogProvider;

        public PartyFixture(ITestOutputHelper output)
        {
            LogProvider = new TestLoggerProvider(output);
        }

        public readonly InMemoryDocumentStore InMemoryDocumentStore =
            new InMemoryDocumentStore();

        public PartyStore PartyStore =>
            new PartyStore(documentStore: InMemoryDocumentStore,
                           logger: LogProvider.BuildLogger<PartyStore>());
    }
}

using Xunit;

namespace EventsManager.Application.Tests.Common;

/// <summary>
/// xUnit collection fixture definition for <see cref="InMemoryDbFixture"/>.
/// Use <c>[Collection("InMemoryDb")]</c> on test classes that need shared InMemory infrastructure,
/// or use <see cref="HandlerTestBase.CreateInMemoryDbContext"/> for per-test isolation (preferred).
/// </summary>
[CollectionDefinition("InMemoryDb")]
public class InMemoryDbCollection : ICollectionFixture<InMemoryDbFixture>
{
}

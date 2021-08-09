using Xunit;

//Optional
// [assembly: CollectionBehavior(DisableTestParallelization = true)]
//Optional
[assembly: TestCaseOrderer("Xunit.Extensions.Ordering.TestCaseOrderer", "Xunit.Extensions.Ordering")]
//Optional
[assembly: TestCollectionOrderer("Xunit.Extensions.Ordering.CollectionOrderer", "Xunit.Extensions.Ordering")]
// You can order test classes in collections
[assembly: TestFramework("Xunit.Extensions.Ordering.TestFramework", "Xunit.Extensions.Ordering")]
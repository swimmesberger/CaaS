using CaaS.Infrastructure.Base.Ado.Impl;
using CaaS.Infrastructure.Base.Ado.Model;
using CaaS.Infrastructure.Base.Ado.Model.Where;
using CaaS.Infrastructure.CartData;
using CaaS.Infrastructure.Gen;
using Xunit.Abstractions;

namespace CaaS.Test.AdoTests; 

public class StatementGeneratorTest {
    private static readonly Guid CustomerId = new Guid("AB5BEC83-2BB6-4F3C-860A-AEE56A0415AA");
    
    private readonly ITestOutputHelper _testOutputHelper;
    
    public StatementGeneratorTest(ITestOutputHelper testOutputHelper) {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void CreateFind() {
        var generator = new AdoStatementGenerator<CartDataModel>(new CartDataRecordMapper(), new AdoStatementMaterializer());
        var statement = generator.CreateFind(new StatementParameters() {
            Select = new[] { nameof(CartDataModel.Id), nameof(CartDataModel.CustomerId) },
            Where = new QueryParameter[] { 
                new(nameof(CartDataModel.CustomerId), CustomerId), 
                new(nameof(CartDataModel.LastAccess), WhereComparator.GreaterOrEqual, DateTimeOffset.Now)
            },
            OrderBy = new OrderParameter[] { new(nameof(CartDataModel.CreationTime)) },
            Limit = 1000
        });
        _testOutputHelper.WriteLine(statement.Materialize().ToString());
    }
}
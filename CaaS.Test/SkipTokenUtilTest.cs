using CaaS.Core.Base.Pagination;
using CaaS.Infrastructure.Base.Ado;
using CaaS.Infrastructure.Gen;
using CaaS.Infrastructure.ProductData;

namespace CaaS.Test; 

public class SkipTokenUtilTest {
    [Fact]
    public void ParseSkipTokenOptimistic() {
        var skipToken = SkipTokenUtil
            .Parse("Name:Coffee+Caramel+Biscotti|Id:4a72eb2a-18f1-48d4-9801-45f278acce68", new ProductDataRecordMapper().MetadataProvider);
        skipToken.PropertyValues[nameof(ProductDataModel.Name)].Should().BeEquivalentTo("Coffee Caramel Biscotti");
        skipToken.PropertyValues[nameof(ProductDataModel.Id)].Should().BeOfType<Guid>().And
            .BeEquivalentTo(Guid.Parse("4a72eb2a-18f1-48d4-9801-45f278acce68"));
    }
    
    [Fact]
    public void SerializeSkipTokenOptimistic() {
        var skipToken = new SkipTokenValue() {
            PropertyValues = new Dictionary<string, object?>() {
                [nameof(ProductDataModel.Name)] = "Coffee Caramel Biscotti",
                [nameof(ProductDataModel.Id)] = Guid.Parse("4a72eb2a-18f1-48d4-9801-45f278acce68"),
            }
        };
        skipToken.ToString().Should().BeEquivalentTo("Name:Coffee+Caramel+Biscotti|Id:4a72eb2a-18f1-48d4-9801-45f278acce68");
    }
    
        [Fact]
        public void ParseSkipTokenWithCommaOptimistic() {
            var skipToken = SkipTokenUtil
                .Parse("Name:Flour+-+Corn,+Fine|Id:4a72eb2a-18f1-48d4-9801-45f278acce68", new ProductDataRecordMapper().MetadataProvider);
            skipToken.PropertyValues[nameof(ProductDataModel.Name)].Should().BeEquivalentTo("Flour - Corn, Fine");
            skipToken.PropertyValues[nameof(ProductDataModel.Id)].Should().BeOfType<Guid>().And
                .BeEquivalentTo(Guid.Parse("4a72eb2a-18f1-48d4-9801-45f278acce68"));
        }
}
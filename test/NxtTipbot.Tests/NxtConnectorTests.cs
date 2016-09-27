using Moq;
using NxtLib;
using NxtLib.Accounts;
using NxtLib.AssetExchange;
using NxtLib.MonetarySystem;
using Xunit;

namespace NxtTipbot.Tests
{
    public class NxtConnectorTests
    {
        private readonly Mock<IServiceFactory> serviceFactoryMock = new Mock<IServiceFactory>();
        private readonly Mock<IAccountService> accountServiceMock = new Mock<IAccountService>();
        private readonly Mock<IMonetarySystemService> monetarySystemServiceMock = new Mock<IMonetarySystemService>();
        private readonly Mock<IAssetExchangeService> assetExchangeServiceMock = new Mock<IAssetExchangeService>();

        private readonly NxtConnector nxtConnector;

        public NxtConnectorTests()
        {
            serviceFactoryMock.Setup(f => f.CreateAccountService()).Returns(accountServiceMock.Object);
            serviceFactoryMock.Setup(f => f.CreateMonetarySystemService()).Returns(monetarySystemServiceMock.Object);
            serviceFactoryMock.Setup(f => f.CreateAssetExchangeService()).Returns(assetExchangeServiceMock.Object);

            nxtConnector = new NxtConnector(serviceFactoryMock.Object);
        }

        [Fact]
        public async void TransferAssetShouldTransferCorrectAmount()
        {
            const decimal amount = 1.123M;
            var expectedQuantity = (long)(amount * 10000);
            assetExchangeServiceMock.Setup(s => s.TransferAsset(It.IsAny<Account>(), It.IsAny<ulong>(), It.IsAny<long>(), It.IsAny<CreateTransactionParameters>()))
                    .ReturnsAsync(new NxtLib.TransactionCreatedReply { TransactionId = 123 });

            await nxtConnector.Transfer(TestConstants.SenderAccount, TestConstants.RecipientAccount.NxtAccountRs, TestConstants.Asset, amount, "TEST");

            assetExchangeServiceMock.Verify(s => s.TransferAsset(
                It.IsAny<Account>(), 
                It.IsAny<ulong>(), 
                It.Is<long>(quantityQnt => quantityQnt == expectedQuantity),
                It.IsAny<CreateTransactionParameters>()));
        }
    }
}
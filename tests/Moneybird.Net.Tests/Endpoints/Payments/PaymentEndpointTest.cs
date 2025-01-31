using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Moneybird.Net.Endpoints.Payments;
using Moneybird.Net.Entities.Payments;
using Moneybird.Net.Http;
using Moq;
using Xunit;

namespace Moneybird.Net.Tests.Endpoints.Payments;

public class PaymentEndpointTest : PaymentTestBase
{
    private static Mock<IRequester> _requester;
    private readonly MoneybirdConfig _config;
    private readonly PaymentEndpoint _paymentEndpoint;
    
    private const string GetPaymentResponsePath = "./Responses/Endpoints/Payments/getPayment.json";
    
    public PaymentEndpointTest()
    {
        _requester = new Mock<IRequester>();
        _config = new MoneybirdConfig();
        _paymentEndpoint = new PaymentEndpoint(_config, _requester.Object);
    }
    
    [Fact]
    public async Task GetPaymentByIdAsync_ByAccessToken_Returns_Payment()
    {
        var paymentJson = await File.ReadAllTextAsync(GetPaymentResponsePath);
        _requester.Setup(moq => moq.CreateGetRequestAsync(It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<string>(), It.IsAny<List<string>>())).ReturnsAsync(paymentJson);
        
        var payment = JsonSerializer.Deserialize<Payment>(paymentJson, _config.SerializerOptions);
        Assert.NotNull(payment);
        
        var actualPayment = await _paymentEndpoint.GetPaymentByIdAsync(AdministrationId, PaymentId, AccessToken);
        Assert.NotNull(actualPayment);
        
        payment.Should().BeEquivalentTo(actualPayment);
    }
}
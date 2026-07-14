using FluentAssertions;
using MediatR;
using MicroDemo.Api.Controllers;
using MicroDemo.Application.Commands.Orders;
using MicroDemo.Application.Common.Models;
using MicroDemo.Application.Dtos;
using MicroDemo.Application.Queries.Orders;
using MicroDemo.UnitTests.Commands;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace MicroDemo.UnitTests.Controllers;

public class OrdersControllerTests
{
    private static (OrdersController controller, Mock<ISender> mediator) CreateController()
    {
        var mediator = new Mock<ISender>();
        var services = new Mock<IServiceProvider>();
        services.Setup(s => s.GetService(typeof(ISender))).Returns(mediator.Object);
        var httpContext = new DefaultHttpContext { RequestServices = services.Object };
        var controller = new OrdersController
        {
            ControllerContext = new ControllerContext { HttpContext = httpContext }
        };
        return (controller, mediator);
    }

    [Fact]
    public async Task GetById_returns_ok_with_the_order()
    {
        var (controller, mediator) = CreateController();
        var dto = new OrderDto { Id = Guid.NewGuid(), UserId = Guid.NewGuid() };
        mediator.Setup(m => m.Send(It.IsAny<GetOrderByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<OrderDto>.Success(dto));

        var response = await controller.GetById(dto.Id);

        response.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeSameAs(dto);
    }

    [Fact]
    public async Task GetById_returns_notfound_when_result_is_notfound()
    {
        var (controller, mediator) = CreateController();
        mediator.Setup(m => m.Send(It.IsAny<GetOrderByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<OrderDto>.NotFound("not found"));

        var response = await controller.GetById(Guid.NewGuid());

        response.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Create_returns_created_on_success()
    {
        var (controller, mediator) = CreateController();
        var id = Guid.NewGuid();
        mediator.Setup(m => m.Send(It.IsAny<CreateOrderCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Guid>.Success(id));

        var response = await controller.Create(CreateOrderCommandData.Valid());

        response.Should().BeOfType<CreatedAtActionResult>().Which.Value.Should().Be(id);
    }

    [Fact]
    public async Task Create_returns_notfound_when_result_is_notfound()
    {
        var (controller, mediator) = CreateController();
        mediator.Setup(m => m.Send(It.IsAny<CreateOrderCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Guid>.NotFound("user not found"));

        var response = await controller.Create(CreateOrderCommandData.Valid());

        response.Should().BeOfType<NotFoundObjectResult>();
    }
}

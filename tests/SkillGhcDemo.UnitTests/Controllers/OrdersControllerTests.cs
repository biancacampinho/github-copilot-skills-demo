using FluentAssertions;
using MediatR;
using SkillGhcDemo.Api.Controllers;
using SkillGhcDemo.Application.Commands.Orders;
using SkillGhcDemo.Application.Common.Models;
using SkillGhcDemo.Application.Dtos;
using SkillGhcDemo.Application.Queries.Orders;
using SkillGhcDemo.UnitTests.Commands;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace SkillGhcDemo.UnitTests.Controllers;

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
    public async Task GetByProductId_returns_ok_with_the_lines()
    {
        var (controller, mediator) = CreateController();
        var productId = Guid.NewGuid();
        var lines = new List<OrderByProductDto> { new() { OrderId = Guid.NewGuid() } };
        mediator.Setup(m => m.Send(It.IsAny<GetOrdersByProductIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IReadOnlyList<OrderByProductDto>>.Success(lines));

        var response = await controller.GetByProductId(productId);

        response.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeSameAs(lines);
    }

    [Fact]
    public async Task GetByProductId_returns_notfound_when_product_does_not_exist()
    {
        var (controller, mediator) = CreateController();
        mediator.Setup(m => m.Send(It.IsAny<GetOrdersByProductIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IReadOnlyList<OrderByProductDto>>.NotFound("not found"));

        var response = await controller.GetByProductId(Guid.NewGuid());

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

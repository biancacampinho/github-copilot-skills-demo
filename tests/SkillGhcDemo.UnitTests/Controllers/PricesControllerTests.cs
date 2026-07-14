using FluentAssertions;
using MediatR;
using SkillGhcDemo.Api.Controllers;
using SkillGhcDemo.Application.Commands.Prices;
using SkillGhcDemo.Application.Common.Models;
using SkillGhcDemo.Application.Dtos;
using SkillGhcDemo.Application.Queries.Prices;
using SkillGhcDemo.UnitTests.Commands;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace SkillGhcDemo.UnitTests.Controllers;

public class PricesControllerTests
{
    private static (PricesController controller, Mock<ISender> mediator) CreateController()
    {
        var mediator = new Mock<ISender>();
        var services = new Mock<IServiceProvider>();
        services.Setup(s => s.GetService(typeof(ISender))).Returns(mediator.Object);
        var httpContext = new DefaultHttpContext { RequestServices = services.Object };
        var controller = new PricesController
        {
            ControllerContext = new ControllerContext { HttpContext = httpContext }
        };
        return (controller, mediator);
    }

    [Fact]
    public async Task GetAll_returns_ok_with_the_list()
    {
        var (controller, mediator) = CreateController();
        IReadOnlyList<PriceDto> data = new List<PriceDto> { new() { Currency = "EUR", Amount = 29.90m } };
        mediator.Setup(m => m.Send(It.IsAny<GetPricesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IReadOnlyList<PriceDto>>.Success(data));

        var response = await controller.GetAll();

        response.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeSameAs(data);
    }

    [Fact]
    public async Task GetById_returns_notfound_when_result_is_notfound()
    {
        var (controller, mediator) = CreateController();
        mediator.Setup(m => m.Send(It.IsAny<GetPriceByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<PriceDto>.NotFound("not found"));

        var response = await controller.GetById(Guid.NewGuid());

        response.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Create_returns_created_on_success()
    {
        var (controller, mediator) = CreateController();
        var id = Guid.NewGuid();
        mediator.Setup(m => m.Send(It.IsAny<CreatePriceCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Guid>.Success(id));

        var response = await controller.Create(CreatePriceCommandData.Valid());

        response.Should().BeOfType<CreatedAtActionResult>().Which.Value.Should().Be(id);
    }

    [Fact]
    public async Task Create_returns_notfound_when_result_is_notfound()
    {
        var (controller, mediator) = CreateController();
        mediator.Setup(m => m.Send(It.IsAny<CreatePriceCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Guid>.NotFound("product not found"));

        var response = await controller.Create(CreatePriceCommandData.Valid());

        response.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Update_returns_badrequest_when_route_id_differs_from_body()
    {
        var (controller, _) = CreateController();
        var command = UpdatePriceCommandData.Valid(Guid.NewGuid());

        var response = await controller.Update(Guid.NewGuid(), command);

        response.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Update_returns_nocontent_on_success()
    {
        var (controller, mediator) = CreateController();
        var id = Guid.NewGuid();
        mediator.Setup(m => m.Send(It.IsAny<UpdatePriceCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        var response = await controller.Update(id, UpdatePriceCommandData.Valid(id));

        response.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Delete_returns_nocontent_on_success()
    {
        var (controller, mediator) = CreateController();
        mediator.Setup(m => m.Send(It.IsAny<DeletePriceCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        var response = await controller.Delete(Guid.NewGuid());

        response.Should().BeOfType<NoContentResult>();
    }
}

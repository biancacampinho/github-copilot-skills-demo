using FluentAssertions;
using MediatR;
using MicroDemo.Api.Controllers;
using MicroDemo.Application.Commands.Users;
using MicroDemo.Application.Common.Models;
using MicroDemo.Application.Dtos;
using MicroDemo.Application.Queries.Users;
using MicroDemo.UnitTests.Commands;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace MicroDemo.UnitTests.Controllers;

public class UsersControllerTests
{
    private static (UsersController controller, Mock<ISender> mediator) CreateController()
    {
        var mediator = new Mock<ISender>();
        var services = new Mock<IServiceProvider>();
        services.Setup(s => s.GetService(typeof(ISender))).Returns(mediator.Object);
        var httpContext = new DefaultHttpContext { RequestServices = services.Object };
        var controller = new UsersController
        {
            ControllerContext = new ControllerContext { HttpContext = httpContext }
        };
        return (controller, mediator);
    }

    [Fact]
    public async Task GetAll_returns_ok_with_the_list()
    {
        var (controller, mediator) = CreateController();
        IReadOnlyList<UserDto> data = new List<UserDto> { new() { FullName = "John Smith" } };
        mediator.Setup(m => m.Send(It.IsAny<GetUsersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IReadOnlyList<UserDto>>.Success(data));

        var response = await controller.GetAll();

        response.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeSameAs(data);
    }

    [Fact]
    public async Task GetById_returns_notfound_when_result_is_notfound()
    {
        var (controller, mediator) = CreateController();
        mediator.Setup(m => m.Send(It.IsAny<GetUserByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<UserDto>.NotFound("not found"));

        var response = await controller.GetById(Guid.NewGuid());

        response.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Create_returns_created_on_success()
    {
        var (controller, mediator) = CreateController();
        var id = Guid.NewGuid();
        mediator.Setup(m => m.Send(It.IsAny<CreateUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Guid>.Success(id));

        var response = await controller.Create(CreateUserCommandData.Valid());

        response.Should().BeOfType<CreatedAtActionResult>().Which.Value.Should().Be(id);
    }

    [Fact]
    public async Task Create_returns_conflict_when_result_is_conflict()
    {
        var (controller, mediator) = CreateController();
        mediator.Setup(m => m.Send(It.IsAny<CreateUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Guid>.Failure("duplicate", ResultErrorType.Conflict));

        var response = await controller.Create(CreateUserCommandData.Valid());

        response.Should().BeOfType<ConflictObjectResult>();
    }

    [Fact]
    public async Task Update_returns_badrequest_when_route_id_differs_from_body()
    {
        var (controller, _) = CreateController();
        var command = UpdateUserCommandData.Valid(Guid.NewGuid());

        var response = await controller.Update(Guid.NewGuid(), command);

        response.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Update_returns_nocontent_on_success()
    {
        var (controller, mediator) = CreateController();
        var id = Guid.NewGuid();
        mediator.Setup(m => m.Send(It.IsAny<UpdateUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        var response = await controller.Update(id, UpdateUserCommandData.Valid(id));

        response.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Delete_returns_nocontent_on_success()
    {
        var (controller, mediator) = CreateController();
        mediator.Setup(m => m.Send(It.IsAny<DeleteUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        var response = await controller.Delete(Guid.NewGuid());

        response.Should().BeOfType<NoContentResult>();
    }
}

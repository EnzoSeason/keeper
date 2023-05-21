using MediatR;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NUnit.Framework;
using Reporting.API.Commands.CreateStatement;
using Reporting.API.Controllers;

namespace Reporting.UTest.Controllers;

public class CreateStatementEndpointTests
{
    private IMediator _mediator;
    private ReportsController _controller;
    
    [SetUp]
    public void SetUp()
    {
        _mediator = Substitute.For<IMediator>();
        _controller = new ReportsController(_mediator);
    }
    
    [Test]
    public async Task NoContent_Success()
    {
        var command = new CreateStatementCommand
        {
            ConfigId = Guid.NewGuid(),
            Year = 2023,
            Month = 3
        };
        _mediator.Send(command).Returns(Task.FromResult(true));
        
        var response = await _controller.Aggregate(command);
        
        Assert.That(response, Is.Not.Null);
        Assert.That(response, Is.InstanceOf<NoContentResult>());
    }
    
    [Test]
    public async Task UnprocessableEntity_UploadCommandFailed()
    {
        var command = new CreateStatementCommand
        {
            ConfigId = Guid.NewGuid(),
            Year = 2023,
            Month = 3
        };
        _mediator.Send(command).Returns(Task.FromResult(false));
        
        var response = await _controller.Aggregate(command);
        
        Assert.That(response, Is.Not.Null);
        Assert.That(response, Is.InstanceOf<UnprocessableEntityObjectResult>());
    }

}
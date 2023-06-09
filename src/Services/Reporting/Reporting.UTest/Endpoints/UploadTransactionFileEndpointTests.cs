using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NUnit.Framework;
using Reporting.API.Commands.UploadTransactionFile;
using Reporting.API.Controllers;

namespace Reporting.UTest.Endpoints;

public class UploadTransactionFileEndpointTests
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
        var file = new FormFile(Stream.Null, 0, 1, "File", "file.csv");
        var command = new UploadTransactionFileCommand
        {
            ConfigId = Guid.NewGuid(),
            Year = 2023,
            Month = 3,
            File = file
        };
        _mediator.Send(command).Returns(Task.FromResult(true));
        
        var response = await _controller.Upload(command);
        
        Assert.That(response, Is.Not.Null);
        Assert.That(response, Is.InstanceOf<NoContentResult>());
    }
    
    [Test]
    public async Task UnprocessableEntity_UploadCommandFailed()
    {
        var file = new FormFile(Stream.Null, 0, 1, "File", "file.csv");
        var command = new UploadTransactionFileCommand
        {
            ConfigId = Guid.NewGuid(),
            Year = 2023,
            Month = 3,
            File = file
        };
        _mediator.Send(command).Returns(Task.FromResult(false));
        
        var response = await _controller.Upload(command);
        
        Assert.That(response, Is.Not.Null);
        Assert.That(response, Is.InstanceOf<UnprocessableEntityObjectResult>());
    }
        
    [Test]
    public async Task BadRequest_FileIsEmpty()
    {
        var emptyFile = new FormFile(Stream.Null, 0, 0, "EmptyFile", "empty.txt");
        var command = new UploadTransactionFileCommand
        {
            ConfigId = Guid.NewGuid(),
            Year = 2023,
            Month = 3,
            File = emptyFile
        };
        
        var response = await _controller.Upload(command);
        
        Assert.That(response, Is.Not.Null);
        Assert.That(response, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task BadRequest_FileIsNotInCSV()
    {
        var file = new FormFile(Stream.Null, 0, 1, "File", "file.txt");
        var command = new UploadTransactionFileCommand
        {
            ConfigId = Guid.NewGuid(),
            Year = 2023,
            Month = 3,
            File = file
        };
        
        var response = await _controller.Upload(command);
        
        Assert.That(response, Is.Not.Null);
        Assert.That(response, Is.InstanceOf<BadRequestObjectResult>());
    }
}
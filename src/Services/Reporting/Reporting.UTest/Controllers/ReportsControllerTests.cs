using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using Reporting.API.Commands;
using Reporting.API.Controllers;

namespace Reporting.UTest.Controllers;

public class ReportsControllerTests
{
    private ReportsController _controller;

    [SetUp]
    public void SetUp()
    {
        _controller = new ReportsController();
    }
        
    [Test]
    public void BadRequest_FileIsEmpty()
    {
        var emptyFile = new FormFile(Stream.Null, 0, 0, "EmptyFile", "empty.txt");
        var command = new UploadTransactionFileCommand
        {
            ConfigId = Guid.NewGuid(),
            FileDate = DateTime.UtcNow,
            File = emptyFile
        };
        var response = _controller.Upload(command);
        
        Assert.That(response, Is.Not.Null);
        Assert.That(response, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public void BadRequest_FileIsNotInCSV()
    {
        var file = new FormFile(Stream.Null, 0, 1, "File", "file.txt");
        var command = new UploadTransactionFileCommand
        {
            ConfigId = Guid.NewGuid(),
            FileDate = DateTime.UtcNow,
            File = file
        };
        var response = _controller.Upload(command);
        
        Assert.That(response, Is.Not.Null);
        Assert.That(response, Is.InstanceOf<BadRequestObjectResult>());
    }
}
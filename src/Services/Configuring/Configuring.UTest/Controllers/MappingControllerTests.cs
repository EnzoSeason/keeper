using Configuring.API.Controllers;
using Configuring.Domain.SourceAggregation;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NUnit.Framework;

namespace Configuring.UTest.Controllers;

public class MappingControllerTests
{
    private const string ConfigIdStr = "3de74b1c-36db-4b19-9694-e6a213252982";
    
    private ISourceRepository _sourceRepository;
    private MappingController _controller;

    [SetUp]
    public void SetUp()
    {
        _sourceRepository = Substitute.For<ISourceRepository>();
        _controller = new MappingController(_sourceRepository);
    }
    
    [Test]
    public async Task Create_Success()
    {
        var source = new Source();
        
        var response = await _controller.Create(source);
        
        Assert.That(response, Is.Not.Null);
        Assert.That(response, Is.InstanceOf<CreatedAtActionResult>());
    }

    [Test]
    public async Task Create_IsFound_Fail()
    {
        var source = new Source();
        _sourceRepository.IsFound(source.ConfigId).Returns(true);
        
        var response = await _controller.Create(source);
        
        Assert.That(response, Is.Not.Null);
        Assert.That(response, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task GetOne_Success()
    {
        var expectedConfigId = Guid.Parse(ConfigIdStr);
        _sourceRepository.Get(expectedConfigId).Returns(Task.FromResult(new Source { ConfigId = expectedConfigId }));

        var response = await _controller.Get(expectedConfigId);
        
        Assert.That(response, Is.Not.Null);
        Assert.That(response, Is.InstanceOf<ActionResult<Source>>());
        Assert.That(response.Value, Is.Not.Null);
        Assert.That(response.Value!.ConfigId, Is.EqualTo(expectedConfigId));
    }

    [Test]
    public async Task GetOne_NotFound_Fail()
    {
        _sourceRepository.Get(Arg.Any<Guid>()).Returns(Task.FromResult<Source>(null!));

        var response = await _controller.Get(Guid.NewGuid());
        
        Assert.That(response, Is.Not.Null);
        Assert.That(response, Is.InstanceOf<ActionResult<Source>>());
        Assert.That(response.Result, Is.InstanceOf<NotFoundResult>());
    }

    [Theory]
    public async Task UpdateOne(bool isReplaced)
    {
        var configId = Guid.Parse(ConfigIdStr);
        _sourceRepository.ReplaceOne(configId, Arg.Any<Source>()).Returns(Task.FromResult(isReplaced));

        var response = await _controller.Update(configId, new Source());
        
        Assert.That(response, Is.Not.Null);
        Assert.That(response, isReplaced ? Is.InstanceOf<NoContentResult>() : Is.InstanceOf<NotFoundResult>());
    }
}
using Configuring.API.Controllers;
using Configuring.Domain.SourceAggregation;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NUnit.Framework;

namespace Configuring.UTest.Controllers;

public class MappingControllerTests
{
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
}
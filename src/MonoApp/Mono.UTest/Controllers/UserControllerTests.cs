using Microsoft.AspNetCore.Mvc;
using Mono.API.Controllers;
using Mono.API.Models;
using Mono.API.Services;
using NSubstitute;
using NUnit.Framework;

namespace Mono.UTest.Controllers;

public class UserControllerTests
{
    private const string ExistedUidStr = "460aee14-d137-46d2-8df6-e1348297014f";
    private const string NotExistedUidStr = "f81d0fb3-9757-493a-a3c4-2de90f41f62d";
    
    private IUsersService _usersService;
    private UserController _userController;
    
    [SetUp]
    public void Setup()
    {
        _usersService = Substitute.For<IUsersService>();
        _userController = new UserController(_usersService);
    }

    [Test]
    public async Task Get_users_success()
    {
        var expectedUsers = new List<User>
        {
            new(Guid.NewGuid(), "toto"),
            new(Guid.NewGuid(), "foo")
        };

        _usersService.GetUsers().Returns(Task.FromResult((IEnumerable<User>)expectedUsers));

        var result = await _userController.GetUsers();
        
        Assert.That(result.Value.SequenceEqual(expectedUsers));
    }
    
    [TestCaseSource(nameof(GetUserTestCases))]
    public async Task Get_user_by_uid_success(Guid uid, User expectedUser)
    {
        var existedUid = Guid.Parse(ExistedUidStr);

        _usersService
            .GetUser(Arg.Is(existedUid))
            .Returns(Task.FromResult(expectedUser));

        _usersService
            .GetUser(Arg.Is<Guid>(uid => uid != existedUid))
            .Returns(Task.FromResult<User>(null));
        
        var result = await _userController.GetUser(uid);
        
        Assert.That(result.Value, Is.EqualTo(expectedUser));
    }
    
    [Test]
    public async Task Create_user_success()
    {
        const string username = "test";
        _usersService.CreateUser(Arg.Any<User>()).Returns(Task.CompletedTask);

        var result = await _userController.CreateUser(username);

        Assert.That((result.Result as CreatedAtActionResult).Value, Is.Not.Null);
    }

    private static IEnumerable<TestCaseData> GetUserTestCases()
    {
        yield return new TestCaseData(
            Guid.Parse(ExistedUidStr),
            new User(Guid.NewGuid(), "toto")
        ).SetName("User exists");

        yield return new TestCaseData(
            Guid.Parse(NotExistedUidStr),
            null
        ).SetName("User doesn't exist");
    }
}
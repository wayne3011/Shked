using Moq;
using Shked.UserDAL.Infrastructure;
using Sked_Authorization.Tests.Base;
using Sked_Authorization.Tests.Test.Data;
using ShkedAuthorization.Application.Data.DTO;
using ShkedAuthorization.Application.Data.Responses;
using ShkedAuthorization.Application.Infrastructure;
using ShkedAuthorization.Application.Services;
using ShkedAuthorization.Application.Services.Validators;

namespace Sked_Authorization.Tests;

public class AuthServiceTests : AuthServiceTestBase
{
    private IUserRepository _userRepository;
    public AuthServiceTests()
    {
        _userRepository = GetUserRepo();
    }
    readonly AuthDTO? StandartAuthDto = new AuthDTO()
    {
        Id = Guid.NewGuid().ToString(),
        RefreshToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9" +
                       ".eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiMTg5NTJlMTgtNjI2NC00OWE5LTlhYjAtN2U3NmYxNTc0NzBjIiwiZXhwIjoxNzAxODgwMTIyLCJpc3MiOiJTa2VkLkF1dGhTZXJ2aWNlIiwiYXVkIjoiU2tlZC5Vc2VycyJ9" +
                       ".cbt5QfEZXrt1CBD-aFwh-cIiPts74R5a_km_UAJWG4M",
        AccessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9" +
                      ".eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiMTg5NTJlMTgtNjI2NC00OWE5LTlhYjAtN2U3NmYxNTc0NzBjIiwiZXhwIjoxNjk0MTA1OTIyLCJpc3MiOiJTa2VkLkF1dGhTZXJ2aWNlIiwiYXVkIjoiU2tlZC5Vc2VycyJ9" +
                      ".kc33U8uLp5dQwmQDBBTidTvWzer1N_SD141g1m1lXv8"
    };   
    [Fact]
    public async void SignUpTest_success()
    {
        //Arrange
        SignUpDTO newUser = new SignUpDTO()
        {
            Email = "test@mail.ru",
            FullName = "Ivan Ivanov",
            Password = "$2a$16$ZGBbp6Ed4YH9Z2h8ELiR0.HeoI.ExdIANlsr0S1erMhd2.kVGWz06",
            Group = "М3О-100Бк-23"
        };
        
        var signUpValidator = new SignUpDTOValidator(_userRepository);
        
        var tokenManagerMock = new Mock<ITokenManager>();
        tokenManagerMock.Setup(manager => manager.IssueToken(It.IsAny<string>())).Returns(() => StandartAuthDto);
        
        var authService = new AuthService(_userRepository, GetMapper(), signUpValidator, null, tokenManagerMock.Object);
        //Act
        var result = await authService.SignUpAsync(newUser);
        //Assert
        Assert.Equal(StandartAuthDto,result.Value);
        Assert.Equal(null,result.ValidateErrors);
    }
    
    [Theory]
    [ClassData(typeof(EmptyPropSignUpData))]
    void SignUp_Should_ReturnError_IfPropNullOrEmpty(SignUpDTO signUpDto)
    {
        //Arrange
        var signUpValidator = new SignUpDTOValidator(_userRepository);
        var tokenManagerMock = new Mock<ITokenManager>();
        tokenManagerMock.Setup(manager => manager.IssueToken(It.IsAny<string>())).Returns(() => StandartAuthDto);
        var authService = new AuthService(_userRepository, GetMapper(), signUpValidator, null, tokenManagerMock.Object);
        //Act
        var result = authService.SignUpAsync(signUpDto).Result;
        //Assert
        Assert.Equal(null,result.Value);
        Assert.True(result.ValidateErrors.Count == 1 && (result.ValidateErrors[0].ErrorCode == (int)AuthResultCode.EmptyFullName 
                                                     || result.ValidateErrors[0].ErrorCode == (int)AuthResultCode.EmptyEmail
                                                     || result.ValidateErrors[0].ErrorCode == (int)AuthResultCode.EmptyGroup
                                                     || result.ValidateErrors[0].ErrorCode == (int)AuthResultCode.EmptyPassword));
    }

    [Fact]
    async void SignUp_Should_ReturnEmailOccupied_IfEmailOccupied()
    {
        //Arrange
        var signUpValidator = new SignUpDTOValidator(_userRepository);
        var tokenManagerMock = new Mock<ITokenManager>();
        tokenManagerMock.Setup(manager => manager.IssueToken(It.IsAny<string>())).Returns(() => StandartAuthDto);
        var authService = new AuthService(_userRepository, GetMapper(), signUpValidator, null, tokenManagerMock.Object);
        var emailOccupiedSignUpDto = new SignUpDTO()
        {
            FullName = "Maks Filatov",
            Email = "mermilov@gmail.com",
            Password = "Qwerty123",
            Group = "М3О-123Бк-23"
        };
        //Act
        var result = await authService.SignUpAsync(emailOccupiedSignUpDto);
        //Assert
        Assert.Equal(null, result.Value);
        Assert.True(result.ValidateErrors.Count == 1 && result.ValidateErrors[0].ErrorCode == (int)AuthResultCode.EmailOccupied);
    }
    [Theory]
    [ClassData(typeof(SimplePassSignUpData))]
    async void SignUp_Should_ReturnInvalidPassError_IfPassIsSimple(SignUpDTO invalidPassSignUpDto)
    {
        //Arrange
        var signUpValidator = new SignUpDTOValidator(_userRepository);
        var tokenManagerMock = new Mock<ITokenManager>();
        tokenManagerMock.Setup(manager => manager.IssueToken(It.IsAny<string>())).Returns(() => StandartAuthDto);
        var authService = new AuthService(_userRepository, GetMapper(), signUpValidator, null, tokenManagerMock.Object);
        //Act
        var result = await authService.SignUpAsync(invalidPassSignUpDto);
        //Assert
        Assert.Equal(null, result.Value);
        Assert.True(result.ValidateErrors.Count == 1 && result.ValidateErrors[0].ErrorCode == (int)AuthResultCode.InvalidPass);
    }

    [Fact]
    async void SignIn_Should_ReturnAuthDto_IfCredentialsIsValid()
    {
        var validCredentials = new SignInDTO()
        {
            Email = "vitka@mail.ru",
            Password = "12345Yg"
        };
        //Arrange
        var signInValidator = new SignInDTOValidator();
        var tokenManagerMock = new Mock<ITokenManager>();
        tokenManagerMock.Setup(manager => manager.IssueToken(It.IsAny<string>())).Returns(() => StandartAuthDto);
        var authService = new AuthService(_userRepository, GetMapper(), null, signInValidator, tokenManagerMock.Object);
        //Act
        var result = await authService.SignInAsync(validCredentials);
        //Assert
        Assert.Equal(result.Value, StandartAuthDto);
    }
    [Theory]
    [ClassData(typeof(EmptyPropSignInData))]
    async void SignIn_Should_ReturnEmptyPropError_IfOneOfPropEmpty(SignInDTO emptySignInDto)
    {
        //Arrange
        var signInValidator = new SignInDTOValidator();
        var tokenManagerMock = new Mock<ITokenManager>();
        tokenManagerMock.Setup(manager => manager.IssueToken(It.IsAny<string>())).Returns(() => StandartAuthDto);
        var authService = new AuthService(_userRepository, GetMapper(), null, signInValidator, tokenManagerMock.Object);
        //Act
        var result = await authService.SignInAsync(emptySignInDto);
        //Assert
        Assert.Equal(null,result.Value);
        Assert.True(result.ValidateErrors.Count() == 1 && 
                    (result.ValidateErrors[0].ErrorCode == (int)AuthResultCode.EmptyEmail 
                     || result.ValidateErrors[0].ErrorCode == (int)AuthResultCode.EmptyPassword));
    }

    [Fact]
    async void SignIn_Should_ReturnInvalidCredentials_IfEmailIsNotValid()
    {
        //Arrange
        var invalidEmailSignInDto = new SignInDTO()
        {
            Email = "invalid@mail.ru",
            Password = "tyuiPh1234"
        };
        var signInValidator = new SignInDTOValidator();
        var tokenManagerMock = new Mock<ITokenManager>();
        tokenManagerMock.Setup(manager => manager.IssueToken(It.IsAny<string>())).Returns(() => StandartAuthDto);
        var authService = new AuthService(_userRepository, GetMapper(), null, signInValidator, tokenManagerMock.Object);
        //Act
        var result = await authService.SignInAsync(invalidEmailSignInDto);
        //Assert
        Assert.Equal(null,result.Value);
        Assert.True(result.ValidateErrors.Count() == 1 && result.ValidateErrors[0].ErrorCode == (int)AuthResultCode.InvalidCredentials);
    }

    [Fact]
    async void SignIn_Should_ReturnInvalidCredentials_IfPasswordIsNotValid()
    {
        //Arrange
        var invalidEmailSignInDto = new SignInDTO()
        {
            Email = "vitka@mail.ru",
            Password = "tyuiPh1234"
        };
        var signInValidator = new SignInDTOValidator();
        var tokenManagerMock = new Mock<ITokenManager>();
        tokenManagerMock.Setup(manager => manager.IssueToken(It.IsAny<string>())).Returns(() => StandartAuthDto);
        var authService = new AuthService(_userRepository, GetMapper(), null, signInValidator, tokenManagerMock.Object);
        //Act
        var result = await authService.SignInAsync(invalidEmailSignInDto);
        //Assert
        Assert.Equal(null,result.Value);
        Assert.True(result.ValidateErrors.Count() == 1 && result.ValidateErrors[0].ErrorCode == (int)AuthResultCode.InvalidCredentials);
    }
    [Fact]
    async void RefreshToken_Should_ReturnNewAuthDto_IfTokenIsValid()
    {
        //Arrange
        var user = _users.First(x => x.Devices.Count() > 0);
        var token = user.Devices.ElementAt(0);
        var previousCount = user.Devices.Count();
        var tokenManagerMock = new Mock<ITokenManager>();
        tokenManagerMock.Setup(manager => manager.IssueToken(It.IsAny<string>())).Returns(() => StandartAuthDto);
        tokenManagerMock.Setup(manager => manager.GetUserId(It.IsAny<string>())).Returns(() => user.Id);
        var authService = new AuthService(_userRepository, GetMapper(), null, null, tokenManagerMock.Object);
        //Act
        var result = await authService.RefreshTokenAsync(token);
        //Assert
        Assert.Equal(StandartAuthDto,result.Value);
        Assert.True(user.Devices.Count() == previousCount);
    }
    [Fact]
    async void RefreshToken_Should_ReturnInvalidTokenError_IfTokenIsUnvalidated()
    {
        //Arrange
        string randomToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9" +
                             ".eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiODM2OTBmZjItNDVhYS00ODg2LTkxYjgtYjNlYmNkMGRkZmFlIiwiZXhwIjoxNjk0MTk2NDE3LCJpc3MiOiJTa2VkLkF1dGhTZXJ2aWNlIiwiYXVkIjoiU2tlZC5Vc2VycyJ9" +
                             ".5uCgK2hixqxFOFNPWoexI4jGdoUSvCm6WFY7ukFtZGY";
        var tokenManagerMock = new Mock<ITokenManager>();
        tokenManagerMock.Setup(manager => manager.IssueToken(It.IsAny<string>())).Returns(() => StandartAuthDto);
        tokenManagerMock.Setup(manager => manager.GetUserId(It.IsAny<string>())).Returns(() => null);
        var authService = new AuthService(_userRepository, GetMapper(), null, null, tokenManagerMock.Object);
        //Act
        var result = await authService.RefreshTokenAsync(randomToken);
        //Assert
        Assert.Equal(null,result.Value);
        Assert.True(result.ValidateErrors.Count() == 1 && result.ValidateErrors[0].ErrorCode == (int)AuthResultCode.InvalidRefreshToken);
    }
    [Fact]
    async void RefreshToken_Should_ReturnInvalidTokenError_IfTokenIsNotADevice()
    {
        //Arrange
        string randomToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9" +
                             ".eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiODM2OTBmZjItNDVhYS00ODg2LTkxYjgtYjNlYmNkMGRkZmFlIiwiZXhwIjoxNjk0MTk2NDE3LCJpc3MiOiJTa2VkLkF1dGhTZXJ2aWNlIiwiYXVkIjoiU2tlZC5Vc2VycyJ9" +
                             ".5uCgK2hixqxFOFNPWoexI4jGdoUSvCm6WFY7ukFtZGY";
        string userId = "84859d87-e5b6-44d5-bb72-574c962f4f9e";
        var tokenManagerMock = new Mock<ITokenManager>();
        tokenManagerMock.Setup(manager => manager.IssueToken(It.IsAny<string>())).Returns(() => StandartAuthDto);
        tokenManagerMock.Setup(manager => manager.GetUserId(It.IsAny<string>())).Returns(() => userId);
        var authService = new AuthService(_userRepository, GetMapper(), null, null, tokenManagerMock.Object);
        //Act
        var result = await authService.RefreshTokenAsync(randomToken);
        //Assert
        Assert.Equal(null,result.Value);
        Assert.True(result.ValidateErrors.Count() == 1 && result.ValidateErrors[0].ErrorCode == (int)AuthResultCode.InvalidRefreshToken);
    }

    [Fact]
    async void Logout_Should_ReturnOkStatus()
    {
        //Arrange
        var user = _users.First(x => x.Devices.Any());
        var token = user.Devices.ElementAt(0);
        var previousDevicesCount = user.Devices.Count();
        var tokenManagerMock = new Mock<ITokenManager>();
        tokenManagerMock.Setup(manager => manager.IssueToken(It.IsAny<string>())).Returns(() => StandartAuthDto);
        tokenManagerMock.Setup(manager => manager.GetUserId(It.IsAny<string>())).Returns(() => user.Id);
        var authService = new AuthService(_userRepository, GetMapper(), null, null, tokenManagerMock.Object);
        //Act
        var result = await authService.Logout(token);
        //Assert
        Assert.True(result.Code == (AuthResultCode.Ok) && previousDevicesCount - 1 == user.Devices.Count() && !user.Devices.Contains(token));
    }

    [Fact]
    async void Logout_Should_ReturnInvalidRefreshTokenError_IfTokenIsNotValid()
    {
        string invalidToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiZWZlZmVmZWZlZmVmZmVmZWYiLCJleHAiOjE3MDE5NzA3MTIsImlzcyI6ImVmZmVmIiwiYXVkIjoiZWZlZmUifQ.Q3q1FIqmI5UdgIYWAwR2gwwFcW889Ptcgvh7Td4i1fA";
        var tokenManagerMock = new Mock<ITokenManager>();
        tokenManagerMock.Setup(manager => manager.IssueToken(It.IsAny<string>())).Returns(() => StandartAuthDto);
        tokenManagerMock.Setup(manager => manager.GetUserId(It.IsAny<string>())).Returns(() => null);
        var authService = new AuthService(_userRepository, GetMapper(), null, null, tokenManagerMock.Object);
        //Act
        var result = await authService.Logout(invalidToken);
        //Assert
        Assert.True(result.Code == AuthResultCode.InvalidRefreshToken);
    }

    [Fact]
    async void LogoutFromAll_Should_DeleteAllDevicesFromUser()
    {
        //Arrange
        var user = _users.First(x => x.Devices.Any());
        var id = user.Id;
        var authService = new AuthService(_userRepository, GetMapper(), null, null, null);
        //Act
        var result = await authService.LogoutFromAll(id);
        //Assert
        Assert.Equal(0, user.Devices.Count());
    }
    [Fact]
    async void LogoutFromAll_Should_ReturnInvalidUserIdError_IfUserIdIsNotValid()
    {
        //Arrange
        var id = "invalid-id";
        var authService = new AuthService(_userRepository, GetMapper(), null, null, null);
        //Act
        var result = await authService.LogoutFromAll(id);
        //Assert
        Assert.Equal(AuthResultCode.InvalidUserId, result.Code);
    }
}
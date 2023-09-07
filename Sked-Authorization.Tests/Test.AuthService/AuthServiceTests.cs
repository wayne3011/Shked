using System.Reflection;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Sked_Authorization.Tests.Base;
using Sked_Authorization.Tests.Test.Data;
using SkedAuthorization.Application.Data.DTO;
using SkedAuthorization.Application.Data.Responses;
using SkedAuthorization.Application.Infrastructure;
using SkedAuthorization.Application.Mapper;
using SkedAuthorization.Application.Services;
using SkedAuthorization.Application.Services.Utils;
using SkedAuthorization.Application.Services.Validators;
using SkedAuthorization.DAL.Entities;
using SkedAuthorization.DAL.Infrastructure;

namespace Sked_Authorization.Tests;

public class AuthServiceTests : AuthServiceTestBase
{
    readonly AuthDTO? StandartDto = new AuthDTO()
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

        var userRepo = GetUserRepo();
        var signUpValidator = new SignUpDTOValidator(userRepo);
        
        var tokenManagerMock = new Mock<ITokenManager>();
        tokenManagerMock.Setup(manager => manager.IssueToken(It.IsAny<string>())).Returns(() => StandartDto);
        
        var authService = new AuthService(userRepo, GetMapper(), signUpValidator, null, tokenManagerMock.Object);
        //Act
        var result = await authService.SignUpAsync(newUser);
        //Assert
        Assert.Equal(StandartDto,result.Value);
        Assert.Equal(null,result.ValidateErrors);
    }
    
    [Theory]
    [ClassData(typeof(EmptyPropSignUpData))]
    void SignUp_Should_ReturnError_IfPropNullOrEmpty(SignUpDTO signUpDto)
    {
        //Arrange
        var userRepo = GetUserRepo();
        var signUpValidator = new SignUpDTOValidator(userRepo);
        var tokenManagerMock = new Mock<ITokenManager>();
        tokenManagerMock.Setup(manager => manager.IssueToken(It.IsAny<string>())).Returns(() => StandartDto);
        var authService = new AuthService(userRepo, GetMapper(), signUpValidator, null, tokenManagerMock.Object);
        //Act
        var result = authService.SignUpAsync(signUpDto).Result;
        //Assert
        Assert.Equal(null,result.Value);
        Assert.True(result.ValidateErrors.Count == 1 && (result.ValidateErrors[0].ErrorCode == (int)AuthResultCode.EmptyFullName 
                                                     || result.ValidateErrors[0].ErrorCode == (int)AuthResultCode.InvalidEmail
                                                     || result.ValidateErrors[0].ErrorCode == (int)AuthResultCode.InvalidGroup
                                                     || result.ValidateErrors[0].ErrorCode == (int)AuthResultCode.EmptyPassword));
    }
}
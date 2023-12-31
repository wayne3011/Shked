﻿using System.Text.Json.Serialization;
using ThirdParty.Json.LitJson;

namespace ShkedAuthorization.Application.Data.DTO;

public class SignInDTO
{
    [JsonPropertyName("email")]
    public string Email { get; set; }
    [JsonPropertyName("password")]
    public string Password { get; set; }
}
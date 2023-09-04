﻿using SkedAuthoriztion.DAL.Entities;

namespace SkedAuthoriztion.DAL.Infrastructure;

public interface IUserRepository
{
    Task<User?> GetById(string id);
    Task<User?> GetByEmail(string email);
    Task Create(User newUser);
    Task<bool> Update(User newUser);
    Task<bool> Delete(string id);
}
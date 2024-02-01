using ShkedAuthorization.Application.Data.DTO;

namespace Sked_Authorization.Tests.Test.Data;

public class EmptyPropSignInData : TheoryData<SignInDTO>
{
    public EmptyPropSignInData()
    {
        Add(new SignInDTO()
        {
            Email = "",
            Password = "Qwerty1"
        });
        Add(new SignInDTO()
        {
            Email = "mermilov@gmail.com",
            Password = ""
        });
    }
}
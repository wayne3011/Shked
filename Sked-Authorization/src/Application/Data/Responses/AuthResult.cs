namespace SkedAuthorization.Application.Data.Responses;

public class AuthResult<T>
{
    public T? Value { get; }
    public AuthResultCode Code { get; }

    public AuthResult(T? value, AuthResultCode code)
    {
        Value = value;
        Code = code;
    }
}

public class AuthResult
{
    public AuthResultCode Code { get; }

    public AuthResult(AuthResultCode code)
    {
        Code = code;
    }
}
public enum AuthResultCode
{
    EmailOccupied,
    Ok,
    InvalidEmail,
    InvalidPass,
    InvalidUserId,
    InvalidRefreshToken
}
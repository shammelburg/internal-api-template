namespace Internal.API.Interfaces;

public interface IEndpoint
{
    static abstract void Configure(IEndpointRouteBuilder app);
}
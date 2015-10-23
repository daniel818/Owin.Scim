namespace Owin.Scim.Tests.Integration.Users.Create
{
    using System.Net.Http;
    using System.Net.Http.Formatting;

    using Machine.Specifications;

    using Model.Users;

    public abstract class when_creating_a_user : using_a_scim_server
    {
        Because of = async () =>
        {
            Response = await Server
                .HttpClient
                .PostAsync("users", new ObjectContent<User>(UserDto, new JsonMediaTypeFormatter()))
                .AwaitResponse()
                .AsTask;
        };
        
        protected static User UserDto;

        protected static HttpResponseMessage Response;
    }
}
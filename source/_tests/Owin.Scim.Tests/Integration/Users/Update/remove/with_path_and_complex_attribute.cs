namespace Owin.Scim.Tests.Integration.Users.Update.remove
{
    using System.Net;
    using System.Net.Http;
    using System.Text;
    
    using Machine.Specifications;

    using Model.Users;

    using v2.Model;

    public class with_path_and_complex_attribute : when_updating_a_user
    {
        Establish context = () =>
        {
            UserToUpdate = new ScimUser2
            {
                UserName = UserNameUtility.GenerateUserName(),
                Name = new Name
                {
                    FamilyName = "Smith",
                    GivenName = "John"
                }
            };

            PatchContent = new StringContent(
                @"
                    {
                        ""schemas"": [""urn:ietf:params:scim:api:messages:2.0:PatchOp""],
                        ""Operations"": [{
                            ""op"": ""remove"",
                            ""path"": ""name.givenName""
                        }]
                    }",
                Encoding.UTF8,
                "application/json");
        };

        It should_return_ok = () => PatchResponse.StatusCode.ShouldEqual(HttpStatusCode.OK);

        It should_update_version = () => UpdatedUser.Meta.Version.ShouldNotEqual(UserToUpdate.Meta.Version);

        It should_update_last_modified = () => UpdatedUser.Meta.LastModified.ShouldBeGreaterThan(UserToUpdate.Meta.LastModified);

        It should_not_remove_other_attributes = () => UpdatedUser.Name.FamilyName.ShouldEqual("Smith");

        It should_remove_the_attribute_value = () => UpdatedUser.Name.GivenName.ShouldBeNull();
    }
}
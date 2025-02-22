using System;
using Newtonsoft.Json;
using Xunit;

namespace RingCentral.Tests
{
    public class ScimTest
    {
        private const string EMAIL = "tyler.liu.test@ringcentral.com";

        [Fact]
        public async void GetServiceProviderConfig()
        {
            using (var rc = new RestClient(
                Environment.GetEnvironmentVariable("RINGCENTRAL_CLIENT_ID"),
                Environment.GetEnvironmentVariable("RINGCENTRAL_CLIENT_SECRET"),
                Environment.GetEnvironmentVariable("RINGCENTRAL_SERVER_URL")
            ))
            {
                await rc.Authorize(
                    Environment.GetEnvironmentVariable("RINGCENTRAL_USERNAME"),
                    Environment.GetEnvironmentVariable("RINGCENTRAL_EXTENSION"),
                    Environment.GetEnvironmentVariable("RINGCENTRAL_PASSWORD")
                );
                var str = await rc.Get<string>("/scim/v2/ServiceProviderConfig");
                Assert.Contains("urn:ietf:params:scim:schemas:core:2.0:ServiceProviderConfig", str);

                var serviceProviderConfig = await rc.Scim().ServiceProviderConfig().Get();
                Assert.Equal(new[] {"urn:ietf:params:scim:schemas:core:2.0:ServiceProviderConfig"},
                    serviceProviderConfig.schemas);
            }
        }

        [Fact]
        public async void CheckHealth()
        {
            using (var rc = new RestClient(
                Environment.GetEnvironmentVariable("RINGCENTRAL_CLIENT_ID"),
                Environment.GetEnvironmentVariable("RINGCENTRAL_CLIENT_SECRET"),
                Environment.GetEnvironmentVariable("RINGCENTRAL_SERVER_URL")
            ))
            {
                await rc.Authorize(
                    Environment.GetEnvironmentVariable("RINGCENTRAL_USERNAME"),
                    Environment.GetEnvironmentVariable("RINGCENTRAL_EXTENSION"),
                    Environment.GetEnvironmentVariable("RINGCENTRAL_PASSWORD")
                );

                var str = await rc.Scim().Health().Get();
                Assert.Equal("OK", str);

                var str2 = await rc.Scim(null).Health().Get();
                Assert.Equal("OK", str2);
            }
        }

        [Fact]
        public async void GetUsers()
        {
            using (var rc = new RestClient(
                Environment.GetEnvironmentVariable("RINGCENTRAL_CLIENT_ID"),
                Environment.GetEnvironmentVariable("RINGCENTRAL_CLIENT_SECRET"),
                Environment.GetEnvironmentVariable("RINGCENTRAL_SERVER_URL")
            ))
            {
                await rc.Authorize(
                    Environment.GetEnvironmentVariable("RINGCENTRAL_USERNAME"),
                    Environment.GetEnvironmentVariable("RINGCENTRAL_EXTENSION"),
                    Environment.GetEnvironmentVariable("RINGCENTRAL_PASSWORD")
                );

                var userSearchResponse = await rc.Scim().Users().List();
                Assert.True(userSearchResponse.Resources.Length > 0);
                Assert.True(userSearchResponse.totalResults > 0);
                Assert.Equal(1, userSearchResponse.startIndex);
            }
        }

        [Fact]
        public async void CreateUser()
        {
            using (var rc = new RestClient(
                Environment.GetEnvironmentVariable("RINGCENTRAL_CLIENT_ID"),
                Environment.GetEnvironmentVariable("RINGCENTRAL_CLIENT_SECRET"),
                Environment.GetEnvironmentVariable("RINGCENTRAL_SERVER_URL")
            ))
            {
                await rc.Authorize(
                    Environment.GetEnvironmentVariable("RINGCENTRAL_USERNAME"),
                    Environment.GetEnvironmentVariable("RINGCENTRAL_EXTENSION"),
                    Environment.GetEnvironmentVariable("RINGCENTRAL_PASSWORD")
                );

                // delete existing user first
                var searchRequest = new SearchRequest
                {
                    count = 1,
                    filter = $"emails eq \"{EMAIL}\""
                };
                var userSearchResponse = await rc.Scim().Users().DotSearch().Post(searchRequest);
                if (userSearchResponse.Resources.Length == 1)
                {
                    await rc.Scim().Users(userSearchResponse.Resources[0].id).Delete();
                }

                // create the user
                var user = new User
                {
                    emails = new[]
                    {
                        new Email
                        {
                            type = "work",
                            value = EMAIL
                        }
                    },
                    name = new Name
                    {
                        familyName = "Liu",
                        givenName = "Tyler"
                    },
                    schemas = new[] {"urn:ietf:params:scim:schemas:core:2.0:User"},
                    userName = EMAIL
                };
                var userResponse = await rc.Scim().Users().Post(user);
                Assert.Equal("Liu", userResponse.name.familyName);
                Assert.Equal("Tyler", userResponse.name.givenName);

                // don't forget to delete it after testing
                var str = await rc.Scim().Users(userResponse.id).Delete();
                Assert.Equal("", str);
            }
        }

        [Fact]
        public async void SearchUser()
        {
            using (var rc = new RestClient(
                Environment.GetEnvironmentVariable("RINGCENTRAL_CLIENT_ID"),
                Environment.GetEnvironmentVariable("RINGCENTRAL_CLIENT_SECRET"),
                Environment.GetEnvironmentVariable("RINGCENTRAL_SERVER_URL")
            ))
            {
                await rc.Authorize(
                    Environment.GetEnvironmentVariable("RINGCENTRAL_USERNAME"),
                    Environment.GetEnvironmentVariable("RINGCENTRAL_EXTENSION"),
                    Environment.GetEnvironmentVariable("RINGCENTRAL_PASSWORD")
                );

                var searchRequest = new SearchRequest
                {
                    count = 10
                };
                var userSearchResponse = await rc.Scim().Users().DotSearch().Post(searchRequest);
                Assert.True(userSearchResponse.Resources.Length > 0);
                Assert.True(userSearchResponse.totalResults > 0);
                Assert.Equal(1, userSearchResponse.startIndex);
            }
        }

        [Fact]
        public async void GetUserById()
        {
            using (var rc = new RestClient(
                Environment.GetEnvironmentVariable("RINGCENTRAL_CLIENT_ID"),
                Environment.GetEnvironmentVariable("RINGCENTRAL_CLIENT_SECRET"),
                Environment.GetEnvironmentVariable("RINGCENTRAL_SERVER_URL")
            ))
            {
                await rc.Authorize(
                    Environment.GetEnvironmentVariable("RINGCENTRAL_USERNAME"),
                    Environment.GetEnvironmentVariable("RINGCENTRAL_EXTENSION"),
                    Environment.GetEnvironmentVariable("RINGCENTRAL_PASSWORD")
                );

                var searchRequest = new SearchRequest
                {
                    count = 10
                };
                var userSearchResponse = await rc.Scim().Users().DotSearch().Post(searchRequest);
                var firstUser = userSearchResponse.Resources[0];

                var user = await rc.Scim().Users(firstUser.id).Get();
                Assert.Equal(user.id, firstUser.id);
                Assert.Equal(user.name.familyName, firstUser.name.familyName);
                Assert.Equal(user.name.givenName, firstUser.name.givenName);
            }
        }

        [Fact]
        public async void UpdateUser()
        {
            using (var rc = new RestClient(
                Environment.GetEnvironmentVariable("RINGCENTRAL_CLIENT_ID"),
                Environment.GetEnvironmentVariable("RINGCENTRAL_CLIENT_SECRET"),
                Environment.GetEnvironmentVariable("RINGCENTRAL_SERVER_URL")
            ))
            {
                await rc.Authorize(
                    Environment.GetEnvironmentVariable("RINGCENTRAL_USERNAME"),
                    Environment.GetEnvironmentVariable("RINGCENTRAL_EXTENSION"),
                    Environment.GetEnvironmentVariable("RINGCENTRAL_PASSWORD")
                );

                var searchRequest = new SearchRequest
                {
                    count = 1,
                    filter = "emails eq \"tyler.liu@ringcentral.com\""
                };
                var userSearchResponse = await rc.Scim().Users().DotSearch().Post(searchRequest);
                if (userSearchResponse.Resources.Length == 1)
                {
                    var userResponse = userSearchResponse.Resources[0];
                    var user = JsonConvert.DeserializeObject<User>(JsonConvert.SerializeObject(userResponse));
                    var guid = Guid.NewGuid().ToString();
                    user.name.familyName = guid;
                    var ur = await rc.Scim().Users(user.id).Put(user);
                    Assert.Equal("tyler.liu@ringcentral.com", ur.emails[0].value);
                    Assert.Equal(guid, ur.name.familyName);
                }
            }
        }

        [Fact]
        public async void PatchUser()
        {
            using (var rc = new RestClient(
                Environment.GetEnvironmentVariable("RINGCENTRAL_CLIENT_ID"),
                Environment.GetEnvironmentVariable("RINGCENTRAL_CLIENT_SECRET"),
                Environment.GetEnvironmentVariable("RINGCENTRAL_SERVER_URL")
            ))
            {
                await rc.Authorize(
                    Environment.GetEnvironmentVariable("RINGCENTRAL_USERNAME"),
                    Environment.GetEnvironmentVariable("RINGCENTRAL_EXTENSION"),
                    Environment.GetEnvironmentVariable("RINGCENTRAL_PASSWORD")
                );

                var searchRequest = new SearchRequest
                {
                    count = 1,
                    filter = "emails eq \"tyler.liu@ringcentral.com\""
                };
                var userSearchResponse = await rc.Scim().Users().DotSearch().Post(searchRequest);
                if (userSearchResponse.Resources.Length == 1)
                {
                    var userResponse = userSearchResponse.Resources[0];

                    var guid = Guid.NewGuid().ToString();
                    var userPatch = new UserPatch
                    {
                        schemas = new[]
                        {
                            "urn:ietf:params:scim:api:messages:2.0:PatchOp"
                        },
                        Operations = new[]
                        {
                            new PatchOperation
                            {
                                op = "replace",
                                path = "name.familyName",
                                value = guid
                            }
                        }
                    };
                    var ur = await rc.Scim().Users(userResponse.id).Patch(userPatch);
                    Assert.Equal("tyler.liu@ringcentral.com", ur.emails[0].value);
                    Assert.Equal(guid, ur.name.familyName);
                }
            }
        }
    }
}
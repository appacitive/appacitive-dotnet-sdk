.NET SDK for appacitive
=====================

This open source library allows you to integrate applications built using Microsoft.NET with the Appacitive platform.

To learn more about the Appacitive platform, please visit [www.appacitive.com](https://www.appacitive.com).

LICENSE

Except as otherwise noted, the .NET SDK for Appacitive is licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0.html).


# Documentation 

## User management

### Adding or updating a new user
To add a new user account, create a new user object, set the relevant fields and call `SaveAsync()`
To update an existing user, simple update the relevant fields and call `SaveAsync()`.
``` C#
var user = new User
{
    Username = "john.doe",
    FirstName = "John",
    LastName = "Doe",
    Email = "john.doe@gmail.com",
    Password = "password",
    Phone = "987543210"
};
await user.SaveAsync();
```

### Get an existing user by id 
To get an existing user by it's system generated id, call `GetUserByIdAsync()`.

``` C#
var user = await User.GetByIdAsync("123456");

// To get only specific fields (username, firstname and lastname)
var user = await User.GetByIdAsync("123456", new [] { "username", "firstname", "lastname" });
```

### Get an existing user by username 
To get an existing user by it's username, call `GetUserByUsernameAsync()`.

``` C#
var user = await User.GetByUsernameAsync("john.doe");
```

### Authenticating an existing user (with username and password)
To authenticate an existing user with username and password, create a new instance of UsernamePasswordCrendentials
with the appropriate values and call `AuthenticateAsync()`. On successful authentication, the method will return the 
user session token and the logged in user object.

``` C#
var creds = new UsernamePasswordCredentials("username", "password")
{
    TimeoutInSeconds = 15 * 60,     // optional, 15 minute validity of user session
    MaxAttempts = int.MaxValue      // optional, limit on number of times the user session can be used in api calls.
};
// Authenticate
var result = await creds.AuthenticateAsync();
User loggedInUser = result.LoggedInUser;
string token = result.UserToken; 
```

### Authenticating via facebook
To authenticate a user via facebook, create an instance of OAuth2Credentials with the facebook access token recieved from 
the facebook OAuth2 handshake. To create a new user incase the user does not already exist in the system, set optional
parameter *CreateUserIfNotExists* as true.

``` C#
var facebookCreds = new OAuth2Credentials(facebookAccessToken, "facebook")
{
    CreateUserIfNotExists = true    // optional, create new user if user does not exist in system. 
};
var result = await facebookCreds.AuthenticateAsync();
```



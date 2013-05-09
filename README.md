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

### Getting the logged in user
To get the logged in user, call 'GetLoggedInUserAsync' on the User object as shown below.

``` C#
var loggedInUser = await User.GetLoggedInUserAsync();
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

### Validating the user session
To validate an existing user session token, call the `IsValidAsync` method on the UserSession object.

``` C#
string userToken; // Contains the user session token
bool isValid = await UserSession.IsValidAsync(userToken);
```

### InValidating the user session
To invalidate an existing user session token, call the `InvalidateAsync` method on the UserSession object.

``` C#
string userToken; // Contains the user session token
await UserSession.InvalidateAsync(userToken);
```


## File management

All file upload and download operations in the SDK are handled by the `FileUpload` and `FileDownload` classes respectively.
All file operations need a file name. This name can be supplied on upload by the user or can be auto generated to be unique 
by the api. Some important things to note about the file name.

1. It is not mandatory.
2. It does not have to be the same is the name of the file being uploaded.
3. Incase it is not supplied, it will be auto generated and returned in the response.
4. It is the only handle to download the file, so do not lose it.

### Uploading a new file

To upload a new file, create a new instance of the `FileUpload` class with the mime type and optional filename.
If you do not supply a file name, an auto generated name will be returned in the response.

``` C#
// To upload an png image with filename testimage.png
var mimeType = "image/png";
var filePath = ...				// Path to the file
var userSpecifiedFileName = "testimage.png";
// filenameOnServer will be testimage.png.
var fileNameOnServer = await new FileUpload(mimeType, userSpecifiedFileName).UploadFileAsync(filePath);



// To upload an png image without specifying a filename
var mimeType = "image/png";
var filePath = ...				// Path to the file
// filenameOnServer will be auto generated
var fileNameOnServer = await new FileUpload(mimeType).UploadFileAsync(filePath);


// To upload an png image as a byte array
var mimeType = "image/png";
byte[] data = ...								// PNG file data
var filePath = ...								// Path to the file
// filenameOnServer will be auto generated
var fileNameOnServer = await new FileUpload(mimeType).UploadAsync(data);

```


### Generate url to upload file to
Incase you want to handle the file upload yourself via some custom control or code, you can generate an upload url which will be available 
for a limited time period to which you can upload the file. The life time of the upload url can be specified in the api call.
The following code snippet shows how this can be done.

``` C#
// Say you want the upload url to upload a PNG image.
var mimeType = "image/png";
int expiryInMinutes = 10;				// Upload url will remain active for next 10 mins
var fileUrl = await new FileUpload(mimeType).GetUploadUrlAsync(expiryInMinutes);

var nameOnServer = fileUrl.FileName;	// This will be the filename on the server (auto generated in this case)
var urlToUpload = fileUrl.Url;			// This will be the url to upload to.
```


### Download an existing file
To download an existing file from the platform, you need the filename of the file returned from the Upload api.
With this filename you can download the file using the `FileDownload` class.

``` C#
// To download a file and save to disk
var filename = ...;			// File name of the file to download.
var saveAsFilePath = ...;	// Path to save the downloaded file.
// The file will be downloaded and saved to the saveAs path specified.
await new FileDownload(filename).DownloadFileAsync(saveAsFilePath);


// To download file contents as byte array
var filename = ...;			// File name of the file to download.
var saveAsFilePath = ...;	// Path to save the downloaded file.
// The file will be downloaded and saved to the saveAs path specified.
byte[] contents = await new FileDownload(filename).DownloadAsync();
```


### Generate a public url for an existing file.
All files uploaded to the Appacitive platform are private by default and cannot be accessed via a http GET to a url alone. 
To generate a public url for a file, use the `FileDownload.GetDownloadUrlAsync()` api. This will return a public url which 
for the specified file which will be available for a specified time only (by default 5 mins). The app can decide the lifetime
of the public url based on the specific usecase.

``` C#

// To generate a public url for a file
var filename = ...;							// File name of the file to download.
var expiryInMinutes = 20 * 365 * 24 * 60;	// Public url that will be active for next 20 years

// Get download url
string publicUrl = await new FileDownload(filename).GetDownloadUrlAsync(expiryInMinutes);

```

### WCF gotchas
When using the SDK on the server side inside a web application or web service, we need to make sure that a lot 
of the ambient user context and sdk state is available on a per request basis instead of being statically stored for
the entire application. The SDK uses the WCF OperationContext to store and manage this information on a per request basis.
However given the fact that the SDK methods are async, special provisions need to be made to ensure that the OperationContext
is available across threads. To do this, service implementations using the SDK must apply the `AllowAsyncService` service
behavior to their service implementations. This can be done in two ways.

#### 1. Via attribution

``` C#
// Add the AllowAsyncService to ensure that OperationContext is propogated across async calls.
[AllowAsyncService]
public class MyWebService : IMyWebService
{
	...
}
```

#### 2. Via web.config

``` xml
<!-- Define a behavior extension inside the wcf service model configuration -->
<behaviorExtensions>
    <add name="allowAsyncCalls" type="Appacitive.Sdk.Wcf.AllowAsyncServiceBehaviorExtension, Appacitive.Sdk.WinRT" />
</behaviorExtensions>

<!-- Use the extension inside your service / endpoint / operation behavior configuration. -->
```


### Pending items
Articles and connections

- [x] Get connected articles.
- [x] Get connected articles with connections - done 
- [ ] Get articles in between
- [x] Multi get articles - done
- [x] Get connection between two articles of a specific type
- [ ] Get connections between two articles of any type of relation.
- [ ] Interconnect
- [ ] Multi get connections
- [x] Multi delete article
- [x] Multi delete connections
- [x] Force delete single article.
- [x] Sorting support on listing calls.
- [x] Update article with revision
- [x] Find all connections

Users
- [x] Authenticate with create new account - done
- [ ] Link account
- [ ] Create with linked account
- [ ] Get linked account
- [ ] Get all linked accounts
- [ ] Add linked account
- [ ] Update linked account
- [ ] Remove linked account
- [ ] Checkin
- [ ] Multi get
- [x] Update user with revision
- [x] Delete user with connection
- [x] Sorting support on listing calls.

File
- [x] Get upload url - done
- [x] Upload file - done
- [x] Download file - done
- [x] Get download url - done
- [x] Get public url - done
- [ ] Update file
- [ ] Delete file

Email
- [ ] Send raw email
- [ ] Send templated email

Push notifications
- [ ] Send push notification 

Device
- [x] CRUD
- [x] Listing
- [x] Find All
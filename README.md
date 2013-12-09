.NET SDK for appacitive
=====================

This open source library allows you to integrate applications built using Microsoft.NET with the Appacitive platform.

To learn more about the Appacitive platform, please visit [www.appacitive.com](https://www.appacitive.com).

LICENSE

Except as otherwise noted, the .NET SDK for Appacitive is licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0.html).


# What's new in this version.

* This version of the SDK targets v1.0 of the appacitive apis.

## Breaking changes

1. Core objects have been renamed to comply with new nomenclature.
	Article		------>		APObject
	Articles	------>		APObjects
	Connection	------>		APConnection
	Connections	------>		APConnections	
	User		------>		APUser
	Users		------>		APUsers
	Device		------>		APDevice
	Devices		------>		APDevices

2. AppacitiveException has been moved from platform specific SDK to Appacitive.SDK.
3. UtcCreateDate and UtcLastUpdated have been renamed to CreatedAt and LastUpdatedAt and will
always consistently return time in local time zone of the client.
4. FindAll() apis will now accept IQuery object instead of a string.
5. To pass a string based query, helper method `WithRawQuery()` has been provided in Query class.
6. Download and upload progress events are now supported with FileUpload and FileDownload classes.	

# Documentation 


## Data storage and retrieval

### Managing articles
Articles in the SDK are managed via the Article object and the Articles static helper class.
The snippets below show how common actions are performed on articles.

#### Creating a new article.
To create a new article, simply instantiate a new article object with the specific schema type.
Initialize its properties and invoke SaveAsync().

``` C#
// Creating a new article
var myScore = new Article("score");
myScore.Set<int>("points", 100);
myScore.Set<bool>("level_completed", true);
await myScore.SaveAsync();

// Creating a new article using dynamic (.NET 4.5)
dynamic myScore = new Article("score");
myScore.Points = 100; // Points is case insensitive.
myScore.Level_Completed = true;
await myScore.SaveAsync();

```

#### Get an existing article

``` C#
// Get score with id 87321
var score = await Articles.GetAsync("score", "87321");

// Get score with id 87321 with points field only
var score = await Articles.GetAsync("score", "1234233434", new [] { "points" });
int points = score.Get<int>("points");

// To multi get a list of articles using a list of ids
var ids = new[] { "4543212", "79782374", "8734734" };
IEnumerable<Article> articles = await Articles.MultiGetAsync("account", ids);

```

#### Updating an existing article.
The SaveAsync() method will update all changes made to an article. Incase you know the article id but do not have
the article instance, simply create a new article object with the id, update the required fields and invoke SaveAsync().
The article object supports partial updates and will only update the fields that have changed explicitly.

``` C#

// Update an existing article
// Incase the article object is not available, simply 
// create a new instance with the id of the article to be updated.
var account = new Article("account", "43234455");	
account.Set<bool>("email_verified", true);			// Set the field value to be updated.
await account.SaveAsync();							// Save async

```


#### Deleting an article.
To delete an article, use the DeleteAsync() method on the Articles helper class as shown in the snippet below.

``` C#
// To delete article of type account with id 53323454
await Articles.DeleteAsync("account", "53323454");

// To delete article of type account with id 32423453 along with all connections
bool deleteConnections = true;
Articles.DeleteAsync("account", "32423453", deleteConnections);

// To delete multiple articles of type account.
var ids = new [] { "234234", "71231230", "97637282" };
await Articles.MultiDeleteAsync("account", ids);
```
## Graph Query support

### Filter query
To run an existing filter query, use the Graph help class with the specific filter query name and arguments.
``` C#
// Filter query with arguments
var matches = await Graph.Filter( "query_name", 
	new Dictionary<string, string> { 
		{ "arg1", "value1" },
		{ "arg2", "value2" }
	});

// Filter query with arguments passed in a query object.
var matches = await Graph.Filter( "query_name", 
	new 
	{
		arg1 = "value1",
		arg2 = "value2"
	});

```

### Projection query
To run an existing filter query, use the Graph help class with the specific filter query name and arguments.
``` C#
// Filter query with arguments
var ids = new [] {"33452852036895518", "33591826925617507"};
var matches = await Graph.Project( "query_name", ids,
	new Dictionary<string, string> { 
		{ "arg1", "value1" },
		{ "arg2", "value2" }
	});

// Filter query with arguments passed in a query object.
var ids = new [] {"33452852036895518", "33591826925617507"};
var matches = await Graph.Project( "query_name", ids,
	new 
	{
		arg1 = "value1",
		arg2 = "value2"
	});

```
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

## Email notifications
To send emails via the SDK use the Email class. Alternatively, you can use the NewEmail fluent interface 
for sending emails as well, as shown below.

``` C#
var to = new [] {"email1", "email2"..}
var cc = new [] {"email1", "email2"..}
var bcc = new [] {"email1", "email2"..}

// Sending out a raw email
await NewEmail
    .Create("This is a raw email test from the .NET SDK.")
    .To(to, cc, bcc)
    .From("from@email.com", "replyto@email.com)
    .WithBody("This is a raw body email.")
    .SendAsync();

// Sending out a templated email
await NewEmail
    .Create("This is a raw email test from the .NET SDK.")
    .To(to, cc, bcc)
    .From("from@email.com", "replyto@email.com)
    .WithTemplateBody( "sample", 
        new Dictionary<string, string> 
        {
            {"username", "john.doe"},
            {"firstname", "John"},
            {"lastname", "Doe"},
        })
    .SendAsync();

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

## Push Notifications
Push notifications in the SDK are sent via the PushNotification fluent interface.
Different platform options for android, ios and windows phone are all specified via the same.
The sample below shows one complete example of how to send push notifications.

``` C#

await PushNotification
		// Send broadcast
		.Broadcast("Push from .NET SDK")
		// Increment existing badge by 1
		.WithBadge("+1")
		// Custom data field1 and field2
		.WithData(new { field1 = "value1", field2 = "value2" })
		// Expiry in seconds
		.WithExpiry(1000)
		// Device platform specific options
		.WithPlatformOptions(
			new IOsOptions
			{
				SoundFile = soundFile
			})
		.WithPlatformOptions(
			new AndroidOptions
			{
				NotificationTitle = title
			})
		.WithPlatformOptions(
			new WindowsPhoneOptions
			{
				Notification = new ToastNotification
				{
					Text1 = wp_text1,
					Text2 = wp_text2,
					Path = wp_path
				}
			})
		.SendAsync();

```


### Different types of push notifications
4 different types of push notifications are supported.

1. Broadcast to all devices.
2. To a list of device ids.
3. Devices belonging to specified list of channels.
4. Devices returned from a query.

The samples below show how to send the specific type of notifications.

``` C#
// Broadcast
await PushNotification.Broadcast("message").SendAsync();
// To a list of ids
await PushNotification.ToDeviceIds("message", devId1, devId2,..devIdN).SendAsync();
// For a list of channels
await PushNotification.ToChannels("message", channel1, channel2,.. channelN).SendAsync();
// For a query (send to all ios devices). Query syntax is also valid here as shown in the second line
await PushNotification.ToQueryResult("message", "*devicetype == 'ios'").SendAsync();
await PushNotification.ToQueryResult("message", Query.Property("devicetype").IsEqualTo("ios").ToString()).SendAsync();

```

### Different options for windows phone.
For windows phone 3 types of notifications are supported.
1. Toast notifications.
2. Tile notifications (flip, cyclic and iconic)
3. Raw notifications (string based raw data)

The windows phone platform options allows you to choose the specific kind of notification to be 
send to each windows phone device type (WP7, WP75 and WP8).
The sample below shows how this can be done.

``` C#
// Toast
await PushNotification
		.Broadcast("message")
		.WithPlatformOptions(
			new WindowsPhoneOptions
			{
				Notification = new ToastNotification
				{
					Text1 = wp_text1,
					Text2 = wp_text2,
					Path = wp_path
				}
			})
		.SendAsync();

// Raw notification
await PushNotification
		.Broadcast("message")
		.WithPlatformOptions(
            new WindowsPhoneOptions
                {
                    Notification = new RawNotification() { RawData = "string data.." }
                })
		.SendAsync();

// Tile notification (Flip tile for all)
await PushNotification
		.Broadcast("message")
		.WithPlatformOptions(
                    new WindowsPhoneOptions
                    {
                        Notification = TileNotification.CreateNewFlipTile( new FlipTile() { FrontTitle = title, .. } )
                    })
		.SendAsync();


// Tile notification (cyclic tile for wp8, flip tile for others)
await PushNotification
		.Broadcast("message")
		.WithPlatformOptions(
                    new WindowsPhoneOptions
                    {
                        Notification = TileNotification.CreateNewCyclicTile( new CyclicTile(), new FlipTile() )
                    })
		.SendAsync();

// Tile notification (iconic tile for wp8, flip tile for others)
await PushNotification
		.Broadcast("message")
		.WithPlatformOptions(
                    new WindowsPhoneOptions
                    {
                        Notification = TileNotification.CreateNewIconicTile( new IconicTile(), new FlipTile() )
                    })
		.SendAsync();

```

## Debugging the SDK

Debugging in the sdk uses standard .NET trace infrastructure via the App.Debug class.
The code below shows how to enable debugging for standard scenarios.

``` C#

	// To enable logging of all transactions.
	App.Debug.ApiLogging.LogEverything();

	// To log only failed calls.
	App.Debug.ApiLogging.LogFailures();
	
	// To log calls taking more than 600ms.
	App.Debug.ApiLogging.LogSlowCalls(600);

	// To log calls based on runtime condition.
	App.Debug.ApiLogging.LogIf(x => x.Status.Code == "400");

	// To log failed and slow calls.
	App.Debug.ApiLogging
			 .LogFailures()
			 .LogSlowCalls(600);

```


## Real time messaging

Real time functionality inside the SDK, leverages real time technology like web sockets, long polling and server sent events
via SignalR. This infrastructure can be used to incorporate real time duplex communication between users and the app backend.

NOTE: This feature is still in early stage beta. Use at your own risk.

There are 2 key terms that are used inside Appacitive for real time functionality.


1. Subscriptions
2. Messaging

### Enabling real time functionality

``` C#

// Enabling real time
App.Initialize(host, apiKey, Environment.Live, new AppacitiveSettings { EnableRealTimeSupport = true }); 

```

### Subscriptions 

Subscriptions allow the client app to subscribe to real time notifications on data changes on the server.
The client can subscribe to all changes (create, update & delete) for a specific type of schema or relation 
as well they can subscribe to all changes for a specific instance of an article or connection.
The sample below shows how this can be done.

``` C#

		// Modification to articles of a specific schema (create,update, delete)
        Subscriptions.ForSchema("post").Created += m =>
        {
            var msg = m as ObjectUpdatedMessage;
            Console.WriteLine("Created new post with id {0}.", msg.ObjectId);
        };

        // Modification to a specific article instance (update, delete)
        Subscriptions.ForArticle("post", "100").Updated += m =>
        {
            var msg = m as ObjectUpdatedMessage;
            Console.WriteLine("Post with id 100 was just updated.");
        };

        // Modification to connections of a specific relation (create, update, delete)
        Subscriptions.ForRelation("friend").Deleted += m =>
        {
            var msg = m as ObjectUpdatedMessage;
            Console.WriteLine("Friend connection with id {0} deleted.", msg.ObjectId);
        };

        // Modification to a specific connection instance (update, delete)
        Subscriptions.ForConnection("friend", "100").Updated += m =>
        {
            var msg = m as ObjectUpdatedMessage;
            Console.WriteLine("Friend connection with id 100 was just updated.");
        };

```


### Messaging

Messaging allows the client app to send real time messages to a given set of users.
The message must be an object that can be serialized properly. If the given set of users
are online, then they would get the appropriate messages.

The code sample below shows how this functionality can be implemented.

``` C#

		// Sending a message to user id 100 and 110
        var msg = new { text = "Come to my party", venue="19.123123,30.123123123" };
        await Messaging.SendMessageAsync(msg, 100, 110);

        // Recieve messages
        Messaging.Inbox.NewMessage += m =>
            {
                var msg = m as NewNotificationMessage;
                Console.WriteLine("Recieved message {0} from user id {1}.", msg.Payload.AsString(), msg.Sender);
            };

```

### Group messaging
Group messaging in Appacitive is implemented via the concept of Hubs.
A hub is a group of connections with a unique name in the scope of your App deployment.
All messages sent to a hub are sent to all connections that are part of the hub.
A hub can be used as a logical abstraction for most multi user scenarios like chatroom, multi player session etc.
The sample below shows how hubs can be used using the SDK.
``` C#

		var hubName = "my_chat_room";
        // Create or join hub
        await Messaging.JoinHubAsync(hubName);

        // Send message to hub
        var msg = new { text = "Come to my party", venue="19.123123,30.123123123" };
        await Messaging.SendMessageAsync(msg, hub);

        // Join notifications
        Messaging.Hubs.Joined += m =>
            {
                var msg = m as JoinedHubMessage;
                Console.WriteLine("User {0} joined hub {1}.", msg.User, msg.Hub);
            };

        // Leave hub notifications
        Messaging.Hubs.Left += m =>
        {
            var msg = m as LeftHubMessage;
            Console.WriteLine("User {0} left hub {1}.", msg.User, msg.Hub);
        };

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
- [x] Send raw email
- [x] Send templated email

Push notifications
- [x] Send push notification 

Device
- [x] CRUD
- [x] Listing
- [x] Find All

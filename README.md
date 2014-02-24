
.NET SDK for appacitive
=====================

This open source library allows you to integrate applications built using Microsoft .NET or Mono with the Appacitive platform.
To learn more about the Appacitive platform, please visit [www.appacitive.com][1].

LICENSE

Except as otherwise noted, the .NET SDK for Appacitive is licensed under the [Apache License, Version 2.0][2].

# Documentation 

## Setup

The appacitive .NET SDK can be added to your project via [nuget](https://www.nuget.org/packages/Appacitive).
Simply run the following command

> ## PM> **Install-Package Appacitive**

on the nuget package manager console to include the appacitive .NET SDK in your project. 

## Initialize

Before we dive into using the SDK, we need to understand a couple of things about using the Appacitive api.

All calls to the appacitive platform are secured using an app specific api key.
To access the api key for your app, go to app listing via the [portal](https://portal.appacitive.com) and click on the key icon on right side. 

!["Getting your apikey"](http:\/\/appacitive.github.io\/images\/portal-apikey-small.png)

####Initialize your SDK, 

```c#
App.Initialize(
    platform,       // Use the Platforms helper class for options. 
    appid,          // Set your application id here.
    apiKey,         // Set your api key here.
    env,            // Set your environment here. Use Environment enum. 
    settings);      // App settings (optional). 

```
#### What is this platform variable?
You can use the .NET sdk as part of your mobile app as well as any server side application like ASP.net or WCF. These enviroments vary quite significantly 
in terms of how certain operations are performed e.g., network calls.

The `platform` input parameter is provided to hint the SDK into the runtime environment in which it is hosted. For example, using `Platforms.Aspnet` essentially tells the sdk that it is being hosted inside a web application. Based on this, the SDK can pick the right mechanism for features like user auth and session management.

Depending upon the type of project that you are using, the Platforms helper class may present one or more of the following options - `Platforms.WindowsPhone7`, `Platforms.WindowsPhone8`, `Platforms.Aspnet`, `Platforms.Wcf` or `Platforms.Nonweb`

Now you are ready to use the SDK

## Conventions
1. The .NET SDK makes extensive use of .NET Tasks along with async \ await feature recently introduced with c# 5.0. As a result, most methods exposed via the SDK are async methods.

2. The SDK follows a certain naming convention for the various appacitive data types.
 - The SDK defines the following primary data types - `APObject`, `APConnection`, `APUser` and `APDevice`.
 - For each primary data type, there is a corresponding helper type with a plural name - `APObjects`, `APConnections`, `APUsers` and `APDevices`.
 - Write operations are performed on instances of a specific type.
 - Read, Delete and Find operations are performed via static operations on the corresponding helper type.
    ```c#
    // To read an object
    // Notice the plural APObject(s) below.
    var obj = await APObjects.GetAsync("object", "123");
    
    // To create or update
    await obj.SaveAsync();
    ```
3. Incase an api call fails, an exception extending from `BaseAppacitiveException` would be raised.

## Debugging the SDK

Debugging in the sdk uses standard .NET trace infrastructure via the App.Debug class. To setup debugging for your app

#### Setup .NET tracing
Add the following configuration to your app.config or web.config. Add a trace listener of your choice. The given configuration will log everything to the console.

```xml
    <system.diagnostics>
        <trace autoflush="true" indentsize="4">
            <listeners>
                <clear />
                <add name="configConsoleListener" type="System.Diagnostics.ConsoleTraceListener" />
            </listeners>
            </trace>
    </system.diagnostics>
```

#### Setup SDK debugging

The code below shows how to enable debugging for standard scenarios.

``` C#
	// To enable logging of all transactions.
	App.Debug.ApiLogging.LogEverything();

	// To log only failed calls.
	App.Debug.ApiLogging.LogFailures();
	
	// To log calls taking more than 600ms.
	App.Debug.ApiLogging.LogSlowCalls(600);

	// To log calls based on runtime condition.
	App.Debug.ApiLogging.LogIf((rq, rs) => rs.Status.Code == "400");

	// To log failed and slow calls.
	App.Debug.ApiLogging
			 .LogFailures()
			 .LogSlowCalls(600);

```
----------


## Data storage and retrieval

All data is represented as objects. This will become clearer as you read on. Let's assume that we are building a game and we need to store player data on the server.

### Creating
To create a player via the sdk, do the following
```c#
var player = new APObject("player");
```
Huh?

An `APObject` comprises of an entity (referred to as `object` in Appacitive jargon). To initialize an object, we need to provide it some options. The mandatory argument is the `__type` argument.

What is a type? In short, think of types as tables in a contemporary relational database. A type has properties which hold values just like columns in a table. A property has a data type and additional constraints are configured according to your application's need. Thus we are specifying that the player is supposed to contain an entity of the type 'player' (which should already be defined in your application).

The player object is an instance of `APObject`. An `APObject` is a class which encapsulates the data (the actual entity or the object) and methods that provide ways to update it, delete it etc. To see the raw entity that is stored within the `APObject`, fire `player.ToJSON()`.

#### Setting Values
Now we need to name our player 'John Doe'. This can be done as follows
```csharp
 // Using setters
 var player = new APObject("player");
 player.Set("name", "John Doe");
 
 // dynamically assigning properties (via the use of the dynamic keyword)
 dynamic player = new APObject("player");
 player.name = "John Doe";
```
#### Getting values
Lets verify that our player is indeed called 'John Doe'
```csharp
// using the getters
var name = player.Get<String>("name");

// Using the getter with a default value
// This will return N.A. incase the name property is not available.
var name = playet.Get<string>("name", "N.A.");

// dynamic usage
// Assuming that player is declared via the dynamic keyword.
// player.Name, player.NAme etc would also work here.
var name = player.name;
```

#### Saving
Saving a player to the server is easy.
```csharp
var player = new APObject("player");
player.Set("name", "John Doe");
player.Set("age", 22);
await player.SaveAsync();
Console.WriteLine("New player created with id {0}.", player.Id);
```
When you call save, the object is taken and stored on Appacitive's servers. A unique object id is generated and is stored along with the player object. This identifier is also returned to the object on the client-side. You can access it directly using `Id` property. This is available in the `player` object after a successful save.

### Retrieving

You can retrieving an existing object instance from the backend via its type and unique id.

```csharp
// retrieve the player
var id = "12345";
var player = await APObjects.GetAsync("player", id);
```

If you want to update an object and need a reference to it without making a call to the server, you can create a new instance by passing the type and id in the constructor. This instance is a loose reference to the object on the server side without actually making a call to the backend.

```csharp
// Retrieve the player
// The existingPlayer object represents the player object with id 12345.
var existingPlayerId = "12345";
var existingPlayer = new APObject("player", existingPlayerId);

```

**Note**:  You can mention exactly which all fields you want returned so as to reduce the overall payload size. By default all fields are returned. Fields `Id` and `Type` are the fields which will always be returned. 

```csharp
// You can choose to get specific fields only by passing the fields needed.
// The following sample will get only the name and age fields.
var player = await APObjects.GetAsync("player", "12345", new[] { "name", "age" });
```
You can also retrieve multiple objects at a time, which will return an array of `APObject` instances. Here's an example

```csharp
var ids = new [] {"14696753262625025", "14696753262625026", "14696753262625027"};
var fields = new[] { "name", "age" };       // optional fields to get
var players = await APObjects.MultiGetAsync("player", ids, fields);


```
### Updating

You can update an existing object via the SaveAsync() instance method on `APObject`.
Saving an object will update the instance with the latest values from the server.

```csharp
// Get existing player
var player = await APObjects.GetAsync("player", "12345");
// Update name and age
player.Set("name", "Jack");
player.Set("age", 24);
// Save
await player.SaveAsync();

```
As you might notice, updates and creates are both done via the `SaveAsync` method. The SDK combines the create operation and the update operation under the hood and provides a unified interface. This is done be detecting the presence of a non-zero `Id` property. 
This also means that the `Id` property on all APObjects or derived types are immutable.

#### Handling concurrent updates

It is possible that multiple clients may be updating the same object instance on the backend. To prevent any accidental bad writes, Appacitive provides a revision number based [Multi Version Concurrency Control (MVCC)][3] mechanism.

Each object instance has a integer based Revision property which is internally incremented on each update. If you want to ensure that you are always updating the correct object, you can pass the current revision number of the object during the update. If this revision number matches the revision on the server, the update will be allowed. In case the revision number does not match, it implies that the object has changed since you last read it. Accordingly the update operation will be cancelled.

```csharp
// Get existing player (say at revision # 3 on the server)
var player = await APObjects.GetAsync("player", "12345");
// Update name and age
player.Set("name", "Jack");
player.Set("age", 24);

// Save (this call will succeed)
var revision = player.Revision;     // revision = 3     
await player.SaveAsync(revision);

// Save again with old revision (this call will fail as revision number is now 4)
await player.SaveAsync(revision);
```

### Deleting

You can delete an APObject via the `DeleteAsync` method on the `APObjects` helper class. Let's say we've had enough of John Doe and delete his player account from the server. Here's how you can do that -

```csharp
// This will delete player with id 12345.
await APObjects.DeleteAsync("player", "12345");
```

You can also delete object along with all its first degree [connections](#connections).
Note that this will only delete the connections and not the connected objects.
```csharp
var deleteConnections = true;
await APObjects.DeleteAsync("player", "12345", deleteConnections);
```
Multiple objects can also be deleted at a time. The delete connections options is not available for multi deletes. Here's an example
```csharp
var ids = var ids = new [] {"14696753262625025", "14696753262625026"};
await APObjects.MultiDeleteAsync("player", ids);
```                                                        

----------

## Connections

All data that resides in the Appacitive platform is relational, like in the real world. This means you can do operations like fetching all games that any particular player has played, adding a new player to a team or disbanding a team whilst still keeping the other teams and their `players` data perfectly intact.

Two entities can be connected via a relation, for example two entites of type `person` might be connected via a relation `friend` or `enemy` and so on. An entity of type `person` might be connected to an entity of type `house` via a relation `owns`. 

One more thing to understand is the concept of labels. Consider an entity of type `person`. This entity is connected to another `person` via relation `marriage`. Within the context of the relation `marriage`, one person is the `husband` and the other is the `wife`. Similarly the same entity can be connected to an entity of type `house` via the relation `house_owner`. In context of this relation, the entity of type `person` can be referred to as the `owner`. 

`Wife`, `husband` and `owner` from the previous example are `labels`. Labels are used within the scope of a relation to give contextual meaning to the entities involved in that relation. They have no meaning or impact outside of the relation.

Let's jump in!


### Creating &amp; Saving

The SDK provides a very fluent interface for creating connections.
You can mix and match various options, to create a connection for your specific application scenario. 

#### New Connection between two existing Objects

Before we go about creating connections, we need two entities. Consider the following

```csharp
var idForJohn = "12345";
var idForJane =  "39209";
// initialize and set up a connection
// This will setup a connection of type sibling between the objects
// with the given ids.
var conn = await APConnection
                .New("marriage")
                .FromExistingObject("husband", idForJohn)
                .ToExistingObject("wife", idForJane)
                .SaveAsync();

```

If you've read the previous guide, most of this should be familiar. What happens in the `APConnection` class is that the relation is configured to actually connect the two entities. We provide the ids of the two entities to be connected and specify which is which. For example here, John is the husband and Jane is the wife. 

In case you are wondering why this is necessary then here is the answer, it allows you to structure queries like 'who is John's wife?' and much more. Queries are covered in later guides.

`conn` is an instance of `APConnection` of type `marriage`. Similar to an entity, you may call `ToJSON` on a connection to get a json representation of the connection.

#### New Connection between two new Objects

Apart from connecting existing objects, you can also create and connect new objects in one go. For example, say I want to create an order and its invoice in one go.

```csharp
// Create new objects for order and invoice
var order = new APObject("order");
order.Set("order_number", 747383);
var invoice = new APObject("invoice");
invoice.Set("invoice_date", DateTime.Now);

// Create both objects and connect them with a connection of type invoices
await APConnection.New("invoices")
            .FromNewObject("order", order)
            .ToNewObject("invoice", invoice)
            .SaveAsync();
Console.WriteLine("Order id: {0}", order.Id);
Console.WriteLine("Invoice id: {0}", invoice.Id);
```

This is the recommended way to do it. In this case, the invoices relation will create the objects order and invoice first and then connect them using the relation `invoices`.

As you can probably guess from the examples above, you can change `FromNewObject` to `FromExistingObject` and `ToNewObject` to `ToExistingObject`, depending on whether you are connecting an new object or an existing one.

#### Connection properties
Like in the case of objects, connections can have user defined properties and attributes as well. For example, a relation of type `employed` between a `user` and a `company` can contain a property called `joining_date`. This makes sense since the joining date is not a property of the user or the company in isolation. 

Properties in connections work exactly in the same way as they work in objects.

```csharp
// This works exactly the same as in case of your standard objects.
// Setting property values
APConnection employed;
employed.Set("joining_date", DateTime.Now);

// Reading property values
var joiningDate = employed.Get<DateTime>("joining_date");
```
### Retrieving

#### Get Connection by Id

```csharp
var conn = await APConnections.GetAsync("employed", "123456");
```
The `APConnection` object is similar to the `APObject`, except you get one new field viz. `Endpoints`. Endpoints represent the two objects that the connection links.

```csharp
var idForJohn = "12345";
var idForJane =  "39209";
var conn = await APConnection
                .New("marriage")
                .FromExistingObject("husband", idForJohn)
                .ToExistingObject("wife", idForJane)
                .SaveAsync();
                
var john = await conn.Endpoints["husband"].GetObjectAsync();
var jane = await conn.Endpoints["wife"].GetObjectAsync();
```

#### Get Connected Objects

Consider you want to get a list of all people who are friends of John. Essentially what you want is all objects of type `person` who are connected to John via the relation 'friend'. 

```csharp
var john = new APObject("person", "12345");
// Returns a paged list of friends. 
// We are assuming that the labels for the friend relationship is the same for both endpoints as a friend relationship is not directed.
var friends = await john.GetConnectedObjectsAsync("friend", pageSize:200);
// Write all names to console.
friends.ForEach( f => Console.WriteLine( f.Get<string>("name"));
// Get next page.
var nextPage = await friends.NextPageAsync();
```

NOTE: Incase the relation you are querying is between the same type with different labels, then you must provide the label of the object that you are querying. Example, consider the relationship `father` between `person` objects. In this relationship, the same object can participate as the `father` as well as the `child` with different objects. When querying the `father` connection for John, you will need to specify the label as `father` to get a list of all John's children.

In all other scenarios, passing a label is optional.

#### Get all Connections for an Endpoint Object Id

Scenarios where you may need to just get all connections of a particular relation for an objectId, this query comes to rescue.

Consider `Jane` is connected to some objects of type `person` via `invite` relationship, that also contains a `bool` property viz. `attending`,  which is false by default and will be set to true if that person is attending marriage.

Now she wants to know who all are attending her marriage without actually fetching their connected `person` object, this can be done as

```javascript
//set an instance of person Object for Jane 
var jane = new Appacitive.Object({ __id : '123345456', __type : 'person');

//call fetchConnectedObjects with all options that're supported by queries syntax
// we'll cover queries in dept in next section
var query = jane.getConnections({
  relation: 'invite', //mandatory
  label: 'invitee', //mandatory
  filter: Appacitive.Filter.Property('attending').equalTo(true)
});

query.fetch().then(function(invites) {
  //invites is an array of connections
  console.log(invites);
});
```

In this query, you provide a relation type (name) and a label of opposite side whose conenction you want to fetch and what is returned is a list of all the connections for above object. 

#### Get Connection by Endpoint Object Ids

Appacitive also provides a reverse way to fetch a connection  between two objects.
If you provide two object ids of same or different type types, all connections between those two objects are returned.

Consider you want to check whether `Tarzan` and `Jane` are married, you can do it as
```javascript
//'marriage' is the relation between person type
//and 'husband' and 'wife' are the endpoint labels
var query = Appacitive.Connection.getBetweenObjectsForRelation({ 
    relation: "marriage", //mandatory
    objectAId : "22322", //mandatory 
    objectBId : "33422", //mandatory
    label : "wife" //madatory for a relation between same type and differenct labels
});

query.fetch().then(function(marriage){
  if(marriage != null) {
      // connection obj is returned as argument to onsuccess
      alert('Tarzan and jane are married at location ', marriage.get('location'));
    } else {
      alert('Tarzan and jane are not married');
    }
});

//For a relation between same type type and differenct endpoint labels
//'label' parameter becomes mandatory for the get call

```

Conside you want to check that a particular `house` is owned by `Jane`, you can do it by fetching connection for relation `owns_house` between `person` and `house`.
```javascript
var query = Appacitive.Connection.getBetweenObjectsForRelation({ 
    relation: "owns_house", 
    objectAId : "22322", // person type entity id
    objectBId : "33422" //house type entity id
});

query.fetch().then(function(obj) {
    if(obj != null) {
      alert('Jane owns this house');
    } else {
      alert("Jane doesn't owns this house");
    }
});
```

#### Get all connections between two Object Ids

Consider `jane` is connected to `tarzan` via a `marriage` and a `freind` relationship. If we want to fetch al connections between them we could do this as

```javascript
var query = Appacitive.Connection.getBetweenObjects({
  objectAId : "22322", // id of jane
    objectBId : "33422" // id of tarzan
});

query.fetch().then(function(connections) {
  console.log(connections);
});
```
On success, we get a list of all connections that connects `jane` and `tarzan`.

#### Get Interconnections between one and multiple Object Ids

Consider, `jane` wants to what type of connections exists between her and a group of persons and houses , she could do this as
```javascript
var query = Appacitive.Connection.getInterconnects({
  objectAId: '13432',
    objectBIds: ['32423423', '2342342', '63453425', '345345342']
});

query.fetch().then(function(connections) {
  console.log(connections);
}, function(err) {
  alert("code:" + err.code + "\nmessage:" + err.message);
});
```

This would return all connections with object id 13432 on one side and '32423423', '2342342', '63453425' or '345345342' on the other side, if they exist.

### Updating


Updating is done exactly in the same way as entities, i.e. via the `save()` method. 

*Important*: Updating the endpoints (the `__endpointa` and the `__endpointb` property) will not have any effect and will fail the call. In case you need to change the connected entities, you need to delete the connection and create a new one. 
```javascript
marriage.set('location', 'Las Vegas');

marriage.save().then(function(obj) {
    alert('saved successfully!');
});
```
As before, do not modify the `__id` property.

 
### Deleting

Deleting is provided via the `del` method.
```javascript
marriage.destroy().then(function() {
  alert('Tarzan and Jane are no longer married.');
});


// Multiple coonection can also be deleted at a time. Here's an example
Appacitive.Object.multiDelete({   
  relation: 'freinds', //name of relation
  ids: ["14696753262625025", "14696753262625026", "14696753262625027"], //array of connection ids to delete
}).then(function() { 
  //successfully deleted all connections
});
```

----------

## Queries

Queries provide a mechanism to search your application data.
All searching in SDK is done via `Appacitive.Sdk.Query` object. You can retrieve many objects at once, put conditions on the objects you wish to retrieve, and much more.

The following basic uqery apis are available inside the SDK. Queries apart from these can be made using the Graph api feature discussed later.

#### Query api

```csharp
/// Find all objects of a given type with the given filters.
Task<PagedList<APObject>> FindAllAsync(
    string type,                                // type to query
    IQuery query = null,                        // query filter
    IEnumerable<string> fields = null,          // fields to retrieve
    int page = 1,                               // page number
    int pageSize = 20,                          // page size
    string orderBy = null,                      // sort field
    SortOrder sortOrder = SortOrder.Descending  // sort order
    )

/// Find all objects connected to the given object with the given filters.
Task<PagedList<APObject>> GetConnectedObjectsAsync(
string relation,                                // relation to query
    string query = null,                        // query filter
    string label = null,                        // label of the object 
    IEnumerable<string> fields = null,          // fields to retrieve
    int pageNumber = 1,                         // page number
    int pageSize = 20,                          // page size
    string orderBy = null,                      // sort field
    SortOrder sortOrder = SortOrder.Descending) // sort order
```

As an example, to find all people with first name as John, we would do the following.

```csharp
List<APObject> allMatches = new List<APObject>();
var query = Query.Property("firstname").IsEqualTo("john");
// Run query. Returns a paged response with pagesize 200.
var matches = await APObjects.FindAllAsync("person", query, pageSize: 200);
allMatches.AddRange(matches);
// Incase more records are present, paginate
while( !matches.IsLastPage )
{
    matches = await matches.GetNextPageAsync();
    allMatches.AddRange(matches);
}
```

### Modifiers

Notice the `page`, `pageNumber`, `orderBy`, `sortOrder`, `query`, and `fields`? These're the optional parameters that you can specify in a query. Lets get to them one by one.

#### Pagination

All queries on the Appacitive platform support pagination and sorting. To specify pagination and sorting on your queries, simply pass the `page`, `pagesize`. The resulting object also contains custom properties like `IsLastPage` as well as methods like `GetNextPageAsync()` to help you get to the next page without having to pass all your parameters again.

#### Sorting
Query results can be sorted using the `orderBy` and `sortOrder` parameters.
The `orderBy` parameter takes the name of the property on which to sort and the `sortOrder` parameter lets you select the order of sorting (ascending or descending).

#### Fields

As in the case of single and multi get apis, you can choose the exact set of fields that you want the api to return. Pass in an array of the properties to be returned in the `fields` parameter to apply this modifier to the query results.

#### Filters

Filters are useful for fine tuning the results of your search. Objects and connections inside Appacitive have 4 different types of data, namely - properties, attributes, aggregates and tags. Filters can be applied on each and every one of these. Combinations of these filters is also possible.

The `Query` object provides a factory for creating filters for appacitive without having to learn the specialized query format used by the underlying REST api.
The typical format for the Query helper class is 
```csharp
Query.{Property|Attribute|Aggregate}("name").<Condition>(condition args);
```
Some sample examples of how it can be used are 
```csharp 
// To query on a property called firstname
var query = Query.Property("firstname").IsEqualTo("John");

// To query on an attribute called nickname
var query = Query.Attribute("nickname").IsEqualTo("John");

// To query on an aggregate called avg_rating
var query = Query.Aggregate("avg_rating").IsGreaterThan(4.5);
```
In response it returns you an `IQuery` object, which encapsulates the specified filter in object form. 


#### List of supported conditions

| Condition | Sample usage |
| ------------- |:-----| 
| **Geography properties** |
| WithinPolygon() | ``` Query.Property("location").WithinPolygon(geocodes); ```
| WithinCircle() | ```Query.Property("location").WithinCircle(geocode, radius); ```|
| **String properties** |
| StartsWith() | ```Query.Property("name").StartsWith("Ja"); ```|
| Like()| ```Query.Property("name").Like("an"); ```|
| FreeTextMatches()**   | ```Query.Property("description").FreeTextMatches("roam~0.8"); ```|
| EndsWith() | ```Query.Property("name").EndsWith("ne"); ``` |
| IsEqualTo() | ```Query.Property("name").IsEqualTo("Jane"); ``` |
| **Text properties** |
| FreeTextMatches()**   |```Query.Property("description").FreeTextMatches("roam~0.8"); ```|
| **Time properties ** |
| BetweenTime() | ```Query.Property("start_time").BetweenTime(startDate, endDate);``` |
| IsEqualToTime() | ```Query.Property("start_time").IsEqualToTime(DateTime.Now);``` |
| IsLessThanTime() | ```Query.Property("start_time").IsLessThanTime(DateTime.Now);``` |
| IsLessThanEqualToTime() | ```Query.Property("start_time").IsLessThanEqualToTime(DateTime.Now);``` |
| IsGreaterThanTime()| ```Query.Property("start_time").IsGreaterThanTime(DateTime.Now);``` |
| IsGreaterThanEqualToTime()| ```Query.Property("start_time").IsGreaterThanEqualToTime(DateTime.Now);``` |
| **Date properties ** |
| BetweenDate() | ```Query.Property("start_at").BetweenDate(startDateTime, endDateTime);``` |
| IsEqualToDate()| ```Query.Property("start_at").IsEqualToDate(DateTime.Now);``` |
| IsLessThanDate() | ```Query.Property("start_at").IsLessThanDate(DateTime.Now);``` | ```Query.Property("start_at").IsLessThanDate(DateTime.Now);``` |
| IsLessThanEqualToDate() | ```Query.Property("start_at").IsLessThanEqualToDate(DateTime.Now);``` |
| IsGreaterThanDate() | ```Query.Property("start_at").IsGreaterThanDate(DateTime.Now);``` |
| IsGreaterThanEqualToDate() | ```Query.Property("start_at").IsGreaterThanEqualToDate(DateTime.Now);``` |
| **Datetime, int and decimal properties** ||
| IsLessThan() | ```Query.Property("field").IsLessThan(value);``` |
| IsLessThanEqualTo() | ```Query.Property("field").IsLessThanEqualTo(value);``` |
| Between() | ```Query.Property("field").Between(start, end);``` |
| IsGreaterThanEqualTo() | ```Query.Property("field").IsGreaterThanEqualTo(value);``` |
| IsGreaterThan() | ```Query.Property("field").IsGreaterThan(value);``` |
| IsEqualTo() | ```Query.Property("field").IsEqualTo(value);``` |
** Supports [Lucene query parser syntax][4] 

#### Geolocation

You can specify a property type as a geography type for a given type or relation. These properties are essential latitude-longitude pairs. Such properties support geo queries based on a user defined radial or polygonal region on the map. These are extremely useful for making map based or location based searches. E.g., searching for a list of all restaurants within 20 miles of a given users locations.

##### Radial Search

A radial search allows you to search for all records of a specific type which contain a geocode which lies within a predefined distance from a point on the map. A radial search requires the following parameters.

```csharp
//create a new Geocode object
var center = new Geocode(36.1749687195m, -115.1372222900m);
//create filter
var query = Query.Property("location").WithinCircle(center, 20.0m, DistanceUnit.Miles);
//create query object
var restaurants = await APObjects.FindAllAsync("restaurant", query);
```

##### Polygon Search

A polygon search is a more generic form of geographcal search. It allows you to specify a polygonal region on the map via a set of geocodes indicating the vertices of the polygon. The search will allow you to query for all data of a specific type that lies within the given polygon. This is typically useful when you want finer grained control on the shape of the region to search.

```csharp
//create geocode objects
var pt1 = new Geocode(36.1749687195m, -115.1372222900m);
var pt2 = new Geocode(34.1749687195m, -116.1372222900m);
var pt3 = new Geocode(35.1749687195m, -114.1372222900m);
var pt4 = new Geocode(36.1749687195m, -114.1372222900m);

//create polygon filter
var query = Query.Property("location").WithinPolygon(pt1, pt2, pt3, pt4);

//create query object
var restaurants = await APObjects.FindAllAsync("restaurant", query);
```

#### Tag Based Searches

The Appacitive platform provides inbuilt support for tagging on all data (objects, connections, users and devices). You can use this tag information to query for a specific data set. The different options available for searching based on tags is detailed in the sections below.

##### Query data tagged with one or more of the given tags

For data of a given type, you can query for all records that are tagged with one or more tags from a given list. For example - querying for all objects of type message that are tagged as personal or private.

```csharp
// Create the query
var query = Query.Tags.MatchOneOrMore("personal", "private");
// Will return messages tagged with either personal or private or both.
var messages = await APObjects.FindAllAsync("message", query);
```

##### Query data tagged with all of the given tags

An alternative variation of the above tag based search allows you to query for all records that are tagged with all the tags from a given list. For example, querying for all objects of type message that are tagged as personal AND private.

```csharp
// Create the query 
var query = Query.Tags.MatchAll("personal", "private");
// Will return messages tagged with both personal and private.
var messages = await APObjects.FindAllAsync("message", query);
```

#### Compound Queries

Compound queries allow you to combine multiple queries into one single query. The multiple queries can be combined using `Query.And` and `Query.Or` operators. 
`NOTE`: All types of queries with the exception of free text queries can be combined into a compound query.

```csharp
/* 
Find all users
- whose first name is John or Jane and 
- who stay within 20 mile radius of the empire state building.
*/
var locationOfEmpireState = new Geocode(40.7484m, 73.9857m);
var query = Query.And(
                Query.Or(
                    Query.Property("firstname").IsEqualTo("john"),
                    Query.Property("firstname").IsEqualTo("jane")
                    ),
                Query.Property("location").WithinCircle(locationOfEmpireState, 20.0m, DistanceUnit.Miles)
                );

var matches = await APObjects.FindAllAsync("person", query);
```

#### FreeText

There are situations when you would want the ability to search across all text content inside your data. Free text queries are ideal for implementing this kind of functionality. As an example, consider a free text lookup for users which searches across the username, firstname, lastname, profile description etc. 

`NOTE`: Free text queries support the [Lucene query parser syntax][4]  for free text search. 

```csharp
var places = await APObjects.FreeTextSearchAsync("places", "+champs +palais", pageSize:200);
```

----------

## Graph Search

Appacitive graph queries offer immense potential when it comes to filtering and retreiving connected data. There are two kinds of graph operations, graph queries and graph apis.

### Creating graph queries

You can create graph queries and graph apis from the management portal. When you create such queries from the portal, you are required to assign a unique name with every saved search query. You can then use this name to execute the query from your app by making the appropriate api call to Appacitive.

### Executing saved graph queries

You can execute a saved graph api (query or api) by using it’s name that you assigned to it while creating it from the management portal. You will need to send any placeholders you might have set up while creating the query as a list of key-value pairs in the body of the request. 

```csharp

// Name of graph query
var graphQueryName = "find_connected_users";  
// any placeholders if provided : optional
var placeHolders = new Dictionary<string, string>
                    {
                        {"key1", "value1"},
                        {"key2", "value2"}
                    };
var matchingIDs = await Graph.Query(graphQueryName, placeHolders);
```

### Executing saved graph api

Executing saved graph apis works the same way as executing saved graph queries. The only difference is that you also need to pass the initial ids as an array of strings to feed the api. The response to a graph api will depend on how you design your graph api. Do test them out using the query builder from the query tab on the management portal and from the test harness.

```csharp

// Name of graph projection query
var graphApiName = "get_my_profile";
// List of ids for which to run the graph api
var userIds = ["34912447775245454", "34322447235528474", "34943243891025029"];
// any placeholders if provided : optional
var placeHolders = new Dictionary<string, string>
                    {
                        {"key1", "value1"},
                        {"key2", "value2"}
                    };
GraphNode[] results = await Graph.Select(graphApiName, userIds, placeHolders);
```
-----------

## User Management

Users represent your app's users. There is a host of different functions/features available in the SDK to make managing users easier. The `APUser` and `APUsers` types deals with user management.

### Create

There are multiple ways to create users.

#### Basic

You create users the same way you create any other data.
```csharp
// set the fields
var user = new APUser();
user.Username = "john.doe";
user.Password = "p@ssw0rd";
user.Email = "john.doe@appacitive.com";
user.FirstName = "John";
user.LastName = "Doe";
// Save the user.
await user.SaveAsync();
```

### Retrieve

There are three ways you could retreive the user

#### By id.
Fetching users by `Id` is exactly like fetching objects and connections. Let's say you want to fetch user with `Id` 12345.
```csharp
var user = await APUsers.GetByIdAsync("12345");
```
**Note**: The APUser class is a subclass of APObject. As a result, all operations supported by `APObject`, can be performed on `APUser` object. So, above data documenation is valid for users too.

#### By username

```csharp
//fetch user by username
var user = await APUsers.GetByUsernameAsync("john.doe");
```
### Update
There's no difference between updating a user and updating an APObject. Updates can be applied to the backend via the `SaveAsync` method.

```csharp
var user = await APUsers.GetByIdAsync("12345");
user.Set("email", "john.doe2@appacitive.com");
await user.SaveAsync();
```

### Delete
You can delete an existing user via the `DeleteUserAsync` method on the APUsers helper class.

```csharp
var userId = "12345";
await APUsers.DeleteUserAsync(userId);
```

### Authentication

Authentication is the core of user management. You can authenticate (log in) users in multiple ways. Once the user has authenticated successfully, you will be provided the user's details and an access token. This access token identifies the currently logged in user session. 
You can also initialize the app with this token. This will send this token with every api call. This way the api can infer that the given call is made in the context of the logged in user. Access control rules may also dictate the need to send this token.

```csharp
// To initialize the app with an existing token.
var token = "1lqsljkasldjalsu1....";
await App.LoginAsync(new UserTokenCredentials(token));

// To authenticate and initialize the app with the logged in user.
var usernamePassword = new UsernamePasswordCredentials(username, password);
await App.LoginAsync(usernamePassword);
```

#### Login via username + password

You can ask your users to authenticate via their username and password.
```csharp
// To simply authenticate the username and password
var credentials = new UsernamePasswordCredentials(username, password);
var userSession = await credentials.AuthenticateAsync();
Console.WriteLine("Logged in user: {0}", userSession.LoggedInUser.Username);
Console.WriteLine("User token: {0}", userSession.UserToken);
```
You can also authenticate the user and set the user as the logged in user for the app in one go.

```csharp
var usernamePassword = new UsernamePasswordCredentials(username, password);
await App.LoginAsync(usernamePassword);
```

#### Login with Facebook

You can ask your users to log in via facebook. To do this, you will need to implement the facebook oauth handshake in your app to get the user access token.
You can use the facebook sdk specific to your platform for this purpose.
Once you have the access token, simply create a new instance of an `OAuth2Credentials` object to authenticate.

```csharp
var authType = "facebook";
var facebookAccessToken = "laksjalsjd... "; // Facebook access token from oauth handshake
var facebookCredentials = new OAuth2Credentials(
    facebookAccessToken, 
    authType);
facebookCredentials.CreateUserIfNotExists = true;  // This will also create a user if it does not existing in the system.
var session = await facebookCredentials.AuthenticateAsync();

// Alternatively, you could have used the following as well.
await App.LoginAsync(facebookCredentials);

```

#### Login with Twitter

You can ask your users to log in via Twitter. This'll require you to implement twitter login and provide the SDK with consumerkey, consumersecret, oauthtoken and oauthtokensecret
```csharp

//For login with twitter, pass twitter credentials to SDK
var twitterCredentials = new OAuth1Credentials(
    consumerKey, 
    consumerSecret, 
    accessToken, 
    accessSecret, 
    "twitter");
twitterCredentials.CreateUserIfNotExists = true;
var session = await twitterCredentials.AuthenticateAsync();
```

#### Current User

Whenever you authenticate a user using the `App.LoginAsync()` method, the user is stored in the platform specific local environment and can be retrieved using `App.Current.GetCurrentUser()`.

```csharp
var userInfo = App.Current.GetCurrentUser();
// If user is not logged in, userInfo would be null.
if( userInfo != null )
{
    var userToken = userInfo.UserToken;
    var loggedInUser = userInfo.User;
}
```
You can clear the current logged in user by calling `App.LogoutAsync()` method.
```csharp
await App.LogoutAsync();
```

### Password Management

#### Reset Password

Users often forget their passwords for your app. So you are provided with an API to reset their passwords. This api emails a single use url for resetting the password to the user's email address. 

```csharp
await APUsers.InitiateResetPasswordAsync(
    username,           // username of the user
    emailSubject        // subject of the password reset email (optional)
    );      
```

----------

## Emails

### Configuring

Sending emails from the sdk is quite easy. There are primarily two types of emails that can be sent

* Raw Emails
* Templated Emails

Before you get to sending emails, you need to configure smtp settings. You can either configure it from the portal or in the api call with your mail provider's settings. To send emails via the SDK use the Email class. Alternatively, you can use the NewEmail fluent interface for sending emails as well, as shown below.

### Sending Raw Emails

A raw email is one where you can specify the entire body of the email. An email has the structure
```csharp
var to = new [] {"email1", "email2"..}
var cc = new [] {"email1", "email2"..}
var bcc = new [] {"email1", "email2"..}

await NewEmail
    .Create("This is a raw email test from the .NET SDK.")
    .To(to, cc, bcc)
    .From("from@email.com", "replyto@email.com)
    .WithBody("This is a raw body email.")
    .SendAsync();
```

### Sending Templated Emails

You can also save email templates in Appacitive and use these templates for sending mails. The template can contain placeholders that can be substituted before sending the mail. 

For example, if you want to send an email to every new registration, it is useful to have an email template with placeholders for username and confirmation link.

Consider we have created an email template called `welcome_email` with the following content.

```csharp
"Welcome [#username] ! Thank you for downloading [#appname]."
```
Here, [#username] and [#appname] denote the placeholders that we would want to substitute while sending an email. To send an email using this tempate, you would use something like this.

```csharp
var to = new [] {"email1", "email2"..}
var cc = new [] {"email1", "email2"..}
var bcc = new [] {"email1", "email2"..}

// Sending out a templated email
await NewEmail
    .Create("Thank you for downloading our app")
    .To(to, cc, bcc)
    .From("from@email.com", "replyto@email.com")
    .WithTemplateBody( "welcome_email", 
        new Dictionary<string, string> 
        {
            {"username", "john.doe"},
            {"appname", "appacitive"}
        })
    .SendAsync();
```
`Note`: Emails are not transactional. This implies that a successful send operation would mean that your email provider was able to dispatch the email. It DOES NOT mean that the intended recipient(s) actually received that email.

----------

## Push Notifications

Using Appacitive platform you can send push notification to iOS devices, Android base devices and Windows phone.
 
You will need to provide some basic one time configurations like certificates, using which we will setup push notification channels for different platforms for you. Also we provide a Push Console using which you can send push notification to the users.

In the .NET SDK, the static object `PushNotification` provides methods to send push notification.

Appacitive provides five ways to select the recipients

* Broadcast
* Platform specific Devices
* Specific List of Devices
* To List of Channels
* Query

First we'll see how to send a push notification and then we will discuss the above methods with their options one by one.

### Broadcast

If you want to send a push notification to all active devices, you can use the following options

```csharp
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

### Platform specific Devices

If you want to send push notifications to specific platforms, you can use this option. To do so you will need to provide the devicetype in the query.

```csharp
await PushNotification
    // Send to specific device types
    .ToQueryResult( Query.Property("devicetype").IsEqualTo("ios"))
    // Increment existing badge by 1
    .WithBadge("+1")
    // Custom data field1 and field2
    .WithData(new { field1 = "value1", field2 = "value2" })
    // Expiry in seconds
    .WithExpiry(1000)
    .SendAsync();
```

### Specific List of Devices

If you want to send push notifications to specific devices, you can use this option. To do so you will need to provide the device ids.

```csharp
var deviceIDs = new [] {"id1", "id2",..}; 
await PushNotification
    // Send specific device ids
    .ToDeviceIds("Push from .NET SDK", deviceIDs);
    // Increment existing badge by 1
    .WithBadge("+1")
    // Custom data field1 and field2
    .WithData(new { field1 = "value1", field2 = "value2" })
    // Expiry in seconds
    .WithExpiry(1000)
    .SendAsync();
```

### To List of Channels

Device object has a Channel property, using which you can club multiple devices. This is helpful if you want to send push notification using channel.

```csharp
var channels = new [] {"channel1", "channel2",..}; 
await PushNotification
    // Send specific channels
    .ToChannels("Push from .NET SDK", channels);
    // Increment existing badge by 1
    .WithBadge("+1")
    // Custom data field1 and field2
    .WithData(new { field1 = "value1", field2 = "value2" })
    // Expiry in seconds
    .WithExpiry(1000)
    .SendAsync();
```
### Query

You can send push notifications to devices using a Query. All the devices which comes out as result of the query will receive the push notification.

```csharp
IQuery query =  Query.Property(..;       // create query
await PushNotification
    // Send to results from a query
    .ToQueryResult(query)
    // Increment existing badge by 1
    .WithBadge("+1")
    // Custom data field1 and field2
    .WithData(new { field1 = "value1", field2 = "value2" })
    // Expiry in seconds
    .WithExpiry(1000)
    .SendAsync();
```
### Platform specific options
The `PushNotification` fluent interface provides a `WithPlatformOptions` method to pass different phone platform specific options as shown in the example below.
```csharp
await PushNotification
        // Send specific device ids
        .Broadcast("Hello from the .NET SDK")
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
                        Notification = TileNotification.CreateNewFlipTile( 
                            new FlipTile() { FrontTitle = title, .. } )
                    })
		.SendAsync();


// Tile notification (cyclic tile for wp8, flip tile for others)
await PushNotification
		.Broadcast("message")
		.WithPlatformOptions(
                    new WindowsPhoneOptions
                    {
                        Notification = TileNotification.CreateNewCyclicTile( 
                            new CyclicTile(), new FlipTile() )
                    })
		.SendAsync();

// Tile notification (iconic tile for wp8, flip tile for others)
await PushNotification
		.Broadcast("message")
		.WithPlatformOptions(
                    new WindowsPhoneOptions
                    {
                        Notification = TileNotification.CreateNewIconicTile( 
                            new IconicTile(), new FlipTile() )
                    })
		.SendAsync();

```
----------------

## Files

Appacitive supports file storage and provides api's for you to easily upload and download file. In the background we use amazon's S3 services for persistance. 
All file upload and download operations in the SDK are handled by the `FileUpload` and `FileDownload` classes respectively. 

`NOTE`: All file operations need a file name. This name can be supplied on upload by the user or can be auto generated to be unique by the api. Some important things to note about the file name are -

* It is not mandatory.
* It does not have to be the same is the name of the file being uploaded.
* Incase it is not supplied, it will be auto generated and returned in the response.
* It is the only handle to download the file, so do not lose it.

### Uploading a new file

To upload a new file, create a new instance of the `FileUpload` class with the mime type and optional filename. If you do not supply a file name, an auto generated name will be returned in the response.

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
Incase you want to handle the file upload yourself via some custom control or code, you can generate an upload url which will be available for a limited time period to which you can upload the file. The life time of the upload url can be specified in the api call.

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
To download an existing file from the platform, you need the filename of the file returned from the Upload api. With this filename you can download the file using the `FileDownload` class.

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
All files uploaded to the Appacitive platform are private by default and cannot be accessed via a http GET to a url alone. You can generate a permanent public url for your file or a limited time public url for your file.
To generate a permanently public url for a file, use the `FileDownload.GetPublicUrlAsync()` api. To generate a limited time public url for your file, use the `FileDownload.GetDownloadUrlAsync` method.

`NOTE`:  One important thing to note is that generating a permanently public url for a file marks the file as public. Such files cannot be used to generate limited time public urls any more.

``` C#

// To generate a limited time (10 mins) public url for a file
var filename = ...;							// File name of the file to download.
var expiryInMinutes = 10;	                // Public url that will be active for next 10 mins

// Get limited time public url
string limitedTimePublicUrl = await new FileDownload(filename).GetDownloadUrlAsync(expiryInMinutes);

// Get permanently public url
string permanentPublicUrl = await new FileDownload(filename).GetPublicUrlAsync();

```

----------

## WCF gotchas
When using the SDK on the server side inside a web application or web service, we need to make sure that the ambient user context and sdk state is available on a per request basis instead of being statically stored for the entire application. The SDK uses the WCF OperationContext to store and manage this information on a per request basis.
However given the fact that the SDK methods are async, special provisions need to be made to ensure that the OperationContext is available across threads. To do this, service implementations using the SDK must apply the `AllowAsyncService` service
behavior to their service implementations. This can be done in two ways.

#### 1. Via attribution
``` C#
/*
Add the AllowAsyncService to ensure that OperationContext 
is propogated across async calls.
*/
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
    <!-- incase of using .NET 4.0 version of sdk -->
    <add name="allowAsyncCalls" type="Appacitive.Sdk.Wcf.AllowAsyncServiceBehaviorExtension, Appacitive.Sdk.Net40" />
    <!-- incase of using .NET 4.5 version of sdk -->
    <add name="allowAsyncCalls" type="Appacitive.Sdk.Wcf.AllowAsyncServiceBehaviorExtension, Appacitive.Sdk.Net45" />
</behaviorExtensions>

<!-- Use the extension inside your service / endpoint / operation behavior configuration. -->
```

  [1]: https://www.appacitive.com
  [2]: http://www.apache.org/licenses/LICENSE-2.0.html
  [3]: http://en.wikipedia.org/wiki/Multiversion_concurrency_control
  [4]: http://lucene.apache.org/core/3_0_3/queryparsersyntax.html
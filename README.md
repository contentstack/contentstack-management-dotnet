[![Contentstack](https://www.contentstack.com/docs/static/images/contentstack.png)](https://www.contentstack.com/)

## Contentstack Management .NET SDK

Contentstack is a headless CMS with an API-first approach. It is a CMS that developers can use to build powerful cross-platform applications in their favorite languages. All you have to do is build your application frontend, and Contentstack will take care of the rest. [Read More](https://www.contentstack.com/).

This SDK uses the [Content Management API](https://www.contentstack.com/docs/developers/apis/content-management-api/) (CMA). The CMA is used to manage the content of your Contentstack account. This includes creating, updating, deleting, and fetching content of your account. To use the CMA, you will need to authenticate your users with a [Management Token](https://www.contentstack.com/docs/developers/create-tokens/about-management-tokens) or an [Authtoken](https://www.contentstack.com/docs/developers/apis/content-management-api/#how-to-get-authtoken). Read more about it in [Authentication](https://www.contentstack.com/docs/developers/apis/content-management-api/#authentication).

Note: By using CMA, you can execute GET requests for fetching content. However, we strongly recommend that you always use the [Content Delivery API](https://www.contentstack.com/docs/developers/apis/content-delivery-api/) to deliver content to your web or mobile properties.

### Prerequisite

You need .NET installed on your machine to use the Contentstack .NET CMA SDK.

### Installation
Open the terminal and install the contentstack module via ‘Package Manager’ command
```
PM> Install-Package Contentstack.Management.Core
```
And via ‘.Net CLI’
```
dotnet add package Contentstack.Management.Core
```
To use the module in your application, you need to first Add Namespace to your class
```c#
using Contentstack.Management.Core; // ContentstackClient, ContentstackClientOptions, ContentstackResponse 
using Contentstack.Management.Core.Models; // Stack, ContentType, Entry, Asset
```

### Authentication
To use this SDK, you need to authenticate your users by using the Authtoken, credentials, or Management Token (stack-level token).
### Authtoken
An [Authtoken](https://www.contentstack.com/docs/developers/create-tokens/types-of-tokens/#authentication-tokens-authtokens-) is a read-write token used to make authorized CMA requests, and it is a **user-specific** token.

```c#
var options = new ContentstackOptions()
{
    Host = "<API_HOST>",
    Authtoken = "<AUTHTOKEN>"
}
ContentstackClient client = new ContentstackClient(new OptionsWrapper<ContentstackClientOptions>(options));
```

### Login
To Login to Contentstack by using credentials, you can use the following lines of code:
```c#
    ContentstackClient client = new ContentstackClient();
    NetworkCredential credentials = new NetworkCredential("<EMAIL>", "<PASSWORD>");

    // Sync call
    ContentstackResponse contentstackResponse = client.Login(credentials);

    // Async call
    client.LoginAsync(credentials).ContinueWith((t) => {
        if (t.IsCompleted && t.Status != System.Threading.Tasks.TaskStatus.Faulted)
        {
            // success
        }
    });
    
```

### Management Token
[Management Tokens](https://www.contentstack.com/docs/developers/create-tokens/about-management-tokens/) are **stack-level** tokens, with no users attached to them.
```c#
    Stack stack = client.Stack("<API_KEY>", "<MANAGEMENT_TOKEN>");
```
### Contentstack Management .NET SDK: 5-minute Quickstart
#### Initializing Your SDK:
To use the .Net CMA SDK, you need to first initialize it. To do this, use the following code:
```c#
using Contentstack.Management.Core;

var options = new ContentstackOptions()
    {
        Host = "<API_HOST>",
        Authtoken = "<AUTHTOKEN>"
    }
ContentstackClient client = new ContentstackClient(new OptionsWrapper<ContentstackClientOptions>(options));
```

##### For Setting the branch:
If you want to initialize the SDK in a particular branch, use the following code:
```c#
using Contentstack.Management.Core

ContentstackClient client = new ContentstackClient();
client.Stack("API_KEY", "MANAGEMENT_TOKEN", "BRANCH");
```
##### Proxy Configuration
Contentstack allows you to define HTTP proxy for your requests with the .NET Management SDK. A proxied request allows you to anonymously access public URLs even from within a corporate firewall through a proxy server.

Here is the basic syntax of the proxy settings that you can pass within fetchOptions of the .NET Management SDK:
```c#
using System.Net;
var contentstackConfig = new ContentstackClientOptions();
contentstackConfig.ProxyHost = "http://127.0.0.1";
contentstackConfig.ProxyPort = 9000;
contentstackConfig.ProxyCredentials = new NetworkCredential(userName: "username", password: "password");
ContentstackClient client = new ContentstackClient(new OptionsWrapper<ContentstackClientOptions>(options));
```
#### Fetch Stack Detail
Use the following lines of code to fetch your stack detail using this SDK:
```c#
ContentstackResponse contentstackResponse = client.Stack("<API_KEY>").Fetch();

var response = contentstackResponse.OpenJObjectResponse();
// or
StackResponse model = contentstackResponse.OpenTResponse<StackResponse>();
```
#### Create an Entry
You can use the following code to create an entry in a specific content type of a stack through the SDK:
```c#
EntryModel entry = new EntryModel() {
   Title: 'Sample Entry',
   Url: '/sampleEntry'
}
ContentstackClient client = new ContentstackClient();
Stack stack = client.Stack("'API_KEY'");
ContentstackResponse contentstackResponse = stack.ContentType("CONTENT_TYPE_UID").Entry().Create(entry);
```
#### Upload Assets
Use the following code snippet to upload assets to your stack through the SDK:
```c#
ContentstackClient client = new ContentstackClient();
Stack stack = client.Stack("'API_KEY'");

var path = Path.Combine(Environment.CurrentDirectory, "path/to/file");
AssetModel asset = new AssetModel("'Asset Title", path, "application/json");
ContentstackResponse response = stack.Asset().Create(asset);
```
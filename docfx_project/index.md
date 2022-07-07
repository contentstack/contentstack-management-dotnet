# Contentstack - .Net Management SDK

.NET SDK for Contentstack's Content Delivery API

Contentstack provides a .NET Management SDK (that uses Content Management APIs) that developers can use to manage the content of your Contentstack account. 
This includes creating, updating, deleting, and fetching content of your account.

> Note: The Contentstack .NET Management SDK supports .NET v3.1 or above.
In order to integrate your .NET app with Contentstack .NET Management SDK, follow the steps mentioned in the Get Started guide.
 
 
## Get Started with .NET Management SDK

This guide will help you get started with Contentstack .NET Management SDK (that uses Content Management APIs) to manage apps powered by Contentstack. 
This includes operations such as creating, updating, deleting, and fetching content of your Contentstack account.

### Prerequisite
You need .NET version 3.1 or above installed to use the Contentstack .NET Management SDK.

### Installation
Open the terminal and install the contentstack module via “Package Manager” command:
 ```sh
 PM> Install-Package contentstack.management.core
```
 
And via “.Net CLI”

```sh
dotnet add package contentstack.management.core
```
 
### To import the SDK, use the following code:
``` c#
using Contentstack.Management.Core
ContentstackClient client = new ContentstackClient();
``` 
Or
``` c#
ContentstackClientOptions options = new ContentstackClientOptions();
ContentstackClient client = new ContentstackClient(new OptionsWrapper<ContentstackClientOptions>(options));
``` 
### Authentication
To use the SDK, you need to authenticate users. You can do this by using an authtoken, credentials, or a management token (stack-level token). 
Let's discuss each of them in detail.
#### Authtoken
An authtoken is a read-write token used to make authorized CMA requests, and it is a user-specific token.
``` c#
ContentstackClientOptions options = new ContentstackClientOptions() {
Authtoken: ‘AUTHTOKEN’
};
ContentstackClient client = new ContentstackClient(new OptionsWrapper<ContentstackClientOptions>(options));
 ```
#### Login
To log in to Contentstack, provide your credentials as shown below.
``` c#
NetworkCredential credentials = new NetworkCredential("EMAIL", "PASSWORD");
ContentstackClient client = new ContentstackClient();
try
{
    ContentstackResponse contentstackResponse = client.Login(credentials);
} catch (Exception e)
{
 
}
 ```
#### Management Token
Management tokens are stack-level tokens with no users attached to them.
``` c#
ContentstackClient client = new ContentstackClient();

client.Stack("API_KEY", "MANAGEMENT_TOKEN");
 ```
 
 
### Initialize your SDK
To use the .NET CMA SDK, you need to first initialize it.
``` c#
using Contentstack.Management.Core
ContentstackClientOptions options = new ContentstackClientOptions() {
Authtoken: "AUTHTOKEN"
};
ContentstackClient client = new ContentstackClient(new OptionsWrapper<ContentstackClientOptions>(options));
``` 
### For Setting the branch:
If you want to initialize SDK in a particular branch use the code given below:
``` c#
using Contentstack.Management.Core
 
ContentstackClient client = new ContentstackClient();
client.Stack("API_KEY", "MANAGEMENT_TOKEN", "BRANCH");
 ```
### Proxy Configuration
Contentstack allows you to define HTTP proxy for your requests with the .NET Management SDK. 
A proxied request allows you to anonymously access public URLs even from within a corporate firewall through a proxy server.
Here is the basic syntax of the proxy settings that you can pass within fetchOptions of the .NET Management SDK:
``` c#
var contentstackConfig = new ContentstackClientOptions();
contentstackConfig.ProxyHost = "http://127.0.0.1"
contentstackConfig.ProxyPort = 9000;
contentstackConfig.ProxyCredentials = new NetworkCredential(userName: "username", password: "password");
ContentstackClient client = new ContentstackClient(new OptionsWrapper<ContentstackClientOptions>(options));
 ```
 
### Fetch Stack Details
To fetch your stack details through the SDK, use the following code:
``` c#
using Contentstack.Management.Core
 
ContentstackClient client = new ContentstackClient();
Stack stack = client.Stack("API_KEY");
ContentstackResponse contentstackResponse = stack.Fetch();
var response = contentstackResponse.OpenJObjectResponse();
 ```
### Create an Entry
You can use the following code to create an entry in a specific content type of a stack through the SDK:
``` c#
EntryModel entry = new EntryModel() {
 Title: 'Sample Entry',
 Url: '/sampleEntry'
}
ContentstackClient client = new ContentstackClient();
Stack stack = client.Stack("API_KEY");
ContentstackResponse contentstackResponse = stack.ContentType(“CONTENT_TYPE_UID”).Entry().Create(entry);
 ```
### Upload Assets
Use the following code snippet to upload assets to your stack through the SDK:
``` c#
ContentstackClient client = new ContentstackClient();
Stack stack = client.Stack("API_KEY");
 
var path = Path.Combine(Environment.CurrentDirectory, "path/to/file");
AssetModel asset = new AssetModel("Asset Title", path, "application/json");
ContentstackResponse response = stack.Asset().Create(asset);
 ```
 ## Helpful Links

-   [Contentstack Website](https://www.contentstack.com/)
-   [Official Documentation](https://contentstack.com/docs)
-   [Content Delivery API Docs](https://contentstack.com/docs/apis/content-delivery-api/)

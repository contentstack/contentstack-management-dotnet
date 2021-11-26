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
```
using Contentstack.Management.Core; // ContentstackClient, ContentstackClientOptions, ContentstackResponse 
using Contentstack.Management.Core.Models; // Stack, ContentType, Entry, Asset
```

### Authentication
To use this SDK, you need to authenticate your users by using the Authtoken, credentials, or Management Token (stack-level token).
### Authtoken
An [Authtoken](https://www.contentstack.com/docs/developers/create-tokens/types-of-tokens/#authentication-tokens-authtokens-) is a read-write token used to make authorized CMA requests, and it is a **user-specific** token.

```
var options = new ContentstackOptions()
{
    Host = "<API_HOST>",
    Authtoken = "<AUTHTOKEN>"
}
ContentstackClient client = new ContentstackClient(new OptionsWrapper<ContentstackClientOptions>(options));
```

### Login
To Login to Contentstack by using credentials, you can use the following lines of code:
```
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
```
    Stack stack = client.Stack("<API_KEY>", "<MANAGEMENT_TOKEN>");
```
### Contentstack Management .NET SDK: 5-minute Quickstart
#### Initializing Your SDK:
To use the .Net CMA SDK, you need to first initialize it. To do this, use the following code:
```
using Contentstack.Management.Core;

var options = new ContentstackOptions()
    {
        Host = "<API_HOST>",
        Authtoken = "<AUTHTOKEN>"
    }
ContentstackClient client = new ContentstackClient(new OptionsWrapper<ContentstackClientOptions>(options));
```
#### Fetch Stack Detail
Use the following lines of code to fetch your stack detail using this SDK:
```
ContentstackResponse contentstackResponse = client.Stack("<API_KEY>").Fetch();

var response = contentstackResponse.OpenJObjectResponse();
// or
StackResponse model = contentstackResponse.OpenTResponse<StackResponse>();
```

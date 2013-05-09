        

# Using Web Api for HTTP services

This is an investigation into how [Web Api](http://www.asp.net/web-api) handles various challenges in building HTTP services. 
A flights service has been built to verify and exemplify the various solutions. 

In some places a comparison with [SericeStack](http://www.servicestack.net/) is made.

1.  [IoC](#IOC)
2.  [Testing](#Testing)
3.  [Javascript interoperability](#Javascript)
4.  [Tracing and Profiling](#Tracing)
5.  [Async](#Async)
6.  [Validation](#Validation)
7.  [Authentication and Authorization](#Authentication)
8.  [CORS](#CORS)
9.  [JSONP](#JSONP)
10.  [OData](#OData)
11.  [PATCH](#PATCH)
12.  [Linux](#Linux)
13.  [Composite Services](#CompositeServices)
14.  [Moving from RESTful services](#BeyondRest)

<a name="IOC"></a> 

## 1. IoC

There are two of ways of supporting IoC and Dependency Injection (DI) for Web Api. The first way is to set the IDependencyResolver on 
the HttpConfiguration object however this suffers, as do all Service Locators, by not providing any context to the GetService call which might
be needed to successfully compose the dependency graph. The second way is to replace the default IHttpControllerActivator service. This provides 
an HttpRequestMessage every time a graph should be composed and is therefore a suitable
[composition root](http://blog.ploeh.dk/2011/07/28/CompositionRoot/). Read [this article](http://blog.ploeh.dk/2012/09/28/DependencyInjectionandLifetimeManagementwithASP.NETWebAPI/) for a full discussion.

The example in this repo uses [CastleWindsor](http://docs.castleproject.org/Default.aspx?Page=MainPage&NS=Windsor&AspxAutoDetectCookieSupport=1) 
and the second method of replacing the default IHttpControllerActivator. [This article](http://blog.ploeh.dk/2012/10/03/DependencyInjectioninASP.NETWebAPIwithCastleWindsor/) was used as a guide.        

See the composition root code in [WebApiSetup.cs](https://github.com/megrogan/webapi-example/blob/master/TravelRepublic.Flights.Web/WebApiSetup.cs)
and Dependency Injection here - [FlightsController.cs](https://github.com/megrogan/webapi-example/blob/master/TravelRepublic.Flights.Service/FlightsController.cs).

<a name="Testing"></a> 

## 2. Testing

### Unit testing

Once you use IoC then unit testing becomes relatively straight-forward. It is however a bit tricky to test controller action methods 
that return HttpResponseMessage requiring extra config. Read [this article](http://www.peterprovost.org/blog/2012/06/16/unit-testing-asp-dot-net-web-api) 
for a discussion. See the example unit tests in [FlightsControllerTests.cs](https://github.com/megrogan/webapi-example/blob/master/TravelRepublic.Flights.Service.Test/FlightsControllerTests.cs)
which also test the POST action which returns an HttpResponseMessage.

### Integration testing

Integration testing of Web Api controllers is also straight-forward. They can either be tested against an in-memory HttpServer or against a fully running HttpServer using 
HttpSelfHostServer and both methods have their place - see [here](http://stackoverflow.com/questions/14698130/unit-testing-web-api-using-httpserver-or-httpselfhostserver) for a quick discussion.
The former method is relatively fast and does not involve exercising the full network stack, and as such could easily be run on a build server as part of CI.
It can also serve to test the application routing and IoC setup which is illustrated in [FlightsControllerIntegrationTests.cs](https://github.com/megrogan/webapi-example/blob/master/TravelRepublic.Flights.Web.Test/FlightsControllerIntegrationTests.cs).
This example also demonstrates how a Web Api service can be called by c# client code rather than by js in a browser.

### Testing with Service Stack

With the [new ServiceStack API](https://github.com/ServiceStack/ServiceStack/wiki/New-API) it does not seem so straight-forward to write pure unit tests, now that services generally derive from Service rather than IService. See a discussion [here](https://github.com/ServiceStack/ServiceStack/issues/435).
No pure unit tests are documented on the ServiceStack site, rather [in-memory integration tests](https://github.com/ServiceStack/ServiceStack/wiki/HowTo-write-unit-integration-tests) seem to be the norm.

<a name="Javascript"></a> 

## 3. Javascript interoperability

### Error handling

There are a number of ways of handling errors with Web Api. By default most exceptions are translated into an HTTP response with status code 500. 
In addition you can throw an HttpResponse exception and specify the http status code. You can also use an exception filter to handle any exception, that
is not an HttpResponseException, in a uniform way. Finally you can return an HttpResponseMessage message directly using HttpError to describe the error. 
This allows custom error responses to be created.
Read [here](http://www.asp.net/web-api/overview/web-api-routing-and-actions/exception-handling ) as a starting point.            

In any case the HTTP response has the appropriate status code and the content has details of the error. The content of a standard error response might look 
like this:

<pre>
{ 
    "Message": "No HTTP resource was found that matches the request URI 'http://localhost/Foo'.", 
    "MessageDetail": "No type was found that matches the controller named 'Foo'." 
}</pre>

The response to an exception when error detail is turned on with config.IncludeErrorDetailPolicy is like this:                        

<pre>
{ 
  "Message": "An error has occurred.", 
  "ExceptionMessage": "Index was outside the bounds of the array.", 
  "ExceptionType": "System.IndexOutOfRangeException", 
  "StackTrace": "   at WebApiTest.TestController.Post(Uri uri) in c:\\Temp\\WebApiTest\\WebApiTest\\TestController.cs:line 18
  at System.Web.Http.Controllers.ReflectedHttpActionDescriptor.ActionExecutor.&lt;&gt;c__DisplayClassf.&lt;GetExecutor&gt;b__9(Object instance, Object[] methodParameters)
  at System.Web.Http.Controllers.ReflectedHttpActionDescriptor.ActionExecutor.Execute(Object instance, Object[] arguments)
  at System.Threading.Tasks.TaskHelpers.RunSynchronously[TResult](Func`1 func, CancellationToken cancellationToken)" 
}</pre>

If error detail is turned off then only the Messsage is included in the response content.           

### Playing nicely with Backbone (and other frameworks) and REST conventions

I have not yet implemented the html client which will use backbone to automatically connect to the RESTful Web API to persist flights, 
however I know this is straight-forward. See [here](http://agilesight.com/blogs/pabloc/?p=11) for an example.

<a name="Tracing"></a> 

## 4. Tracing and Profiling

Tracing can be added to Web Api by implementing ITraceWriter and replacing the default SimpleTracer. 
See [here](http://www.asp.net/web-api/overview/testing-and-debugging/tracing-in-aspnet-web-api)
for the Microsoft introduction which includes some performance profiling. 
Sophisticated tracing using Systems.Diagnostocs can also be simply enabled with 
[SystemDiagnosticsTraceWriter](https://github.com/ASP-NET-MVC/aspnetwebstack/blob/master/src/System.Web.Http.Tracing/SystemDiagnosticsTraceWriter.cs) - 
read [here](http://himanshudesai.wordpress.com/2013/02/19/web-api-introduction-to-systemdiagnosticstracewriter/) for an intro. It can
also be seen in action when you run the example 
[integration tests](https://github.com/megrogan/webapi-example/blob/master/TravelRepublic.Flights.Web.Test/FlightsControllerIntegrationTests.cs).
For a discussion of the merits of System.Diagniostics [see here](http://stackoverflow.com/questions/576185/logging-best-practices).    
For completeness [here](http://www.symbolsource.org/Public/Metadata/NuGet/Project/WebApiContrib.Tracing.Log4Net/0.7.0.5/Release/.NETFramework,Version%3Dv4.0/WebApiContrib.Tracing.Log4Net/WebApiContrib.Tracing.Log4Net/Log4NetTraceWriter.cs?ImageName=WebApiContrib.Tracing.Log4Net) 
is an ITraceWriter implementation using log4net.                    

<a name="Async"></a> 

## 5. Async

Asynchronous operations are fully supported by Web Api and the HttpClient - see 
[here](http://www.asp.net/web-api/overview/web-api-clients/calling-a-web-api-from-a-wpf-application) for the Microsoft introduction to 
handling asynchronous operations in HttpClient, and [here](http://blogs.msdn.com/b/henrikn/archive/2012/02/24/async-actions-in-asp-net-web-api.aspx),
[here](http://weblogs.asp.net/andresv/archive/2012/12/12/asynchronous-streaming-in-asp-net-webapi.aspx), 
and [here](http://www.strathweb.com/2013/01/asynchronously-streaming-video-with-asp-net-web-api/),
for async streaming with Web Api.

<a name="Validation"></a> 

## 6. Validation

Validation of incoming data in Web Api is supported by [DataAnnotations]( System.ComponentModel.DataAnnotations) on the DTO.
Read the [Microsoft doc](http://www.asp.net/web-api/overview/formats-and-model-binding/model-validation-in-aspnet-web-api) for an introduction.
You can also configure a filter to return validation errors back to the client in a uniform way for all controllers and actions or you can define
attributes to decorate individual controllers or actions to handle validation errors.             

If you need more sophisitcated DTO validation such as might be required for PATCH requests you can roll your own validation as I have done in the 
[example application](https://github.com/megrogan/webapi-example/blob/master/TravelRepublic.Flights.Service/FlightsController.cs). This 
should really return a more verbose error message detailing the particular validation failure(s).

<a name="Authentication"></a> 

## 7. Authentication and Authorization

Authentication is not provided directly by the Web Api itself. Either authentication is handled by the host in which case you can configure your 
project to use any of the authentication modules built in to IIS or ASP.NET, or write your own HTTP module to perform custom authentication.
Or you can put authentication logic into an HTTP message handler. In that case, the message handler examines the HTTP request and sets the principal.
[Here](http://www.asp.net/web-api/overview/security/authentication-and-authorization-in-aspnet-web-api) is the Microsoft introductory article.

Authorization is supported with Authorization filters and the [Authorize] attribute which can be used to restrict access globally, at the controller level, 
or at the level of individual actions.

[Here](http://vimeo.com/43603474) is a great video about securing Web Apis - the associated code is [here.](http://goo.gl/00Oc2)

<a name="CORS"></a> 

## 8. CORS

Unitl recently you could either roll your own CORS support or use a 3rd party library - 
[here](http://stackoverflow.com/questions/12732147/cors-with-webapi-for-xmlhttprequest) is a good stackoverflow question.

Now, if you have .NET 4.5+, you can get the soon to be built-in CORS support - see 
[here](http://aspnetwebstack.codeplex.com/wikipage?title=CORS%20support%20for%20ASP.NET%20Web%20API "CORS%20support%20for%20ASP.NET%20Web%20API").
In the latest build the EnableCorsAttribute call requires parameters 
- see [WebApiSetup.cs](https://github.com/megrogan/webapi-example/blob/master/TravelRepublic.Flights.Web/WebApiSetup.cs)
for a commented out example which would enable full global CORS access for all controllers in the app.           

<a name="JSONP"></a> 

## 9. JSONP

At the time of writing the Web Api does not have built-in support for jsonp but you can use 3rd party code which extends a JsonMediaTypeFormatter - 
see [here](http://stackoverflow.com/questions/9421312/jsonp-with-asp-net-web-api) for a stackoverflow question on the topic.

In [WebApiSetup.cs](https://github.com/megrogan/webapi-example/blob/master/TravelRepublic.Flights.Web/WebApiSetup.cs) you can
see that a jsonp formatter is added using 
[Rick Strahl's implementation](http://www.west-wind.com/weblog/posts/2012/Apr/02/Creating-a-JSONP-Formatter-for-ASPNET-Web-API).

<a name="OData"></a> 

## 10. OData

In short Web Api supports oData queries by the simple expedient of returning an IQueryable&lt;T&gt; from a controller action. 
See [here](http://www.asp.net/web-api/overview/odata-support-in-aspnet-web-api) for the Microsoft introduction.
See [here](http://stackoverflow.com/questions/9577938/odata-with-servicestack) for an opinion by ServiceStack
on the shortcomings of oData.

<a name="PATCH"></a> 

## 11. PATCH

To build a modestly complex REST API it soon becomes apparent that PATCH semantics are required to partially update an object. 
Without PATCH it is necessary for a client to do a GET followed by a PUT which can be undesirable. 
At the time of writing Patch has been supported with a supplementary package for the Web Api. You can read about it in
[this](http://blogs.msdn.com/b/alexj/archive/2012/08/15/odata-support-in-asp-net-web-api.aspx) MSDN blog post. 
A PATCH command is included in [FlightsController.cs](https://github.com/megrogan/webapi-example/blob/master/TravelRepublic.Flights.Service/FlightsController.cs).

<a name="Linux"></a> 

## 12. Linux

At the time of writing the Web Api is not fully supported in Mono AFAIK.

<a name="CompositeServices"></a> 

## 13. Composite Services

In ServiceStack you can delegate and create composite services using the base.ResolveService&lt;T&gt;() method which returns an auto-wired instance of the selected service.
AFAIK no such capability exists in Web Api.

<a name="BeyondRest"></a> 

## 14. Moving from RESTful services

In ServiceStack it is possible to create services which can both be called via Web Service or via MQ - 
see [here](https://github.com/ServiceStack/ServiceStack/wiki/Messaging-and-redis).
This is not possible with Web Api.            

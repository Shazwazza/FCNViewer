# FCNViewer

Utility to view FCN (File Change Notification), directory and file monitors in ASP.NET web applications

Install via Nuget:

```
Install-Package FCNViewer
```

To enable this you need to enable the FCN Viewer route in your WebApi config:

```cs
config.Routes.MapFcnViewerRoute();
```

then visit `/fcn` to view your FCN settings, this will also format the results as XML or JSON if you request the data with those mime types

alternatively if you want to change the path:

```cs
config.Routes.MapFcnViewerRoute("mycustompath");
```

The FCN watchers will grow depending on your settings and the more your browse your site, compile views, return assets, etc...

You can change the FCN mode in ASP.NET using the web.config setting:

```xml
<!--Default:-->
<httpRuntime targetFramework="4.5" fcnMode="NotSet"/>

<!--<httpRuntime targetFramework="4.5" fcnMode="Single"/>-->
<!--<httpRuntime targetFramework="4.5" fcnMode="Disabled"/>-->
```

For more information about FCN and how it can affect your application see:

* http://shazwazza.com/post/all-about-aspnet-file-change-notification-fcn/

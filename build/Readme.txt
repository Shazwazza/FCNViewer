To enable this you need to enable the FCN Viewer route in your WebApi config:

config.Routes.MapFcnViewerRoute();

then visit /fcn to view your FCN settings, this will also format the results as XML or JSON if you request the data with those mime types

alternatively if you want to change the path:

config.Routes.MapFcnViewerRoute("mycustompath");


The FCN watchers will grow depending on your settings and the more your browse your site, compile views, return assets, etc...
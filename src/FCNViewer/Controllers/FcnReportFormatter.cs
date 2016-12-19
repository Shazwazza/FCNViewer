using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using FCNViewer.Models;
using RazorEngine;
using RazorEngine.Templating; // For extension methods.

namespace FCNViewer.Controllers
{
    /// <summary>
    /// Writes out a text report for FCN vals
    /// </summary>
    public class FcnReportFormatter : BufferedMediaTypeFormatter
    {
        public FcnReportFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
        }

        public override bool CanReadType(Type type)
        {
            return false;
        }

        public override bool CanWriteType(Type type)
        {
            return type == typeof(FcnModel);
        }

        public override void WriteToStream(Type type, object value, Stream writeStream, HttpContent content)
        {
            using (var writer = new StreamWriter(writeStream))
            {
                var fcnModel = value as FcnModel;
                if (fcnModel == null)
                {
                    throw new InvalidOperationException("Cannot serialize type");
                }
                
                Engine.Razor.RunCompile(
                    new LoadedTemplateSource(FCNViewer.ViewsResources.Report),
                    "FcnReport",
                    writer,
                    typeof(FcnModel),
                    fcnModel);                
            }
        }        
    }
}
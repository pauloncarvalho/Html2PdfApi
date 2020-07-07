using iText.Html2pdf;
using iText.Html2pdf.Resolver.Font;
using iText.IO.Font;
using iText.Kernel.Events;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Layout;
using iText.Layout.Element;
using System.IO;

namespace Html2PdfApi.Helpers
{
    public class PdfHtmlHeaderAndFooter
    {
        protected readonly ConvertRequest _convertRequest;

        public PdfHtmlHeaderAndFooter(ConvertRequest convertRequest)
        {
            _convertRequest = convertRequest;
        }

        public MemoryStream ManipulatePdf()
        {
            MemoryStream memoryStream = new MemoryStream();

            ConverterProperties properties = new ConverterProperties();
            DefaultFontProvider defaultFontProvider = new DefaultFontProvider(true, true, true);
            defaultFontProvider.AddFont(FontProgramFactory.CreateFont("./Fonts/calibri.ttf"));
            defaultFontProvider.AddFont(FontProgramFactory.CreateFont("./Fonts/calibril.ttf"));
            properties.SetFontProvider(defaultFontProvider);

            using (var pdfWriter = new PdfWriter(memoryStream))
            {
                pdfWriter.SetCloseStream(false);
                PdfDocument pdfDocument = new PdfDocument(pdfWriter);

                if (!string.IsNullOrEmpty(_convertRequest.Header))
                {
                    Header headerHandler = new Header(_convertRequest.Header);
                    pdfDocument.AddEventHandler(PdfDocumentEvent.START_PAGE, headerHandler);
                }

                if (!string.IsNullOrEmpty(_convertRequest.Footer))
                {
                    Footer footerHandler = new Footer(_convertRequest.Footer);
                    pdfDocument.AddEventHandler(PdfDocumentEvent.END_PAGE, footerHandler);
                }

                Document document = HtmlConverter.ConvertToDocument(_convertRequest.Content, pdfDocument, properties);
                document.Close();
            }

            memoryStream.Position = 0;
            return memoryStream;
        }

        // Header event handler
        protected class Header : IEventHandler
        {
            protected readonly string _header;

            public Header(string header)
            {
                _header = header;
            }

            public virtual void HandleEvent(Event @event)
            {
                PdfDocumentEvent docEvent = (PdfDocumentEvent)@event;

                PdfPage page = docEvent.GetPage();
                Rectangle pageSize = page.GetPageSize();

                Canvas canvas = new Canvas(new PdfCanvas(page), pageSize);

                // Write text at position
                foreach (IElement element in HtmlConverter.ConvertToElements(_header))
                {
                    canvas.Add((IBlockElement)element);
                }
                canvas.Close();
            }
        }

        // Footer event handler
        protected class Footer : IEventHandler
        {
            protected readonly string _footer;

            public Footer(string footer)
            {
                _footer = footer;
            }

            public virtual void HandleEvent(Event @event)
            {
                PdfDocumentEvent docEvent = (PdfDocumentEvent)@event;
                PdfDocument pdf = docEvent.GetDocument();

                PdfPage page = docEvent.GetPage();
                Rectangle pageSize = page.GetPageSize();

                Canvas canvas = new Canvas(new PdfCanvas(page), pageSize);
                canvas.SetFontProvider(new DefaultFontProvider(true, true, true));
                string pageFooter = _footer.Replace("{pageNumber}", pdf.GetPageNumber(page).ToString());

                ConverterProperties properties = new ConverterProperties();
                DefaultFontProvider defaultFontProvider = new DefaultFontProvider(true, true, true);
                defaultFontProvider.AddFont(FontProgramFactory.CreateFont("./Fonts/calibri.ttf"));
                defaultFontProvider.AddFont(FontProgramFactory.CreateFont("./Fonts/calibril.ttf"));
                properties.SetFontProvider(defaultFontProvider);

                foreach (IElement element in HtmlConverter.ConvertToElements(pageFooter, properties))
                {
                    canvas.Add((IBlockElement)element);
                }

                canvas.Close();
            }
        }
    }
}

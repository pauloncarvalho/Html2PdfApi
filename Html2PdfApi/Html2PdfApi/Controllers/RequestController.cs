using Html2PdfApi.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Html2PdfApi.Controllers
{
    [ApiController]
    [Route("/")]
    public class RequestController : ControllerBase
    {
        [HttpGet]
        public FileStreamResult Converter([FromBody] ConvertRequest convertRequest)
        {
            PdfHtmlHeaderAndFooter pdf = new PdfHtmlHeaderAndFooter(convertRequest);
            return new FileStreamResult(pdf.ManipulatePdf(), "application/pdf");
        }
    }
}

using System.Web.Mvc;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.PowerPoint;

namespace AdminPanel.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            Microsoft.Office.Interop.PowerPoint.Application pptApplication = new Microsoft.Office.Interop.PowerPoint.Application();
            Presentation pptPresentation = pptApplication.Presentations
                                            .Open("E:\\Presentation2.pptx", MsoTriState.msoFalse, MsoTriState.msoFalse, MsoTriState.msoFalse);

            for (int i = 0; i < pptPresentation.Slides.Count; i++)
            {
                pptPresentation.Slides[i + 1].Export("E:\\Presentation" + i + ".png", "png");
            }

            pptPresentation.Close();
            return View();
        }
    }
}
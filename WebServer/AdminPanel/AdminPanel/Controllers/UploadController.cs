using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Microsoft.Office.Core;
using Microsoft.Office.Interop.PowerPoint;

namespace AdminPanel.Controllers
{
    public class UploadController : Controller
    {
        // GET: Upload
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Upload()
        {
            if( Request.Files.Count > 0 )
            {
                var file = Request.Files[0];
                if( file != null && file.ContentLength > 0 )
                {
                    var filename = Path.GetFileName(file.FileName);                    
                    var path = Path.Combine(Server.MapPath("~/pptx/"), filename);
                    file.SaveAs(path);

                    convertPPTXToImages(path);
                }
            }
            return RedirectToAction("/Home/Index");
        }

        [HttpPost]
        public string pptxpost(HttpPostedFile profileImage)
        {
            string[] extensions = { ".pptx", ".ppt"};
            if (!extensions.Any(x => x.Equals(Path.GetExtension(profileImage.FileName.ToLower()), StringComparison.OrdinalIgnoreCase)))
            {
                return "";
            }

            // Other code goes here

            return "/path/to/image.png";
        }

        public void convertPPTXToImages(String path)
        {
            var filename = Path.GetFileNameWithoutExtension(path);
            Microsoft.Office.Interop.PowerPoint.Application pptApplication = new Microsoft.Office.Interop.PowerPoint.Application();
            Presentation pptPresentation = pptApplication.Presentations
                                            .Open(path, MsoTriState.msoFalse, MsoTriState.msoFalse, MsoTriState.msoFalse);

            for (int i = 0; i < pptPresentation.Slides.Count; i++)
            {
                var image_path = Path.Combine(Server.MapPath("~/Images/"), filename + "_" + (i + 1) + ".png" );

                pptPresentation.Slides[i + 1].Export(image_path, "png");
            }

            pptPresentation.Close();
        }
    }
}
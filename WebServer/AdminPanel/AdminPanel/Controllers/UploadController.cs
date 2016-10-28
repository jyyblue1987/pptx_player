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
            String[] save_path = null;

            
            if ( Request.Files.Count > 0 )
            {
                var file = Request.Files[0];
                if( file != null && file.ContentLength > 0 )
                {
                    var filename = Path.GetFileName(file.FileName);                    
                    var path = Path.Combine(Server.MapPath("~/pptx/"), filename);
                    file.SaveAs(path);

                    save_path = convertPPTXToImages(path);
                }
            }

            return Json(new { foo = save_path });    
        }



        public String [] convertPPTXToImages(String path)
        {
            var filename = Path.GetFileNameWithoutExtension(path);
            Microsoft.Office.Interop.PowerPoint.Application pptApplication = new Microsoft.Office.Interop.PowerPoint.Application();
            Presentation pptPresentation = pptApplication.Presentations
                                            .Open(path, MsoTriState.msoFalse, MsoTriState.msoFalse, MsoTriState.msoFalse);

            var http = "http://";
            if (Request.IsSecureConnection)
                http = "https://";

            var server_address = http + Request.Url.Host + ":" + Request.Url.Port;

            String[] save_path = new String[pptPresentation.Slides.Count];
            for (int i = 0; i < pptPresentation.Slides.Count; i++)
            {
                var saved_name = filename + "_" + (i + 1) + ".png";
                var image_path = Path.Combine(Server.MapPath("~/Images/"), saved_name);
                
                pptPresentation.Slides[i + 1].Export(image_path, "png");
                save_path[i] = server_address + "/Images/" + saved_name;
            }

            pptPresentation.Close();

            return save_path;
        }
    }
}
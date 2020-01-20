using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;

namespace CRM.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UploadController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration Configuration;

        // Constructor
        public UploadController(IWebHostEnvironment environment, IConfiguration configuration)
        {
            _environment = environment;
            Configuration = configuration;
        }


        [HttpPost]
        public IActionResult Post()
        {
            try
            {
                var urls = new List<string>();

                if (HttpContext.Request.Form.Files != null)
                {
                    var newFileName = string.Empty;
                    var fileName = string.Empty;
                    string PathDB = string.Empty;

                    var files = HttpContext.Request.Form.Files;
                    var path = Configuration.GetSection("Upload:CaminhoNoServidor").Value;
                    var url = Configuration.GetSection("Upload:UrlDoArquivo").Value;


                    foreach (var file in files)
                    {
                        if (file.Length > 0)
                        {
                            //Getting FileName
                            fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');

                            //Assigning Unique Filename (Guid)
                            var myUniqueFileName = Convert.ToString(Guid.NewGuid());

                            //Getting file Extension
                            var FileExtension = Path.GetExtension(fileName);

                            // concating  FileName + FileExtension
                            newFileName = myUniqueFileName + FileExtension;

                            // Combines two strings into a path.
                            fileName = path + newFileName;

                            // if you want to store path of folder in database
                            urls.Add(url + newFileName);

                            using (FileStream fs = System.IO.File.Create(fileName))
                            {
                                file.CopyTo(fs);
                                fs.Flush();
                            }
                        }
                    }
                }

                return Ok(urls.ToArray());
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
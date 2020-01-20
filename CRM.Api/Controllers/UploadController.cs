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
                if (HttpContext.Request.Form.Files == null)
                {
                    return BadRequest("Operação não é válida!");
                }

                var newFileName = string.Empty;
                var fileName = string.Empty;
                string PathDB = string.Empty;

                var file = HttpContext.Request.Form.Files[0];
                var path = Configuration.GetSection("Upload:CaminhoNoServidor").Value;
                var url = Configuration.GetSection("Upload:UrlDoArquivo").Value;

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

                    using (FileStream fs = System.IO.File.Create(fileName))
                    {
                        file.CopyTo(fs);
                        fs.Flush();
                    }
                }
                return Ok(url + newFileName);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
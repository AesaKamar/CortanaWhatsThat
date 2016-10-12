using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.Vision;
using System.IO;

namespace MicrosoftCognitiveVisionRepository
{
    public sealed class MicrosoftCognitiveVisionClient
    {
        public async Task<string> Run(string base64EncodedImage)
        {
            var VisionClient = new Microsoft.ProjectOxford.Vision.VisionServiceClient(
                //FIXME Private Information
                //==================================================
                "9ae131be7a4b43d68a08bccb1375bf8b"
                //==================================================
            );
            var response = await VisionClient.DescribeAsync(new MemoryStream(Convert.FromBase64String(base64EncodedImage)));

            return response.Description.Captions.First()?.Text;
        }
    }
}

using System;
using System.Threading.Tasks;

using Google.Apis.Vision.v1;
using Google.Apis.Vision.v1.Data;
using Google.Apis.Services;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace GoogleVisionAPI
{
    /// <summary>
    /// This example uses the discovery API to list all APIs in the discovery repository.
    /// https://developers.google.com/discovery/v1/using.
    /// https://sergeytihon.wordpress.com/2016/05/06/google-cloud-vision-api-from-netf-oauth2-with-serviceaccount-json/
    /// <summary>
    public class VisionClient
    {
        public async Task<List<string>> Run(string base64EncodedImage)
        {
            var service = new VisionService(new BaseClientService.Initializer
            {
                ApplicationName = "CortanaWhatsThat",
                //FIXME Private Information
                //==================================================
                ApiKey = "AIzaSyBLJacVUI5sxuir2RHwkDNE2fveMT1-T0U",
                //==================================================
                
            });

            var resource = new ImagesResource(service);

            var googleImage = new Image() { Content = base64EncodedImage };
            var googleFeature = new Feature() { Type = "LABEL_DETECTION" };
            var googleAnnotateRequest = new AnnotateImageRequest()
            {
                Features = new List<Feature>()
                {
                    googleFeature
                },
                Image = googleImage
                //Image =  new Image { Content = "" }
            };
            var googleBatchAnnotateRequest = new BatchAnnotateImagesRequest()
            {
                Requests = new List<AnnotateImageRequest>()
                {
                    googleAnnotateRequest
                }
            };

            var request = resource.Annotate(googleBatchAnnotateRequest);
            var response = await request.ExecuteAsync();


            List<string> listToReturn = new List<string>();
            response.Responses[0].LabelAnnotations.ToList()
                .ForEach(x => {
                    listToReturn.Add(x.Description);
                });
            return listToReturn;
        }
    }
}
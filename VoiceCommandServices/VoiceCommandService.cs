using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.VoiceCommands;

namespace VoiceCommandService
{
    /// <summary>
    /// The VoiceCommandService implements the entry point for all voice commands.
    /// The individual commands supported are described in the VCD xml file. 
    /// The service entry point is defined in the appxmanifest.
    /// </summary>
    public sealed class VoiceCommandService : IBackgroundTask
    {
        /// <summary>
        /// The background task entrypoint. 
        /// 
        /// Background tasks must respond to activation by Cortana within 0.5 seconds, and must 
        /// report progress to Cortana every 5 seconds (unless Cortana is waiting for user
        /// input). There is no execution time limit on the background task managed by Cortana,
        /// but developers should use plmdebug (https://msdn.microsoft.com/library/windows/hardware/jj680085%28v=vs.85%29.aspx)
        /// on the Cortana app package in order to prevent Cortana timing out the task during
        /// debugging.
        /// 
        /// The Cortana UI is dismissed if Cortana loses focus. 
        /// The background task is also dismissed even if being debugged. 
        /// Use of Remote Debugging is recommended in order to debug background task behaviors. 
        /// Open the project properties for the app package (not the background task project), 
        /// and enable Debug -> "Do not launch, but debug my code when it starts". 
        /// Alternatively, add a long initial progress screen, and attach to the background task process while it executes.
        /// </summary>
        /// <param name="taskInstance">Connection to the hosting background service process.</param>
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();
            taskInstance.Canceled += TaskInstanceCanceled;

            var triggerDetails = taskInstance.TriggerDetails as AppServiceTriggerDetails;

            if (triggerDetails?.Name != nameof(VoiceCommandService))
                return;

            var connection = VoiceCommandServiceConnection.FromAppServiceTriggerDetails(triggerDetails);

            connection.VoiceCommandCompleted += ConnectionOnVoiceCommandCompleted;

            var command = await connection.GetVoiceCommandAsync();

            switch (command.CommandName)
            {
                case "Identify":
                    await HandleReadImageLabelsCommandAsync(connection);
                    break;
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        private static async Task HandleReadImageLabelsCommandAsync(VoiceCommandServiceConnection connection)
        {
            //Initialize some stuff
            var CaptureClient = new CameraCaptureService.CameraCaptureService();
            await CaptureClient.Init();
            var GoogleVisionClient = new GoogleVisionAPI.VisionClient();

            //Tell the user that we are doing something
            var userMessage = new VoiceCommandUserMessage();
            userMessage.DisplayMessage = "Analyzing";
            userMessage.SpokenMessage = "Analyzing";
            var response = VoiceCommandResponse.CreateResponse(userMessage);
            await connection.ReportProgressAsync(response);

            //TODO: Get the image
            string imageString = await CaptureClient.Capture();

            //TODO: Send the Image through the Vision Client
            var annotationResponse = await GoogleVisionClient.Run(imageString);
            var joinedResponse = string.Join(", ", annotationResponse);

            //TODO: Set the User Message, Display & Spoken
            userMessage.DisplayMessage = joinedResponse;
            userMessage.SpokenMessage = joinedResponse;
            response = VoiceCommandResponse.CreateResponse(userMessage);

            await connection.ReportSuccessAsync(response);

        }

        private void ConnectionOnVoiceCommandCompleted(VoiceCommandServiceConnection sender, VoiceCommandCompletedEventArgs args)
        {
            
        }

        private void TaskInstanceCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.VoiceCommands;

namespace VoiceCommandServices
{
    public sealed class VoiceCommandService : IBackgroundTask
    {
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
                case "WhatsThat":
                    await HandleReadImageLabelsCommandAsync(connection);
                    break;
            }

        }

        private static async Task HandleReadImageLabelsCommandAsync(VoiceCommandServiceConnection connection)
        {
            var userMessage = new VoiceCommandUserMessage();
            userMessage.DisplayMessage = "Analyzing";
            userMessage.SpokenMessage = "Analyzing";
            var response = VoiceCommandResponse.CreateResponse(userMessage);
            await connection.ReportProgressAsync(response);

            //TODO: Get the image

            //TODO: Send the Image through the Vision Client

            //TODO: Set the User Message, Display & Spoken
            userMessage.DisplayMessage = "Done";
            userMessage.SpokenMessage = "Done";
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

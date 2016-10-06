using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace BackgroundServices
{
    public sealed class BackgroundService : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            throw new NotImplementedException();
        }

        public static async void Register ()
        {
            //Grab the registration info for background tasks
            var isRegistered = BackgroundTaskRegistration.AllTasks.Values.Any(
                t => t.Name == nameof(BackgroundService));
            //Check if the background task is already registered
            if (isRegistered)
                return;
            //If its not, request registration access
            if (await BackgroundExecutionManager.RequestAccessAsync()
                == BackgroundAccessStatus.Denied)
                return;

            //Build the background task 
            var builder = new BackgroundTaskBuilder
            {
                Name = nameof(BackgroundService),
                TaskEntryPoint = $"{nameof(BackgroundServices)}.{nameof(BackgroundService)}"
            };
            //Register the background task
            builder.SetTrigger(new TimeTrigger(120, false));
            builder.Register();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace CortanaWhatsThat.BackgroundServices
{
    public sealed class CortanaIdentifier : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            throw new NotImplementedException();
        }

        public static async void Register ()
        {
            //Grab the registration info for background tasks
            var isRegistered = BackgroundTaskRegistration.AllTasks.Values.Any(
                t => t.Name == nameof(CortanaIdentifier));
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
                Name = nameof(CortanaIdentifier),
                TaskEntryPoint = $"{nameof(BackgroundServices)}.{nameof(CortanaIdentifier)}"
            };
            //Register the background task
            builder.SetTrigger(new TimeTrigger(120, false));
            builder.Register();
        }
    }
}

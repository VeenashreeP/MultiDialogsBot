namespace MultiDialogsBot.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.FormFlow;
    using Microsoft.Bot.Connector;

    [Serializable]
    public class WatchDemoDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {

            var message = context.MakeMessage();
            var attachment = GetVideoCard();
            message.Attachments.Add(attachment);
            await context.PostAsync(message);

            context.Wait(this.MessageReceivedAsync);

        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {

            await context.PostAsync("Website Demo");

            //context.Call(new RootDialog(), OnOptionSelectedAsync);
        }

        private static Attachment GetVideoCard()
        {
            var videoCard = new VideoCard
            {
                Title = "Watch An Automation Script In Action",
                Subtitle = "Process Automation Demo",
                Media = new List<MediaUrl>()
                            {
                                new MediaUrl()
                                {
                                   // Url = "https://youtu.be/a02i_Ik4GGM"
                                    Url="https://blobstoragechatbot.blob.core.windows.net/autobotmedia/sample.mp4"
                                }
                            },
            };
            return videoCard.ToAttachment();

        }
    }
}
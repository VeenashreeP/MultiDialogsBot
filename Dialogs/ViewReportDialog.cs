namespace MultiDialogsBot.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.FormFlow;
    using Microsoft.Bot.Connector;

    [Serializable]
    public class ViewReportDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {

            var message = context.MakeMessage();
            var attachment = ViewReport();
            message.Attachments.Add(attachment);
            await context.PostAsync(message);

            context.Wait(this.MessageReceivedAsync);

        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {

            await context.PostAsync("Website Launched");

            //context.Call(new RootDialog(), OnOptionSelectedAsync);
        }
        private static Attachment ViewReport()
        {
            var heroCard = new HeroCard
            {
                Title = "Execution Report",
                //Subtitle = "Mi tarjeta",
                //Text = "mi texto",
                //Images = new List<CardImage> { new CardImage("https://blobstoragechatbot.blob.core.windows.net/autobotmedia/McAfee_Website.PNG") },
                // Images = new List<CardImage> { new CardImage("~/Images/McAfee_Website.PNG") },
                Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Build Results", value: "http://cldlvuatdb0024/ReportServer?%2FAutomation_Report%2FBuild_Results&rs%3AParameterLanguage=en-US") }
            };
            return heroCard.ToAttachment();
        }
    }
}
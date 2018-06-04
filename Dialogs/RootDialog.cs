namespace MultiDialogsBot.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;

    [Serializable]
    public class RootDialog : IDialog<object>
    {
       
        private const string Website = "Visit Automation Website";
        private const string ExecuteProject = "Execute A Project";
        private const string ViewReport = "View Execution Report";
        private const string WatchDemo = "Watch A Demo";
        private const string GobackOption = "Go Back To All Options";

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(this.MessageReceivedAsync);
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
           // var message = await result;
            var message = context.MakeMessage();
            var attachment = GetHeroCardWelcome();
            message.Attachments.Add(attachment);
            await context.PostAsync(message);

            this.ShowOptions(context);
            
        }
        private static Attachment GetHeroCardWelcome()
        {
            var heroCard = new HeroCard
            {
                Title = "IT Automation Team",
                Subtitle = "CHATBOT",
                Text = "We Make Software Life Cycle Efficient,Quick and Bug Free",
                Images = new List<CardImage> { new CardImage("https://blobstoragechatbot.blob.core.windows.net/autobotmedia/Automation.gif") },
                Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "About Us", value: "https://mcafee.sharepoint.com/sites/Automation/SitePages/Home.aspx") }
            };

            return heroCard.ToAttachment();
        }

        private void ShowOptions(IDialogContext context)
        {
           
            PromptDialog.Choice(
                context: context,
                //resume: Selection,
                this.OnOptionSelectedAsync,                
                new List<string>() { Website, ExecuteProject, ViewReport, WatchDemo, GobackOption },
                prompt: "Select An Option",
                retry: "I didn't understand.Please try again.",
                promptStyle: PromptStyle.Auto
                );
        }

        public async Task OnOptionSelectedAsync(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                string optionSelected = await result;

                switch (optionSelected)
                {
                   
                    case Website:
                        context.Call(new WebsiteDialog(), this.ResumeAfterOptionDialog);
                        break;

                    case ExecuteProject:
                        context.Call(new ExecuteProjectDialog(), this.ResumeAfterOptionDialog);
                        break;

                    case ViewReport:
                        context.Call(new ViewReportDialog(), this.ResumeAfterOptionDialog);
                        break;

                    case WatchDemo:
                        context.Call(new WatchDemoDialog(), this.ResumeAfterOptionDialog);
                        break;

                    //case GobackOption:
                    //    context.Done<object>(null);
                    //    break;
                }
            }
            catch (TooManyAttemptsException ex)
            {
                await context.PostAsync($"Ooops! Too many attempts :(. But don't worry, I'm handling that exception and you can try again! {ex.Message}");

                context.Wait(this.MessageReceivedAsync);
            }
        }

        private async Task ResumeAfterOptionDialog(IDialogContext context, IAwaitable<object> result)
        {
            try
            {
                //var message = await result;
                //await context.PostAsync("Website Launched Successfully");                
                //context.Done<object>(null);

                //await context.SayAsync(
                //text: "Would you like to go back try something else?");
                

            }
            catch (Exception ex)
            {
                await context.PostAsync($"Failed with message: {ex.Message}");
            }
            finally
            {
                context.Wait(this.MessageReceivedAsync);
            }
        }
    }

       
}


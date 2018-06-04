namespace MultiDialogsBot.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.FormFlow;
    using Microsoft.Bot.Connector;

    [Serializable]
    public class WebsiteDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
           // await context.PostAsync("Welcome to Website Dialog");
            var message = context.MakeMessage();
            var attachment = GetWebsiteForm();
            message.Attachments.Add(attachment);
            await context.PostAsync(message);

            var AfterAllDoneAsync = FormDialog.FromForm(this.GetWelcomeForm, FormOptions.PromptFieldsWithValues);
            context.Call(AfterAllDoneAsync, this.ResumeAfterOptionDialog);
           // context.Wait(this.MessageReceivedAsync);
        }

        private IForm<BackToRoot> GetWelcomeForm()
        {
            OnCompletionAsyncDelegate<BackToRoot> processHotelsSearch = async (context, state) =>
            {
                await context.PostAsync($"Taking you to Automation Website!!");

                //await this.ShowJobSuites(context);
            };
            return new FormBuilder<BackToRoot>()
                    .Field(nameof(BackToRoot.Start))
                   //.Message("Loading Main Menu {Start}")               
                //    .AddRemainingFields()
                .OnCompletion(processHotelsSearch)
                .Build();
        }

        private async Task AfterPromptAsync(IDialogContext context, IAwaitable<string> result)
        {
            var confirm = await result;
            if (confirm == "Yes")
            {
                // call child dialog
                await context.Forward(new RootDialog(), AfterAllDoneAsync, context.Activity, CancellationToken.None);
            }
            else
            {
                context.Wait(MessageReceivedAsync);
            }
        }
        public async Task AfterAllDoneAsync(IDialogContext context, IAwaitable<object> argument)
        {
            await context.PostAsync("It was really great talking to you!");
            context.Wait(MessageReceivedAsync);
        }
        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {

            var opt = new PromptOptions<String>("Choose from following -", "Take me to main menu", "is AbandonedMutexException done", new string[] { "one", "two", "three" });
            PromptDialog.Choice(context, AfterPromptAsync, opt);

            // await context.PostAsync("Website Launched Successfully!!");
            //  context.Done<object>(null);
            // context.Call(new RootDialog(), ResumeAfterOptionDialog);
            ////var promptOptions = new PromptOptions<string>(
            ////      prompt:
            ////      "Want to try something else?You can say either Yes or No",
            ////      retry:
            ////      "If you like, I can let your manager know that you will not be in. Would oyu like me to?  You can say either Yes or No",

            ////PromptDialog.Confirm(context, GetMessageReceivedAsync, promptOptions);

        }


        private static Attachment GetBackToWelcome()
        {
            var heroCard = new HeroCard
            {
                Title = "Take Me To Main Menu",
                //Subtitle = "CHATBOT",
               // Text = "We Make Software Life Cycle Efficient,Quick and Bug Free",
                //Images = new List<CardImage> { new CardImage("https://blobstoragechatbot.blob.core.windows.net/autobotmedia/Automation.gif") },
                Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Start Over", value: "https://mcafee.sharepoint.com/sites/Automation/SitePages/Home.aspx") }
            };

            return heroCard.ToAttachment();
        }
        private Task GetMessageReceivedAsync(IDialogContext context, IAwaitable<bool> result)
        {
            throw new NotImplementedException();
        }

        private static Attachment GetWebsiteForm()
        {
            
            
            HeroCard heroCard = new HeroCard()
            {
                Title = $"Visit Automation Website!",
                Subtitle = $"itautomation@mcafee.onmicrosoft.com",
                Images = new List<CardImage>()
                        {
                            new CardImage() { Url = "https://blobstoragechatbot.blob.core.windows.net/autobotmedia/McAfee_Website.PNG" }
                        },
                Buttons = new List<CardAction>()
                        {
                            new CardAction()
                            {
                                Title = $"Sharepoint",
                                Type = ActionTypes.OpenUrl,
                                Value = $"https://mcafee.sharepoint.com/sites/Automation/SitePages/Home.aspx"
                            }
                        }
            };

            return heroCard.ToAttachment();
        }
        private async Task ResumeAfterOptionDialog(IDialogContext context, IAwaitable<object> result)
        {
            try
            {
                var message = await result;
                await context.PostAsync("Website Launched Successfully");
                //context.Done<object>(null);
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

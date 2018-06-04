namespace MultiDialogsBot.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.FormFlow;
    using Microsoft.Bot.Connector;

    [Serializable]
    public class PBCDialog : IDialog<object>
    {
        private const string PBC_BVT = "PBC BVT Suite";
        private const string PBC_P1 = "PBC Priority1 Suite";
        private const string PBC_Regression = "PBC Regression Suite";
        public async Task StartAsync(IDialogContext context)
        {
            //var message = context.MakeMessage();
            //var attachment = GetPBCTestSuites();
            //message.Attachments.Add(attachment);
            //await context.PostAsync(message);

            context.Wait(this.MessageReceivedAsync);
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            //var message = await result;
            //if (message.Text.ToLower().Contains("PBC"))
            //{
            //    await context.Forward(new PBCDialog(), this.ResumeAfterPBCDialog, message, CancellationToken.None);
            //}
            //if (message.Text.ToLower().Contains("UAPI"))
            //{
            //    await context.Forward(new UAPIDialog(), this.ResumeAfterUAPIDialog, message, CancellationToken.None);
            //}
            //else
            //{
            //    //this.ShowOptions(context);
            //}
            this.ShowPBCJobSuites(context);
        }

        public async Task ShowPBCJobSuites(IDialogContext context)
        {

            PromptDialog.Choice(
                context: context,
                //resume: Selection,
                this.OnPBCJobSelectedAsync,
                new List<string>() { PBC_BVT, PBC_P1, PBC_Regression },
                prompt: "Select A Job",
                retry: "I didn't understand.Please try again.",
                promptStyle: PromptStyle.Auto
                );
        }

        private async Task OnPBCJobSelectedAsync(IDialogContext context, IAwaitable<string> result)
        {

            try
            {
                string optionSelected = await result;

                switch (optionSelected)
                {

                    case PBC_BVT:
                        context.Call(new PBCDialog(), this.ResumeAfterPBCExecuteProjectDialog);
                        break;

                    case PBC_P1:
                        context.Call(new UAPIDialog(), this.ResumeAfterPBCExecuteProjectDialog);
                        break;

                    case PBC_Regression:
                        context.Call(new UAPIDialog(), this.ResumeAfterPBCExecuteProjectDialog);
                        break;

                }
            }
            catch (TooManyAttemptsException ex)
            {
                await context.PostAsync($"Ooops! Too many attempts :(. But don't worry, I'm handling that exception and you can try again! {ex.Message}");

                context.Wait(this.MessageReceivedAsync);
            }
        }

        private Task ResumeAfterPBCExecuteProjectDialog(IDialogContext context, IAwaitable<object> result)
        {
            throw new NotImplementedException();
        }

        private static Attachment GetPBCTestSuites()
        {
            HeroCard heroCard = new HeroCard()
            {
                Title = $"Select Test Suite",
                //Subtitle = $"itautomation@mcafee.onmicrosoft.com",
                //Images = new List<CardImage>()
                //        {
                //            new CardImage() { Url = "https://blobstoragechatbot.blob.core.windows.net/autobotmedia/McAfee_Website.PNG" }
                //        },
                Buttons = new List<CardAction>()
                        {
                            new CardAction()
                            {
                                Title = $"PBC BVT",
                                Type = ActionTypes.OpenUrl,
                                Value = $"http://10.95.112.29:8081/job/PBC_BVT/build?delay=0sec"
                            },
                             new CardAction(){
                                Title = $"PBC P1",
                                Type = ActionTypes.PostBack,
                                Value = $"http://10.95.112.29:8081/job/PBC_BVT_Priority1/"
                            },
                             new CardAction(){
                                Title = $"PBC Full Suite",
                                Type = ActionTypes.PostBack,
                                Value = $"http://10.95.112.29:8081/job/PBC_AutomationSuite/"
                            }
            }
            };

            return heroCard.ToAttachment();

        }
    }
}
namespace MultiDialogsBot.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.FormFlow;
    using Microsoft.Bot.Connector;

    [Serializable]
    public class UAPIDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            //context.Fail(new NotImplementedException("Flights Dialog is not implemented and is instead being used to show context.Fail"));
            await context.PostAsync("Welcome to UAPI Dialog!");

            var reply = context.MakeMessage();
            var attachment = GetPBCTestSuites();
            reply.Attachments.Add(attachment);
            //var hotelsFormDialog = FormDialog.FromForm(this.BuildHotelsForm, FormOptions.PromptInStart);

            //context.Call(hotelsFormDialog, this.ResumeAfterHotelsFormDialog);
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
                                Title = $"UAPI BVT",
                                Type = ActionTypes.OpenUrl,
                                Value = $"http://10.95.112.29:8081/job/PBC_BVT/build?delay=0sec"
                            },
                             new CardAction(){
                                Title = $"UAPI P1",
                                Type = ActionTypes.PostBack,
                                Value = $"http://10.95.112.29:8081/job/PBC_BVT_Priority1/"
                            },
                             new CardAction(){
                                Title = $"UAPI Full Suite",
                                Type = ActionTypes.PostBack,
                                Value = $"http://10.95.112.29:8081/job/PBC_AutomationSuite/"
                            }
            }
            };

            return heroCard.ToAttachment();

        }
    }
}
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
    public class SelectTestSuiteDialog : IDialog<object>
    {
        protected int count = 1;
        private const string PBC_BVT = "PBC BVT Suite";
        private const string PBC_P1 = "PBC Priority1 Suite";
        private const string PBC_Regression = "PBC Regression Suite";
        private const string UAPI_BVT = "UAPI BVT Suite";
        private const string UAPI_Regression = "UAPI Regression Suite";
        public async Task StartAsync(IDialogContext context)
        {
        //context.Fail(new NotImplementedException("Flights Dialog is not implemented and is instead being used to show context.Fail"));
            await context.PostAsync("Welcome to Select Test Suite Dialog!");

            context.Wait(MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
           
            var message = await result;
            if (message.Text == "PBC")
            {
                PromptDialog.Choice(
                 context: context,
                 //resume: Selection,
                 this.OnJobSelectedAsync,
                 new List<string>() { PBC_BVT, PBC_P1, PBC_Regression },
                 prompt: "Select A PBC Job",
                 retry: "I didn't understand.Please try again.",
                 promptStyle: PromptStyle.Auto
                 );
            }
            else if (message.Text == "UAPI") {
                PromptDialog.Choice(
                context: context,
                //resume: Selection,
                this.OnJobSelectedAsync,
                new List<string>() { UAPI_BVT, UAPI_Regression },
                prompt: "Select A UAPI Job",
                retry: "I didn't understand.Please try again.",
                promptStyle: PromptStyle.Auto
                );
            }
            else
            {
                await context.PostAsync($"{this.count++}: You said {message.Text}");
                context.Wait(MessageReceivedAsync);
            }
        }
        private async Task OnJobSelectedAsync(IDialogContext context, IAwaitable<string> result)
        {

            try
            {
                string optionSelected = await result;

                switch (optionSelected)
                {

                    case PBC_BVT:
                        context.Call(new PBCDialog(), this.ResumeAfterExecuteProjectDialogAsync);
                        break;

                    case PBC_P1:
                        context.Call(new PBCDialog(), this.ResumeAfterExecuteProjectDialogAsync);
                        break;

                    case PBC_Regression:
                        context.Call(new PBCDialog(), this.ResumeAfterExecuteProjectDialogAsync);
                        break;
                    case UAPI_BVT:
                        context.Call(new UAPIDialog(), this.ResumeAfterExecuteProjectDialogAsync);
                        break;

                    case UAPI_Regression:
                        context.Call(new UAPIDialog(), this.ResumeAfterExecuteProjectDialogAsync);
                        break;

                }
            }
            catch (TooManyAttemptsException ex)
            {
                await context.PostAsync($"Ooops! Too many attempts :(. But don't worry, I'm handling that exception and you can try again! {ex.Message}");

                context.Wait(this.MessageReceivedAsync);
            }
        }
        private async Task ResumeAfterExecuteProjectDialogAsync(IDialogContext context, IAwaitable<object> result)
        {
            await context.PostAsync($"Ooops! Too many attempt");
        }

        public async Task AfterResetAsync(IDialogContext context, IAwaitable<bool> argument)
        {
            var confirm = await argument;
            if (confirm)
            {
                this.count = 1;
                await context.PostAsync("Reset count.");
            }
            else
            {
                await context.PostAsync("Did not reset count.");
            }
            context.Wait(MessageReceivedAsync);
        }
    }

   

    //    private Task ResumeAfterUAPIDialog(IDialogContext context, IAwaitable<object> result)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    private Task ResumeAfterPBCDialog(IDialogContext context, IAwaitable<object> result)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    private static Attachment GetTestSuites()
    //{
    //        HeroCard heroCard = new HeroCard()
    //        {
    //            Title = $"Select Test Suite",
    //            //Subtitle = $"itautomation@mcafee.onmicrosoft.com",
    //            //Images = new List<CardImage>()
    //            //        {
    //            //            new CardImage() { Url = "https://blobstoragechatbot.blob.core.windows.net/autobotmedia/McAfee_Website.PNG" }
    //            //        },
    //            Buttons = new List<CardAction>()
    //                    {
    //                        new CardAction()
    //                        {
    //                            Title = $"PBC BVT",
    //                            Type = ActionTypes.OpenUrl,
    //                            Value = $"http://10.95.112.29:8081/job/PBC_BVT/build?delay=0sec"
    //                        },
    //                         new CardAction(){
    //                            Title = $"PBC P1",
    //                            Type = ActionTypes.PostBack,
    //                            Value = $"http://10.95.112.29:8081/job/PBC_BVT_Priority1/build?delay=0sec"
    //                        },
    //                         new CardAction(){
    //                            Title = $"PBC Full Suite",
    //                            Type = ActionTypes.PostBack,
    //                            Value = $"http://10.95.112.29:8081/job/PBC_AutomationSuite/build?delay=0sec"
    //                        }
    //        }
    //        };

    //        return heroCard.ToAttachment();

    //    }
    //    private static Attachment GetUAPITestSuites()
    //    {
    //        HeroCard heroCard = new HeroCard()
    //        {
    //            Title = $"Select Test Suite",
    //            //Subtitle = $"itautomation@mcafee.onmicrosoft.com",
    //            //Images = new List<CardImage>()
    //            //        {
    //            //            new CardImage() { Url = "https://blobstoragechatbot.blob.core.windows.net/autobotmedia/McAfee_Website.PNG" }
    //            //        },
    //            Buttons = new List<CardAction>()
    //                    {
    //                        new CardAction()
    //                        {
    //                            Title = $"UAPI BVT",
    //                            Type = ActionTypes.OpenUrl,
    //                            Value = $"http://10.95.112.29:8081/job/PBC_BVT/build?delay=0sec"
    //                        },                            
    //                        new CardAction(){
    //                            Title = $"UAPI Full Suite",
    //                            Type = ActionTypes.PostBack,
    //                            Value = $"http://10.95.112.29:8081/job/PBC_AutomationSuite/build?delay=0sec"
    //                        }
    //        }
    //        };

    //        return heroCard.ToAttachment();

    //    }
    }


namespace MultiDialogsBot.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.FormFlow;
    using Microsoft.Bot.Connector;

    [Serializable]
    public class ExecuteProjectDialog : IDialog<object>
    {
        private const string PBC = "PBC";
        private const string UAPI = "UAPI";


        private const string PBC_BVT = "PBC_BVT";
        private const string PBC_P1 = "PBC_Priority1";
        private const string PBC_Regression = "PBC Regression Suite";

        private const string UAPI_BVT = "UAPI_BVT";
        private const string UAPI_P1 = "UAPI_Priority1";
        private const string UAPI_Regression = "UAPI Regression Suite";

        private const string StartOver = "Start Over";
        private const string Exit = "Exit";
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("Welcome to Project Selector");

            var message = context.MakeMessage();
            var attachment = GetProjects();
            message.Attachments.Add(attachment);
            await context.PostAsync(message);

            var hotelsFormDialog = FormDialog.FromForm(this.GetProjectsForm, FormOptions.PromptFieldsWithValues);            
            context.Call(hotelsFormDialog, this.ResumeAfterHotelsFormDialog);

        }

        private IForm<HotelsQuery> GetProjectsForm()
        {
            OnCompletionAsyncDelegate<HotelsQuery> processHotelsSearch = async (context, state) =>
            {
                await context.PostAsync($"Ok.Taking you to Jenkins Build.Just Hold on!!");

               
                var message = context.MakeMessage();
                var attachment = GetJenkinsBuild();
                message.Attachments.Add(attachment);
                await context.PostAsync(message);

               // this.ShowOptionsEnd(context);
              
                // await context.PostAsync($"Want to check status of execution?");
                // await context.SayAsync($"Want to check status of execution?");
                //context.EndConversation(EndOfConversationCodes.CompletedSuccessfully);
                //await this.ShowJobSuites(context);
            };           
            return new FormBuilder<HotelsQuery>()                
            //    .Field(nameof(HotelsQuery.TestsuiteName))
            //    .Message("Loading Test Suites in {TestsuiteName}")               
            //    .AddRemainingFields()
                 .OnCompletion(processHotelsSearch)                
                .Build();
        }
        private void ShowOptionsEnd(IDialogContext context)
        {
            PromptDialog.Choice(
           context: context,
           //resume: Selection,
           this.OnOptionSelectAsync,
           new List<string>() { StartOver, Exit },
           prompt: "Would You Like To Do Something Else?",
           retry: "I didn't understand.Please try again.",
           promptStyle: PromptStyle.Auto
           );
        }
        public async Task OnOptionSelectAsync(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                string optionSelected = await result;

                switch (optionSelected)
                {

                    case StartOver:
                        context.Call(new RootDialog(), this.ResumeAfterStartOverDialog);
                        break;

                    case Exit:
                        context.Call(new ExitDialog(), this.ResumeAfterExitDialog);
                        break;

                }
            }
            catch (TooManyAttemptsException ex)
            {
                await context.PostAsync($"Ooops! Too many attempts :(. But don't worry, I'm handling that exception and you can try again! {ex.Message}");

                context.Wait(this.MessageReceivedAsync);
            }
        }

        private Task ResumeAfterExitDialog(IDialogContext context, IAwaitable<object> result)
        {
            throw new NotImplementedException();
        }

        private Task ResumeAfterStartOverDialog(IDialogContext context, IAwaitable<object> result)
        {
            throw new NotImplementedException();
        }

        private async Task ResumeAfterHotelsFormDialog(IDialogContext context, IAwaitable<HotelsQuery> result)
        {
            try
            {
                var searchQuery = await result;
                //context.Wait(MessageReceivedAsync);

               

                var hotels = await this.GetHotelsAsync(searchQuery);

               
            }
            catch (FormCanceledException ex)
            {
                string reply;

                if (ex.InnerException == null)
                {
                    reply = "You have canceled the operation. Quitting from the HotelsDialog";
                }
                else
                {
                    reply = $"Oops! Something went wrong :( Technical Details: {ex.InnerException.Message}";
                }

                await context.PostAsync(reply);
            }
            finally
            {
                context.Done<object>(null);
            }
        }
        private async Task<IEnumerable<Hotel>> GetHotelsAsync(HotelsQuery searchQuery)
        {
            var hotels = new List<Hotel>();

            // Filling the hotels results manually just for demo purposes
            for (int i = 1; i <= 5; i++)
            {
                var random = new Random(i);
                Hotel hotel = new Hotel()
                {
                    Name = $"{searchQuery.TestsuiteName} Hotel {i}",
                    Location = searchQuery.TestsuiteName,
                    Rating = random.Next(1, 5),
                    NumberOfReviews = random.Next(0, 5000),
                    PriceStarting = random.Next(80, 450),
                    Image = $"https://placeholdit.imgix.net/~text?txtsize=35&txt=Hotel+{i}&w=500&h=260"
                };

                hotels.Add(hotel);
            }

            hotels.Sort((h1, h2) => h1.PriceStarting.CompareTo(h2.PriceStarting));

            return hotels;
        }



        private async Task ResumeAfterTestSuitesFormDialog(IDialogContext context, IAwaitable<HotelsQuery> result)
        {
            try
            {
                var searchQuery = await result;



                var hotels = await this.GetTestSuitesAsync(searchQuery);

                await context.PostAsync($"I found in total {hotels.Count()} hotels for your dates:");

                var resultMessage = context.MakeMessage();
                resultMessage.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                resultMessage.Attachments = new List<Attachment>();

                foreach (var hotel in hotels)
                {
                    HeroCard heroCard = new HeroCard()
                    {
                        Title = hotel.Name,
                        Subtitle = $"{hotel.Rating} starts. {hotel.NumberOfReviews} reviews. From ${hotel.PriceStarting} per night.",
                        Images = new List<CardImage>()
                        {
                            new CardImage() { Url = hotel.Image }
                        },
                        Buttons = new List<CardAction>()
                        {
                            new CardAction()
                            {
                                Title = "More details",
                                Type = ActionTypes.OpenUrl,
                                Value = $"https://www.bing.com/search?q=hotels+in+" + HttpUtility.UrlEncode(hotel.Location)
                            }
                        }
                    };

                    resultMessage.Attachments.Add(heroCard.ToAttachment());
                }

                await context.PostAsync(resultMessage);
            }
            catch (FormCanceledException ex)
            {
                string reply;

                if (ex.InnerException == null)
                {
                    reply = "You have canceled the operation. Quitting from the HotelsDialog";
                }
                else
                {
                    reply = $"Oops! Something went wrong :( Technical Details: {ex.InnerException.Message}";
                }

                await context.PostAsync(reply);
            }
            finally
            {
                context.Done<object>(null);
            }
        }
        private async Task<IEnumerable<Hotel>> GetTestSuitesAsync(HotelsQuery searchQuery)
        {
            var hotels = new List<Hotel>();

            // Filling the hotels results manually just for demo purposes
            for (int i = 1; i <= 5; i++)
            {
                var random = new Random(i);
                Hotel hotel = new Hotel()
                {
                    Name = $"{searchQuery.TestsuiteName} Hotel {i}",
                    Location = searchQuery.TestsuiteName,
                    Rating = random.Next(1, 5),
                    NumberOfReviews = random.Next(0, 5000),
                    PriceStarting = random.Next(80, 450),
                    Image = $"https://placeholdit.imgix.net/~text?txtsize=35&txt=Hotel+{i}&w=500&h=260"
                };

                hotels.Add(hotel);
            }

            hotels.Sort((h1, h2) => h1.PriceStarting.CompareTo(h2.PriceStarting));

            return hotels;
        }
        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {

            var message = await result;

            this.ShowJobSuites(context);          

        }

        private Task ResumeAfterSelectTestSuiteDialog(IDialogContext context, IAwaitable<object> result)
        {
            throw new NotImplementedException();
        }


        private Task ResumeAfterUAPIDialog(IDialogContext context, IAwaitable<object> result)
        {
            throw new NotImplementedException();
        }

        private Task ResumeAfterPBCDialog(IDialogContext context, IAwaitable<object> result)
        {
            throw new NotImplementedException();
        }

      
        private static Attachment GetProjects()
        {
            
            ThumbnailCard thumbbnailCard = new ThumbnailCard()
            {
                Title = $"Select A Project",
                Subtitle = $"Applications Automated",
                Images = new List<CardImage>()
                        {
                            new CardImage() { Url = "https://blobstoragechatbot.blob.core.windows.net/autobotmedia/Jenkins.jpg" }
                        },
                Buttons = new List<CardAction> { new CardAction(ActionTypes.ImBack, "PBC", value: "Selected PBC"),
                                                 new CardAction(ActionTypes.ImBack, "UAPI", value: "Selected UAPI")}
           
        };

            return thumbbnailCard.ToAttachment();
            
        }

        private static Attachment GetJenkinsBuild()
        {
            
            ThumbnailCard thumbbnailCard = new ThumbnailCard()
            {
                Title = $"Start The Execution Yourself",
                Subtitle = $"Click on the Execute Button.Once the Jekins Page Opens,Enter Your Email ID and Click on build",
                Images = new List<CardImage>()
                        {
                            new CardImage() { Url = "https://blobstoragechatbot.blob.core.windows.net/autobotmedia/Jenkins.jpg" }
                        },
                Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Execute", value: "http://10.95.112.29:8081/job/Automation_Insight_Support/build?delay=0sec"),
                    // new CardAction(ActionTypes.ImBack, "UAPI", value: "Selected UAPI")
                }

            };

            return thumbbnailCard.ToAttachment();

        }

        public async Task ShowJobSuites(IDialogContext context)
        {
            
            PromptDialog.Choice(
                context: context,
                //resume: Selection,
                this.OnJobSelectedAsync,
                new List<string>() { PBC, UAPI},
                prompt: "Select A Job",
                retry: "I didn't understand.Please try again.",
                promptStyle: PromptStyle.Auto
                );
        }

        public async Task ShowTestSuitesUAPI(IDialogContext context)
        {

            PromptDialog.Choice(
                context: context,
                //resume: Selection,
                this.OnJobSelectedAsync,
                new List<string>() { UAPI_BVT, UAPI_P1, UAPI_Regression },
                prompt: "Select A Test Suite",
                retry: "I didn't understand.Please try again.",
                promptStyle: PromptStyle.Auto
                );
        }

        private async Task OnJobSelectedAsync(IDialogContext context, IAwaitable<string> result)
        {

            try
            {
                string optionSelected = await result;

                switch (optionSelected)
                {

                    case PBC:
                        context.Call(new PBCDialog(), this.ResumeAfterExecuteProjectDialog);
                        break;
                    
                    case UAPI:
                        context.Call(new UAPIDialog(), this.ResumeAfterExecuteProjectDialog);
                        break;

                }
            }
            catch (TooManyAttemptsException ex)
            {
                await context.PostAsync($"Ooops! Too many attempts :(. But don't worry, I'm handling that exception and you can try again!");

                context.Wait(this.MessageReceivedAsync);
            }
        }

        private Task ResumeAfterExecuteProjectDialog(IDialogContext context, IAwaitable<object> result)
        {
            throw new NotImplementedException();
        }
    }
}
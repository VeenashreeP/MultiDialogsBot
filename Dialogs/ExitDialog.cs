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
    public class ExitDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.EndConversation(EndOfConversationCodes.CompletedSuccessfully);
        }
    }
}
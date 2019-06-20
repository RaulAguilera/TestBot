using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;


namespace EmptyBot1
{
    public class SuggestActionBot : ActivityHandler
    {
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken = default)
        {
            
            var reply = MessageFactory.Text("What's your favorite color");

            reply.SuggestedActions = new SuggestedActions()
            {
                Actions = new List<CardAction>() {
                    new CardAction() { Title = "Red", Type = ActionTypes.ImBack, Value = "red"},
                    new CardAction() { Title = "Yellow", Type = ActionTypes.ImBack, Value = "yellow"}
                }
            };

            await turnContext.SendActivityAsync(reply, cancellationToken);

        }
       
    }
}


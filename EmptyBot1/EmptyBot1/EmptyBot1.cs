// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

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
    public class EmptyBot : ActivityHandler
    {
        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Hello world!"), cancellationToken);
                }
            }
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken = default)
        {
            var receivedMessage = turnContext.Activity.Text;


            if (turnContext.Activity.Attachments != null && turnContext.Activity.Attachments.Any())
                await turnContext.SendActivityAsync(HandleIncomingAttachment(turnContext.Activity));
            else
            {

                switch (receivedMessage)
                {
                    case "image":
                        var reply = MessageFactory.Text("Here you have an image");
                        reply.Attachments = new List<Attachment> { GetImageAttachment() };
                        await turnContext.SendActivityAsync(reply);
                        break;
                    default:
                        await turnContext.SendActivityAsync(MessageFactory.Text($"I got your message.Yours was {receivedMessage}"));
                        break;
                }
            }

        }

        private static Attachment GetImageAttachment()
        {
            var imagePath = @"wwwroot\fretboard.png";
            var imageData = Convert.ToBase64String(File.ReadAllBytes(imagePath));

            return new Attachment
            {
                Name = @"wwwroot\fretboard.png",
                ContentType = "image/png",
                ContentUrl = $"data:image/png;base64,{imageData}",
            };

        }

        private static IMessageActivity HandleIncomingAttachment(IMessageActivity activity)
        {
            string replyText = string.Empty;
            foreach (var file in activity.Attachments)
            {
                // Determine where the file is hosted.
                var remoteFileUrl = file.ContentUrl;

                // Save the attachment to the system temp directory.
                var localFileName = Path.Combine(Path.GetTempPath(), file.Name);

                // Download the actual attachment
                using (var webClient = new WebClient())
                {
                    webClient.DownloadFile(remoteFileUrl, localFileName);
                }

                replyText += $"Attachment \"{file.Name}\"" +
                             $" has been received and saved to \"{localFileName}\"\r\n";
            }

            return MessageFactory.Text(replyText);
        }
    }
}

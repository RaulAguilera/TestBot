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
    public class AttachmentsBot : ActivityHandler
    {
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken = default)
        {
            var receivedMessage = turnContext.Activity.Text;


            if (turnContext.Activity.Attachments != null && turnContext.Activity.Attachments.Any())
                await turnContext.SendActivityAsync(HandleIncomingAttachment(turnContext.Activity));
            else
            {
                var reply = MessageFactory.Text("Here you have an image");
                switch (receivedMessage)
                {
                    case "image":
                        
                        reply.Attachments = new List<Attachment> { GetImageAttachment() };
                        await turnContext.SendActivityAsync(reply);
                        break;
                    case "imageUrl":
                       
                        reply.Attachments = new List<Attachment> { GetImageAttachmentFromUrl() };
                        await turnContext.SendActivityAsync(reply);
                        break;
                    default:
                        await turnContext.SendActivityAsync(MessageFactory.Text($"I got your message. Yours was {receivedMessage}"));
                        break;
                }
            }

        }

        private Attachment GetImageAttachmentFromUrl()
        {
            return new Attachment
            {
                Name = "New attachment",
                ContentType = "image/jpg",
                ContentUrl = @"https://b.kisscc0.com/20180717/yiw/kisscc0-columbidae-rock-dove-computer-icons-release-dove-dove-5b4db27d2b7fd6.9022942915318186211782.jpg"
            };
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


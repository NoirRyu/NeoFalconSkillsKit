using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
//using Microsoft.Bot.Builder.Dialogs.Internals;
//using Microsoft.Bot.Builder.Internals.Fibers;
//using Autofac;
//using System.Threading;
using System.Diagnostics;

namespace NeoFalconSkill
{
    //public sealed class TextToSpeakActivityMapper : IMessageActivityMapper
    //{
    //    private readonly IChannelCapability capability;

    //    public TextToSpeakActivityMapper(IChannelCapability capability)
    //    {
    //        SetField.NotNull(out this.capability, nameof(capability), capability);

    //    }

    //    public IMessageActivity Map(IMessageActivity message)
    //    {
    //        // only set the speak if it is not set by the developer.
    //        if (this.capability.SupportsSpeak() && !string.IsNullOrEmpty(message.Text) && string.IsNullOrEmpty(message.Speak))
    //        {
    //            message.Speak = message.Text;
    //            // Setting to ExpectingInput - safe to do in this sample, since this ActivityMapper is here to support FormFlow
    //            // and we are setting Speak and InputHint explicitly for all other responses.
    //            message.InputHint = InputHints.ExpectingInput;
    //        }
    //        return message;
    //    }
    //}

    //[BotAuthentication]
    //public class MessagesController : ApiController
    //{
    //    private readonly static IContainer Container;

    //    static MessagesController()
    //    {
    //        var builder = new ContainerBuilder();
    //        builder.RegisterModule(new DialogModule());

    //        // Add TextToSpeak mapper to list of mappers
    //        builder
    //            .RegisterType<TextToSpeakActivityMapper>()
    //            .As<IMessageActivityMapper>()
    //            .PreserveExistingDefaults()
    //            .InstancePerLifetimeScope();

    //        builder
    //            .RegisterInstance(new RootDialog())
    //            .AsSelf()
    //            .As<IDialog<object>>();

    //        Container = builder.Build();
    //    }

    //    /// <summary>
    //    /// POST: api/Messages
    //    /// Receive a message from a user and reply to it
    //    /// </summary>
    //    public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
    //    {
    //        if (activity != null)
    //        {
    //            // one of these will have an interface and process it
    //            switch (activity.GetActivityType())
    //            {
    //                case ActivityTypes.Message:
    //                    using (var scope = DialogModule.BeginLifetimeScope(Container, activity))
    //                    {
    //                        var task = scope.Resolve<IPostToBot>();
    //                        await task.PostAsync(activity, CancellationToken.None);
    //                    }
    //                    break;
    //                case ActivityTypes.EndOfConversation:
    //                    // delete Conversation and PrivateConversation data when cortana channel sending EndOfConversation
    //                    if (activity.ChannelId == ChannelIds.Cortana)
    //                    {
    //                        using (var scope = DialogModule.BeginLifetimeScope(Container, activity))
    //                        {
    //                            var botData = scope.Resolve<IBotData>();
    //                            await botData.LoadAsync(CancellationToken.None);
    //                            botData.ConversationData.Clear();
    //                            botData.PrivateConversationData.Clear();
    //                            await botData.FlushAsync(CancellationToken.None);
    //                        }
    //                    }
    //                    break;
    //                case ActivityTypes.ConversationUpdate:
    //                case ActivityTypes.ContactRelationUpdate:
    //                case ActivityTypes.Typing:
    //                case ActivityTypes.DeleteUserData:
    //                case ActivityTypes.Ping:
    //                default:
    //                    Trace.TraceError($"Unknown activity type ignored: {activity.GetActivityType()}");
    //                    break;
    //            }
    //        }
    //        return new HttpResponseMessage(System.Net.HttpStatusCode.Accepted);
    //    }

    //}

    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                await Conversation.SendAsync(activity, () => new Dialogs.RootDialog());
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}
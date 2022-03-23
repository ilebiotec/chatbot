using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CamBot.Common.Cards
{
    public class MainOptionsCard
    {
        public static async Task ToShow(DialogContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync(activity: CreateCarusel(), cancellationToken);
        }
        private static Activity CreateCarusel()
        {
            var cardCitasMedicas = new HeroCard
            {
                Title = "Citas",
                Subtitle = "Opciones",
                Images = new List<CardImage> { new CardImage("https://essaludbotstorage.blob.core.windows.net/images/menu_01.png") },
                Buttons = new List<CardAction>()
                {
                    new CardAction(){Title = "Crear cita ", Value = "Crear cita médica", Type = ActionTypes.ImBack},
                    new CardAction(){Title = "Ver mis citas", Value = "Ver mis citas", Type = ActionTypes.ImBack}
                }
            };
            var cardInformacionContacto = new HeroCard
            {
                Title = "Información de contacto",
                Subtitle = "Opciones",
                Images = new List<CardImage> { new CardImage("https://essaludbotstorage.blob.core.windows.net/images/menu_02.png") },
                Buttons = new List<CardAction>()
                {
                    new CardAction(){Title = "Centro de contacto", Value = "Centro de contacto", Type = ActionTypes.ImBack},
                    new CardAction(){Title = "Sitio web", Value = "https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-howto-add-media-attachments?view=azure-bot-service-4.0&tabs=csharp", Type = ActionTypes.OpenUrl},
                }
            };
            var cardSiguenosRedes = new HeroCard
            {
                Title = "Síguenos en las redes",
                Subtitle = "Opciones",
                Images = new List<CardImage> { new CardImage("https://essaludbotstorage.blob.core.windows.net/images/menu_03.png") },
                Buttons = new List<CardAction>()
                {
                    new CardAction(){Title = "Facebook", Value = "https://www.facebook.com/profile.php?id=100063498366393", Type = ActionTypes.OpenUrl},
                    // new CardAction(){Title = "Instagram", Value = "", Type = ActionTypes.OpenUrl},
                    new CardAction(){Title = "Twitter", Value = "https://twitter.com/EsSaludPeru?ref_src=twsrc%5Egoogle%7Ctwcamp%5Eserp%7Ctwgr%5Eauthor", Type = ActionTypes.OpenUrl},
                }
            };
            var cardCalificación = new HeroCard
            {
                Title = "Calificación",
                Subtitle = "Opciones",
                Images = new List<CardImage> { new CardImage("https://essaludbotstorage.blob.core.windows.net/images/menu_04.png") },
                Buttons = new List<CardAction>()
                {
                    new CardAction(){Title = "Calificar Bot", Value = "Calificar Bot", Type = ActionTypes.ImBack}
                }
            };
            var optionsAttachments = new List<Attachment>()
            {
                cardCitasMedicas.ToAttachment(),
                cardInformacionContacto.ToAttachment(),
                cardSiguenosRedes.ToAttachment(),
                cardCalificación.ToAttachment()
            };
            var reply = MessageFactory.Attachment(optionsAttachments);
            reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            return reply as Activity;
        }
    }
}

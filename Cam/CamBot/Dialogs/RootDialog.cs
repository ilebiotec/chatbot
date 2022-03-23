using CamBot.Common.Cards;
using CamBot.Data;
using CamBot.Dialogs.CreateAppointment;
using CamBot.Dialogs.Qualification;
using CamBot.infrastructure.Luis;
using CamBot.infrastructure.QnAMakerAI;
using CamBot.infrastructure.SendGridEmail;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CamBot.Dialogs
{
    public class RootDialog: ComponentDialog
    {
        private readonly ILuisService _luisService;
        private readonly IDataBaseService _databaseService;
        private readonly ISendGridEmailService _sendGridEmailService;
        //private readonly UserState _userState;
        private readonly IQnAMakerAIService _qnAMakerAIService;

        public RootDialog(ILuisService luisService, IDataBaseService databaseService, UserState userState, ISendGridEmailService sendGridEmailService, IQnAMakerAIService qnAMakerAIService)
        {
            _qnAMakerAIService = qnAMakerAIService;
            _sendGridEmailService = sendGridEmailService;
            _luisService = luisService;
            _databaseService = databaseService;
            var waterfallSteps = new WaterfallStep[]
            {
                InitialProcess,
                FinalProcess
            };
            AddDialog(new QualificationDialog(_databaseService));
            AddDialog(new CreateAppointmentDialog(_databaseService, userState, _sendGridEmailService, _luisService));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            InitialDialogId = nameof(WaterfallDialog);
        }   

        private async Task<DialogTurnResult> InitialProcess(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var luisResult = await _luisService._luisRecognizer.RecognizeAsync(stepContext.Context, cancellationToken);
            return await ManageIntentions(stepContext, luisResult, cancellationToken);
        }

        private async Task<DialogTurnResult> ManageIntentions(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            var topIntent = luisResult.GetTopScoringIntent();
            switch (topIntent.intent)
            {
                case "Saludar":
                    await IntentSaludar(stepContext, luisResult, cancellationToken);
                    break;
                case "Agradecer":
                    await IntentAgradecer(stepContext, luisResult, cancellationToken);
                    break;
                case "Despedir":
                    await IntentDespedir(stepContext, luisResult, cancellationToken);
                    break;
                case "VerOpciones":
                    await IntentVerOpciones(stepContext, luisResult, cancellationToken);
                    break;
                case "VerCentroContacto":
                    await IntentVerCentroContacto(stepContext, luisResult, cancellationToken);
                    break;
                case "Calificar":
                    return await IntentCalificar(stepContext, luisResult, cancellationToken);
                
                case "CrearCita":
                    return await IntentCrearCita(stepContext, luisResult, cancellationToken);
                case "VerCita":
                    await IntentVerCita(stepContext, luisResult, cancellationToken);
                    break;
                case "None":
                    await IntentNone(stepContext, luisResult, cancellationToken);
                    break;
                default:
                    break;

            }
            return await stepContext.NextAsync(cancellationToken: cancellationToken);
        }









        #region  IntentLuis
        private async Task IntentVerCita(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync("Un momento por favor...⏳", cancellationToken: cancellationToken);
            await Task.Delay(1000);

            string idUser = stepContext.Context.Activity.From.Id;
            var medicalData = _databaseService.MedicalAppointment.Where(x => x.idUser == idUser).ToList();

            if (medicalData.Count > 0)
            {
                var pending = medicalData.Where(p => p.date >= DateTime.Now.Date).ToList();

                if (pending.Count > 0)
                {
                    await stepContext.Context.SendActivityAsync("Estas son tus citas pendientes:", cancellationToken: cancellationToken);

                    foreach (var item in pending)
                    {
                        await Task.Delay(1000);
                        if (item.date == DateTime.Now.Date && item.time < DateTime.Now.Hour)
                            continue;
                        string summaryMedical = $"📅 Fecha: {item.date.ToShortDateString()}" +
                            $"{Environment.NewLine}⏰ Hora: {item.time}";
                        await stepContext.Context.SendActivityAsync(summaryMedical, cancellationToken: cancellationToken);
                    }
                }
                else
                    await stepContext.Context.SendActivityAsync("Lo siento pero no cuentas con citas pendientes", cancellationToken: cancellationToken);
            }
            else
                await stepContext.Context.SendActivityAsync("Lo siento pero no cuentas con citas pendientes", cancellationToken: cancellationToken);

           // await stepContext.Context.SendActivityAsync("¿En qué más puedo ayudarte?", cancellationToken: cancellationToken);
        }

        private async Task<DialogTurnResult> IntentCrearCita(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            return await stepContext.BeginDialogAsync(nameof(CreateAppointmentDialog), null, cancellationToken);
        }

        private async Task<DialogTurnResult> IntentCalificar(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            return await stepContext.BeginDialogAsync(nameof(QualificationDialog), null, cancellationToken);
        }

        private async Task IntentVerCentroContacto(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            string phoneDetail = $"Nuestros número de atención son los siguientes:{Environment.NewLine}" +
                $"📞 +51 928802792{Environment.NewLine}📞 +51 977885012";

            string addressDetail = $"🏥 Estamos ubicados en:{Environment.NewLine} Av.Peru N°130, Sicuani, Cusco";

            await stepContext.Context.SendActivityAsync(phoneDetail, cancellationToken: cancellationToken);
            await Task.Delay(1000);
            await stepContext.Context.SendActivityAsync(addressDetail, cancellationToken: cancellationToken);
            await Task.Delay(1000);
            await stepContext.Context.SendActivityAsync("¿En qué más puedo ayudarte?", cancellationToken: cancellationToken);
        }

        private async Task IntentVerOpciones(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync("Aquí tengo mis opciones", cancellationToken: cancellationToken);
            await MainOptionsCard.ToShow(stepContext, cancellationToken);

        }
        private async Task IntentSaludar(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync("Hola, que gusto verte.", cancellationToken: cancellationToken);
        }

        private async Task IntentAgradecer(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync("No te preocupes, me gusta ayudar.", cancellationToken: cancellationToken);
        }

        private async Task IntentDespedir(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync("Espero verte pronto.", cancellationToken: cancellationToken);
        }

        private async Task IntentNone(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            var resultQnA = await _qnAMakerAIService._qnaMakerResult.GetAnswersAsync(stepContext.Context);

            var score = resultQnA.FirstOrDefault()?.Score;
            string response = resultQnA.FirstOrDefault()?.Answer;

            if (score >= 0.5)
            {
                await stepContext.Context.SendActivityAsync(response, cancellationToken: cancellationToken);
            }
            else
            {
                await stepContext.Context.SendActivityAsync($"No entiendo lo que me dices", cancellationToken: cancellationToken);
                await Task.Delay(1000);
                await IntentVerOpciones(stepContext, luisResult, cancellationToken);
            }
        }
        #endregion
        private async Task<DialogTurnResult> FinalProcess(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }
    }
}

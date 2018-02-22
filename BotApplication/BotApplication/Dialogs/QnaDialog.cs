using Microsoft.Bot.Builder.CognitiveServices.QnAMaker;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BotApplication.Dialogs
{
    [Serializable]
    public class QnaDialog : QnAMakerDialog
    {
        public QnaDialog() : base(
            new QnAMakerService(
                new QnAMakerAttribute(
#pragma warning disable GCop164 // Instead use {0}
                     subscriptionKey: ConfigurationManager.AppSettings["QnaSubscriptionKey"]
                    , knowledgebaseId: ConfigurationManager.AppSettings["QnaKnowledgeBaseId"]
#pragma warning restore GCop164 // Instead use {0}
                    , defaultMessage: "não encontrei sua mensagem"
                    , scoreThreshold: 0.5
                )
            )
        )
        {
            var bruno = ConfigurationManager.GetSection("appSettings");
        }

        protected override async Task RespondFromQnAMakerResultAsync(IDialogContext context
        , IMessageActivity message
        , QnAMakerResults result)
        {
            //return base.RespondFromQnAMakerResultAsync(context, message, result);
            var primeiraResposta = result.Answers.FirstOrDefault().Answer;
            var resp = (context.Activity as Activity).CreateReply();
            var dadosResp = primeiraResposta.Split(';');
            if (dadosResp.Length == 1)
            {
                await context.PostAsync(primeiraResposta);
                return;
            }

            var titulo = dadosResp[0].Trim();
            var descricao = dadosResp[1].Trim();
            var url = dadosResp[2].Trim();
            var urlImagem = dadosResp[3].Trim();

            HeroCard card = new HeroCard()
            {
                Title = titulo,
                Subtitle = descricao
            };

            card.Buttons = new List<CardAction>
            {
            new CardAction(type:ActionTypes.OpenUrl, title:"Compre agora", value: url)
            };

            card.Images = new List<CardImage> {
                new CardImage(url = urlImagem)
            };

            resp.Attachments.Add(card.ToAttachment());
            await context.PostAsync(resp);
        }
    }
}
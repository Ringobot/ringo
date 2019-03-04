using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using RingoBotNet.Helpers;
using RingoBotNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RingoBotNet
{
    public static class RingoBotMessages
    {
        public static IMessageActivity CouldNotFindStation(ConversationInfo info, string query)
        {
            var heroCard = NewHeroCard();

            string messageText = string.IsNullOrEmpty(query)
                ? "Could not find a Station to join 🤔"
                : $"Could not find Station {query} 🤔";

            if (info.IsGroup)
            {
                messageText += $" Would you like to start a Station? Play a Playlist in Spotify. Then click/tap **Play**";

                heroCard.Buttons.Add(
                    new CardAction
                    {
                        Title = $"Play",
                        Value = "play",
                        Type = ActionTypes.ImBack,
                    });
            }

            return MessageFactory.Attachment(
                new Attachment
                {
                    ContentType = HeroCard.ContentType,
                    Content = heroCard
                },
                text: messageText);
        }

        public static IMessageActivity JoinWhat() => MessageFactory.Text("Which Station would you like to Join? Try `\"join #channel_name\"` or `\"join @username\"`");

        public static IMessageActivity NowPlayingStation(ConversationInfo info, Station station)
        {
            var heroCard = NewHeroCard();

            if (station.Playlist.Images.Any())
            {
                heroCard.Images.Add(
                    new CardImage
                    {
                        Url = station.Playlist.Images[0].Url,
                        Alt = station.Playlist.Name
                    });
            }

            string messageText = info.IsGroup
                ? $"@{info.FromName} is now playing \"{station.Name}\" in #{info.ConversationName}! 📢"
                : $"Now playing \"{station.Name}\". Friends can type `\"join @{RingoBotHelper.ToHashtag(info.FromName)}\"` to join in! 🎉";

            if (info.IsGroup)
            {
                heroCard.Buttons.Add(
                    new CardAction
                    {
                        Title = $"Join \"{station.Playlist.Name}\" in #{station.Hashtag}",
                        Value = "join",
                        Type = ActionTypes.ImBack,
                    });
            }

            return MessageFactory.Attachment(
                new Attachment
                {
                    ContentType = HeroCard.ContentType,
                    Content = heroCard
                },
                text: messageText);
        }

        public static IMessageActivity StationNoLongerPlaying(ConversationInfo info, Station station)
        {
            var heroCard = NewHeroCard();

            heroCard.Buttons.Add(
                new CardAction
                {
                    Title = $"Play {station.Playlist.Name}",
                    Value = $"{RingoBotHelper.RingoHandleIfGroupChat(info)}play {station.Playlist.Uri}",
                    Type = ActionTypes.ImBack,
                });

            return MessageFactory.Attachment(
                new Attachment
                {
                    ContentType = HeroCard.ContentType,
                    Content = heroCard
                }, 
                text: $"#{station.Hashtag} is no longer playing. Would you like to play \"{station.Playlist.Name}\"?");
        }

        public static IMessageActivity UserHasJoined(ConversationInfo info, Station station)
        {
            var heroCard = NewHeroCard();

            //if (station.Playlist.Images.Any())
            //{
            //    heroCard.Images.Add(new CardImage { Url = station.Playlist.Images[0].Url, Alt = station.Playlist.Name });
            //}

            string messageText = null;

            if (info.IsGroup)
            {
                messageText = $"@{info.FromName} has joined \"{station.Playlist.Name}\" in #{station.Hashtag}! 🎉";

                heroCard.Buttons.Add(
                    new CardAction
                    {
                        Title = $"Join \"{station.Playlist.Name}\" in #{station.Hashtag}",
                        Value = "join",
                        Type = ActionTypes.ImBack,
                    });

                return MessageFactory.Attachment(
                        new Attachment { ContentType = HeroCard.ContentType, Content = heroCard }, text: messageText);

            }

            return MessageFactory.Text($"You have joined @{station.Hashtag}! 🎉");
        }

        private static HeroCard NewHeroCard(string text = null) 
            => new HeroCard
            {
                Buttons = new List<CardAction>(),
                Images = new List<CardImage>(),
                Text = text
            };

    }
}

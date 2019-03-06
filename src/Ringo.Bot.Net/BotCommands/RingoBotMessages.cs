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
                messageText += $" Would you like to start a Station? Play some music in Spotify. Then click/tap **Play**";

                heroCard.Buttons.Add(
                    new CardAction
                    {
                        Title = "Play",
                        Value = $"{RingoBotHelper.RingoHandleIfGroupChat(info)}play",
                        Type = ActionTypes.ImBack,
                    });
            }

            return MessageAttachment(heroCard, messageText);
        }

        public static IMessageActivity JoinWhat() => MessageFactory.Text("Which Station would you like to Join? Try `\"join #channel_name\"` or `\"join @username\"`");

        public static IMessageActivity NotPlayingAnything(ConversationInfo info)
        {
            var heroCard = NewHeroCard();

            heroCard.Buttons.Add(
                new CardAction
                {
                    Title = "Spotify is playing - Try again!",
                    Value = $"{RingoBotHelper.RingoHandleIfGroupChat(info)}play",
                    Type = ActionTypes.ImBack,
                });

            return MessageAttachment(
                heroCard,
                "You are not currently playing anything 🤔 Play some music in Spotify and try again.");
        }

        public static IMessageActivity NowPlayingNotSupported(ConversationInfo info, string type)
        {
            var heroCard = NewHeroCard();

            heroCard.Buttons.Add(
                new CardAction
                {
                    Title = "Try again!",
                    Value = $"{RingoBotHelper.RingoHandleIfGroupChat(info)}play",
                    Type = ActionTypes.ImBack,
                });

            return MessageAttachment(
                heroCard,
                $"You playing a {type} in Spotify which Ringo does not support 🤔 Play a Playlist, Album or Artists in Spotify and try again.");
        }

        public static IMessageActivity NowPlayingStation(ConversationInfo info, Station station)
        {
            var heroCard = NewHeroCard();

            var images = station.Album?.Images ?? station.Artist?.Images ?? station.Playlist?.Images;

            if (images.Any())
            {
                heroCard.Images.Add(
                    new CardImage
                    {
                        Url = images[0].Url,
                        Alt = station.Name
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
                        Title = $"Join \"{station.Name}\" in #{station.Hashtag}",
                        Value = "join",
                        Type = ActionTypes.ImBack,
                    });
            }

            return MessageAttachment(heroCard, messageText);
        }

        public static IMessageActivity StationNoLongerPlaying(ConversationInfo info, Station station)
        {
            var heroCard = NewHeroCard();
            string uri = station.Album?.Uri ?? station.Artist?.Uri ?? station.Playlist?.Uri;

            heroCard.Buttons.Add(
                new CardAction
                {
                    Title = $"Play {station.Name}",
                    Value = $"{RingoBotHelper.RingoHandleIfGroupChat(info)}play {uri}",
                    Type = ActionTypes.ImBack,
                });

            return MessageAttachment(
                heroCard, 
                $"#{station.Hashtag} is no longer playing. Would you like to play \"{station.Name}\"?");
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
                messageText = $"@{info.FromName} has joined \"{station.Name}\" in #{station.Hashtag}! 🎉";

                heroCard.Buttons.Add(
                    new CardAction
                    {
                        Title = $"Join \"{station.Name}\" in #{station.Hashtag}",
                        Value = "join",
                        Type = ActionTypes.ImBack,
                    });

                return MessageAttachment(heroCard, messageText);
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

        private static IMessageActivity MessageAttachment(HeroCard heroCard, string messageText)
            => MessageFactory.Attachment(
                new Attachment
                {
                    ContentType = HeroCard.ContentType,
                    Content = heroCard
                },
                text: messageText);
    }
}

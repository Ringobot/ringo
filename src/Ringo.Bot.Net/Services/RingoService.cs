using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Ringo.Bot.Net.State;
using SpotifyApi.NetCore;

namespace Ringo.Bot.Net.Services
{
    public class RingoService : IRingoService
    {
        private readonly HttpClient _http;
        private readonly ISearchApi _search;
        private readonly IPlayerApi _player;

        public RingoService(HttpClient httpClient, ISearchApi search, IPlayerApi player)
        {
            _http = httpClient;
            _search = search;
            _player = player;
        }

        public async Task PlayPlaylist(
            ITurnContext turnContext,
            string searchText,
            string accessToken,
            CancellationToken cancellationToken)
        {
            // search for the Playlist
            var results = await _search.Search(
                searchText,
                SpotifySearchTypes.Playlist,
                accessToken: accessToken);

            // if none found, return
            if (results.Playlists.Total == 0)
            {
                await turnContext.SendActivityAsync($"No playlists found!", cancellationToken: cancellationToken);
                return;
            }

            // Play the first playlist found
            await turnContext.SendActivityAsync(
                $"{results.Playlists.Total.ToString("N0")} playlists found.",
                cancellationToken: cancellationToken);

            try
            {
                await _player.PlayPlaylist(results.Playlists.Items[0].Id, accessToken);
                await turnContext.SendActivityAsync(
                    $"{turnContext.Activity.From.Name} is playing \"{results.Playlists.Items[0].Name}\"",
                    cancellationToken: cancellationToken);
            }
            catch (SpotifyApiErrorException ex)
            {
                await turnContext.SendActivityAsync(ex.Message, cancellationToken: cancellationToken);
                return;
            }
        }

        public async Task JoinPlaylist(
            ITurnContext turnContext,
            string joinUsername,
            ConversationData conversationData,
            string token,
            CancellationToken cancellationToken)
        {
            // is there a token for the playing user?
            if (!conversationData.ConversationUserTokens.ContainsKey(joinUsername))
            {
                await turnContext.SendActivityAsync(
                    $"Join failed. {joinUsername} has not asked Ringo to play anything.",
                    cancellationToken: cancellationToken);
                return;
            }

            var playingUsertoken = conversationData.ConversationUserTokens[joinUsername];

            try
            {
                // is the playing user playing anything?
                var info = await _player.GetCurrentPlaybackInfo(playingUsertoken.Token);

                if (!info.IsPlaying)
                {
                    await turnContext.SendActivityAsync(
                        $"Join failed. {joinUsername} is no longer playing anything.",
                        cancellationToken: cancellationToken);
                    return;
                }

                // is the playing user playing a playlist?
                if (info.Context.Type != "playlist")
                {
                    await turnContext.SendActivityAsync(
                        $"Join failed. {joinUsername} is no longer playing a Playlist.",
                        cancellationToken: cancellationToken);
                    return;
                }

                // play from offset
                await _player.PlayPlaylistOffset(info.Context.Uri, info.Item.Id, accessToken: token, positionMs: info.ProgressMs);

                await turnContext.SendActivityAsync(
                    $"{turnContext.Activity.From.Name} has joined {joinUsername} playing \"{info.Item.Name}\"",
                    cancellationToken: cancellationToken);
            }
            catch (SpotifyApiErrorException ex)
            {
                await turnContext.SendActivityAsync(ex.Message, cancellationToken: cancellationToken);
                return;
            }
        }
    }
}

using Microsoft.Bot.Builder;
using SpotifyApi.NetCore;
using System;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

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
            string text,
            string accessToken,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}

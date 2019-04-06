// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents.Client;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Integration;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Configuration;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RingoBotNet.Data;
using RingoBotNet.Helpers;
using RingoBotNet.Services;
using RingoBotNet.State;
using SpotifyApi.NetCore;

namespace RingoBotNet
{
    /// <summary>
    /// The Startup class configures services and the request pipeline.
    /// </summary>
    public class Startup
    {
        private ILoggerFactory _loggerFactory;
        private bool _isProduction = false;
        private readonly ILogger _logger;

        public Startup(IHostingEnvironment env, ILogger<Startup> logger)
        {
            _isProduction = env.IsProduction();
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();

            ConfigHelper.CheckConfig(Configuration);

            _logger = logger;
        }

        /// <summary>
        /// Gets the configuration that represents a set of key/value application configuration properties.
        /// </summary>
        /// <value>
        /// The <see cref="IConfiguration"/> that represents a set of key/value application configuration properties.
        /// </value>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> specifies the contract for a collection of service descriptors.</param>
        /// <seealso cref="IStatePropertyAccessor{T}"/>
        /// <seealso cref="https://docs.microsoft.com/en-us/aspnet/web-api/overview/advanced/dependency-injection"/>
        /// <seealso cref="https://docs.microsoft.com/en-us/azure/bot-service/bot-service-manage-channels?view=azure-bot-service-4.0"/>
        public void ConfigureServices(IServiceCollection services)
        {
            // Loads .bot configuration file and adds a singleton that your Bot can access through dependency injection.
            var secretKey = Configuration.GetSection("botFileSecret")?.Value;
            var botFilePath = Configuration.GetSection("botFilePath")?.Value;
            if (!File.Exists(botFilePath))
            {
                throw new FileNotFoundException($"The .bot configuration file was not found. botFilePath: {botFilePath}");
            }
            var botConfig = BotConfiguration.Load(botFilePath ?? @".\BotConfiguration.bot", secretKey);

            // Creates a logger for the application to use.
            // services.AddSingleton<ILogger>(_logger);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // don't serialise self-referencing loops
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };


            services.AddBot<RingoBot3>(options =>
            {
                services.AddSingleton(sp => botConfig ?? throw new InvalidOperationException($"The .bot config file could not be loaded. ({botConfig})"));

                // Retrieve current endpoint.
                var environment = _isProduction ? "production" : "development";
                var service = botConfig.Services.Where(s => s.Type == "endpoint" && s.Name == environment).FirstOrDefault();
                if (!(service is EndpointService endpointService))
                {
                    throw new InvalidOperationException($"The .bot file does not contain an endpoint with name '{environment}'.");
                }

                options.CredentialProvider = new SimpleCredentialProvider(
                    endpointService.AppId ?? Configuration[ConfigHelper.BotServiceEndpointAppId], 
                    endpointService.AppPassword ?? Configuration[ConfigHelper.BotServiceEndpointAppPassword]);

                // Catches any errors that occur during a conversation turn and logs them.
                options.OnTurnError = async (context, exception) =>
                {
                    _logger.LogError($"Exception caught : {exception}");
                    await context.SendActivityAsync("Sorry, it looks like something went wrong.");
                };

            });

            IStorage storage = new Microsoft.Bot.Builder.Azure.AzureBlobStorage(
                Configuration[ConfigHelper.StorageConnectionString], 
                Configuration[ConfigHelper.StorageStateContainer]);

            ConversationState conversationState = new ConversationState(storage);
            UserState userState = new UserState(storage);
            DialogState dialogState = new DialogState();

            services.AddSingleton(sp =>
            {
                // Create the custom state accessor.
                return new RingoBotAccessors(conversationState, userState)
                {
                    ConversationDataAccessor = conversationState.CreateProperty<ConversationData>(RingoBotAccessors.ConversationDataName),
                    UserProfileAccessor = userState.CreateProperty<UserProfile>(RingoBotAccessors.UserProfileName),
                    DialogState = conversationState.CreateProperty<DialogState>(RingoBotAccessors.DialogStateName),
                };
            });

            services.AddSingleton<ISpotifyService, SpotifyService>();
            services.AddSingleton<IRingoService, RingoService>();
            services.AddSingleton<IAlbumsApi, AlbumsApi>();
            services.AddSingleton<IArtistsApi, ArtistsApi>();
            services.AddSingleton<IPlaylistsApi, PlaylistsApi>();
            services.AddSingleton<IPlayerApi, PlayerApi>();
            services.AddSingleton<HttpClient, HttpClient>();
            services.AddSingleton<IUserAccountsService, UserAccountsService>();
            services.AddSingleton<IAccountsService, AccountsService>();
            services.AddSingleton<IAuthService, AuthService>();
            services.AddSingleton<IRingoBotCommands, RingoBotCommands>();
            services.AddSingleton<IUserData, UserData>();
            services.AddSingleton<IStationData, StationData>();
            services.AddSingleton<IListenerData, ListenerData>();
            services.AddSingleton<IStateData, StateData>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            SpotifyApi.NetCore.Logger.LoggerFactory = loggerFactory;

            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseMvc()
                .UseDefaultFiles()
                .UseStaticFiles()
                .UseBotFramework();
        }
    }
}

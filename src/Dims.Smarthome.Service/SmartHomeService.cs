///-----------------------------------------------------------------
///   File:         SmartHomeService.cs
///   Author:   	Andre Laskawy           
///   Date:         26.10.2018 17:50:31
///-----------------------------------------------------------------

namespace Dims.Smarthome.Service
{
    using Dims.Common.Models;
    using Google.Protobuf.WellKnownTypes;
    using Grpc.Core.Logging;
    using Nanomite;
    using Nanomite.Core.Network;
    using Nanomite.Core.Network.Common;
    using Nanomite.Core.Network.Common.Models;
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="SmartHomeService" />
    /// </summary>
    public sealed class SmartHomeService
    {
        /// <summary>
        /// The client
        /// </summary>
        private static NanomiteClient client;

        /// <summary>
        /// The smart home connection
        /// </summary>
        private static SmarthomeHandler smarthomeHandler;

        /// <summary>
        /// Gets the BrokerAddress
        /// </summary>
        public static string BrokerAddress { get; private set; }

        /// <summary>
        /// Gets the User
        /// </summary>
        public static string User { get; private set; }

        /// <summary>
        /// Gets the Pass
        /// </summary>
        public static string Pass { get; private set; }

        /// <summary>
        /// Gets the Secret
        /// </summary>
        public static string Secret { get; private set; }

        /// <summary>
        /// Gets the SmartHomeUser
        /// </summary>
        public static string SmartHomeUser { get; private set; }

        /// <summary>
        /// Gets the SmartHomePass
        /// </summary>
        public static string SmartHomePass { get; private set; }

        /// <summary>
        /// Gets the LivingRoomId
        /// </summary>
        public static string LivingRoomId { get; private set; }

        /// <summary>
        /// The current log level.
        /// </summary>
        public static LogLevel LoggingLevel { get; set; }

        /// <summary>
        /// The service guard
        /// </summary>
        private static Timer serviceGuard;

        /// <summary>
        /// The Init
        /// </summary>
        /// <param name="brokerAddress">The brokerAddress<see cref="string"/></param>
        /// <param name="user">The user<see cref="string"/></param>
        /// <param name="pass">The pass<see cref="string"/></param>
        /// <param name="secret">The secret<see cref="string"/></param>
        /// <param name="smarthHomeUser">The smarthHomeUser<see cref="string"/></param>
        /// <param name="smartHomePass">The smartHomePass<see cref="string"/></param>
        /// <param name="livingRoomId">The livingRoomId<see cref="string"/></param>
        public static async Task Run(string brokerAddress, string user, string pass, string secret,
            string smarthHomeUser, string smartHomePass, string livingRoomId)
        {
            LoggingLevel = LogLevel.Info;
            BrokerAddress = brokerAddress;
            User = user;
            Pass = pass;
            SmartHomeUser = smarthHomeUser;
            SmartHomePass = smartHomePass;
            LivingRoomId = livingRoomId;
            Secret = secret;

            // Init module to interact with smart home
            smarthomeHandler = new SmarthomeHandler(SmartHomeUser, SmartHomePass, LivingRoomId);
            
            /// Init connection to broker
            await InitBrokerConnection(smarthomeHandler, BrokerAddress, User, User, Pass, Secret);
            
            // reinit connection to smart home in the given intervall
            serviceGuard = new Timer(Connect, null, 1000, 1000 * 60 * 60);
        }

        /// <summary>
        /// The Run
        /// </summary>
        private static void Connect(object state)
        {
            try
            {
                Log(LogLevel.Debug, "Trying to connect to smart home api...");
                smarthomeHandler.Init();
                Log(LogLevel.Debug, "Connected to smart home api sucessfully.");
            }
            catch (Exception ex)
            {
                Log(ex);
            }
        }

        /// <summary>
        /// Runs the service.
        /// </summary>
        /// <param name="handler">The smarthome handler.</param>
        /// <returns>a task</returns>
        private static async Task InitBrokerConnection(SmarthomeHandler handler, string brokerAddress, string clientId, string user, string pass, string secret)
        {
            try
            {
                client = NanomiteClient.CreateGrpcClient(brokerAddress, clientId);
                client.OnConnected = async () => 
                {
                    SubscriptionMessage subscriptionMessage = new SubscriptionMessage() { Topic = "LivingRoomLightOn" };
                    await client.SendCommandAsync(subscriptionMessage, StaticCommandKeys.Subscribe);

                    subscriptionMessage = new SubscriptionMessage() { Topic = "LivingRoomLightOff" };
                    await client.SendCommandAsync(subscriptionMessage, StaticCommandKeys.Subscribe);

                    subscriptionMessage = new SubscriptionMessage() { Topic = "SetLogLevel" };
                    await client.SendCommandAsync(subscriptionMessage, StaticCommandKeys.Subscribe);
                };
                client.OnCommandReceived = (cmd, c) =>
                {
                    switch (cmd.Topic)
                    {
                        case "LivingRoomLightOn":
                            handler.TurnLightOnLivingRoom();
                            break;

                        case "LivingRoomLightOff":
                            handler.TurnLightOffLivingRoom();
                            break;

                        case "SetLogLevel":
                            var level = cmd.Data[0].CastToModel<LogLevelInfo>()?.Level;
                            LoggingLevel = (LogLevel)System.Enum.Parse(typeof(LogLevel), level);
                            break;
                    }
                };
                await client.ConnectAsync(user, pass, secret, true);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Logs a message to the server
        /// </summary>
        /// <param name="level">The level<see cref="LoggingLevel"/></param>
        /// <param name="message">The message<see cref="string"/></param>
        private static async void Log(LogLevel level, string message)
        {
            Debug.WriteLine(message);
            if (level >= LoggingLevel)
            {
                var cmd = new Command() { Type = CommandType.Action, Topic = level.ToString() };
                LogMessage logMessage = new LogMessage()
                {
                    Level = level.ToString(),
                    Message = message
                };
                cmd.Data.Add(Any.Pack(logMessage));

                await client.SendCommandAsync(cmd);
            }
        }

        /// <summary>
        /// Logs an error to the server
        /// </summary>
        /// <param name="ex">The ex<see cref="Exception"/></param>
        private static async void Log(Exception ex)
        {
            Debug.WriteLine(ex);
            if (LogLevel.Error >= LoggingLevel)
            {
                var cmd = new Command() { Type = CommandType.Action, Topic = LogLevel.Error.ToString() };
                LogMessage logMessage = new LogMessage()
                {
                    Level = LogLevel.Error.ToString(),
                    Message = ex.ToText(),
                    StackTrace = ex.StackTrace
                };
                cmd.Data.Add(Any.Pack(logMessage));

                await client.SendCommandAsync(cmd);
            }
        }
    }
}

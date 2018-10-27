///-----------------------------------------------------------------
///   File:         SmartHomeService.cs
///   Author:   	Andre Laskawy           
///   Date:         26.10.2018 17:50:31
///-----------------------------------------------------------------

namespace Dims.Smarthome.Service
{
    using Nanomite.Core.Network;
    using Nanomite.Core.Network.Common;
    using Nanomite.Core.Network.Common.Models;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="SmartHomeService" />
    /// </summary>
    public sealed class SmartHomeService
    {
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
        public static void Run(string brokerAddress, string user, string pass, string secret,
            string smarthHomeUser, string smartHomePass, string livingRoomId)
        {
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
            InitBrokerConnection(smarthomeHandler, BrokerAddress, User, User, Pass, Secret);
            
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
                smarthomeHandler.Init();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        /// <summary>
        /// Runs the service.
        /// </summary>
        /// <param name="handler">The smarthome handler.</param>
        /// <returns>a task</returns>
        private static async void InitBrokerConnection(SmarthomeHandler handler, string brokerAddress, string clientId, string user, string pass, string secret)
        {
            try
            {
                NanomiteClient client = NanomiteClient.CreateGrpcClient(brokerAddress, clientId);
                client.OnConnected = () => { Console.WriteLine("Connected"); };
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
                    }
                };
                await client.ConnectAsync(user, pass, secret);
                await Task.Delay(1000);

                SubscriptionMessage subscriptionMessage = new SubscriptionMessage() { Topic = "LivingRoomLightOn" };
                await client.SendCommandAsync(subscriptionMessage, StaticCommandKeys.Subscribe);

                subscriptionMessage = new SubscriptionMessage() { Topic = "LivingRoomLightOff" };
                await client.SendCommandAsync(subscriptionMessage, StaticCommandKeys.Subscribe);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}

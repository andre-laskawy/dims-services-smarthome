///-----------------------------------------------------------------
///   File:         Service.cs
///   Author:   	Andre Laskawy           
///   Date:         13.10.2018 09:04:40
///-----------------------------------------------------------------

namespace Dims.Smarthome.Service
{
    using Dims.Smarthome.Service.Helper;
    using Nanomite.Core.Network;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="Service" />
    /// </summary>
    public class Service
    {
        /// <summary>
        /// Gets or sets the quit event.
        /// </summary>
        public ManualResetEvent QuitEvent { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Service" /> class.
        /// </summary>
        public Service()
        {
            this.QuitEvent = new ManualResetEvent(false);
        }

        /// <summary>
        /// Runs the service.
        /// </summary>
        /// <param name="handler">The smarthome handler.</param>
        /// <returns>a task</returns>
        public async Task Run(SmarthomeHandler handler, ConfigHelper config)
        {
            try
            {
                NanomiteClient client = NanomiteClient.CreateGrpcClient(config.BrokerAddress, config.ClientId);
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
                await client.ConnectAsync(config.User, config.Pass, config.Secret);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}

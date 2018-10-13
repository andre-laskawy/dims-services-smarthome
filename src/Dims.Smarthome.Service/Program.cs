///-----------------------------------------------------------------
///   File:         program.cs
///   Author:   	Andre Laskawy           
///   Date:         13.10.2018 09:05:42
///-----------------------------------------------------------------

namespace Dims.Smarthome.Service
{
    using Dims.Smarthome.Service.Helper;
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="Program" />
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Defines the client
        /// </summary>
        private static Service service = new Service();

        /// <summary>
        /// The Main
        /// </summary>
        /// <param name="args">The args<see cref="string[]"/></param>
        public static void Main(string[] args)
        {
            try
            {
                ConfigHelper config = new ConfigHelper();
                SmarthomeHandler smarthomeHandler = new SmarthomeHandler(config.SmartHomeUser, config.SmartHomePass, config.LivingRoomId);
                smarthomeHandler.Init();

                new Task(async () =>
                {
                    await service.Run(smarthomeHandler, config);
                }).Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            service.QuitEvent.WaitOne();
        }
    }
}

///-----------------------------------------------------------------
///   File:         ConfigHelper.cs
///   Author:   	Andre Laskawy           
///   Date:         02.10.2018 16:47:28
///-----------------------------------------------------------------

namespace Dims.Smarthome.Service.Helper
{
    using Microsoft.Extensions.Configuration;
    using System;
    using System.IO;
    using System.Net;

    /// <summary>
    /// Defines the <see cref="ConfigHelper" />
    /// </summary>
    public class ConfigHelper
    {
        /// <summary>
        /// Defines the configuration
        /// </summary>
        private static IConfigurationRoot configuration = null;

        /// <summary>
        /// Gets or sets the secret.
        /// </summary>
        public string Secret
        {
            get
            {
                return Configuration["Secret"];
            }
            set
            {
                Configuration["Secret"] = value.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the SrcDeviceId
        /// Gets the source device identifier.
        /// </summary>
        public string ClientId
        {
            get
            {
                var id = Convert.ToString(Configuration["ClientId"]);
                if (string.IsNullOrEmpty(id))
                {
                    return Dns.GetHostName();
                }
                else
                {
                    return id;
                }
            }
            set { }
        }

        /// <summary>
        /// Gets or sets the broker user.
        /// </summary>
        public string User
        {
            get
            {
                return Configuration["User"];
            }
            set
            {
                Configuration["User"] = value.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the broker pass.
        /// </summary>
        public string Pass
        {
            get
            {
                return Configuration["Pass"];
            }
            set
            {
                Configuration["Pass"] = value.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the LocalCloudAddress
        /// Gets the local cloud address.
        /// </summary>
        public string BrokerAddress
        {
            get
            {
                return Convert.ToString(Configuration["BrokerAddress"]);
            }
            set
            {
                Configuration["BrokerAddress"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the smart home user.
        /// </summary>
        public string SmartHomeUser
        {
            get
            {
                return Configuration["SmartHomeUser"];
            }
            set
            {
                Configuration["SmartHomeUser"] = value.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the smart home pass.
        /// </summary>
        public string SmartHomePass
        {
            get
            {
                return Configuration["SmartHomePass"];
            }
            set
            {
                Configuration["SmartHomePass"] = value.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the living room identifier.
        /// </summary>
        public string LivingRoomId
        {
            get
            {
                return Configuration["LivingRoomId"];
            }
            set
            {
                Configuration["LivingRoomId"] = value.ToString();
            }
        }

        /// <summary>
        /// Gets the Configuration
        /// </summary>
        private IConfigurationRoot Configuration
        {
            get
            {
                if (configuration == null)
                {
                    configuration = GetConfig();
                }

                return configuration;
            }
        }

        /// <summary>
        /// The GetConfig
        /// </summary>
        /// <returns>The <see cref="IConfiguration"/></returns>
        private IConfigurationRoot GetConfig()
        {
            string assemblyPath = AppContext.BaseDirectory;

            if (!File.Exists(Path.Combine(assemblyPath, "appsettings.json")))
            {
                assemblyPath = AppContext.BaseDirectory;
            }

            var builder = new ConfigurationBuilder()
                   .SetBasePath(assemblyPath)
                   .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                   .AddJsonFile($"appsettings.Dev.json", optional: true);
            return builder.Build();
        }
    }
}

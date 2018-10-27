///-----------------------------------------------------------------
///   File:         SmarthomeHandler.cs
///   Author:   	Andre Laskawy           
///   Date:         27.10.2018 12:11:13
///-----------------------------------------------------------------

namespace Dims.Smarthome.Service
{
    using SmartHome.Innogy;
    using SmartHome.Innogy.Models;
    using System;

    /// <summary>
    /// Defines the <see cref="SmarthomeHandler" />
    /// </summary>
    internal class SmarthomeHandler
    {
        /// <summary>
        /// The session
        /// </summary>
        private SmarthomeSession session;

        /// <summary>
        /// The living room identifier
        /// </summary>
        private string livingRoomId;

        /// <summary>
        /// The user name
        /// </summary>
        private string userName;

        /// <summary>
        /// The password
        /// </summary>
        private string password;

        /// <summary>
        /// Initializes a new instance of the <see cref="SmarthomeHandler"/> class.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="pass">The pass.</param>
        /// <param name="livingRoomId">The living room identifier.</param>
        public SmarthomeHandler(string user, string pass, string livingRoomId)
        {
            this.livingRoomId = livingRoomId;
            this.userName = user;
            this.password = pass;
        }

        /// <summary>
        /// Initializes the connection.
        /// </summary>
        public void Init()
        {
            session = new SmarthomeSession();
            var success = session.Login(this.userName, this.password);
            if (success)
            {
                session.InitializeData();
            }
        }

        /// <summary>
        /// Turns the light of living room on.
        /// </summary>
        public void TurnLightOnLivingRoom()
        {
            ActionInfo action = new ActionInfo();
            action.ID = this.livingRoomId;
            action.Setting = "OnState";
            action.Value = true;
            try
            {
                session.DoAction(action);
            }
            catch (Exception ex)
            {
                Init();
            }
        }

        /// <summary>
        /// Turns the light of living room off.
        /// </summary>
        public void TurnLightOffLivingRoom()
        {
            ActionInfo action = new ActionInfo();
            action.ID = this.livingRoomId;
            action.Setting = "OnState";
            action.Value = false;
            try
            {
                session.DoAction(action);
            }
            catch (Exception ex)
            {
                Init();
            }
        }
    }
}
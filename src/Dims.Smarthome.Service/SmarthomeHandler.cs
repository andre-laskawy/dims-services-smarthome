using SmartHome.Innogy;
using SmartHome.Innogy.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Dims.Smarthome.Service
{
    public class SmarthomeHandler
    {
        /// <summary>
        /// The session
        /// </summary>
        private SmarthomeSession session = new SmarthomeSession();

        /// <summary>
        /// The timer
        /// </summary>
        private Timer timer = null;

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
        /// Initializes a new instance of the <see cref="SmarthomeHandler" /> class.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="pass">The pass.</param>
        /// <param name="livingRoomId">The living room identifier.</param>
        public SmarthomeHandler(string user, string pass, string livingRoomId)
        {
            this.livingRoomId = livingRoomId;
            this.userName = user;
            this.password = pass;
            timer = new Timer(Refresh, null, 1000 * 15, 1000 * 60 * 60 * 1);
        }

        /// <summary>
        /// Initializes the connection.
        /// </summary>
        public void Init()
        {
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
                Console.WriteLine(ex);
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
                Console.WriteLine(ex);
            }
        }

        /// <summary>
        /// Refreshes the specified state.
        /// </summary>
        /// <param name="state">The state.</param>
        private void Refresh(object state)
        {
            try
            {
                // myses.RefreshToken();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}

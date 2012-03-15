using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;

namespace DeckBuilder.Models
{
    public class PlayerIdentity : IIdentity
    {
        private System.Web.Security.FormsAuthenticationTicket ticket;

        public PlayerIdentity(System.Web.Security.FormsAuthenticationTicket ticket)
        {
            this.ticket = ticket;
        }

        public string AuthenticationType
        {
            get { return "Nerd"; }
        }

        public bool IsAuthenticated
        {
            get { return true; }
        }

        public string Name
        {
            get { return ticket.Name; }
        }

        public string FriendlyName
        {
            get { return ticket.UserData; }
        }

    }
}

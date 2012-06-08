using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using SignalR.Hubs;
using System.Web.Mvc;
using System.Security.Principal;
using DeckBuilder.Models;

using SignalR.Infrastructure;
using SignalR;
using SignalR.Hosting.AspNet;


namespace DeckBuilder.Async
{
    public class NotificationsHub : Hub
    {
        private DeckBuilderContext db = new DeckBuilderContext();

        public void ActivateHub(string name)
        {
            Caller.name = name;
            AddToGroup(name);
        }

        public static void UpdateNotifications(string name, int id)
        {
            IConnectionManager connectionManager = AspNetHost.DependencyResolver.Resolve<IConnectionManager>();
            dynamic clients = connectionManager.GetClients<NotificationsHub>();

            clients[name].updateNotifications(id);
        }


    }
}
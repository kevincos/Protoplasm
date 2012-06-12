using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using SignalR.Hubs;
using System.Web.Mvc;
using System.Security.Principal;
using DeckBuilder.Models;
using DeckBuilder.ViewModels;

using SignalR.Infrastructure;
using SignalR;
using SignalR.Hosting.AspNet;


namespace DeckBuilder.Async
{
    public class NotificationsHub : Hub
    {
        public void ActivateHub(string name)
        {
            Caller.name = name;
            AddToGroup("ALERT" + name);
        }

        public static void UpdateNotifications(string name, int id)
        {
            IConnectionManager connectionManager = AspNetHost.DependencyResolver.Resolve<IConnectionManager>();
            dynamic clients = connectionManager.GetClients<NotificationsHub>();

            clients["ALERT" + name].updateNotifications(id);
        }

        public static void AddMatch(DeckBuilderContext db, Seat seat, Table table)
        {
            SeatViewModel viewModel = new SeatViewModel(seat);
            String message = "";
            if (seat.Waiting == true)
                message = "Your turn in " + table.Game.Name + " with " + viewModel.formattedOpponentNames + ". (Ranked) " + DateTime.Now;
            else
                message = "A new game of " + table.Game.Name + " has been started with " + viewModel.formattedOpponentNames + ". (Ranked) " + DateTime.Now;

            Notification n = new Notification { PlayerID = seat.PlayerId, Message = message, TableID = table.TableID, DatePosted = DateTime.Now, Read = false, Url = "/Table/Play/" + table.TableID };
            db.Notifications.Add(n);

            NotificationsHub.UpdateNotifications(seat.Player.Name, seat.PlayerId);
        }

        public static void Challenge(DeckBuilderContext db, Seat seat, Table table)
        {
            SeatViewModel viewModel = new SeatViewModel(seat);
            String message = "";
            if (seat.Accepted == true)
                message = "You started a game of " + table.Game.Name + " with " + viewModel.formattedOpponentNames + ". (TableID:" + table.TableID + ") " + DateTime.Now;
            else
                message = "A new game of " + table.Game.Name + " has been proposed with " + viewModel.formattedOpponentNames + ". (TableID:" + table.TableID + ") " + DateTime.Now;

            Notification n = new Notification { PlayerID = seat.PlayerId, Message = message, TableID = table.TableID, DatePosted = DateTime.Now, Read = false, Url = "/Table/Play/" + table.TableID };
            db.Notifications.Add(n);
            NotificationsHub.UpdateNotifications(seat.Player.Name, seat.PlayerId);
        }

        public static void MarkAsRead(DeckBuilderContext db, Seat seat, Table table)
        {
            Notification existingNotification = db.Notifications.SingleOrDefault(n => n.PlayerID == seat.PlayerId && n.TableID == table.TableID);
            if (existingNotification != null)
            {
                existingNotification.Read = true;
                db.SaveChanges();
            }
            
        }

        public static void UpdateNotificationState(DeckBuilderContext db,Seat seat, Table table, bool previousWaitingState)
        {
            int targetPlayerId = seat.PlayerId;
            if (seat.Waiting != previousWaitingState && seat.Waiting == true)
            {
                // ADD NOTIFICATION

                Notification existingNotification = db.Notifications.FirstOrDefault(n => n.PlayerID == targetPlayerId && n.TableID == table.TableID);
                SeatViewModel viewModel = new SeatViewModel(seat);
                String newMessage = "Your turn in " + table.Game.Name + " with " + viewModel.formattedOpponentNames + ". (TableID:" + table.TableID + ") " + DateTime.Now;

                if (existingNotification == null)
                {
                    Notification n = new Notification { PlayerID = seat.PlayerId, Message = newMessage, TableID = table.TableID, DatePosted = DateTime.Now, Read = false, Url = "/Table/Play/" + table.TableID };
                    db.Notifications.Add(n);
                }
                else
                {
                    existingNotification.DatePosted = DateTime.Now;
                    existingNotification.Read = false;
                    existingNotification.Suppressed = false;
                    existingNotification.Url = "/Table/Play/" + table.TableID;
                    existingNotification.Message = newMessage;
                }
                db.SaveChanges();
                NotificationsHub.UpdateNotifications(seat.Player.Name, seat.PlayerId);
            }
            else if (seat.Waiting != previousWaitingState && seat.Waiting == false)
            {
                Notification existingNotification = db.Notifications.SingleOrDefault(n => n.PlayerID == targetPlayerId && n.TableID == table.TableID);
                if (existingNotification != null)
                {
                    existingNotification.Read = true;
                    existingNotification.Suppressed = true;
                    NotificationsHub.UpdateNotifications(seat.Player.Name, seat.PlayerId);
                    db.SaveChanges();
                }
            }
            else if (seat.Waiting == true)
            {
                Notification existingNotification = db.Notifications.SingleOrDefault(n => n.PlayerID == targetPlayerId && n.TableID == table.TableID);
                if (existingNotification != null)
                {
                    existingNotification.Read = true;
                    NotificationsHub.UpdateNotifications(seat.Player.Name, seat.PlayerId);
                    db.SaveChanges();
                }
            }
        }

    }
}
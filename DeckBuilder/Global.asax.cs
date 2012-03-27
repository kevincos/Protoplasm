using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Data.Entity;
using System.Web.Security;
using System.Security.Principal;
using DeckBuilder.Models;
using DeckBuilder.DAL;

namespace DeckBuilder
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        public override void Init()
        {
            this.AuthenticateRequest += new EventHandler(MvcApplication_AuthenticateRequest);
            this.PostAuthenticateRequest += new EventHandler(MvcApplication_PostAuthenticateRequest);
            base.Init();
        }

        protected void Application_Start()
        {
            //Database.SetInitializer<DeckBuilderContext>(null);
            //Database.SetInitializer<DeckBuilderContext>(new DeckBuilderInitializer());
            //Database.SetInitializer<DeckBuilderContext>(new Devtalk.EF.CodeFirst.DontDropDbJustCreateTablesIfModelChanged<DeckBuilderContext>());

            //Database.SetInitializer<DeckBuilderContext>(new DeckBuilder.App_Start.DontDropDbJustCreateTablesIfModelChanged<DeckBuilderContext>());
            //Database.SetInitializer<DeckBuilderContext>(new DeckBuilderInitializer());

            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            ValueProviderFactories.Factories.Add(new JsonValueProviderFactory());
        }

        void MvcApplication_PostAuthenticateRequest(object sender, EventArgs e)
        {
            HttpCookie authCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null)
            {
                string encTicket = authCookie.Value;
                if (!String.IsNullOrEmpty(encTicket))
                {
                    FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(encTicket);
                    PlayerIdentity id = new PlayerIdentity(ticket);
                    GenericPrincipal prin = new GenericPrincipal(id, null);
                    HttpContext.Current.User = prin;
                }
            }
        }

        void MvcApplication_AuthenticateRequest(object sender, EventArgs e)
        {
        }
    }
}
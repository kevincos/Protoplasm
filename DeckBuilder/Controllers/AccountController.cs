using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI;
using System.Web.Routing;
using DeckBuilder.Models;

using System.Runtime.Serialization.Json;
using System.Text;
using System.IO;
using System.Net;

using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OpenId;
using DotNetOpenAuth.OpenId.Extensions.SimpleRegistration;
using DotNetOpenAuth.OpenId.RelyingParty;
using DotNetOpenAuth.OpenId.Extensions.AttributeExchange;

namespace DeckBuilder.Controllers
{
    
    public class AccountController : Controller
    {

        private DeckBuilderContext db = new DeckBuilderContext();

        private static OpenIdRelyingParty openid = new OpenIdRelyingParty();

        public IFormsAuthenticationService FormsService { get; set; }
        public IMembershipService MembershipService { get; set; }

        protected override void Initialize(RequestContext requestContext)
        {
            if (FormsService == null) { FormsService = new FormsAuthenticationService(); }
            if (MembershipService == null) { MembershipService = new AccountMembershipService(); }

            base.Initialize(requestContext);
        }

        public ActionResult LogOn()
        {

            return View();
        }

        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (MembershipService.ValidateUser(model.UserName, model.Password))
                {
                    if (db.Players.Where(p => p.Name == model.UserName).Count() == 0)
                    {
                        Player playerToAdd = db.Players.Add(new Player
                        {
                            Name = model.UserName
                        });
                        db.SaveChanges();
                        FormsService.SignIn(model.UserName, false /* createPersistentCookie */);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        Player player = db.Players.Single(p => p.Name == model.UserName);
                        FormsService.SignIn(model.UserName, false /* createPersistentCookie */);
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult LogOff()
        {

            FormsService.SignOut();

            return RedirectToAction("Index", "Home");
        }



        public ActionResult Register(string OpenID, string FacebookID)
        {
            ViewBag.PasswordLength = MembershipService.MinPasswordLength;
            ViewBag.OpenID = OpenID;
            ViewBag.FacebookID = FacebookID;
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user

                String uniqueID = null;
                if (model.OpenID != null)
                    uniqueID = "OPENID" + model.OpenID;
                if (model.FacebookID != null)
                    uniqueID = "FACEBOOKID" + model.FacebookID;

                MembershipCreateStatus createStatus = MembershipService.CreateUser(model.UserName, model.Password, model.Email, uniqueID);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    if (db.Players.Where(p => p.Name == model.UserName).Count() == 0)
                    {
                        Player playerToAdd = db.Players.Add(new Player
                        {
                            Name = model.UserName
                        });
                        db.SaveChanges();
                        FormsService.SignIn(model.UserName, false /* createPersistentCookie */);
                        return RedirectToAction("Edit", "Player", new { id = playerToAdd.PlayerID });
                    }
                    else
                    {
                        Player player = db.Players.Single(p => p.Name == model.UserName);
                        FormsService.SignIn(model.UserName, false /* createPersistentCookie */);
                        return RedirectToAction("Edit", "Player", new { id = player.PlayerID });
                    }
                }
                else
                {
                    ModelState.AddModelError("", AccountValidation.ErrorCodeToString(createStatus));
                }
            }

            // If we got this far, something failed, redisplay form
            ViewBag.PasswordLength = MembershipService.MinPasswordLength;
            return View(model);
        }

        [Authorize]
        public ActionResult ChangePassword()
        {

            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;

            return View();
        }

        [Authorize]
        [HttpPost]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Exceptions result in password not being changed.")]
        public ActionResult ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {

            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;

            if (!ValidateChangePassword(currentPassword, newPassword, confirmPassword))
            {
                return View();
            }

            try
            {
                if (MembershipService.ChangePassword(User.Identity.Name, currentPassword, newPassword))
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("_FORM", "The current password is incorrect or the new password is invalid.");
                    return View();
                }
            }
            catch
            {
                ModelState.AddModelError("_FORM", "The current password is incorrect or the new password is invalid.");
                return View();
            }
        }

        public ActionResult ChangePasswordSuccess()
        {

            return View();
        }

        //this is the statically typed representation of the JSON object that will get returned from: https://graph.facebook.com/me
        public class FacebookUser
        {
            public long id { get; set; } //yes. the user id is of type long...dont use int
            public string first_name { get; set; }
            public string last_name { get; set; }
            public string name { get; set; }
            public string email { get; set; }

        }

        [HttpGet]
        [ActionName("FacebookLogin")]
        public ActionResult FacebookLogin()
        {
            //redirect to https://graph.facebook.com/oauth/authorize giving Facebook my application id, the request type and the redirect url
            return new RedirectResult("https://graph.facebook.com/oauth/authorize? type=web_server& client_id=451051788239586& redirect_uri=http://localhost:9047/Account/Handshake");
        }


        //this controller action will be called when Facebook redirects
        [HttpGet]
        [ActionName("handshake")]
        public ActionResult Handshake(string code)
        {
            //after authentication, Facebook will redirect to this controller action with a QueryString parameter called "code" (this is Facebook's Session key)

            //example uri: http://www.examplewebsite.com/facebook/handshake/?code=2.DQUGad7_kFVGqKTeGUqQTQ__.3600.1273809600-1756053625|dil1rmAUjgbViM_GQutw-PEgPIg.

            //this is your Facebook App ID
            string clientId = "451051788239586";

            //this is your Secret Key
            string clientSecret = "52d898f5348ea388526e2bf98cfa14a1";

            //we have to request an access token from the following Uri
            string url = "https://graph.facebook.com/oauth/access_token?client_id={0}&redirect_uri={1}&client_secret={2}&code={3}";

            //your redirect uri must be EXACTLY the same Uri that caused the initial authentication handshake
            string redirectUri = "http://localhost:9047/Account/Handshake";

            //Create a webrequest to perform the request against the Uri
            WebRequest request = WebRequest.Create(string.Format(url, clientId, redirectUri, clientSecret, code));

            //read out the response as a utf-8 encoding and parse out the access_token
            WebResponse response = request.GetResponse();
            Stream stream = response.GetResponseStream();
            Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
            StreamReader streamReader = new StreamReader(stream, encode);
            string accessToken = streamReader.ReadToEnd().Replace("access_token=", "");
            streamReader.Close();
            response.Close();

            //set the access token to some session variable so it can be used through out the session
            Session["FacebookAccessToken"] = accessToken;

            //now that we have an access token, query the Graph Api for the JSON representation of the User
            url = "https://graph.facebook.com/me?access_token={0}";

            //create the request to https://graph.facebook.com/me
            request = WebRequest.Create(string.Format(url, accessToken));

            //Get the response
            response = request.GetResponse();

            //Get the response stream
            stream = response.GetResponseStream();

            //Take our statically typed representation of the JSON User and deserialize the response stream
            //using the DataContractJsonSerializer
            DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(typeof(FacebookUser));
            FacebookUser facebookUser = new FacebookUser();
            facebookUser = dataContractJsonSerializer.ReadObject(stream) as FacebookUser;

            //close the stream
            response.Close();

            //capture the UserId
            Session["FacebookUserId"] = facebookUser.id;

            //Set the forms authentication auth cookie
            FormsAuthentication.SetAuthCookie(facebookUser.email, false);

            //redirect to home page so that user can start using your application    
            LogOnModel lm = new LogOnModel();
            lm.FacebookID = facebookUser.id.ToString();
            //check if user exist
            MembershipUser user = MembershipService.GetUser("FACEBOOKID" + lm.FacebookID); //TODO
            if (user != null)
            {
                lm.UserName = user.UserName;
                if (db.Players.Where(p => p.Name == user.UserName).Count() == 0)
                {
                    Player playerToAdd = db.Players.Add(new Player
                    {
                        Name = user.UserName
                    });
                    db.SaveChanges();
                    FormsService.SignIn(user.UserName, false /* createPersistentCookie */);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    Player player = db.Players.Single(p => p.Name == user.UserName);
                    FormsService.SignIn(user.UserName, false /* createPersistentCookie */);
                    return RedirectToAction("Index", "Home");
                }
            }

            return View("LogOn", lm);
        }

        [ValidateInput(false)]
        public ActionResult Authenticate(string returnUrl)
        {
            var response = openid.GetResponse();
            if (response == null)
            {
                //Let us submit the request to OpenID provider
                Identifier id;
                if (Identifier.TryParse(Request.Form["openid_identifier"], out id))
                {
                    try
                    {
                        var request = openid.CreateRequest(Request.Form["openid_identifier"]);
                        return request.RedirectingResponse.AsActionResult();
                    }
                    catch (ProtocolException ex)
                    {
                        ViewBag.Message = ex.Message;
                        return View("LogOn");
                    }
                }

                ViewBag.Message = "Invalid identifier";
                return View("LogOn");
            }

            //Let us check the response
            switch (response.Status)
            {

                case AuthenticationStatus.Authenticated:
                    LogOnModel lm = new LogOnModel();
                    lm.OpenID = response.ClaimedIdentifier;
                    //check if user exist
                    MembershipUser user = MembershipService.GetUser("OPENID" + lm.OpenID);
                    if (user != null)
                    {
                        lm.UserName = user.UserName;
                        if (db.Players.Where(p => p.Name == user.UserName).Count() == 0)
                        {
                            Player playerToAdd = db.Players.Add(new Player
                            {
                                Name = user.UserName
                            });
                            db.SaveChanges();
                            FormsService.SignIn(user.UserName, false /* createPersistentCookie */);
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            Player player = db.Players.Single(p => p.Name == user.UserName);
                            FormsService.SignIn(user.UserName, false /* createPersistentCookie */);
                            return RedirectToAction("Index", "Home");
                        }
                    }

                    return View("LogOn", lm);

                case AuthenticationStatus.Canceled:
                    ViewBag.Message = "Canceled at provider";
                    return View("LogOn");
                case AuthenticationStatus.Failed:
                    ViewBag.Message = response.Exception.Message;
                    return View("LogOn");
            }

            return new EmptyResult();
        }



        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.User.Identity is WindowsIdentity)
            {
                throw new InvalidOperationException("Windows authentication is not supported.");
            }
        }

        #region Validation Methods

        private bool ValidateChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            if (String.IsNullOrEmpty(currentPassword))
            {
                ModelState.AddModelError("currentPassword", "You must specify a current password.");
            }
            if (newPassword == null || newPassword.Length < MembershipService.MinPasswordLength)
            {
                ModelState.AddModelError("newPassword",
                    String.Format(CultureInfo.CurrentCulture,
                         "You must specify a new password of {0} or more characters.",
                         MembershipService.MinPasswordLength));
            }

            if (!String.Equals(newPassword, confirmPassword, StringComparison.Ordinal))
            {
                ModelState.AddModelError("_FORM", "The new password and confirmation password do not match.");
            }

            return ModelState.IsValid;
        }

        private bool ValidateLogOn(string userName, string password)
        {
            if (String.IsNullOrEmpty(userName))
            {
                ModelState.AddModelError("username", "You must specify a username.");
            }
            if (String.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("password", "You must specify a password.");
            }
            if (!MembershipService.ValidateUser(userName, password))
            {
                ModelState.AddModelError("_FORM", "The username or password provided is incorrect.");
            }

            return ModelState.IsValid;
        }

        private bool ValidateRegistration(string userName, string email, string password, string confirmPassword)
        {
            if (String.IsNullOrEmpty(userName))
            {
                ModelState.AddModelError("username", "You must specify a username.");
            }
            if (String.IsNullOrEmpty(email))
            {
                ModelState.AddModelError("email", "You must specify an email address.");
            }
            if (password == null || password.Length < MembershipService.MinPasswordLength)
            {
                ModelState.AddModelError("password",
                    String.Format(CultureInfo.CurrentCulture,
                         "You must specify a password of {0} or more characters.",
                         MembershipService.MinPasswordLength));
            }
            if (!String.Equals(password, confirmPassword, StringComparison.Ordinal))
            {
                ModelState.AddModelError("_FORM", "The new password and confirmation password do not match.");
            }
            return ModelState.IsValid;
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://msdn.microsoft.com/en-us/library/system.web.security.membershipcreatestatus.aspx for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "Username already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A username for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}

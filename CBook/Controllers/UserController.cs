using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using CBook.Models;
using System.Net;
using Antlr.Runtime.Misc;
using System.Web.Security;

namespace CBook.Controllers
{
  public class UserController : Controller
  {
    //Registration Action
    [HttpGet]
    public ActionResult Registration()
    {
      return View();
    }
    //Registration POST Action
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Registration([Bind(Exclude = "IsEmailVerified,ActivationCode")]User user)
    {
      bool Status = false;
      string Message = "";

      //Model Validation
      if (ModelState.IsValid)
      {
        //email already exist
        var isExist = IsEmailExist(user.EmailID);
        if (isExist)
        {
          ModelState.AddModelError("EmailExist", "Email already Exist");
          return View(user);
        }
        //Activation code generate
        user.ActivationCode = Guid.NewGuid();

        //Password Hashing
        user.Password = Crypto.Hash(user.Password);
        user.ConfirmPassword = Crypto.Hash(user.ConfirmPassword);
        user.IsEmailVerified = false;
        //save data to database
        using (CBookEntities dc = new CBookEntities())
        {
          dc.Users.Add(user);
          dc.SaveChanges();
          //send email to user
          SendVerificationLinkEmail(user.EmailID, user.ActivationCode.ToString());
          Message = "Registration successfully done ! account activation link has been send to your email id " + user.EmailID;
          Status = true;
        }
      }
      else
      {
        Message = "Invalid Request";
      }
      ViewBag.Message = Message;
      ViewBag.Status = Status;
      return View(user);
    }

    //Verify account
    [HttpGet]
    public ActionResult verifyAccount(string id)
    {
      bool Status = false;
      using (CBookEntities dc = new CBookEntities())
      {
        dc.Configuration.ValidateOnSaveEnabled = false;
        var v = dc.Users.Where(a => a.ActivationCode == new Guid(id)).FirstOrDefault();
        if (v != null)
        {
          v.IsEmailVerified = true;
          dc.SaveChanges();
          Status = true;
        }
        else
        {
          ViewBag.Message = "Invalid request";
        }
      }
      ViewBag.Status = Status;
      return View();
    }

    //Login
    [HttpGet]
    public ActionResult Login()
    {
      return View();
    }

    //Login POST

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Login(UserLogin login,string ReturnUrl)
    {
      string Message = "";
      using (CBookEntities dc = new CBookEntities())
      {
        var v = dc.Users.Where(a => a.EmailID == login.EmailID).FirstOrDefault();
        if(v != null)
        {
          if (string.Compare(Crypto.Hash(login.Password),v.Password) == 0)
          {
            int timeout = login.RememberMe ? 525600 : 1;
            var ticket = new FormsAuthenticationTicket(login.EmailID, login.RememberMe, timeout);
            string encrypted = FormsAuthentication.Encrypt(ticket);
            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
            cookie.Expires = DateTime.Now.AddMinutes(timeout);
            cookie.HttpOnly = true;
            Response.Cookies.Add(cookie);
            TempData["EmailID"] = login.EmailID;
            if (Url.IsLocalUrl(ReturnUrl))
            {
              return Redirect(ReturnUrl);
            }
            else
            {
              return RedirectToAction("Index","Home");
            }
          }
          else
          {
            Message = "Invalid Credentials provided";
          }
        }
        else
        {
          Message = "Invalid Credentials provided";
        }
      }
      ViewBag.Message = Message;
      return View();

    }

    //Logout
  [Authorize]
  [HttpPost]
  public ActionResult Logout()
   {
      FormsAuthentication.SignOut();
      return RedirectToAction("Login", "User");

   }

    [NonAction]
    public bool IsEmailExist(string emailID)
    {
      using (CBookEntities dc = new CBookEntities())
      {
        var v = dc.Users.Where(a => a.EmailID == emailID).FirstOrDefault();
        return v != null;
      }

    }

    [NonAction]
    public void SendVerificationLinkEmail(string emailID, string activationCode)
    {
      var verifyUrl = "/User/VerifyAccount/" + activationCode;
      var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

      var fromEmail = new MailAddress("neenasusan90@gmail.com", "Dotnet Awesome");
      var toEmail = new MailAddress(emailID);
      var fromEmailPassword = "susansusanneena90"; // Replace with actual password
      string subject = "Your account is successfully created!";

      string body = "<br/><br/>We are excited to tell you that your Dotnet Awesome account is" +
          " successfully created. Please click on the below link to verify your account" +
          " <br/><br/><a href='" + link + "'>" + link + "</a> ";

      var smtp = new SmtpClient
      {
        Host = "smtp.gmail.com",
        Port = 587,
        EnableSsl = true,
        DeliveryMethod = SmtpDeliveryMethod.Network,
        UseDefaultCredentials = false,
        Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
      };

      using (var message = new MailMessage(fromEmail, toEmail)
      {
        Subject = subject,
        Body = body,
        IsBodyHtml = true
      })
        smtp.Send(message);
    }

  }

}

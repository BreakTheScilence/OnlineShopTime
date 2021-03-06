﻿using Microsoft.AspNet.Identity;
using OnlineShopTime.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc; 

namespace OnlineShopTime.Controllers
{
    public class HomeController : Controller
    {
        IndexDataModel IndexViewData;

        public ActionResult Index()
        {
            IndexViewData = (IndexDataModel)Session["IndexData"];
            WorkWithTags WWT = new WorkWithTags();
            WorkWithOffers WWO = new WorkWithOffers(Server);
            WorkWithUsers WWU = new WorkWithUsers();

            if (IndexViewData == null)
            {
                IndexViewData = new IndexDataModel();
                IndexViewData.ShowString = "NewOffers";
                IndexViewData.NewOffers = WWO.GetNewOffers();
            }

            IndexViewData.WeightTags = WWT.GetWeightTags();
            ViewBag.ViewData = IndexViewData;
            Session["IndexData"] = IndexViewData;

            if (User.Identity.Name != "")
            {
                if (WWU.GetUserRole(IdentityExtensions.GetUserId(User.Identity)) == "Banned")
                   return RedirectToAction("UserBanned", "AccessDenied");
            }

            if (IndexViewData.ShowString == "NewOffers")
                IndexViewData.NewOffers = WWO.GetNewOffers();
            if (IndexViewData.ShowString == "TopUsers")
                IndexViewData.TopUsers = WWU.GetTopUsers();
            if (IndexViewData.ShowString == "TopOffers")
                IndexViewData.NewOffers = WWO.GetTopOffers();

            return View();
        }

        public ActionResult TabClick(int TabID)
        {
            IndexViewData = (IndexDataModel)Session["IndexData"];
            WorkWithOffers WWO = new WorkWithOffers(Server);            
            if (IndexViewData == null)
                IndexViewData = new IndexDataModel();
            switch (TabID)
            {

                case 1:                    
                    IndexViewData.ShowTopOffers();
                    IndexViewData.TopOffers = WWO.GetTopOffers();
                    break;
                case 2:
                    WorkWithUsers WWU = new WorkWithUsers();
                    IndexViewData.ShowTopUsers();
                    IndexViewData.TopUsers = WWU.GetTopUsers();
                    break;
                case 3:
                    IndexViewData.ShowNewOffers();
                    IndexViewData.NewOffers = WWO.GetNewOffers();
                    break;
            }

            Session["IndexData"] = IndexViewData;
            return RedirectToAction("Index", "Home");
        }

        public ActionResult ChangeCulture(string lang)
        {
            string callbackUrl = Request.UrlReferrer.AbsolutePath;
            var langCookie = new HttpCookie("lang", lang) { HttpOnly = true };
            Response.AppendCookie(langCookie);
            return Redirect(callbackUrl);
        }

        public ActionResult ChangeStyle(string style)
        {
            string callbackUrl = Request.UrlReferrer.AbsolutePath;
            var styleCookie = new HttpCookie("style", style) { HttpOnly = true };
            Response.AppendCookie(styleCookie);
            return Redirect(callbackUrl);
        }

        public void SetOfferRating()
        {
            float value = float.Parse(Request.Form["value"]);
            string offerID = Request.Form["id"];
            string userID = Request.Form["userID"];

            using (ShopDBEntities db  =  new ShopDBEntities()) {
                if (db.OfferRaiting.Any(m => m.OfferID == offerID && m.UserID == userID))
                    db.OfferRaiting.First(m => m.OfferID == offerID && m.UserID == userID).Raiting = value;
                else
                    db.OfferRaiting.Add(new OfferRaiting() { OfferID = offerID, Raiting = value, UserID = userID });
                db.SaveChanges();
            }
        }
        public ActionResult Search(string TagID)
        {
            if (TagID == null)
                TagID = (string)Session["TagID"];
            SearchTools ST = new SearchTools();
            ICollection<Offers> FiltredOffers = ST.GetOffersByTag(TagID);
            Session["TagID"] = TagID;
            return View(FiltredOffers);
        }
        public void SetLike()
        {
            SetUserRating(1);
        }

        public void SetDislike()
        {
            SetUserRating(-1);
        }

        private void SetUserRating(float val)
        {
            string userID = Request.Form["userID"];
            string voterID = Request.Form["voterID"];

            using (ShopDBEntities db = new ShopDBEntities())
            {
                if (db.UserRaiting.Any(m => m.UserID == userID && m.VoterID == voterID))
                {
                    var userRaiting = db.UserRaiting.First(m => m.UserID == userID && m.VoterID == voterID);
                    if (userRaiting.Rating != val)
                    {
                        Response.Write("raiting changed");
                        userRaiting.Rating = val;
                    }
                    else
                    {
                        Response.Write("nothing changed");
                    }
                }
                else
                {
                    Response.Write("new raiting");
                    db.UserRaiting.Add(new UserRaiting() { UserID = userID, VoterID = voterID, Rating = val });
                }
                db.SaveChanges();
            }
        }
    }
}
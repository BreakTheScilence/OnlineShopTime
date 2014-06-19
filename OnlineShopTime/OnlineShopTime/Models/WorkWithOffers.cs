﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineShopTime.Models;
using Microsoft.AspNet.Identity;
using System.Web.Security;


namespace OnlineShopTime.Models
{
    public class WorkWithOffers
    {
        ShopDBEntities Db;

        string UserID;

        string UserName; //For god know purpose.

        public WorkWithOffers(string UserName)
        {
            this.UserName = UserName;
            Db = new ShopDBEntities();
            GetActiveUserID(UserName);
        }
        public void GetActiveUserID(string UserName)
        {
            UserID = (from rec in Db.Users where rec.UserName == UserName select rec.UserID).FirstOrDefault();
        }
        public void GetTopOffers()
        {
            //Empty for now.
        }
        public void AddNewOffer(Offers newOffer)
        {
            newOffer.Users = (from rec in Db.Users where rec.UserID == UserID select rec).FirstOrDefault();
            newOffer.DateAndTime = DateTime.Now;
            newOffer.OfferedBy = UserID;
            newOffer.OfferID = Guid.NewGuid().ToString();
            Db.Offers.Add(newOffer);
            Db.SaveChanges();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RenoRatorLibrary;

namespace RenoRator.Models
{
    public class RegisterModel
    {
        
        public int ID { get; private set; }
        public int userTypeID { get; set; }
        public string fname { get; set; }
        public string lname { get; set; }
        public string email { get; set; }
        public string emailConfirm { get; set; }
        public string bio { get; set; }
        public string password { get; set; }
        public string passwordConfirm { get; set; }
        public int profileGalleryID { get; set; }
        public int profilePhotoID { get; set; }
        public int addressID { get; set; }
        public int portfolioGalleryID { get; set; }

        public void Save() {
            var db = new renoRatorDBEntities();
            User newUser = new User();
            newUser.userTypeID = this.userTypeID;
            newUser.fname = this.fname;
            newUser.lname = this.lname;
            newUser.email = this.email;
            newUser.password = this.password;
            if(!String.IsNullOrEmpty(this.bio))
                newUser.bio = this.bio;
            if(this.profileGalleryID > 0)
                newUser.profileGalleryID = this.profileGalleryID;
            if (this.profilePhotoID > 0)
                newUser.profilePhotoID = this.profilePhotoID;
            if (this.addressID > 0)
                newUser.addressID = this.addressID;
            if (this.portfolioGalleryID > 0)
                newUser.portfolioGalleryID = this.portfolioGalleryID;

            // salt and hash the password
            string salt = PasswordFunctions.CreateSalt(8);
            newUser.salt = salt;
            newUser.password = PasswordFunctions.CreateHash(newUser.password, salt);

            db.AddToUsers1(newUser);
            db.SaveChanges();
        }
    }


}
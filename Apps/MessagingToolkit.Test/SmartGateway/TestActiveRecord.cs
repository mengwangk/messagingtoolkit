//===============================================================================
// OSML - Open Source Messaging Library
//
//===============================================================================
// Copyright © TWIT88.COM.  All rights reserved.
//
// This file is part of Open Source Messaging Library.
//
// Open Source Messaging Library is free software: you can redistribute it 
// and/or modify it under the terms of the GNU General Public License version 3.
//
// Open Source Messaging Library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this software.  If not, see <http://www.gnu.org/licenses/>.
//===============================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;


namespace MessagingToolkit.Common.Test
{
    [TestFixture]
    public class TestActiveRecord
    {        
        /***
        public void TestInsertUser()
        {
            User user = new User();
            user.Name = "Administrator";
            user.Mobtel = "+60192292309";
            user.Email = "admin@twit88.com";
            user.LoginId = "Administrator";
            user.Password =  UserBo.GenerateHashedPassword("password");
            user.Save();
        }

       
        public void TestGeneratePassword()
        {
            string storedPassword = UserBo.GenerateHashedPassword("password");
            Console.WriteLine(storedPassword);

            Console.WriteLine(UserBo.VerifyPassword(storedPassword, "password1"));
        }

        [Test]
        public void TestRetrieveUser()
        {            
            User user = User.SingleOrDefault(x => x.Name.ToLower() == "administrator".ToLower());
            Console.WriteLine(user.Name);
        }
        ***/
    }
}

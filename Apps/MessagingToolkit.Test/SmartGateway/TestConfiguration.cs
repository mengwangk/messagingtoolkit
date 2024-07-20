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
using System.Collections;
using System.Data;

using MessagingToolkit.SmartGateway.Core.Data.ActiveRecord;

using NUnit.Framework;

namespace MessagingToolkit.Test.SmartGateway
{
    /// <summary>
    /// Configuration test class
    /// </summary>
    [TestFixture]
    public class TestConfiguration
    {
        /// <summary>
        /// Tests the menu.
        /// </summary>       
        public void TestMenu()
        {
            Dictionary<long, List<TreeMenu>> menuLevelLookup = new Dictionary<long, List<TreeMenu>>(10);

            foreach (TreeMenu menu in TreeMenu.All().OrderBy(m => m.Id))
            {
                List<TreeMenu> menuList;
                if (!menuLevelLookup.TryGetValue((long)menu.ParentId, out menuList))
                {
                    menuList = new List<TreeMenu>(5);
                    menuList.Add(menu);
                    menuLevelLookup.Add((long)menu.ParentId, menuList);
                }
                else
                {
                    menuList.Add(menu);
                }
            }
           
            /*
            Dictionary<long, Menu> menuLookup = new Dictionary<long, Menu>(10);

            int minLevel = (int)(from m in Menu.All() select m.ParentId).Min();
            int maxLevel = (int)(from m in Menu.All() select m.ParentId).Max();
            for (int level = minLevel; level <= maxLevel; level++)
            {
                IOrderedEnumerable<Menu> menus = Menu.Find(m => m.ParentId == level).OrderBy(x=>x.Sequence);
                foreach (Menu menu in menus)
                {
                    List<Menu> menuList;
                    if (!menuLevelLookup.TryGetValue((long)menu.ParentId, out menuList))
                    {
                        menuList = new List<Menu>(5);
                        menuList.Add(menu);
                        menuLevelLookup.Add((long)menu.ParentId, menuList);
                    }
                    else
                    {
                        menuList.Add(menu);
                    }
                    
                    menuLookup.Add(menu.Id, menu);                    
                }
            }
            */

            /*
            for (int level = minLevel; level <= maxLevel; level++)
            {

            }
            */
           
        }

       
    }
}

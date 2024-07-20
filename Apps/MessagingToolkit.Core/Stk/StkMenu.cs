using System.Collections.Generic;
using System.Collections;
using MessagingToolkit.Core.Properties;

namespace MessagingToolkit.Core.Stk
{

    /// <summary>
    /// STK menu.
    /// </summary>
    public class StkMenu : StkResponse
    {
        private readonly string title;
        private readonly IList<StkMenuItem> menuItems;

        /// <summary>
        /// Initializes a new instance of the <see cref="StkMenu" /> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="menuItems">The menu items.</param>
        /// <exception cref="System.ArgumentException"></exception>
        public StkMenu(string title, params object[] menuItems)
        {
            this.title = title;

            IList<StkMenuItem> tempMenuItems = new List<StkMenuItem>(menuItems.Length);
            foreach (object m in menuItems)
            {
                if (m is string)
                {
                    tempMenuItems.Add(new StkMenuItem("", (string)m, ""));
                }
                else if (m is StkMenuItem)
                {
                    tempMenuItems.Add((StkMenuItem)m);
                }
                else
                {
                    throw new System.ArgumentException();
                }
            }
            this.menuItems = tempMenuItems;
        }

        public virtual string Title
        {
            get
            {
                return title;
            }
        }

        /// <summary>
        /// SIM toolkit get request.
        /// </summary>
        /// <param name="menuOption"></param>
        /// <returns></returns>
        public virtual StkRequest GetRequest(string menuOption)
        {
            foreach (StkMenuItem m in this.menuItems)
            {
                if (m.Text.Equals(menuOption))
                {
                    return m;
                }
            }
            throw new GatewayException(Resources.Exception_STKMenuItemNotFoundException);
        }

        public virtual int ItemCount
        {
            get
            {
                return this.menuItems.Count;
            }
        }

        public virtual IList<StkMenuItem> MenuItems
        {
            get
            {
                return this.menuItems;
            }
        }

        public virtual IList<StkMenuItem> Items
        {
            get
            {
                return this.menuItems;
            }
        }
    }

}
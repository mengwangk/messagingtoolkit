

namespace MessagingToolkit.Core.Stk
{

    /// <summary>
    /// STK menu item.
    /// </summary>
    public class StkMenuItem : StkRequest
    {
        /// <summary>
        /// ID of this item 
        /// </summary>
        private readonly string id;
        
        private readonly string text;
        
        /// <summary>
        /// ID of the menu this item belongs to 
        /// </summary>
        private readonly string menuId;

        public StkMenuItem(string id, string text, string menuId)
        {
            this.id = id;
            this.text = text;
            this.menuId = menuId;
        }

        public virtual string Text
        {
            get
            {
                return text;
            }
        }

        public virtual string MenuId
        {
            get
            {
                return menuId;
            }
        }

        public virtual string Id
        {
            get
            {
                return id;
            }
        }

        public virtual StkRequest Request
        {
            get
            {
                return this;
            }
        }
    }

}
namespace MessagingToolkit.Core.Stk
{

    /// <summary>
    /// STK notification.
    /// </summary>
	public class StkNotification : StkResponse
	{
		private readonly string text;

        /// <summary>
        /// Initializes a new instance of the <see cref="StkNotification" /> class.
        /// </summary>
        /// <param name="text">The text.</param>
		public StkNotification(string text)
		{
			this.text = text;
		}

		public virtual string Text
		{
			get
			{
				return text;
			}
		}
	}

}
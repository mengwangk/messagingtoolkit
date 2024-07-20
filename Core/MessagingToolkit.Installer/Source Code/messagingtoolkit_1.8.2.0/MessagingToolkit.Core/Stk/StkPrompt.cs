namespace MessagingToolkit.Core.Stk
{

    /// <summary>
    /// STK prompt.
    /// </summary>
	public abstract class StkPrompt : StkResponse
	{
		private readonly string text;

		public StkPrompt(string promptText)
		{
			this.text = promptText;
		}

		public abstract StkRequest Request {get;}

		public virtual string Text
		{
			get
			{
				return this.text;
			}
		}
	}

}
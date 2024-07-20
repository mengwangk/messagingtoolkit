

namespace MessagingToolkit.Core.Stk
{

    /// <summary>
    /// STK confirmation prompt
    /// </summary>
	public class StkConfirmationPrompt : StkPrompt
	{
		public static readonly StkRequest REQUEST = new StkRequestBase();

		public StkConfirmationPrompt(string promptText) : base(promptText)
		{
		}

		public override StkRequest Request
		{
			get
			{
				return REQUEST;
			}
		}
	}

}


namespace MessagingToolkit.Core.Stk
{
    /// <summary>
    /// STK value prompt.
    /// </summary>
	public class StkValuePrompt : StkPrompt
	{
        public static readonly StkRequest REQUEST = new StkRequestBase();

        /// <summary>
        /// Initializes a new instance of the <see cref="StkValuePrompt" /> class.
        /// </summary>
        /// <param name="promptText">The prompt text.</param>
		public StkValuePrompt(string promptText) : base(promptText)
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
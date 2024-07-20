namespace MessagingToolkit.Barcode.DataMatrix.Encoder
{
    internal class DataMatrixChannel
    {
        byte[] _encodedWords;

        internal byte[] Input { get; set; }

        internal DataMatrixScheme EncScheme { get; set; }

        internal DataMatrixChannelStatus Invalid { get; set; }

        internal int InputIndex { get; set; }

        internal int EncodedLength { get; set; }

        internal int CurrentLength { get; set; }

        internal int FirstCodeWord { get; set; }

        internal byte[] EncodedWords
        {
            get { return this._encodedWords ?? (this._encodedWords = new byte[1558]); }
        }
    }

    internal class DmtxChannelGroup
    {
        DataMatrixChannel[] _channels;

        internal DataMatrixChannel[] Channels
        {
            get
            {
                if (_channels == null)
                {
                    _channels = new DataMatrixChannel[6];
                    for (int i = 0; i < 6; i++)
                    {
                        _channels[i] = new DataMatrixChannel();
                    }
                }
                return _channels;
            }
        }
    }
}

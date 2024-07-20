using System;
using System.Text;

using MessagingToolkit.Barcode.Common;

namespace MessagingToolkit.Barcode.MaxiCode.Decoder
{
	/// <summary>
	/// MaxiCodes can encode text or structured information as bits in one of several modes,
	/// with multiple character sets in one code. This class decodes the bits back into text.
	/// </summary>
	internal sealed class DecodedBitStreamParser {
	
		private const char SHIFTA = '\uFFF0';
		private const char SHIFTB = '\uFFF1';
		private const char SHIFTC = '\uFFF2';
		private const char SHIFTD = '\uFFF3';
		private const char SHIFTE = '\uFFF4';
		private const char TWOSHIFTA = '\uFFF5';
		private const char THREESHIFTA = '\uFFF6';
		private const char LATCHA = '\uFFF7';
		private const char LATCHB = '\uFFF8';
		private const char LOCK = '\uFFF9';
		private const char ECI = '\uFFFA';
		private const char NS = '\uFFFB';
		private const char PAD = '\uFFFC';
		private const char FS = '\u001C';
		private const char GS = '\u001D';
		private const char RS = '\u001E';

        private const string NINE_DIGITS = "000000000";
        private const string THREE_DIGITS = "000";

		//private static readonly NumberFormat NINE_DIGITS = new DecimalFormat("000000000");
		//private static readonly NumberFormat THREE_DIGITS = new DecimalFormat("000");
	
		private static readonly String[] SETS = {
				"\nABCDEFGHIJKLMNOPQRSTUVWXYZ" + ECI + FS + GS + RS + NS + ' ' + PAD + "\"#$%&'()*+,-./0123456789:" + SHIFTB + SHIFTC + SHIFTD + SHIFTE + LATCHB,
				"`abcdefghijklmnopqrstuvwxyz" + ECI + FS + GS + RS + NS + '{' + PAD + "}~\u007F;<=>?[\\]^_ ,./:@!|" + PAD + TWOSHIFTA + THREESHIFTA + PAD + SHIFTA + SHIFTC + SHIFTD + SHIFTE + LATCHA,
				"\u00C0\u00C1\u00C2\u00C3\u00C4\u00C5\u00C6\u00C7\u00C8\u00C9\u00CA\u00CB\u00CC\u00CD\u00CE\u00CF\u00D0\u00D1\u00D2\u00D3\u00D4\u00D5\u00D6\u00D7\u00D8\u00D9\u00DA" + ECI + FS + GS + RS
						+ "\u00DB\u00DC\u00DD\u00DE\u00DF\u00AA\u00AC\u00B1\u00B2\u00B3\u00B5\u00B9\u00BA\u00BC\u00BD\u00BE\u0080\u0081\u0082\u0083\u0084\u0085\u0086\u0087\u0088\u0089" + LATCHA + ' '
						+ LOCK + SHIFTD + SHIFTE + LATCHB,
				"\u00E0\u00E1\u00E2\u00E3\u00E4\u00E5\u00E6\u00E7\u00E8\u00E9\u00EA\u00EB\u00EC\u00ED\u00EE\u00EF\u00F0\u00F1\u00F2\u00F3\u00F4\u00F5\u00F6\u00F7\u00F8\u00F9\u00FA" + ECI + FS + GS + RS
						+ NS + "\u00FB\u00FC\u00FD\u00FE\u00FF\u00A1\u00A8\u00AB\u00AF\u00B0\u00B4\u00B7\u00B8\u00BB\u00BF\u008A\u008B\u008C\u008D\u008E\u008F\u0090\u0091\u0092\u0093\u0094" + LATCHA
						+ ' ' + SHIFTC + LOCK + SHIFTE + LATCHB,
				"\u0000\u0001\u0002\u0003\u0004\u0005\u0006\u0007\u0008\u0009\n\u000B\u000C\r\u000E\u000F\u0010\u0011\u0012\u0013\u0014\u0015\u0016\u0017\u0018\u0019\u001A" + ECI + PAD + PAD + '\u001B'
						+ NS + FS + GS + RS + "\u001F\u009F\u00A0\u00A2\u00A3\u00A4\u00A5\u00A6\u00A7\u00A9\u00AD\u00AE\u00B6\u0095\u0096\u0097\u0098\u0099\u009A\u009B\u009C\u009D\u009E" + LATCHA + ' '
						+ SHIFTC + SHIFTD + LOCK + LATCHB,
				"\u0000\u0001\u0002\u0003\u0004\u0005\u0006\u0007\u0008\u0009\n\u000B\u000C\r\u000E\u000F\u0010\u0011\u0012\u0013\u0014\u0015\u0016\u0017\u0018\u0019\u001A\u001B\u001C\u001D\u001E\u001F\u0020\u0021\"\u0023\u0024\u0025\u0026\u0027\u0028\u0029\u002A\u002B\u002C\u002D\u002E\u002F\u0030\u0031\u0032\u0033\u0034\u0035\u0036\u0037\u0038\u0039\u003A\u003B\u003C\u003D\u003E\u003F" };
	
		private DecodedBitStreamParser() {
		}
	
		static internal DecoderResult decode(byte[] bytes, int mode) {
			StringBuilder result = new StringBuilder(144);
			switch (mode) {
			case 2:
			case 3:
				String postcode;
				if (mode == 2) {
					int pc = GetPostCode2(bytes);
					// NumberFormat df = new DecimalFormat("0000000000".Substring(0,(GetPostCode2Length(bytes))-(0)));
                    postcode = pc.ToString("0000000000".Substring(0, (GetPostCode2Length(bytes)) - (0)));
				} else {
					postcode = GetPostCode3(bytes);
				}
				//String country = THREE_DIGITS.Format(GetCountry(bytes));
                //String service = THREE_DIGITS.Format(GetServiceClass(bytes));
                String country = GetCountry(bytes).ToString(THREE_DIGITS);;
                String service = GetServiceClass(bytes).ToString(THREE_DIGITS);
				result.Append(GetMessage(bytes, 10, 84));
				if (result.ToString().StartsWith("[)>" + RS + "01" + GS)) {
					result.Insert(9, postcode + GS + country + GS + service + GS);
				} else {
					result.Insert(0, postcode + GS + country + GS + service + GS);
				}
				break;
			case 4:
				result.Append(GetMessage(bytes, 1, 93));
				break;
			case 5:
				result.Append(GetMessage(bytes, 1, 77));
				break;
			}
			return new DecoderResult(bytes, result.ToString(), null, mode.ToString());
		}
	
		private static int GetBit(int bit, byte[] bytes) {
			bit--;
			return ((bytes[bit / 6] & (1 << (5 - (bit % 6)))) == 0) ? 0 : 1;
		}
	
		private static int GetInt(byte[] bytes, byte[] x) {
			int val = 0;
			for (int i = 0; i < x.Length; i++) {
				val += GetBit(x[i], bytes) << (x.Length - i - 1);
			}
			return val;
		}
	
		private static int GetCountry(byte[] bytes) {
			return GetInt(bytes, new byte[] { 53, 54, 43, 44, 45, 46, 47, 48, 37, 38 });
		}
	
		private static int GetServiceClass(byte[] bytes) {
			return GetInt(bytes, new byte[] { 55, 56, 57, 58, 59, 60, 49, 50, 51, 52 });
		}
	
		private static int GetPostCode2Length(byte[] bytes) {
			return GetInt(bytes, new byte[] { 39, 40, 41, 42, 31, 32 });
		}
	
		private static int GetPostCode2(byte[] bytes) {
			return GetInt(bytes, new byte[] { 33, 34, 35, 36, 25, 26, 27, 28, 29, 30, 19, 20, 21, 22, 23, 24, 13, 14, 15, 16, 17, 18, 7, 8, 9, 10, 11, 12, 1, 2 });
		}
	
		private static String GetPostCode3(byte[] bytes) {
			return new char[] { SETS[0][GetInt(bytes, new byte[] { 39, 40, 41, 42, 31, 32 })], SETS[0][GetInt(bytes, new byte[] { 33, 34, 35, 36, 25, 26 })],
							SETS[0][GetInt(bytes, new byte[] { 27, 28, 29, 30, 19, 20 })], SETS[0][GetInt(bytes, new byte[] { 21, 22, 23, 24, 13, 14 })],
							SETS[0][GetInt(bytes, new byte[] { 15, 16, 17, 18, 7, 8 })], SETS[0][GetInt(bytes, new byte[] { 9, 10, 11, 12, 1, 2 })], }.ToString();
		}
	
		private static String GetMessage(byte[] bytes, int start, int len) {
			StringBuilder sb = new StringBuilder();
			int shift = -1;
			int set = 0;
			int lastset = 0;
			for (int i = start; i < start + len; i++) {
				char c = SETS[set][bytes[i]];
				switch ((int) c) {
				case LATCHA:
					set = 0;
					shift = -1;
					break;
				case LATCHB:
					set = 1;
					shift = -1;
					break;
				case SHIFTA:
				case SHIFTB:
				case SHIFTC:
				case SHIFTD:
				case SHIFTE:
					lastset = set;
					set = c - SHIFTA;
					shift = 1;
					break;
				case TWOSHIFTA:
					lastset = set;
					set = 0;
					shift = 2;
					break;
				case THREESHIFTA:
					lastset = set;
					set = 0;
					shift = 3;
					break;
				case NS:
					int nsval = (bytes[++i] << 24) + (bytes[++i] << 18) + (bytes[++i] << 12) + (bytes[++i] << 6) + bytes[++i];
					//sb.Append(NINE_DIGITS.Format(nsval));
                    nsval.ToString(NINE_DIGITS);
					break;
				case LOCK:
					shift = -1;
					break;
				default:
					sb.Append(c);
					break;
				}
				if (shift-- == 0) {
					set = lastset;
				}
			}
			while (sb.Length > 0 && sb[sb.Length - 1] == PAD) {
				sb.Length=sb.Length - 1;
			}
			return sb.ToString();
		}
	
	}
}

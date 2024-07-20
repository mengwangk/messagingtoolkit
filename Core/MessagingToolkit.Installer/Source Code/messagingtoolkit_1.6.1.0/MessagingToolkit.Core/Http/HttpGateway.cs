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

namespace MessagingToolkit.Core.Http
{
    /// <summary>
    /// Base HTTP gateway class
    /// </summary>
    public abstract class HttpGateway: 
    {
        
	public HttpGateway(String id)
	{
		super(id);
	}

	List<String> HttpPost(URL url, List<HttpHeader> requestList)
	{
		List<String> responseList = new ArrayList<String>();
		URLConnection con;
		BufferedReader in;
		OutputStreamWriter out;
		StringBuffer req;
		String line;
		getService().getLogger().logInfo("HTTP POST: " + url, null, getGatewayId());
		con = url.openConnection();
		con.setConnectTimeout(20000);
		con.setDoInput(true);
		con.setDoOutput(true);
		con.setUseCaches(false);
		con.setRequestProperty("Content-Type", "application/x-www-form-urlencoded");
		out = new OutputStreamWriter(con.getOutputStream());
		req = new StringBuffer();
		for (int i = 0, n = requestList.size(); i < n; i++)
		{
			if (i != 0) req.append("&");
			req.append(requestList.get(i).key);
			req.append("=");
			if (requestList.get(i).unicode)
			{
				StringBuffer tmp = new StringBuffer(200);
				byte[] uniBytes = requestList.get(i).value.getBytes("UnicodeBigUnmarked");
				for (int j = 0; j < uniBytes.length; j++)
					tmp.append(Integer.toHexString(uniBytes[j]).length() == 1 ? "0" + Integer.toHexString(uniBytes[j]) : Integer.toHexString(uniBytes[j]));
				req.append(tmp.toString().replaceAll("ff", ""));
			}
			else req.append(URLEncoder.encode(requestList.get(i).value, "utf-8"));
		}
		out.write(req.toString());
		out.flush();
		out.close();
		in = new BufferedReader(new InputStreamReader((con.getInputStream())));
		while ((line = in.readLine()) != null)
			responseList.add(line);
		in.close();
		return responseList;
	}

	List<String> HttpGet(URL url) throws IOException
	{
		List<String> responseList = new ArrayList<String>();
		getService().getLogger().logInfo("HTTP GET: " + url, null, getGatewayId());
		URLConnection con = url.openConnection();
		con.setConnectTimeout(20000);
		con.setAllowUserInteraction(false);
		BufferedReader in = new BufferedReader(new InputStreamReader(con.getInputStream()));
		String inputLine;
		while ((inputLine = in.readLine()) != null)
			responseList.add(inputLine);
		in.close();
		return responseList;
	}

	String ExpandHttpHeaders(List<HttpHeader> httpHeaderList)
	{
		StringBuffer buffer = new StringBuffer();
		for (HttpHeader h : httpHeaderList)
		{
			buffer.append(h.key);
			buffer.append("=");
			buffer.append(h.value);
			buffer.append("&");
		}
		return buffer.toString();
	}

	class HttpHeader
	{
		public String key;

		public String value;

		public boolean unicode;

		public HttpHeader()
		{
			this.key = "";
			this.value = "";
			this.unicode = false;
		}

		public HttpHeader(String myKey, String myValue, boolean myUnicode)
		{
			this.key = myKey;
			this.value = myValue;
			this.unicode = myUnicode;
		}
	}

	String calculateMD5(String in)
	{
		try
		{
			MessageDigest md = MessageDigest.getInstance("MD5");
			byte[] pre_md5 = md.digest(in.getBytes("LATIN1"));
			String md5 = "";
			for (int i = 0; i < 16; i++)
			{
				if (pre_md5[i] < 0)
				{
					md5 += Integer.toHexString(256 + pre_md5[i]);
				}
				else if (pre_md5[i] > 15)
				{
					md5 += Integer.toHexString(pre_md5[i]);
				}
				else
				{
					md5 += "0" + Integer.toHexString(pre_md5[i]);
				}
			}
			return md5;
		}
		catch (UnsupportedEncodingException ex)
		{
			getService().getLogger().logError("Unsupported encoding.", ex, getGatewayId());
			return "";
		}
		catch (NoSuchAlgorithmException ex)
		{
			getService().getLogger().logError("No such algorithm.", ex, getGatewayId());
			return "";
		}
	    }

	   
	    public override int getQueueSchedulingInterval()
	    {
		    return 500;
	    }
    }
}

﻿//===============================================================================
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
   /// 
   /// </summary>
   public class ClickatellGateway: HttpGateway
{
	string apiId, username, password;
	string sessionId;
	KeepAlive keepAlive;

	boolean secure;

	Object SYNC_Commander;

	String HTTP = "http://";

	String HTTPS = "https://";

	String URL_BALANCE = "api.clickatell.com/http/getbalance";

	String URL_COVERAGE = "api.clickatell.com/utils/routeCoverage.php";

	String URL_QUERYMSG = "api.clickatell.com/http/querymsg";

	String URL_AUTH = "api.clickatell.com/http/auth";

	String URL_PING = "api.clickatell.com/http/ping";

	String URL_SENDMSG = "api.clickatell.com/http/sendmsg";

	String URL_SENDWAPSI = "api.clickatell.com/mms/si_push";

	public ClickatellHTTPGateway(String id, String myApiId, String myUsername, String myPassword)
	{
		super(id);
		this.apiId = myApiId;
		this.username = myUsername;
		this.password = myPassword;
		this.sessionId = null;
		this.secure = false;
		this.SYNC_Commander = new Object();
		setAttributes(AGateway.GatewayAttributes.SEND | AGateway.GatewayAttributes.WAPSI | AGateway.GatewayAttributes.CUSTOMFROM | AGateway.GatewayAttributes.BIGMESSAGES | AGateway.GatewayAttributes.FLASHSMS);
	}

	/**
	 * Sets whether the gateway works in unsecured (HTTP) or secured (HTTPS)
	 * mode. False denotes unsecured.
	 * 
	 * @param mySecure
	 *            True for HTTPS, false for plain HTTP.
	 */
	public void setSecure(boolean mySecure)
	{
		this.secure = mySecure;
	}

	/**
	 * Return the operation mode (HTTP or HTTPS).
	 * 
	 * @return True for HTTPS, false for HTTP.
	 * @see #setSecure(boolean)
	 */
	public boolean getSecure()
	{
		return this.secure;
	}

	@Override
	public void startGateway() throws TimeoutException, GatewayException, IOException, InterruptedException
	{
		getService().getLogger().logInfo("Starting gateway.", null, getGatewayId());
		connect();
		super.startGateway();
		setKeepAlive(new KeepAlive("KeepAlive [" + getGatewayId() + "]", getService(), 60 * 1000));
	}

	@Override
	public void stopGateway() throws TimeoutException, GatewayException, IOException, InterruptedException
	{
		getService().getLogger().logInfo("Stopping gateway.", null, getGatewayId());
		if (getKeepAlive() != null)
		{
			getKeepAlive().cancel();
			setKeepAlive(null);
		}
		super.stopGateway();
		this.sessionId = null;
	}

	@Override
	public float queryBalance() throws TimeoutException, GatewayException, IOException, InterruptedException
	{
		URL url;
		List<HttpHeader> request = new ArrayList<HttpHeader>();
		List<String> response;
		if (this.sessionId == null) throw new GatewayException("Internal Clickatell Gateway error.");
		url = new URL((this.secure ? this.HTTPS : this.HTTP) + this.URL_BALANCE);
		request.add(new HttpHeader("session_id", this.sessionId, false));
		synchronized (this.SYNC_Commander)
		{
			response = HttpPost(url, request);
		}
		if (response.get(0).indexOf("Credit:") == 0) return Float.parseFloat(response.get(0).substring(response.get(0).indexOf(':') + 1));
		return -1;
	}

	@Override
	public boolean queryCoverage(OutboundMessage msg) throws TimeoutException, GatewayException, IOException, InterruptedException
	{
		URL url;
		List<HttpHeader> request = new ArrayList<HttpHeader>();
		List<String> response;
		if (this.sessionId == null) throw new GatewayException("Internal Clickatell Gateway error.");
		url = new URL((this.secure ? this.HTTPS : this.HTTP) + this.URL_COVERAGE);
		request.add(new HttpHeader("session_id", this.sessionId, false));
		request.add(new HttpHeader("msisdn", msg.getRecipient().substring(1), false));
		synchronized (this.SYNC_Commander)
		{
			response = HttpPost(url, request);
		}
		if (response.get(0).indexOf("OK") == 0) return true;
		return false;
	}

	@Override
	public DeliveryStatuses queryMessage(String refNo) throws TimeoutException, GatewayException, IOException, InterruptedException
	{
		URL url;
		List<HttpHeader> request = new ArrayList<HttpHeader>();
		List<String> response;
		int pos;
		if (this.sessionId == null) throw new GatewayException("Internal Clickatell Gateway error.");
		url = new URL((this.secure ? this.HTTPS : this.HTTP) + this.URL_QUERYMSG);
		request.add(new HttpHeader("session_id", this.sessionId, false));
		request.add(new HttpHeader("apimsgid", refNo, false));
		synchronized (this.SYNC_Commander)
		{
			response = HttpPost(url, request);
		}
		pos = response.get(0).indexOf("Status:");
		setDeliveryErrorCode(Integer.parseInt(response.get(0).substring(pos + 7).trim()));
		switch (getDeliveryErrorCode())
		{
			case 1:
				return DeliveryStatuses.UNKNOWN;
			case 2:
			case 3:
			case 8:
			case 11:
				return DeliveryStatuses.KEEPTRYING;
			case 4:
				return DeliveryStatuses.DELIVERED;
			case 5:
			case 6:
			case 7:
				return DeliveryStatuses.ABORTED;
			case 9:
			case 10:
				return DeliveryStatuses.ABORTED;
			case 12:
				return DeliveryStatuses.ABORTED;
			default:
				return DeliveryStatuses.UNKNOWN;
		}
	}

	void connect() throws GatewayException, IOException
	{
		try
		{
			if (!authenticate()) throw new GatewayException("Cannot authenticate to Clickatell.");
		}
		catch (MalformedURLException e)
		{
			throw new GatewayException("Internal Clickatell Gateway error.");
		}
	}

	boolean authenticate() throws IOException, MalformedURLException
	{
		URL url;
		List<HttpHeader> request = new ArrayList<HttpHeader>();
		List<String> response;
		getService().getLogger().logDebug("Authenticate().", null, getGatewayId());
		url = new URL((this.secure ? this.HTTPS : this.HTTP) + this.URL_AUTH);
		request.add(new HttpHeader("api_id", this.apiId, false));
		request.add(new HttpHeader("user", this.username, false));
		request.add(new HttpHeader("password", this.password, false));
		synchronized (this.SYNC_Commander)
		{
			response = HttpPost(url, request);
		}
		if (response.get(0).indexOf("ERR:") == 0)
		{
			this.sessionId = null;
			return false;
		}
		this.sessionId = response.get(0).substring(4);
		return true;
	}

	boolean ping() throws IOException, MalformedURLException
	{
		URL url;
		List<HttpHeader> request = new ArrayList<HttpHeader>();
		List<String> response;
		getService().getLogger().logDebug("Ping()", null, getGatewayId());
		url = new URL((this.secure ? this.HTTPS : this.HTTP) + this.URL_PING);
		request.add(new HttpHeader("session_id", this.sessionId, false));
		synchronized (this.SYNC_Commander)
		{
			response = HttpPost(url, request);
		}
		if (response.get(0).indexOf("ERR:") == 0) return false;
		return true;
	}

	@Override
	public boolean sendMessage(OutboundMessage msg) throws TimeoutException, GatewayException, IOException, InterruptedException
	{
		URL url;
		List<HttpHeader> request = new ArrayList<HttpHeader>();
		List<String> response;
		int requestFeatures = 0;
		boolean ok = false;
		if (this.sessionId == null)
		{
			getService().getLogger().logError("No session defined.", null, getGatewayId());
			msg.setFailureCause(FailureCauses.GATEWAY_FAILURE);
			return false;
		}
		getService().getLogger().logDebug("sendMessage()", null, getGatewayId());
		try
		{
			if (msg.getType() == MessageTypes.OUTBOUND) url = new URL((this.secure ? this.HTTPS : this.HTTP) + this.URL_SENDMSG);
			else if (msg.getType() == MessageTypes.WAPSI) url = new URL((this.secure ? this.HTTPS : this.HTTP) + this.URL_SENDWAPSI);
			else
			{
				msg.setFailureCause(FailureCauses.BAD_FORMAT);
				getService().getLogger().logError("Incorrect message format.", null, getGatewayId());
				return false;
			}
			request.add(new HttpHeader("session_id", this.sessionId, false));
			request.add(new HttpHeader("to", msg.getRecipient().substring(1), false));
			request.add(new HttpHeader("concat", "3", false));
			if (msg.getPriority() < 0) request.add(new HttpHeader("queue", "3", false));
			else if (msg.getPriority() == 0) request.add(new HttpHeader("queue", "2", false));
			else if (msg.getPriority() >= 0) request.add(new HttpHeader("queue", "1", false));
			if (msg.getFrom() != null && msg.getFrom().length() != 0) request.add(new HttpHeader("from", msg.getFrom(), false));
			else if (getFrom() != null && getFrom().length() != 0) request.add(new HttpHeader("from", getFrom(), false));
			if (msg.getFlashSms()) request.add(new HttpHeader("msg_type", "SMS_FLASH", false));
			if (msg.getType() == MessageTypes.OUTBOUND)
			{
				if ((msg.getSrcPort() != -1) || (msg.getDstPort() != -1)) request.add(new HttpHeader("udh", msg.getPduUserDataHeader(), false));
				if (msg.getEncoding() == MessageEncodings.ENC7BIT) request.add(new HttpHeader("text", msg.getText(), false));
				else if (msg.getEncoding() == MessageEncodings.ENCUCS2)
				{
					request.add(new HttpHeader("unicode", "1", false));
					request.add(new HttpHeader("text", msg.getText(), true));
				}
			}
			else if (msg.getType() == MessageTypes.WAPSI)
			{
				request.add(new HttpHeader("si_id", String.valueOf(msg.getId()), false));
				if (((OutboundWapSIMessage) msg).getCreateDate() != null) request.add(new HttpHeader("si_created", formatDateUTC(((OutboundWapSIMessage) msg).getCreateDate()), false));
				if (((OutboundWapSIMessage) msg).getExpireDate() != null) request.add(new HttpHeader("si_expires", formatDateUTC(((OutboundWapSIMessage) msg).getExpireDate()), false));
				request.add(new HttpHeader("si_action", formatSignal(((OutboundWapSIMessage) msg).getSignal()), false));
				request.add(new HttpHeader("si_url", ((OutboundWapSIMessage) msg).getUrl().toString(), false));
				request.add(new HttpHeader("si_text", ((OutboundWapSIMessage) msg).getIndicationText(), false));
			}
			if (msg.getStatusReport()) request.add(new HttpHeader("deliv_ack", "1", false));
			if ((getFrom() != null && getFrom().length() != 0) || (msg.getFrom() != null && msg.getFrom().length() != 0)) requestFeatures += 16 + 32;
			if (msg.getFlashSms()) requestFeatures += 512;
			if (msg.getStatusReport()) requestFeatures += 8192;
			request.add(new HttpHeader("req_feat", "" + requestFeatures, false));
			synchronized (this.SYNC_Commander)
			{
				response = HttpPost(url, request);
			}
			if (response.get(0).indexOf("ID:") == 0)
			{
				msg.setRefNo(response.get(0).substring(4));
				msg.setDispatchDate(new Date());
				msg.setGatewayId(getGatewayId());
				msg.setMessageStatus(MessageStatuses.SENT);
				incOutboundMessageCount();
				ok = true;
			}
			else if (response.get(0).indexOf("ERR:") == 0)
			{
				switch (Integer.parseInt(response.get(0).substring(5, 8)))
				{
					case 1:
					case 2:
					case 3:
					case 4:
					case 5:
					case 6:
					case 7:
						msg.setFailureCause(FailureCauses.GATEWAY_AUTH);
						break;
					case 101:
					case 102:
					case 105:
					case 106:
					case 107:
					case 112:
					case 116:
					case 120:
						msg.setFailureCause(FailureCauses.BAD_FORMAT);
						break;
					case 114:
						msg.setFailureCause(FailureCauses.NO_ROUTE);
						break;
					case 301:
					case 302:
						msg.setFailureCause(FailureCauses.NO_CREDIT);
						break;
					default:
						msg.setFailureCause(FailureCauses.UNKNOWN);
						break;
				}
				msg.setRefNo(null);
				msg.setDispatchDate(null);
				msg.setMessageStatus(MessageStatuses.FAILED);
				ok = false;
			}
		}
		catch (MalformedURLException e)
		{
			getService().getLogger().logError("Malformed URL.", e, getGatewayId());
		}
		catch (IOException e)
		{
			getService().getLogger().logError("I/O error.", e, getGatewayId());
		}
		return ok;
	}

	String formatDateUTC(Date d)
	{
		String strDate = "", tmp = "";
		Calendar cal = Calendar.getInstance();
		cal.setTime(d);
		strDate = String.valueOf(cal.get(Calendar.YEAR));
		tmp = String.valueOf(cal.get(Calendar.MONTH) + 1);
		if (tmp.length() != 2) tmp = "0" + tmp;
		strDate += "-" + tmp;
		tmp = String.valueOf(cal.get(Calendar.DATE));
		if (tmp.length() != 2) tmp = "0" + tmp;
		strDate += "-" + tmp;
		tmp = String.valueOf(cal.get(Calendar.HOUR_OF_DAY));
		if (tmp.length() != 2) tmp = "0" + tmp;
		strDate += "T" + tmp;
		tmp = String.valueOf(cal.get(Calendar.MINUTE));
		if (tmp.length() != 2) tmp = "0" + tmp;
		strDate += ":" + tmp;
		tmp = String.valueOf(cal.get(Calendar.SECOND));
		if (tmp.length() != 2) tmp = "0" + tmp;
		strDate += ":" + tmp + "Z";
		return strDate;
	}

	String formatSignal(WapSISignals signal)
	{
		if (signal == WapSISignals.NONE) return "signal-none";
		else if (signal == WapSISignals.LOW) return "signal-low";
		else if (signal == WapSISignals.MEDIUM) return "signal-medium";
		else if (signal == WapSISignals.HIGH) return "signal-high";
		else if (signal == WapSISignals.DELETE) return "signal-delete";
		else return "signal-none";
	}

	class KeepAlive extends AServiceThread
	{
		public KeepAlive(String name, Service service, int delay)
		{
			super(name, service, delay, 0, true);
		}

		@Override
		public void process() throws Exception
		{
			if (ClickatellHTTPGateway.this.sessionId == null) return;
			synchronized (ClickatellHTTPGateway.this.SYNC_Commander)
			{
				ping();
			}
		}
	}

	KeepAlive getKeepAlive()
	{
		return this.keepAlive;
	}

	void setKeepAlive(KeepAlive keepAlive)
	{
		this.keepAlive = keepAlive;
	}
}

}

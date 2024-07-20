// SMSLib for Java v3
// A Java API library for sending and receiving SMS via a GSM modem
// or other supported gateways.
// Web Site: http://www.smslib.org
//
// Copyright (C) 2002-2009, Thanasis Delenikas, Athens/GREECE.
// SMSLib is distributed under the terms of the Apache License version 2.0
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

package org.smslib.helper;

import org.smslib.Settings;

public class Logger
{
	org.slf4j.Logger logger;

	private Settings S;

	public Logger(Settings myS)
	{
		setSettings(myS);
		this.logger = org.slf4j.LoggerFactory.getLogger("smslib");
	}

	public Logger(org.slf4j.Logger myLogger)
	{
		this.logger = myLogger;
	}

	public org.slf4j.Logger getLogger()
	{
		return this.logger;
	}

	public void logInfo(String message, Exception e, String gatewayId)
	{
		if (e == null) getLogger().info(formatMessage(message, gatewayId));
		else getLogger().info(formatMessage(message, gatewayId), e);
	}

	public void logWarn(String message, Exception e, String gatewayId)
	{
		if (e == null) getLogger().warn(formatMessage(message, gatewayId));
		else getLogger().warn(formatMessage(message, gatewayId), e);
	}

	public void logDebug(String message, Exception e, String gatewayId)
	{
		if (e == null) getLogger().debug(formatMessage(message, gatewayId));
		else getLogger().debug(formatMessage(message, gatewayId), e);
	}

	public void logError(String message, Exception e, String gatewayId)
	{
		if (e == null) getLogger().error(formatMessage(message, gatewayId));
		else getLogger().error(formatMessage(message, gatewayId), e);
	}

	private String formatMessage(String message, String gatewayId)
	{
		return ((gatewayId == null) ? message : "GTW: " + gatewayId + ": " + message);
	}

	Settings getSettings()
	{
		return this.S;
	}

	void setSettings(Settings myS)
	{
		this.S = myS;
	}
}

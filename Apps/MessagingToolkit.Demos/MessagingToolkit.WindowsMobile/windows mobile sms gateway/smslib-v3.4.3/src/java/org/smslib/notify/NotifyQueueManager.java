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

package org.smslib.notify;

import java.util.concurrent.PriorityBlockingQueue;
import org.smslib.Service;
import org.smslib.threading.AServiceThread;

public class NotifyQueueManager
{
	Service service;

	NotificationQueueManager notificationQueueManager;

	PriorityBlockingQueue<Notification> notifyQueue;

	public NotifyQueueManager(Service service)
	{
		setService(service);
		setNotifyQueue(new PriorityBlockingQueue<Notification>());
		setNotifyQueueManager(new NotificationQueueManager("NotifyQueueManager", service, 100));
	}

	public void cancel()
	{
		while (getNotifyQueue().size() != 0)
		{
			try
			{
				Thread.sleep(200);
			}
			catch (Exception e)
			{
				// Swallow this... its an artificial delay to drain queue of events...
			}
		}
		getNotifyQueueManager().cancel();
	}

	protected Service getService()
	{
		return this.service;
	}

	protected void setService(Service service)
	{
		this.service = service;
	}

	public PriorityBlockingQueue<Notification> getNotifyQueue()
	{
		return this.notifyQueue;
	}

	public void setNotifyQueue(PriorityBlockingQueue<Notification> notifyQueue)
	{
		this.notifyQueue = notifyQueue;
	}

	protected NotificationQueueManager getNotifyQueueManager()
	{
		return this.notificationQueueManager;
	}

	protected void setNotifyQueueManager(NotificationQueueManager notifyQueueManager)
	{
		this.notificationQueueManager = notifyQueueManager;
	}

	class NotificationQueueManager extends AServiceThread
	{
		public NotificationQueueManager(String name, Service service, int delay)
		{
			super(name, service, delay, 0, true);
		}

		@Override
		public void process() throws Exception
		{
			getService().getLogger().logDebug("NotifyQueueManager running...", null, null);
			Notification notification = getNotifyQueue().take();
			if (notification instanceof GatewayStatusNotification)
			{
				if (getService().getGatewayStatusNotification() != null)
				{
					GatewayStatusNotification n = (GatewayStatusNotification) notification;
					getService().getGatewayStatusNotification().process(n.getGatewayId(), n.getOldStatus(), n.getNewStatus());
				}
			}
			else if (notification instanceof CallNotification)
			{
				if (getService().getCallNotification() != null)
				{
					CallNotification n = (CallNotification) notification;
					getService().getCallNotification().process(n.getGatewayId(), n.getCallerId());
				}
			}
			else if (notification instanceof InboundMessageNotification)
			{
				if (getService().getInboundMessageNotification() != null)
				{
					InboundMessageNotification n = (InboundMessageNotification) notification;
					getService().getInboundMessageNotification().process(n.getGatewayId(), n.getMsgType(), n.getMsg());
				}
			}
			else if (notification instanceof OutboundMessageNotification)
			{
				if (getService().getOutboundMessageNotification() != null)
				{
					OutboundMessageNotification n = (OutboundMessageNotification) notification;
					getService().getOutboundMessageNotification().process(n.getGatewayId(), n.getMsg());
				}
			}
			getService().getLogger().logDebug("NotifyQueueManager end...", null, null);
		}
	}
}

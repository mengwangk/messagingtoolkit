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

package org.smslib.smsserver.interfaces;

import java.net.URL;
import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Statement;
import java.sql.Timestamp;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.Collection;
import java.util.Date;
import java.util.Properties;
import org.smslib.InboundMessage;
import org.smslib.OutboundBinaryMessage;
import org.smslib.OutboundMessage;
import org.smslib.OutboundWapSIMessage;
import org.smslib.StatusReportMessage;
import org.smslib.Message.MessageEncodings;
import org.smslib.Message.MessageTypes;
import org.smslib.OutboundMessage.MessageStatuses;
import org.smslib.OutboundWapSIMessage.WapSISignals;
import org.smslib.smsserver.SMSServer;

/**
 * Interface for database communication with SMSServer. <br />
 * Inbound messages and calls are logged in special tables, outbound messages
 * are retrieved from another table.
 */
public class Database extends Interface<Integer>
{
	public Database(String myInterfaceId, Properties myProps, SMSServer myServer, InterfaceTypes myType)
	{
		super(myInterfaceId, myProps, myServer, myType);
		setDescription("Default database interface.");
	}

	@Override
	public void start() throws Exception
	{
		Connection con;
		Statement cmd;
		Class.forName(getProperty("driver"));
		con = getDbConnection();
		if (con != null)
		{
			cmd = con.createStatement(ResultSet.TYPE_FORWARD_ONLY, ResultSet.CONCUR_READ_ONLY);
			cmd.executeUpdate("update " + getProperty("tables.sms_out", "smsserver_out") + " set status = 'U' where status = 'Q'");
			con.commit();
			cmd.close();
			con.close();
		}
		super.start();
	}

	@Override
	public void stop() throws Exception
	{
		Connection con;
		Statement cmd;
		con = getDbConnection();
		if (con != null)
		{
			cmd = con.createStatement(ResultSet.TYPE_FORWARD_ONLY, ResultSet.CONCUR_READ_ONLY);
			cmd.executeUpdate("update " + getProperty("tables.sms_out", "smsserver_out") + " set status = 'U' where status = 'Q'");
			con.commit();
			cmd.close();
			con.close();
		}
		super.stop();
	}

	@Override
	public void CallReceived(String gtwId, String callerId) throws Exception
	{
		Connection con;
		Statement cmd;
		ResultSet rs;
		con = getDbConnection();
		cmd = con.createStatement(ResultSet.TYPE_SCROLL_SENSITIVE, ResultSet.CONCUR_UPDATABLE);
		rs = cmd.executeQuery("select id, call_date, gateway_id, caller_id from " + getProperty("tables.calls", "smsserver_calls") + " where id = -1");
		rs.moveToInsertRow();
		rs.updateTimestamp("call_date", new Timestamp(new java.util.Date().getTime()));
		rs.updateString("gateway_id", gtwId);
		rs.updateString("caller_id", callerId);
		rs.insertRow();
		con.commit();
		rs.close();
		cmd.close();
		con.close();
	}

	@Override
	public void MessagesReceived(Collection<InboundMessage> msgList) throws Exception
	{
		Connection con;
		Statement cmd;
		ResultSet rs;
		con = getDbConnection();
		cmd = con.createStatement(ResultSet.TYPE_FORWARD_ONLY, ResultSet.CONCUR_UPDATABLE);
		rs = cmd.executeQuery("select id, process, originator, type, encoding, message_date, receive_date, text, original_ref_no, original_receive_date, gateway_id from " + getProperty("tables.sms_in", "smsserver_in") + " where id = -1");
		for (InboundMessage msg : msgList)
		{
			if ((msg.getType() == MessageTypes.INBOUND) || (msg.getType() == MessageTypes.STATUSREPORT))
			{
				rs.moveToInsertRow();
				rs.updateInt("process", 0);
				switch (msg.getEncoding())
				{
					case ENC7BIT:
						rs.updateString("encoding", "7");
						break;
					case ENC8BIT:
						rs.updateString("encoding", "8");
						break;
					case ENCUCS2:
						rs.updateString("encoding", "U");
						break;
					case ENCCUSTOM:
						rs.updateString("encoding", "C");
						break;
				}
				switch (msg.getType())
				{
					case INBOUND:
						rs.updateString("type", "I");
						rs.updateString("originator", msg.getOriginator());
						if (msg.getDate() != null) rs.updateTimestamp("message_date", new Timestamp(msg.getDate().getTime()));
						rs.updateString("original_ref_no", null);
						rs.updateTimestamp("original_receive_date",null);
						break;
					case STATUSREPORT:
						rs.updateString("type", "S");
						rs.updateString("originator", ((StatusReportMessage) msg).getRecipient());
						if (((StatusReportMessage) msg).getSent() != null) rs.updateTimestamp("message_date", new Timestamp(((StatusReportMessage) msg).getSent().getTime()));
						rs.updateString("original_ref_no", ((StatusReportMessage) msg).getRefNo());
						if (((StatusReportMessage) msg).getReceived() != null) rs.updateTimestamp("original_receive_date", new Timestamp(((StatusReportMessage) msg).getReceived().getTime()));
						if (getProperty("update_outbound_on_statusreport", "no").equalsIgnoreCase("yes"))
						{
							Statement cmd2;
							ResultSet rs2;
							cmd2 = con.createStatement(ResultSet.TYPE_FORWARD_ONLY, ResultSet.CONCUR_UPDATABLE);
							rs2 = cmd2.executeQuery("select id, status from " + getProperty("tables.sms_out", "smsserver_out") + " where recipient = '" + ((StatusReportMessage) msg).getRecipient() + "' and ref_no = '" + ((StatusReportMessage) msg).getRefNo() + "'");
							if (rs2.next())
							{
								switch (((StatusReportMessage) msg).getStatus())
								{
									case DELIVERED:
										rs2.updateString("status", "D");
										break;
									case KEEPTRYING:
										rs2.updateString("status", "P");
										break;
									case ABORTED:
										rs2.updateString("status", "A");
										break;
									case UNKNOWN:
										break;
								}
								rs2.updateRow();
							}
							rs2.close();
							cmd2.close();
						}
						break;
					default:
						break;
				}
				rs.updateTimestamp("receive_date", new Timestamp(new java.util.Date().getTime()));
				if (msg.getEncoding() == MessageEncodings.ENC8BIT) 	rs.updateString("text", msg.getPduUserData());
				else rs.updateString("text", (msg.getText().length() == 0 ? "" : msg.getText()));
				rs.updateString("gateway_id", msg.getGatewayId());
				rs.insertRow();
			}
		}
		rs.close();
		cmd.close();
		con.commit();
		con.close();
	}

	@Override
	public Collection<OutboundMessage> getMessagesToSend() throws Exception
	{
		Collection<OutboundMessage> msgList = new ArrayList<OutboundMessage>();
		OutboundMessage msg;
		Connection con;
		Statement cmd;
		ResultSet rs;
		int msgCount;
		msgCount = 1;
		con = getDbConnection();
		if (con != null)
		{
			cmd = con.createStatement(ResultSet.TYPE_SCROLL_SENSITIVE, ResultSet.CONCUR_UPDATABLE);
			rs = cmd.executeQuery("select id, type, recipient, text, wap_url, wap_expiry_date, wap_signal, create_date, originator, encoding, status_report, flash_sms, src_port, dst_port, sent_date, ref_no, priority, status, errors, gateway_id from " + getProperty("tables.sms_out", "smsserver_out") + " where status = 'U' order by priority desc");
			while (rs.next())
			{
				if (msgCount > Integer.parseInt(getProperty("batch_size"))) break;
				if (getServer().checkPriorityTimeFrame(rs.getInt("priority")))
				{
					switch (rs.getString("type").charAt(0))
					{
						case 'O':
							switch (rs.getString("encoding").charAt(0))
							{
								case '7':
									msg = new OutboundMessage(rs.getString("recipient").trim(), rs.getString("text").trim());
									msg.setEncoding(MessageEncodings.ENC7BIT);
									break;
								case '8':
									{
										String text = rs.getString("text").trim();
										byte bytes[] = new byte[text.length() / 2];
										for (int i = 0; i < text.length(); i +=2 )
										{
											int value = (Integer.parseInt("" + text.charAt(i), 16) * 16) + (Integer.parseInt("" + text.charAt(i + 1), 16));
											bytes[i / 2] = (byte) value;
										}
										msg = new OutboundBinaryMessage(rs.getString("recipient").trim(), bytes);
									}
									break;
								case 'U':
									msg = new OutboundMessage(rs.getString("recipient").trim(), rs.getString("text").trim());
									msg.setEncoding(MessageEncodings.ENCUCS2);
									break;
								default:
									msg = new OutboundMessage(rs.getString("recipient").trim(), rs.getString("text").trim());
									msg.setEncoding(MessageEncodings.ENC7BIT);
									break;
							}
							if (rs.getInt("flash_sms") == 1) msg.setFlashSms(true);
							if (rs.getInt("src_port") != -1)
							{
								msg.setSrcPort(rs.getInt("src_port"));
								msg.setDstPort(rs.getInt("dst_port"));
							}
							break;
						case 'W':
							Date wapExpiryDate;
							WapSISignals wapSignal;
							if (rs.getTime("wap_expiry_date") == null)
							{
								Calendar cal = Calendar.getInstance();
								cal.setTime(new Date());
								cal.add(Calendar.DAY_OF_YEAR, 7);
								wapExpiryDate = cal.getTime();
							}
							else wapExpiryDate = rs.getTimestamp("wap_expiry_date");
							if (rs.getString("wap_signal") == null) wapSignal = WapSISignals.NONE;
							else
							{
								switch (rs.getString("wap_signal").charAt(0))
								{
									case 'N':
										wapSignal = WapSISignals.NONE;
										break;
									case 'L':
										wapSignal = WapSISignals.LOW;
										break;
									case 'M':
										wapSignal = WapSISignals.MEDIUM;
										break;
									case 'H':
										wapSignal = WapSISignals.HIGH;
										break;
									case 'D':
										wapSignal = WapSISignals.DELETE;
										break;
									default:
										wapSignal = WapSISignals.NONE;
								}
							}
							msg = new OutboundWapSIMessage(rs.getString("recipient").trim(), new URL(rs.getString("wap_url").trim()), rs.getString("text").trim(), wapExpiryDate, wapSignal);
							break;
						default:
							throw new Exception("Message type '" + rs.getString("type") + "' is unknown!");
					}
					msg.setPriority(rs.getInt("priority"));
					if (rs.getInt("status_report") == 1) msg.setStatusReport(true);
					if ((rs.getString("originator") != null) && (rs.getString("originator").length() > 0)) msg.setFrom(rs.getString("originator").trim());
					msg.setGatewayId(rs.getString("gateway_id").trim());
					msgList.add(msg);
					getMessageCache().put(msg.getMessageId(), rs.getInt("id"));
					rs.updateString("status", "Q");
					rs.updateRow();
					con.commit();
					msgCount++;
				}
			}
			rs.close();
			cmd.close();
			con.close();
		}
		return msgList;
	}

	@Override
	public int getPendingMessagesToSend()
	{
		Connection con;
		Statement cmd;
		ResultSet rs;
		int count = -1;

		try
		{
			con = getDbConnection();
			if (con != null)
			{
				cmd = con.createStatement(ResultSet.TYPE_SCROLL_SENSITIVE, ResultSet.CONCUR_READ_ONLY);
				rs = cmd.executeQuery("select count(*) as cnt from " + getProperty("tables.sms_out", "smsserver_out") + " where status in ('U', 'Q')");
				if (rs.next()) count = rs.getInt("cnt");
				rs.close();
				cmd.close();
				con.close();
			}
		}
		catch (Exception e)
		{
			// Swallow this...
		}
		return count;
	}

	@Override
	public void markMessage(OutboundMessage msg) throws Exception
	{
		Connection con;
		PreparedStatement selectStatement, updateStatement;
		ResultSet rs;
		int errors;

		con = getDbConnection();

		selectStatement = con.prepareStatement("select errors from " + getProperty("tables.sms_out", "smsserver_out") + " where id = ?");
		selectStatement.setInt(1, getMessageCache().get(msg.getMessageId()));
		rs = selectStatement.executeQuery();
		rs.next();
		errors = rs.getInt("errors");
		rs.close();
		selectStatement.close();
		if (msg.getMessageStatus() == MessageStatuses.SENT)
		{
			updateStatement = con.prepareStatement("update " + getProperty("tables.sms_out", "smsserver_out") + " set status = ?, sent_date = ?, gateway_id = ?, ref_no = ? where id = ?");
			updateStatement.setString(1, "S");
			updateStatement.setTimestamp(2, new Timestamp(msg.getDispatchDate().getTime()));
			updateStatement.setString(3, msg.getGatewayId());
			updateStatement.setString(4,  msg.getRefNo());
			updateStatement.setInt(5, getMessageCache().get(msg.getMessageId()));
			updateStatement.executeUpdate();
			con.commit();
			updateStatement.close();
		}
		else
		{
			updateStatement = con.prepareStatement("update " + getProperty("tables.sms_out", "smsserver_out") + " set status = ?, errors = ? where id = ?");
			errors ++;
			if (errors > Integer.parseInt(getProperty("retries", "2"))) updateStatement.setString(1, "F");
			else updateStatement.setString(1, "U");
			updateStatement.setInt(2,errors);
			updateStatement.setInt(3, getMessageCache().get(msg.getMessageId()));
			updateStatement.executeUpdate();
			con.commit();
			updateStatement.close();
		}
		con.close();
		getMessageCache().remove(msg.getMessageId());
	}

	private Connection getDbConnection()
	{
		Connection dbCon = null;
		try
		{
			dbCon = DriverManager.getConnection(getProperty("url"), getProperty("username", ""), getProperty("password", ""));
			dbCon.setAutoCommit(false);
		}
		catch (SQLException e)
		{
			getService().getLogger().logError("SMSServer: Database Interface: error!", e, null);
		}
		return dbCon;
	}
}

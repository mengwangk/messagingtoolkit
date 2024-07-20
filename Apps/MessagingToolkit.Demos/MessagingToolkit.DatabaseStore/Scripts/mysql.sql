# HeidiSQL Dump 
#
# --------------------------------------------------------
# Host:                 127.0.0.1
# Database:             messagingtoolkit
# Server version:       5.1.30-community
# Server OS:            Win32
# Target-Compatibility: Standard ANSI SQL
# HeidiSQL version:     3.2 Revision: 1129
# --------------------------------------------------------

/*!40100 SET CHARACTER SET latin1;*/
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='ANSI';*/
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;*/


#
# Database structure for database 'messagingtoolkit'
#

CREATE DATABASE /*!32312 IF NOT EXISTS*/ "messagingtoolkit" /*!40100 DEFAULT CHARACTER SET utf8 */;

USE "messagingtoolkit";


#
# Table structure for table 'incomingcall'
#

CREATE TABLE /*!32312 IF NOT EXISTS*/ "incomingcall" (
  "Id" int(10) unsigned NOT NULL AUTO_INCREMENT,
  "CallDate" datetime NOT NULL,
  "CallerId" varchar(50) NOT NULL,
  "GatewayId" varchar(50) NOT NULL,
  PRIMARY KEY ("Id")
) /*!40100 DEFAULT CHARSET=utf8*/;



#
# Dumping data for table 'incomingcall'
#

TRUNCATE TABLE "incomingcall";
# (No data found.)



#
# Table structure for table 'incomingmessage'
#

CREATE TABLE /*!32312 IF NOT EXISTS*/ "incomingmessage" (
  "Id" int(10) unsigned NOT NULL AUTO_INCREMENT,
  "Process" int(10) unsigned NOT NULL,
  "Originator" varchar(50) NOT NULL,
  "Type" varchar(50) NOT NULL,
  "Encoding" varchar(50) NOT NULL,
  "MessageDate" datetime NOT NULL,
  "ReceivedDate" datetime NOT NULL,
  "Text" text NOT NULL,
  "OriginatorRefNo" varchar(50) DEFAULT NULL,
  "OriginatorReceivedDate" datetime DEFAULT NULL,
  "GatewayId" tinyint(50) DEFAULT NULL,
  PRIMARY KEY ("Id")
) /*!40100 DEFAULT CHARSET=utf8*/;



#
# Dumping data for table 'incomingmessage'
#

TRUNCATE TABLE "incomingmessage";
# (No data found.)



#
# Table structure for table 'outgoingcall'
#

CREATE TABLE /*!32312 IF NOT EXISTS*/ "outgoingcall" (
  "Id" int(10) unsigned NOT NULL AUTO_INCREMENT,
  "CallDate" datetime NOT NULL,
  "CallerId" varchar(50) NOT NULL,
  "GatewayId" varchar(50) NOT NULL,
  PRIMARY KEY ("Id")
) /*!40100 DEFAULT CHARSET=utf8*/;



#
# Dumping data for table 'outgoingcall'
#

TRUNCATE TABLE "outgoingcall";
# (No data found.)



#
# Table structure for table 'outgoingmessage'
#

CREATE TABLE /*!32312 IF NOT EXISTS*/ "outgoingmessage" (
  "Id" int(10) unsigned NOT NULL AUTO_INCREMENT,
  "Type" varchar(50) NOT NULL,
  "Recipient" varchar(50) NOT NULL,
  "Text" text NOT NULL,
  "WapUrl" varchar(255) NOT NULL,
  "WapExpiryDate" datetime DEFAULT NULL,
  "WapSignal" varchar(50) DEFAULT NULL,
  "CreateDate" datetime NOT NULL,
  "Originator" varchar(50) NOT NULL,
  "Encoding" varchar(50) NOT NULL DEFAULT '7',
  "RefNo" varchar(50) DEFAULT NULL,
  "StatusReport" int(50) NOT NULL DEFAULT '0',
  "Priority" int(10) unsigned NOT NULL DEFAULT '0',
  "FlashSms" int(10) unsigned NOT NULL DEFAULT '0',
  "SrcPort" int(11) NOT NULL DEFAULT '-1',
  "DestPort" int(11) NOT NULL DEFAULT '-1',
  "SentDate" datetime DEFAULT NULL,
  "Status" varchar(50) DEFAULT 'U',
  "Errors" varchar(500) DEFAULT NULL,
  "GatewayId" varchar(50) DEFAULT '*',
  PRIMARY KEY ("Id")
) /*!40100 DEFAULT CHARSET=utf8*/;



#
# Dumping data for table 'outgoingmessage'
#

TRUNCATE TABLE "outgoingmessage";
# (No data found.)



#
# Table structure for table 'privilege'
#

CREATE TABLE /*!32312 IF NOT EXISTS*/ "privilege" (
  "Id" int(10) unsigned NOT NULL AUTO_INCREMENT,
  "Name" varchar(50) NOT NULL,
  "Desc" varchar(50) NOT NULL,
  PRIMARY KEY ("Id")
) /*!40100 DEFAULT CHARSET=utf8*/;



#
# Dumping data for table 'privilege'
#

TRUNCATE TABLE "privilege";
# (No data found.)



#
# Table structure for table 'role'
#

CREATE TABLE /*!32312 IF NOT EXISTS*/ "role" (
  "Id" int(10) unsigned NOT NULL AUTO_INCREMENT,
  "Name" varchar(50) NOT NULL,
  "Desc" varchar(50) NOT NULL,
  PRIMARY KEY ("Id")
) /*!40100 DEFAULT CHARSET=utf8*/;



#
# Dumping data for table 'role'
#

TRUNCATE TABLE "role";
# (No data found.)



#
# Table structure for table 'roleprivilegemap'
#

CREATE TABLE /*!32312 IF NOT EXISTS*/ "roleprivilegemap" (
  "RoleId" int(10) unsigned NOT NULL,
  "PrivilegeId" int(10) unsigned NOT NULL,
  PRIMARY KEY ("PrivilegeId","RoleId")
) /*!40100 DEFAULT CHARSET=utf8*/;



#
# Dumping data for table 'roleprivilegemap'
#

TRUNCATE TABLE "roleprivilegemap";
# (No data found.)



#
# Table structure for table 'user'
#

CREATE TABLE /*!32312 IF NOT EXISTS*/ "user" (
  "Id" int(10) unsigned NOT NULL AUTO_INCREMENT,
  "Name" varchar(50) NOT NULL,
  "Mobtel" varchar(50) NOT NULL,
  "Email" varchar(200) NOT NULL,
  "LoginId" varchar(50) NOT NULL,
  "Password" varchar(50) NOT NULL,
  PRIMARY KEY ("Id")
) /*!40100 DEFAULT CHARSET=utf8*/;



#
# Dumping data for table 'user'
#

TRUNCATE TABLE "user";
# (No data found.)



#
# Table structure for table 'userrolemap'
#

CREATE TABLE /*!32312 IF NOT EXISTS*/ "userrolemap" (
  "UserId" int(10) unsigned NOT NULL,
  "RoleId" int(10) unsigned NOT NULL
) /*!40100 DEFAULT CHARSET=utf8*/;



#
# Dumping data for table 'userrolemap'
#

TRUNCATE TABLE "userrolemap";
# (No data found.)

/*!40101 SET SQL_MODE=@OLD_SQL_MODE;*/
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;*/

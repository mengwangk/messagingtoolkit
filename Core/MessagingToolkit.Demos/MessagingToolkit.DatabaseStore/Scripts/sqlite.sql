CREATE TABLE IncomingCall
(
       Id INTEGER PRIMARY KEY AUTOINCREMENT,
       CallDate DATETIME NOT NULL,
       CallerId VARCHAR(50) NOT NULL,
       GatewayId VARCHAR(50) NOT NULL
);

CREATE TABLE IncomingMessage
(
       Id INTEGER PRIMARY KEY AUTOINCREMENT,
       Process INTEGER NOT NULL,
       Originator VARCHAR(50) NOT NULL,
       Type VARCHAR(50) NOT NULL,
       Encoding VARCHAR(50) NOT NULL,
       MessageDate DATETIME NOT NULL,
       ReceivedDate DATETIME NOT NULL,
       Text TEXT NOT NULL,
       OriginatorRefNo VARCHAR(50),
       OriginatorReceivedDate DATETIME,
       GatewayId VARCHAR(50) NOT NULL
);

CREATE TABLE OutgoingCall
(
       Id INTEGER PRIMARY KEY AUTOINCREMENT,
       CallDate DATETIME NOT NULL,
       CallerId VARCHAR(50) NOT NULL,
       GatewayId VARCHAR(50) NOT NULL
);

CREATE TABLE OutgoingMessage
(
       Id INTEGER PRIMARY KEY AUTOINCREMENT,
       Type VARCHAR(50) NOT NULL,
       Recipient VARCHAR(50) NOT NULL,
       Text TEXT NOT NULL,
       WapUrl VARCHAR(50),
       WapExpiryDate DATETIME,
       WapSignal VARCHAR(50),
       CreateDate DATETIME NOT NULL DEFAULT "datetime('now')" ,
       Originator VARCHAR(50) NOT NULL DEFAULT "''" ,
       Encoding VARCHAR(50) NOT NULL DEFAULT "'7'" ,
       StatusReport INTEGER NOT NULL DEFAULT "0" ,
       FlashSms INTEGER NOT NULL DEFAULT "0" ,
       SrcPort INTEGER NOT NULL DEFAULT "-1" ,
       DestPort INTEGER NOT NULL DEFAULT "-1" ,
       RefNo VARCHAR(50),
       SentDate DATETIME,
       Priority INTEGER NOT NULL DEFAULT "0" ,
       Status VARCHAR(50) DEFAULT "'U'" ,
       Errors VARCHAR(50),
       GatewayId VARCHAR(50) DEFAULT "'*'" 
);

CREATE TABLE Privilege
(
       Id INTEGER PRIMARY KEY AUTOINCREMENT,
       Name VARCHAR(50) NOT NULL,
       Desc VARCHAR(50) NOT NULL
);

CREATE TABLE Role
(
       Id INTEGER PRIMARY KEY AUTOINCREMENT,
       Name VARCHAR(50) NOT NULL,
       Desc VARCHAR(50) NOT NULL
);

CREATE TABLE RolePrivilegeMap
(
       RoleId INTEGER NOT NULL,
       PrivilegeId INTEGER NOT NULL
);

CREATE TABLE User
(
       Id INTEGER PRIMARY KEY AUTOINCREMENT,
       Name VARCHAR(50) NOT NULL,
       Mobtel VARCHAR(50) NOT NULL,
       Email VARCHAR(50) NOT NULL,
       LoginId VARCHAR(50) NOT NULL,
       Password VARCHAR(50) NOT NULL
);

CREATE TABLE UserRoleMap
(
       UserId INTEGER NOT NULL,
       RoleId INTEGER NOT NULL
);


CREATE TABLE GatewayConfig
(
       Id VARCHAR(50) PRIMARY KEY,
	   ComPort VARCHAR(50) NOT NULL,
	   BaudRate VARCHAR(50) NOT NULL,
	   DataBits VARCHAR(50) NOT NULL,
	   Parity VARCHAR(50) NOT NULL,
	   StopBits VARCHAR(50) NOT NULL,
	   OwnNumber VARCHAR(50),
	   Smsc VARCHAR(50),
	   Pin VARCHAR(50),
	   MessageValidity VARCHAR(50),
	   LongMessageOption VARCHAR(50),
	   MessageMemory VARCHAR(50),
	   Function VARCHAR(50),
	   LogSettings VARCHAR(50)	   
);

CREATE TABLE AppConfig
(
	Name VARCHAR(50) PRIMARY KEY,
	Value VARCHAR(255) NOT NULL	
);




CREATE SEQUENCE   "SMSSERVER_IN_SEQ"  MINVALUE 1 MAXVALUE 999999999999999999999999999 INCREMENT BY 1 START WITH 107 CACHE 20 NOORDER  NOCYCLE
/

CREATE SEQUENCE   "SMSSERVER_OUT_SEQ"  MINVALUE 1 MAXVALUE 999999999999999999999999999 INCREMENT BY 1 START WITH 107 CACHE 20 NOORDER  NOCYCLE
/

CREATE SEQUENCE   "SMSSERVER_CALLS_SEQ"  MINVALUE 1 MAXVALUE 999999999999999999999999999 INCREMENT BY 1 START WITH 107 CACHE 20 NOORDER  NOCYCLE
/

CREATE TABLE  "SMSSERVER_CALLS" 
   (	"ID" NUMBER(10,0) NOT NULL ENABLE, 
	"CALL_DATE" TIMESTAMP (6), 
	"GATEWAY_ID" VARCHAR2(64), 
	"CALLER_ID" VARCHAR2(16), 
	 CONSTRAINT "SMSSERVER_CALLS_PK" PRIMARY KEY ("ID") ENABLE
   )
/

CREATE OR REPLACE TRIGGER  "BI_SMSSERVER_CALLS" 
  before insert on "SMSSERVER_CALLS"               
  for each row  
begin   
    select "SMSSERVER_CALLS_SEQ".nextval into :NEW.ID from dual; 
end; 
/

ALTER TRIGGER  "BI_SMSSERVER_CALLS" ENABLE
/

CREATE TABLE  "SMSSERVER_IN" 
   (	"ID" NUMBER(10,0) NOT NULL ENABLE, 
	"PROCESS" NUMBER(2,0), 
	"ORIGINATOR" VARCHAR2(16), 
	"TYPE" VARCHAR2(1), 
	"ENCODING" VARCHAR2(1), 
	"MESSAGE_DATE" TIMESTAMP (6), 
	"RECEIVE_DATE" TIMESTAMP (6), 
	"TEXT" VARCHAR2(1000), 
	"ORIGINAL_REF_NO" VARCHAR2(64), 
	"ORIGINAL_RECEIVE_DATE" TIMESTAMP (6), 
	"GATEWAY_ID" VARCHAR2(64), 
	 CONSTRAINT "SMSSERVER_IN_PK" PRIMARY KEY ("ID") ENABLE
   )
/

CREATE OR REPLACE TRIGGER  "BI_SMSSERVER_IN" 
  before insert on "SMSSERVER_IN"               
  for each row  
begin   
    select "SMSSERVER_IN_SEQ".nextval into :NEW.ID from dual; 
end; 
/

ALTER TRIGGER  "BI_SMSSERVER_IN" ENABLE
/

CREATE TABLE  "SMSSERVER_OUT" 
   (	"ID" NUMBER(10,0) NOT NULL ENABLE, 
	"TYPE" VARCHAR2(1),
	"RECIPIENT" VARCHAR2(16), 
	"TEXT" VARCHAR2(1000), 
	"WAP_URL" VARCHAR2(100),
	"WAP_EXPIRY_DATE" TIMESTAMP (6),
	"WAP_SIGNAL" VARCHAR2(1),
	"CREATE_DATE" TIMESTAMP (6), 
	"ORIGINATOR" VARCHAR2(16), 
	"ENCODING" VARCHAR2(1), 
	"STATUS_REPORT" NUMBER(1,0), 
	"FLASH_SMS" NUMBER(1,0), 
	"SRC_PORT" NUMBER(6,0), 
	"DST_PORT" NUMBER(6,0), 
	"SENT_DATE" TIMESTAMP (6), 
	"REF_NO" VARCHAR2(64), 
	"PRIORITY" NUMBER(3,0), 
	"STATUS" VARCHAR2(1), 
	"ERRORS" NUMBER(2,0), 
	"GATEWAY_ID" VARCHAR2(16), 
	 CONSTRAINT "SMSSERVER_OUT_PK" PRIMARY KEY ("ID") ENABLE
   )
/

CREATE OR REPLACE TRIGGER  "BI_SMSSERVER_OUT" 
  before insert on "SMSSERVER_OUT"
  for each row
begin
    select "SMSSERVER_OUT_SEQ".nextval into :NEW.ID from dual;
end;
/

ALTER TRIGGER  "BI_SMSSERVER_OUT" ENABLE
/

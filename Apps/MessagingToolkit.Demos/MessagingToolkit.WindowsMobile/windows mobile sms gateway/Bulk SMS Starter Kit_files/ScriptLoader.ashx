var s_account="msstocodeplex";
var omniGuidPath = "://www.codeplex.com/site/analyticsid.aspx";
if (window.location.hostname.toLowerCase().indexOf("codeplex.com") == -1 || window.location.hostname.toLowerCase().indexOf("www.ppe") > -1) {
	// make sure all rsids end in dev. if not, append dev
	var s2 = s_account.split(",");
	for (var i=0; i < s2.length; i++) {
		if (s2[i].search(/dev$/) == -1) s2[i] +="dev";
	}
	s_account = s2.join(",");
}

/* SiteCatalyst code version: H.19
Copyright 1997-2008 Omniture, Inc. More info available at
http://www.omniture.com */
/************************ ADDITIONAL FEATURES ************************
*/
function omniGetCookie(name){
	var pos=document.cookie.indexOf(name+"=");
	if (pos!=-1){
		var start=pos+name.length+1;
		var end=document.cookie.indexOf(";",start);
		if (end==-1){
			end=document.cookie.length;
		}
		return unescape(document.cookie.substring(start,end));
	}
	return null;
}

var gAnalyticsId = omniGetCookie('omniID');
var gAnalyticsIdStatus = (gAnalyticsId) ? "fpc" : "guid";
if (!gAnalyticsId) {
		document.write("<SCR"+"IPT TYPE='text/javascript' SRC='"+"http"+(window.location.protocol.indexOf('https:')==0?'s':'')+omniGuidPath+"'><\/SCR"+"IPT>");
}

function omni_S4() {
	return (((1+Math.random())*0x10000)|0).toString(16).substring(1);
}
function createRandomOmniId(e) {
	return (e+"_"+omni_S4()+"_"+omni_S4()+"_"+omni_S4()+"_"+omni_S4()+omni_S4()+omni_S4());
}

function setOmniID() { 
	if (!gAnalyticsId) {
		gAnalyticsIdStatus="fallback";
	}
	else {
		gAnalyticsId = gAnalyticsId.replace(/\-/g,"_");
	}
	var e=new Date(); 
	var eTime = e.getTime();
	tempID = (gAnalyticsId) ? gAnalyticsId : createRandomOmniId(eTime);
	//alert("in setOmniID\ngAnalyticsId="+gAnalyticsId+"\ntempID="+tempID);
	e.setTime(eTime+366*24*60*60*1000);
	s.c_w('omniID',tempID,e);
	return tempID;
}

function normalizePagename(pval) {	
	if (pval == "" || pval == "default.aspx" || pval == "default.mspx") pval = "homepage"; 
	return pval;
}

function getOmniMetaTagData(val) {
  var metas = document.getElementsByTagName('META');
  var i;
  for (i = 0; i < metas.length; i++) {
	var strX = metas[i].getAttribute('NAME');
	if(strX) 
        {
		if(strX.toLowerCase() == val) {
			return metas[i].getAttribute('CONTENT');
		} 
	}
   }
   if (window[val]) return window[val];
   
   return false;
}

// check metadata and create javascript variable of same name, normalizing to lowercase
function checkMeta() {
  var metas = document.getElementsByTagName('META');
  var i;
  for (i = 0; i < metas.length; i++) {
		var strX = metas[i].getAttribute('NAME');
		if(strX) {
			if(strX.indexOf("t_omni_") >= 0) {
				window[strX.toLowerCase()]=metas[i].getAttribute('CONTENT');
			} 
		}
	}
}

function findsprops() {
	if(window.t_omni_sprops) {
		t_omni_spropsArr = t_omni_sprops.split(",");
		for(i = 0; i < t_omni_spropsArr.length; i++){
			var splitArr = t_omni_spropsArr[i].split("|");
			s[splitArr[0].toLowerCase()]=splitArr[1].toLowerCase();
		}
	}
}

function findevars() {
	if(window.t_omni_evars) {
		t_omni_evarsArr = t_omni_evars.split(",");
		for(i = 0; i < t_omni_evarsArr.length; i++){
			var splitArr = t_omni_evarsArr[i].split("|");
			s[splitArr[0]]=splitArr[1].toLowerCase();
		}
	}
}

function findqueryparams() {
	if(window.t_omni_queryparams) {
		var temp_qparamsArr = t_omni_queryparams.split(",");
		for(i = 0; i < temp_qparamsArr.length; i++){
		 try {
			var temp_qArr = temp_qparamsArr[i].split("|");
			var qsparam = temp_qArr[0];
			var prefix	= temp_qArr[1];
			var qsvar	= temp_qArr[2]; 
			if(qsvar) {
				if(queryString(qsparam)!='false') {
					s[qsvar] = prefix+queryString(qsparam);
				}
			}
		 } catch(err) {}
		}
	}
}

function getOmniPath(pval,skipBeginning,skipEnd) {
	if (skipEnd > 0) pval += "o"; // handle URLs ending in just /
	var pathArray = pval.split( '/' );
	var len = pathArray.length - skipEnd;
	var path = "";
	for ( i = skipBeginning; i < len; i++ ) {
	path += pathArray[i];
	(i < (len - 1)) ? path += ": " : ""
	}
	return path;
}
function PageQuery(q) {
	if(q.length > 1) this.q = q.substring(1, q.length);
	else this.q = null;
	this.keyValuePairs = new Array();
	if(q) {
		for(var i=0; i < this.q.split("&").length; i++) {
			this.keyValuePairs[i] = this.q.split("&")[i];
		}
	}
	this.getKeyValuePairs = function() { return this.keyValuePairs; }
	this.getValue = function(s) {
		for(var j=0; j < this.keyValuePairs.length; j++) {
			if(this.keyValuePairs[j].split("=")[0] == s)
				return this.keyValuePairs[j].split("=")[1];
		}
		return false;
	}
	this.getParameters = function() {
		var a = new Array(this.getLength());
		for(var j=0; j < this.keyValuePairs.length; j++) {
			a[j] = this.keyValuePairs[j].split("=")[0];
		}
		return a;
	}
	this.getLength = function() { return this.keyValuePairs.length; }	
}

function queryString(key){
	var page = new PageQuery(window.location.search); 
	return unescape(page.getValue(key)); 
}

function omni_StripProtocol(url) {
     // strip out protocol
     var strip = new Array('http://','https://');
     for (var i=0; i<strip.length; i++) {
          url=url.replace(strip[i],'');
     }    
	return url;
}

function omni_StripQS(url) {
     // strip out query string params
     if (url.indexOf('?')!=-1) {
          url=url.substring(0, url.indexOf('?'));
     }          
     return url;
}

function findevents() {
	if(window.t_omni_events) {
		if (s.events) s.events += window.t_omni_events
		else { s.events = window.t_omni_events; }
	}
}

function omniAddPVEvent() {
     if (!s.linkType) { 
		 s.eVar17 = s.pageName;  
		 s.events=s.apl(s.events,"event1",",",2);
	}
}

// removed site specific report site determination code
var t_omni_hname = window.location.hostname.toLowerCase();  
var extraRsids = getOmniMetaTagData("t_omni_rsids");
if (extraRsids) s_account = s_account + "," + extraRsids;

var s=s_gi(s_account);

s.events='';

//Call Helper Functions
checkMeta();
findsprops();
findevars();
findqueryparams();
findevents();

/************************** CONFIG SECTION **************************/
/* You may add or alter any code config here. */
/* Conversion Config */
s.currencyCode="USD"
/* Link Tracking Config */
s.trackDownloadLinks=true
s.trackExternalLinks=true
s.trackInlineStats=true
s.linkDownloadFileTypes="exe,zip,wav,mp3,mov,mpg,avi,wmv,pdf,doc,docx,xls,xlsx,ppt,pptx,asx"

// linkInternalFilters can get overridden on the page
s.linkInternalFilters="javascript:,microsoft.com/windows"
s.linkLeaveQueryString=false
s.linkTrackVars="None"
s.linkTrackEvents="None"
/* Configure Modules and Plugins */
/*
s.loadModule("Media")
s.Media.autoTrack=false
s.Media.trackVars="None"
s.Media.trackEvents="None"
s.Media.trackWhilePlaying=true
s.Media.trackSeconds="30"
*/


/* Page Name Plugin Config */
s.siteID=""            // leftmost value in pagename
s.defaultPage=""       // filename to add when none exists
s.queryVarsList=""     // query parameters to keep
s.pathExcludeDelim=";" // portion of the path to exclude
s.pathConcatDelim=""   // page name component separator
s.pathExcludeList=""   // elements to exclude from the path

/* Plugin Config */
s.usePlugins=true

function s_doPlugins(s) {
	// check for page metadata overrides
	if (getOmniMetaTagData("t_omni_errorpage")) { s.pageType = "errorPage"; }
	var tvar="";
	tvar=getOmniMetaTagData("t_omni_pagename");  if (tvar) { s.pageName = tvar; }
	tvar=getOmniMetaTagData("t_omni_channel");  if (tvar) { s.channel = tvar; }
	tvar=getOmniMetaTagData("t_omni_pagetitle");  if (tvar) { s.prop3 = tvar; }

	omniAddPVEvent();
	
	// External Campaigns 
	if(!s.campaign)
		s.campaign=s.getQueryParam('mc_id'); // reuse webtrends
	if(!s.campaign)
		s.campaign=s.getQueryParam('cmxid'); // used due to comscore
	if(!s.campaign)
		s.campaign=s.getQueryParam('ocid');  // common tracking code at Microsoft
	
	s.campaign=s.getValOnce(s.campaign,"s_campaign",0)


/*	commenting out until full taxonomy is determined

	// Internal Campaigns
	if(!s.eVar31)
		s.eVar31=s.getQueryParam('icid');
	s.eVar31=s.getValOnce(s.eVar31,"s_eVar31",0)

	// treatment ID 
	if(!s.eVar21)
		s.eVar21=s.getQueryParam('trid');
	s.eVar21=s.getValOnce(s.eVar21,"s_eVar21",0);

	// Plugin detectSilverlight v0.1 
	s.detectSilverlight('prop12', 'svc', '');
	if (s.prop12) s.eVar12 = s.prop12;

	// Plugin: detectFlash v0.6-H
	s.prop11=s.getFlash('s_fv');
	if(!s.prop11){
	s.detectFlash('s_fv','10');
	s.prop11=s.getFlash('s_fv');
	}
	
	// Plugin Example: getNewRepeat 1.0 
	s.prop28=s.getNewRepeat();

		// Plugin Example: getVisitNum 2.0 
	s.prop27=s.getVisitNum();

*/

	s.prop29 = window.location.hostname.toLowerCase() + window.location.pathname.toLowerCase();

	
	/* Plugin Example: getPagename v2.1 */
	if(!s.pageType && !s.pageName)
		s.pageName=s.getPageName();

		
	/* Plugin Example: exitLinkHandler v0.5 */
	var url=s.exitLinkHandler();
	if (url) {
		// generic exit link handler to track exits by page
		var tempdest = omni_StripProtocol(url.toLowerCase());
		if(tempdest.indexOf('atdmt.com')>-1) { // strip out query string and protocol for query string
			tempdest = omni_StripQS(tempdest);
		}
		s.prop43=s.pageName+" _ "+tempdest;
		s.linkTrackVars=s.apl(s.linkTrackVars,"prop43",",",2);
	}
	
	var dlurl=s.downloadLinkHandler();
	if (dlurl) {
		// generic download link handler to track downloads by page
		var tempdest = omni_StripProtocol(dlurl.toLowerCase());
		if(tempdest.indexOf('atdmt.com')>-1) { // strip out query string and protocol for query string
			tempdest = omni_StripQS(tempdest);
		}
		s.prop43=s.pageName+" _ "+tempdest;
		s.linkTrackVars=s.apl(s.linkTrackVars,"prop43",",",2);
	}	
	
	s.prop45 = gAnalyticsIdStatus;
	if (gAnalyticsIdStatus =="fallback") {
		 s.events=s.apl(s.events,"event10",",",2);
	}

	/* Copy Props to eVars */
	if(s.prop1){s.eVar1=s.prop1}
	if(s.prop2){s.eVar2=s.prop2}
	if(s.prop3){s.eVar3=s.prop3}
	if(s.prop4){s.eVar4=s.prop4}
	if(s.prop5){s.eVar5=s.prop5}
	//if(s.prop6){s.eVar6=s.prop6}
	
	s.linkTrackVars=s.apl(s.linkTrackVars,"visitorID",",",2);
}
s.doPlugins=s_doPlugins
/************************** PLUGINS SECTION *************************/
/* You may insert any plugins you wish to use here.                 */

/*
 * Plugin: getQueryParam 2.1 - return query string parameter(s)
 */
s.getQueryParam=new Function("p","d","u",""
+"var s=this,v='',i,t;d=d?d:'';u=u?u:(s.pageURL?s.pageURL:s.wd.locati"
+"on);if(u=='f')u=s.gtfs().location;while(p){i=p.indexOf(',');i=i<0?p"
+".length:i;t=s.p_gpv(p.substring(0,i),u+'');if(t)v+=v?d+t:t;p=p.subs"
+"tring(i==p.length?i:i+1)}return v");
s.p_gpv=new Function("k","u",""
+"var s=this,v='',i=u.indexOf('?'),q;if(k&&i>-1){q=u.substring(i+1);v"
+"=s.pt(q,'&','p_gvf',k)}return v");
s.p_gvf=new Function("t","k",""
+"if(t){var s=this,i=t.indexOf('='),p=i<0?t:t.substring(0,i),v=i<0?'T"
+"rue':t.substring(i+1);if(p.toLowerCase()==k.toLowerCase())return s."
+"epa(v)}return ''");

/*
 * Plugin: getValOnce 0.2 - get a value once per session or number of days
 */
s.getValOnce=new Function("v","c","e",""
+"var s=this,k=s.c_r(c),a=new Date;e=e?e:0;if(v){a.setTime(a.getTime("
+")+e*86400000);s.c_w(c,v,e?a:0);}return v==k?'':v");

/*
 * Plugin: exitLinkHandler 0.5 - identify and report exit links
 */
s.exitLinkHandler=new Function("p",""
+"var s=this,h=s.p_gh(),n='linkInternalFilters',i,t;if(!h||(s.linkTyp"
+"e&&(h||s.linkName)))return '';i=h.indexOf('?');t=s[n];s[n]=p?p:t;h="
+"s.linkLeaveQueryString||i<0?h:h.substring(0,i);if(s.lt(h)=='e')s.li"
+"nkType='e';else h='';s[n]=t;return h;");

/*
 * Plugin: downloadLinkHandler 0.5 - identify and report download links
 */
s.downloadLinkHandler=new Function("p",""
+"var s=this,h=s.p_gh(),n='linkDownloadFileTypes',i,t;if(!h||(s.linkT"
+"ype&&(h||s.linkName)))return '';i=h.indexOf('?');t=s[n];s[n]=p?p:t;"
+"if(s.lt(h)=='d')s.linkType='d';else h='';s[n]=t;return h;");


/*
 * Plugin Utility: apl v1.1
 */
s.apl=new Function("l","v","d","u",""
+"var s=this,m=0;if(!l)l='';if(u){var i,n,a=s.split(l,d);for(i=0;i<a."
+"length;i++){n=a[i];m=m||(u==1?(n==v):(n.toLowerCase()==v.toLowerCas"
+"e()));}}if(!m)l=l?l+d+v:v;return l");

/*
 * Utility Function: split v1.5 (JS 1.0 compatible)
 */
s.split=new Function("l","d",""
+"var i,x=0,a=new Array;while(l){i=l.indexOf(d);i=i>-1?i:l.length;a[x"
+"++]=l.substring(0,i);l=l.substring(i+d.length);}return a");

/*
 * Custom Plugin:  getSilverlightVersion
 */
 

s.detectSilverlight=new Function("smv", "return 'silverlight not detected.';");
if (s.apv >= 4.1) {
s.detectSilverlight=null;
s.detectSilverlight=new Function("svr", "scn", "smv", ""
+"smv=smv?smv:2;var s=this,sk=s.c_r(scn),k=s.c_w('s_cc','true',0)?'Y'"
+":'N';if(k=='Y'&&!sk){var ap=null,sv='';try{ap=new ActiveXObject('Ag"
+"Control.AgControl');for(var i=smv;i>0;i--){for(var j=9;j>=0;j--){if"
+"(ap.isVersionSupported(i+'.'+j)){sv=i+'.'+j;break;}}if(sv){break;}}"
+"}catch(e){try{ap=navigator.plugins['Silverlight Plug-In'];sv=ap.des"
+"cription.substring(0,ap.description.indexOf('.')+2)}catch(e){}}if(s"
+"v){sv='silverlight '+sv;}else{sv='silverlight not detected.';}s.c_w"
+"(scn,sv,0);s[svr]=sv;}"
);
}


/********************************************************************
 *
 * Supporting functions that may be shared between plug-ins
 *
 *******************************************************************/
/*
 * Utility Function: p_gh
 */
s.p_gh=new Function(""
+"var s=this;if(!s.eo&&!s.lnk)return '';var o=s.eo?s.eo:s.lnk,y=s.ot("
+"o),n=s.oid(o),x=o.s_oidt;if(s.eo&&o==s.eo){while(o&&!n&&y!='BODY'){"
+"o=o.parentElement?o.parentElement:o.parentNode;if(!o)return '';y=s."
+"ot(o);n=s.oid(o);x=o.s_oidt}}return o.href?o.href:'';");

s.p_gh_windows_new=new Function(""
+"var s=this;if(!s.eo&&!s.lnk)return '';var o=s.eo?s.eo:s.lnk,y=s.ot("
+"o),n=s.oid(o),x=o.s_oidt;if(s.eo&&o==s.eo){while(o&&!n&&y!='BODY'){"
+"o=o.parentElement?o.parentElement:o.parentNode;if(!o)return '';y=s."
+"ot(o);n=s.oid(o);x=o.s_oidt}}return o.href?o:'';");

/*
 * Plugin: Flash Detection 0.6 - Detect Flash version number
 */
s.detectFlash=new Function("cn","mfv","vr",""
+"var s=this,fv=-1,dwi=0,r,w,mt=s.n.mimeTypes,fk=s.c_r(cn),k=s.c_w('s"
+"_cc','true',0)?'Y':'N';if(k=='Y'&&!fk){if(s.pl&&s.pl.length){if(s.p"
+"l['Shockwave Flash 2.0'])fv=2;x=s.pl['Shockwave Flash'];if(x){fv=0;"
+"z=x.description;if(z)fv=z.substring(16,z.indexOf('.'));}}else if(mt"
+"&&mt.length){x=mt['application/x-shockwave-flash'];if(x&&x.enabledP"
+"lugin)fv=0;}if(fv<=0)dwi=1;w=s.u.indexOf('Win')!=-1?1:0;if(dwi&&s.i"
+"sie&&w&&execScript){result=false;for(var i=mfv;i>=3&&result!=true;i"
+"--){execScript('on error resume next: result = IsObject(CreateObjec"
+"t(\"ShockwaveFlash.ShockwaveFlash.'+i+'\"))','VBScript');fv=i;}}r=f"
+"v==-1?'flash not detected':fv==0?'flash enabled (no version)':'flas"
+"h '+fv;s.c_w(cn,r,0);return 'true';}else return '';");
s.getFlash=new Function("cn",""
+"var s=this;if(cn&&s.c_r(cn))return s.c_r(cn);");
s.returnFlash=new Function("cn","vr","to",""
+"setTimeout(\"var cn,vr,to,s_dfv=s_gi(s_account);s_dfv.linkTrackVars"
+"=vr,s_dfv.vr=s.getFlash();s_dfv.tl(this,'o','Flash Version Detectio"
+"n')\",to);");

/*
 * Plugin: getNewRepeat 1.0 - Return whether user is new or repeat
 */
s.getNewRepeat=new Function(""
+"var s=this,e=new Date(),cval,ct=e.getTime(),y=e.getYear();e.setTime"
+"(ct+30*24*60*60*1000);cval=s.c_r('s_nr');if(cval.length==0){s.c_w("
+"'s_nr',ct,e);return 'New';}if(cval.length!=0&&ct-cval<30*60*1000){s"
+".c_w('s_nr',ct,e);return 'New';}if(cval<1123916400001){e.setTime(cv"
+"al+30*24*60*60*1000);s.c_w('s_nr',ct,e);return 'Repeat';}else retur"
+"n 'Repeat';");

/*
 * Plugin: getPageName v2.1 - parse URL and return
 */
s.getPageName=new Function("u",""
+"var s=this,v=u?u:''+s.wd.location,x=v.indexOf(':'),y=v.indexOf('/',"
+"x+4),z=v.indexOf('?'),c=s.pathConcatDelim,e=s.pathExcludeDelim,g=s."
+"queryVarsList,d=s.siteID,n=d?d:'',q=z<0?'':v.substring(z+1),p=v.sub"
+"string(y+1,q?z:v.length);z=p.indexOf('#');p=z<0?p:s.fl(p,z);x=e?p.i"
+"ndexOf(e):-1;p=x<0?p:s.fl(p,x);p+=!p||p.charAt(p.length-1)=='/'?s.d"
+"efaultPage:'';y=c?c:'/';while(p){x=p.indexOf('/');x=x<0?p.length:x;"
+"z=s.fl(p,x);if(!s.pt(s.pathExcludeList,',','p_c',z))n+=n?y+z:z;p=p."
+"substring(x+1)}y=c?c:'?';while(g){x=g.indexOf(',');x=x<0?g.length:x"
+";z=s.fl(g,x);z=s.pt(q,'&','p_c',z);if(z){n+=n?y+z:z;y=c?c:'&'}g=g.s"
+"ubstring(x+1)}return n");

/*                                                                  
 * Plugin: Visit Number By Month 2.0 - Return the user visit number 
 */
s.getVisitNum=new Function(""
+"var s=this,e=new Date(),cval,cvisit,ct=e.getTime(),c='s_vnum',c2='s"
+"_invisit';e.setTime(ct+30*24*60*60*1000);cval=s.c_r(c);if(cval){var"
+" i=cval.indexOf('&vn='),str=cval.substring(i+4,cval.length),k;}cvis"
+"it=s.c_r(c2);if(cvisit){if(str){e.setTime(ct+30*60*1000);s.c_w(c2,'"
+"true',e);return str;}else return 'unknown visit number';}else{if(st"
+"r){str++;k=cval.substring(0,i);e.setTime(k);s.c_w(c,k+'&vn='+str,e)"
+";e.setTime(ct+30*60*1000);s.c_w(c2,'true',e);return str;}else{s.c_w"
+"(c,ct+30*24*60*60*1000+'&vn=1',e);e.setTime(ct+30*60*1000);s.c_w(c2"
+",'true',e);return 1;}}"
);

/*
 * Utility Function: p_c
 */
s.p_c=new Function("v","c",""
+"var x=v.indexOf('=');return c.toLowerCase()==v.substring(0,x<0?v.le"
+"ngth:x).toLowerCase()?v:0");

/* WARNING: Changing any of the below variables will cause drastic
changes to how your visitor data is collected.  Changes should only be
made when instructed to do so by your account manager.*/


s.visitorNamespace="microsoftsto"
s.dc=112

/****************************** MODULES *****************************/
/* Module: Media  , updated 2/25/09 with v14.3 */
s.m_Media_c="(`OWhilePlaying~='s_media_'+m._in+'_~unc^D(~;`E~m.ae(mn,l,\"'+p+'\",~){var m=this~o;w.percent=((w.off^e+1)/w`X)*100;w.percent=w.percent>1~o.'+f~=new ~o.Get~:Math.floor(w.percent);w.timeP"
+"layed=i.t~}`x p');p=tcf(o)~Time~x,x!=2?p:-1,o)}~if(~m.monitor)m.monitor(m.s,w)}~m.s.d.getElementsByTagName~ersionInfo~'^N_c_il['+m._in+'],~'o','var e,p=~else~i.to~=Math.floor(~}catch(e){p=~m.track~"
+"s.wd.addEventListener~.name~m.s.rep(~layState~||^8~Object~m.s.wd[f1]~^A+=i.t+d+i.s+d+~.length~parseInt(~Player '+~s.wd.attachEvent~'a','b',c~Media~pe='m~;o[f1]~m.s.isie~.current~);i.~p<p2||p-p2>5)~"
+".event=~m.close~i.lo~vo.linkTrack~=v+',n,~.open~){w.off^e=~;n=m.cn(n);~){this.e(n,~v=e='None';~Quick~MovieName()~);o[f~out(\"'+v+';~return~1000~i.lx~m.ol~o.controls~m.s.ape(i.~load',m.as~)}};m.~scr"
+"ipt';x.~,t;try{t=~Version()~n==~'--**--',~pev3~o.id~i.ts~tion~){mn=~1;o[f7]=~();~(x==~){p='~&&m.l~l[n])~:'')+i.e~':'E')+o~var m=s~!p){tcf~xc=m.s.~Title()~()/~7+'~+1)/i.l~;i.e=''~3,p,o);~m.l[n]=~Dat"
+"e~5000~;if~i.lt~';c2='~tm.get~Events~set~Change~)};m~',f~(x!=~4+'=n;~~^N.m_i('`c');m.cn=f`2n`5;`x `Rm.s.rep(`Rn,\"\\n\",''),\"\\r\",''),^9''^g`o=f`2n,l,p,b`5,i`8`U,tm`8^X,a='',x`ql=`Yl)`3!l)l=1`3n&"
+"&p){`E!m.l)m.l`8`U`3m.^K`k(n)`3b&&b.id)a=b.id;for (x in m.l)`Em.l[x]^J[x].a==a)`k(m.l[x].n`hn=n;i.l=l;i.p=m.cn(p`ha=a;i.t=0;^C=0;i.s`M^c`C^R`y`hlx=0;^a=i.s;`l=0^U;`L=-1;^Wi}};`k=f`2n`r0,-1^g.play=f"
+"`2n,o`5,i;i=m.e(n,1,o`hm`8F`2`Ii`3m.l){i=m.l[\"'+`Ri.n,'\"','\\\\\"')+'\"]`3i){`E`z==1)m.e(i.n,3,-1`hmt=^e`Cout(i.m,^Y)}}'`hm(^g.stop=f`2n,o`r2,o)};`O=f`2n`5^Z `0) {m.e(n,4,-1^4e=f`2n,x,o`5,i,tm`8^"
+"X,ts`M^c`C^R`y),ti=`OSeconds,tp=`OMilestones,z`8Array,j,d=^9t=1,b,v=`OVars,e=`O^d,`dedia',^A,w`8`U,vo`8`U`qi=n^J&&m.l[n]?m.l[n]:0`3i){w`Q=n;w`X=i.l;w.playerName=i.p`3`L<0)w`j\"OPEN\";`K w`j^H1?\"PL"
+"AY\":^H2?\"STOP\":^H3?\"MONITOR\":\"CLOSE\")));w`o`C`8^X^Gw`o`C.^e`C(i.s*`y)`3x>2||^i`z&&^i2||`z==1))) {b=\"`c.\"+name;^A = ^2n)+d+i.l+d+^2p)+d`3x){`Eo<0&&^a>0){o=(ts-^a)+`l;o=o<i.l?o:i.l-1}o`Mo)`3"
+"x>=2&&`l<o){i.t+=o-`l;^C+=o-`l;}`Ex<=2){i.e+=^H1?'S^M;`z=x;}`K `E`z!=1)m.e(n,1,o`hlt=ts;`l=o;`W`0&&`L>=0?'L'+`L^L+^i2?`0?'L^M:'')^Z`0){b=0;`d_o'`3x!=4`p`600?100`A`3`F`E`L<0)`d_s';`K `Ex==4)`d_i';`K"
+"{t=0;`sti=ti?`Yti):0;z=tp?m.s.sp(tp,','):0`3ti&&^C>=ti)t=1;`K `Ez){`Eo<`L)`L=o;`K{for(j=0;j<z`X;j++){ti=z[j]?`Yz[j]):0`3ti&&((`L^T<ti/100)&&((o^T>=ti/100)){t=1;j=z`X}}}}}}}`K{m.e(n,2,-1)^Z`0`pi.l`6"
+"00?100`A`3`F^W0`3i.e){`W`0&&`L>=0?'L'+`L^L^Z`0){`s`d_o'}`K{t=0;m.s.fbr(b)}}`K t=0;b=0}`Et){`mVars=v;`m^d=e;vo.pe=pe;vo.^A=^A;m.s.t(vo,b)^Z`0){^C=0;`L=o^U}}}}`x i};m.ae=f`2n,l,p,x,o,b){`En&&p`5`3!m."
+"l||!m.^Km`o(n,l,p,b);m.e(n,x,o^4a=f`2o,t`5,i=^B?^B:o`Q,n=o`Q,p=0,v,c,c1,c2,^Ph,x,e,f1,f2`1oc^h3`1t^h4`1s^h5`1l^h6`1m^h7`1c',tcf,w`3!i){`E!m.c)m.c=0;i`1'+m.c;m.c++}`E!^B)^B=i`3!o`Q)o`Q=n=i`3!^0)^0`8"
+"`U`3^0[i])`x;^0[i]=o`3!xc)^Pb;tcf`8F`2`J0;try{`Eo.v`H&&o`g`c&&^1)p=1`N0`B`3^O`8F`2`J0^6`9`t`C^7`3t)p=2`N0`B`3^O`8F`2`J0^6`9V`H()`3t)p=3`N0`B}}v=\"^N_c_il[\"+m._in+\"],o=^0['\"+i+\"']\"`3p==1^IWindo"
+"ws `c `Zo.v`H;c1`np,l,x=-1,cm,c,mn`3o){cm=o`g`c;c=^1`3cm&&c^Ecm`Q?cm`Q:c.URL;l=cm.dura^D;p=c`gPosi^D;n=o.p`S`3n){`E^88)x=0`3^83)x=1`3^81`T2`T4`T5`T6)x=2;}^b`Ex>=0)`4`D}';c=c1+c2`3`f&&xc){x=m.s.d.cr"
+"eateElement('script');x.language='j^5type='text/java^5htmlFor=i;x`j'P`S^f(NewState)';x.defer=true;x.text=c;xc.appendChild(x`v6]`8F`2c1+'`E^83){x=3;'+c2+'}^e`Cout(`76+',^Y)'`v6]()}}`Ep==2^I`t`C `Z(`"
+"9Is`t`CRegistered()?'Pro ':'')+`9`t`C^7;f1=f2;c`nx,t,l,p,p2,mn`3o^E`9`u?`9`u:`9URL^Gn=`9Rate^Gt=`9`CScale^Gl=`9Dura^D^Rt;p=`9`C^Rt;p2=`75+'`3n!=`74+'||`i{x=2`3n!=0)x=1;`K `Ep>=l)x=0`3`i`42,p2,o);`4"
+"`D`En>0&&`7^S>=10){`4^V`7^S=0}`7^S++;`7^j`75+'=p;^e`C`w`72+'(0,0)\",500)}'`e`8F`2`b`v4]=-^F0`e(0,0)}`Ep==3^IReal`Z`9V`H^Gf1=n+'_OnP`S^f';c1`nx=-1,l,p,mn`3o^E`9^Q?`9^Q:`9Source^Gn=`9P`S^Gl=`9Length^"
+"R`y;p=`9Posi^D^R`y`3n!=`74+'){`E^83)x=1`3^80`T2`T4`T5)x=2`3^80&&(p>=l||p==0))x=0`3x>=0)`4`D`E^83&&(`7^S>=10||!`73+')){`4^V`7^S=0}`7^S++;`7^j^b`E`72+')`72+'(o,n)}'`3`V)o[f2]=`V;`V`8F`2`b1+c2)`e`8F`2"
+"`b1+'^e`C`w`71+'(0,0)\",`73+'?500:^Y);'+c2`v4]=-1`3`f)o[f3]=^F0`e(0,0^4as`8F`2'e',`Il,n`3m.autoTrack&&`G){l=`G(`f?\"OBJECT\":\"EMBED\")`3l)for(n=0;n<l`X;n++)m.a(^K;}')`3`a)`a('on^3);`K `E`P)`P('^3,"
+"false)";
s.m_i("Media");

/************* DO NOT ALTER ANYTHING BELOW THIS LINE ! S_CODE H19 **************/
var s_code='',s_objectID;function s_gi(un,pg,ss){var c="=fun^I(~){`Ls=^Z~$y ~.substring(~.indexOf(~;@u~`c@u~=new Fun^I(~.toLowerCase()~};s.~.length~s_c_il['+s@4n+']~=new Object~`ZMigrationServer~.toU"
+"pperCase~){@u~`U$z=^O=s.`W`q=s.`W^c=`I^zobjectID=s.ppu=$9=$9v1=$9v2=$9v3=~','~s.wd~t^S~')q='~var ~s.pt(~=new Array~ookieDomainPeriods~.location~^KingServer~dynamicAccount~s.apv~BufferedRequests~);s"
+".~)@ux^w!Object$rObject.prototype$rObject.prototype[x])~link~s.m_~Element~visitor~$q@h~referrer~else ~.get#B()~}c#D(e){~.lastIndexOf(~.protocol~=new Date~=''~;@d^ss[k],255)}~javaEnabled~conne^I^c~^"
+"zc_i~:'')~onclick~}@u~Name~ternalFilters~javascript~s.dl~@9s.b.addBehavior(\"# default# ~for(~=parseFloat(~'+tm.get~typeof(v)==\"~window~cookie~s.rep(~s.vl_g~tfs~s.un~&&s.~o^zoid~browser~.parent~do"
+"cument~colorDepth~String~while(~.host~s.maxDelay~r=s.m(f)?s[f](~s.sq~parseInt(~ction~t=s.ot(o)~track~nload~j='1.~#NURL~s.eo~lugins~'){q='~dynamicVariablePrefix~=='~set#Bout(~Sampling~s.rc[un]~Event"
+"~;i++)~');~this~resolution~}else{~Type~s.c_r(~s.c_w(~s.eh~s.isie~s.vl_l~s.vl_t~Secure~Height~t,h#Wt?t~tcf~isopera~ismac~escape(~.href~screen.~s.fl(~s=s_gi(~Version~harCode~&&(~_'+~variableProvider~"
+".s_~f',~){s.~)?'Y':'N'~:'';h=h?h~._i~e&&l!='SESSION'~s_sv(v,n[k],i)}~name~home#N~;try{~s.ssl~s.oun~s.rl[u~Width~o.type~\"m_\"+n~Lifetime~s.gg('objectID~sEnabled~.mrq($tun+'\"~ExternalLinks~charSet~"
+"onerror~currencyCode~.src~disable~etYear(~MigrationKey~&&!~Opera~'s_~Math.~s.fsg~s.$z~s.ns6~InlineStats~&&l!='NONE'~Track~'0123456789~s[k]=~'+n+'~loadModule~+\"_c\"]~s.ape(~s.epa(~t.m_nl~m._d~n=s.o"
+"id(o)~,'sqs',q);~LeaveQuery~(''+~')>=~'=')~){n=~\",''),~&&t!='~if(~vo)~s.sampled~=s.oh(o);~+(y<1900?~n]=~true~sess~campaign~lif~ in ~'http~,100)~s.co(~ffset~s.pe~'&pe~m._l~s.c_d~s.brl~s.nrs~s.gv(~s"
+"[mn]~s.qav~,'vo~s.pl~=(apn~Listener~\"s_gs(\")~vo._t~b.attach~2o7.net'~d.create~=s.n.app~n){~t&&~)+'/~s()+'~){p=~():''~a):f(~'+n;~+1))~a['!'+t]~){v=s.n.~channel~.target~x.split~o.value~[\"s_\"+g~s_"
+"si(t)~')dc='1~\".tl(\")~etscape~s_')t=t~omePage~s.d.get~')<~='+~||!~'||~\"'+~[b](e);~\"){n[k]~a+1,b):~m[t+1](~return~lnk~mobile~height~events~random~code~wd.~=un~un,~,pev~'MSIE ~rs,~Time~floor(~atc"
+"h~s.num(~s.pg~m._e~s.c_gd~,'lt~.inner~transa~;s.gl(~',s.bc~page~Group,~.fromC~sByTag~?'&~+';'~&&o~1);~}}}}~){t=~[t]=~[n];~>=5)~[t](~!a[t])~~s._c=@Uc';`I=`z`5!`I`m$S`I`ml`N;`I`mn=0;}s@4l=`I`ml;s@4n="
+"`I`mn;s@4l[s@4@zs;`I`mn++;s.m`0m){`2@om)`4'{$p0`9fl`0x,l){`2x?@ox)`30,l):x`9co`0o`F!o)`2o;`Ln`C,x;`vx$4o)@ux`4'select$p0&&x`4'filter$p0)n[x]=o[x];`2n`9num`0x){x`i+x;`v`Lp=0;p<x`A;p++)@u(@c')`4x`3p,"
+"p$a<0)`20;`21`9rep=s_r;s.spf`0t,a){a[a`A]=t;`20`9sp`0x,d`1,a`N`5$f)a=$f(d);`c`Mx,d,'sp@0a);`2a`9ape`0x`1,h=@cABCDEF',i,c=s.@L,n,l,e,y`i;c=c?c`E$X`5x){x`i+x`5c^SAUTO'^w'').c^vAt){`vi=0;i<x`A^X{c=x`3"
+"i,i+#Un=x.c^vAt(i)`5n>127){l=0;e`i;^Cn||l<4){e=h`3n%16,n%16+1)+e;n=(n-n%16)/16;l++}y+='%u'+e}`6c^S+')y+='%2B';`cy+=^pc)}x=y^bx=x?^1^p''+x),'+`H%2B'):x`5x&&c^5em==1&&x`4'%u$p0&&x`4'%U$p0){i=x`4'%^Y^"
+"Ci>=0){i++`5h`38)`4x`3i,i+1)`E())>=0)`2x`30,i)+'u00'+x`3i);i=x`4'%',i)#V`2x`9epa`0x`1;`2x?un^p^1''+x,'+`H ')):x`9pt`0x,d,f,a`1,t=x,z=0,y,r;^Ct){y=t`4d);y=y<0?t`A:y;t=t`30,y);^Ft,$Yt,a)`5r)`2r;z+=y+"
+"d`A;t=x`3z,x`A);t=z<x`A?t:''}`2''`9isf`0t,a){`Lc=a`4':')`5c>=0)a=a`30,c)`5t`30,2)^S$m`32);`2(t!`i&&t==a)`9fsf`0t,a`1`5`Ma,`H,'is@0t))@W+=(@W!`i?`H`n+t;`20`9fs`0x,f`1;@W`i;`Mx,`H,'fs@0f);`2@W`9si`0w"
+"d`1,c`i+s_gi,a=c`4\"{\"),b=c`f\"}\"),m;c=s_fe(a>0&&b>0?c`3$w0)`5wd&&#5^9&&c){#5^T'fun^I s_sv(o,n,k){`Lv=o[k],i`5v`F`ystring\"||`ynumber\")n[k]=v;`cif (`yarray$v`N;`vi=0;i<v`A^X@6`cif (`yobject$v`C;"
+"`vi$4v)@6}}fun^I $i{`Lwd=`z,s,i,j,c,a,b;wd^zgi`7\"un\",\"pg\",\"ss\",$tc+'\");#5^t$t@B+'\");s=#5s;s.sa($t^4+'\"`U^3=wd;`M^2,\",\",\"vo1\",t`G\\'\\'`5t.m_l&&@j)`vi=0;i<@j`A^X{n=@j[i]`5$Sm=t#Yc=t[@F]"
+"`5m&&c){c=\"\"+c`5c`4\"fun^I\")>=0){a=c`4\"{\");b=c`f\"}\");c=a>0&&b>0?c`3$w0;s[@F@g=c`5#G)s.@f(n)`5s[n])`vj=0;j<$B`A;j++)s_sv(m,s[n],$B[j])#V}`Le,o,t@9o=`z.opener`5o#T^zgi#Wo^zgi($t^4+'\")`5t)$i}`"
+"e}',1)}`9c_d`i;#Hf`0t,a`1`5!#Et))`21;`20`9c_gd`0`1,d=`I`P^D@7,n=s.fpC`O,p`5!n)n=s.c`O`5d@S$C@rn?^Hn):2;n=n>2?n:2;p=d`f'.')`5p>=0){^Cp>=0&&n>1$Wd`f'.',p-#Un--}$C=p>0&&`Md,'.`Hc_gd@00)?d`3p):d}}`2$C`"
+"9c_r`0k`1;k=@hk);`Lc=' '+s.d.^0,i=c`4' '+k+@q,e=i<0?i:c`4';',i),v=i<0?'':@ic`3i+2+k`A,e<0?c`A:e));`2v!='[[B]]'?v:''`9c_w`0k,v,e`1,d=#H(),l=s.^0@G,t;v`i+v;l=l?@ol)`E$X`5@5@a#W(v!`i?^Hl?l:0):-60)`5t)"
+"{e`h;e.set#B(e`d+(t*1000))}`pk@a@1d.^0=k+'`av!`i?v:'[[B]]')+'; path=/;'+(@5?' expires$qe.toGMT^B()#S`n+(d?' domain$qd#S`n;`2^dk)==v}`20`9eh`0o,e,r,f`1,b='s^xe+'^xs@4n,n=-1,l,i,x`5!^fl)^fl`N;l=^fl;`"
+"vi=0;i<l`A&&n<0;i++`Fl[i].o==o&&l[i].e==e)n=i`pn<0@ri;l[n]`C}x=l#Yx.o=o;x.e=e;f=r?x.b:f`5r||f){x.b=r?0:o[e];x.o[e]=f`px.b){x.o[b]=x.b;`2b}`20`9cet`0f,a,t,o,b`1,r,^m`5`S>=5^w!s.^n||`S>=7)){^m`7's`Hf"
+"`Ha`Ht`H`Le,r@9^F$Ya)`er=s.m(t)?s#ae):t(e)}`2r^Yr=^m(s,f,a,t)^b@us.^o^5u`4#94@p0)r=s.m(b)?s[b](a):b(a);else{^f(`I,'@M',0,o);^F$Ya`Ueh(`I,'@M',1)}}`2r`9g^3et`0e`1;`2s.^3`9g^3oe`7'e`H`Ls=`B,c;^f(`z,"
+"\"@M\",1`Ue^3=1;c=s.t()`5c)s.d.write(c`Ue^3=0;`2$0'`Ug^3fb`0a){`2`z`9g^3f`0w`1,p=w^8,l=w`P;s.^3=w`5p&&p`P!=l&&p`P^D==l^D@1^3=p;`2s.g^3f(s.^3)}`2s.^3`9g^3`0`1`5!s.^3@1^3=`I`5!s.e^3)s.^3=s.cet('g^3@0"
+"s.^3,'g^3et',s.g^3oe,'g^3fb')}`2s.^3`9mrq`0u`1,l=@C],n,r;@C]=0`5l)`vn=0;n<l`A;n++){r=l#Ys.mr(0,0,r.r,0,r.t,r.u)}`9br`0id,rs`1`5s.@P`T$r^e@Ubr',rs))$D=rs`9flush`T`0`1;s.fbr(0)`9fbr`0id`1,br=^d@Ubr')"
+"`5!br)br=$D`5br`F!s.@P`T)^e@Ubr`H'`Umr(0,0,br)}$D=0`9mr`0$1,q,#Aid,ta,u`1,dc=s.dc,t1=s.`Q,t2=s.`Q^j,tb=s.`QBase,p='.sc',ns=s.`Z`qspace,un=u?u:(ns?ns:s.fun),unc=^1#7'_`H-'),r`C,l,imn=@Ui^x(un),im,b,"
+"e`5!rs`Ft1`Ft2^5ssl)t1=t2^b@u!ns)ns#6c`5!tb)tb='$P`5dc)dc=@odc)`8;`cdc='d1'`5tb^S$P`Fdc^Sd1$j12';`6dc^Sd2$j22';p`i}t1=ns+'.'+dc+'.'+p+tb}rs=$5'+(@A?'s'`n+'://'+t1+'/b/ss/'+^4+'/'+(s.#0?'5.1':'1'$UH"
+".19.4/'+$1+'?AQB=1&ndh=1'+(q?q`n+'&AQE=1'`5^g@Ss.^o`F`S>5.5)rs=^s#A4095);`crs=^s#A2047)`pid@1br(id,rs);$y}`ps.d.images&&`S>=3^w!s.^n||`S>=7)^w@Y<0||`S>=6.1)`F!s.rc)s.rc`C`5!^V){^V=1`5!s.rl)s.rl`C;@"
+"Cn]`N;^T'@u`z`ml)`z.`B@J)',750)^bl=@Cn]`5l){r.t=ta;r.u#6;r.r=rs;l[l`A]=r;`2''}imn+='^x^V;^V++}im=`I[imn]`5!im)im=`I[im@znew Image;im^zl=0;im.o^L`7'e`H^Z^zl=1;`Lwd=`z,s`5wd`ml){s=#5`B;s@J`Unrs--`5!$"
+"E)`Xm(\"rr\")}')`5!$E@1nrs=1;`Xm('rs')}`c$E++;im@O=rs`5rs`4$A=@p0^w!ta||ta^S_self$sta^S_top$s(`I.@7&&ta==`I.@7))){b=e`h;^C!im^zl&&e`d-b`d<500)e`h}`2''}`2'<im'+'g sr'+'c=$trs+'\" width=1 #1=1 border"
+"=0 alt=\"\">'`9gg`0v`1`5!`I['s^xv])`I['s^xv]`i;`2`I['s^xv]`9glf`0t,a`Ft`30,2)^S$m`32);`Ls=^Z,v=s.gg(t)`5v)s#Xv`9gl`0v`1`5#F)`Mv,`H,'gl@00)`9gv`0v`1;`2s['vpm^xv]?s['vpv^xv]:(s[v]?s[v]`n`9havf`0t,a`1"
+",b=t`30,4),x=t`34),n=^Hx),k='g^xt,m='vpm^xt,q=t,v=s.`W@bVa#Ae=s.`W@b^Ws,mn;@d$Ft)`5s[k]`F$9||@X||^O`F$9){mn=$9`30,1)`E()+$9`31)`5$G){v=$G.^KVars;e=$G.^K^Ws}}v=v?v+`H+^h+`H+^h2:''`5v@S`Mv,`H,'is@0t)"
+")s[k]`i`5`J#2'&&e)@ds.fs(s[k],e)}s[m]=0`5`J^R`KD';`6`J`ZID`Kvid';`6`J^N^Qg'`j`6`J`b^Qr'`j`6`Jvmk$s`J`Z@R`Kvmt';`6`J`D^Qvmf'`5@A^5`D^j)s[k]`i}`6`J`D^j^Qvmf'`5!@A^5`D)s[k]`i}`6`J@L^Qce'`5s[k]`E()^SAU"
+"TO')@d'ISO8859-1';`6s.em==2)@d'UTF-8'}`6`J`Z`qspace`Kns';`6`Jc`O`Kcdp';`6`J^0@G`Kcl';`6`J^y`Kvvp';`6`J@N`Kcc';`6`J$d`Kch';`6`J#K^IID`Kxact';`6`J$2`Kv0';`6`J^a`Ks';`6`J^A`Kc';`6`J`s^u`Kj';`6`J`k`Kv'"
+";`6`J^0@I`Kk';`6`J^7@D`Kbw';`6`J^7^k`Kbh';`6`J`l`Kct';`6`J@8`Khp';`6`Jp^P`Kp';`6#Ex)`Fb^Sprop`Kc$Z`6b^SeVar`Kv$Z`6b^Slist`Kl$Z`6b^Shier^Qh'+n`j`ps[k]@t`W`q'@t`W^c')$H+='&'+q+'$q(t`30,3)!='pev'?@hs["
+"k]):s[k]);}`2''`9hav`0`1;$H`i;`M^i,`H,'hav@00);`2$H`9lnf`0^l`8@3`8:'';`Lte=t`4@q`5$Tte>0&&h`4t`3te$a>=0)`2t`30,te);`2''`9ln`0h`1,n=s.`W`qs`5n)`2`Mn,`H,'ln@0h);`2''`9ltdf`0^l`8@3`8:'';`Lqi=h`4'?^Yh="
+"qi>=0?h`30,qi):h`5$Th`3h`A-(t`A$a^S.'+t)`21;`20`9ltef`0^l`8@3`8:''`5$Th`4t)>=0)`21;`20`9lt`0h`1,lft=s.`WDow^LFile^cs,lef=s.`WEx`r,$3=s.`WIn`r;$3=$3?$3:`I`P^D@7;h=h`8`5s.^KDow^LLinks&&lf$T`Mlft,`H#I"
+"d@0h))`2'd'`5s.^K@K&&h`30,1)!='# '^wlef||$3)^w!lef||`Mlef,`H#Ie@0h))^w!$3$r`M$3,`H#Ie@0h)))`2'e';`2''`9lc`7'e`H`Ls=`B,b=^f(^Z,\"`o\"`U$z=$7^Z`Ut(`U$z=0`5b)`2^Z$u`2$0'`Ubc`7'e`H`Ls=`B,f,^m`5s.d^5d.a"
+"ll^5d.all.cppXYctnr)$y;^O=e@O`Y?e@O`Y:e$e;^m`7\"s\",\"`Le@9@u^O^w^O.tag`q||^O^8`Y||^O^8Node))s.t()`e}\");^m(s`Ueo=0'`Uoh`0o`1,l=`I`P,h=o^q?o^q:'',i,j,k,p;i=h`4':^Yj=h`4'?^Yk=h`4'/')`5h^wi<0||(j>=0&"
+"&i>j)||(k>=0&&i>k))$Wo`g#T`g`A>1?o`g:(l`g?l`g`n;i=l.path@7`f'/^Yh=(p?p+'//'`n+(o^D?o^D:(l^D?l^D`n)+(h`30,1)!='/'?l.path@7`30,i<0?0:i$U'`n+h}`2h`9ot`0o){`Lt=o.tag`q;t=$Tt`E?t`E$X`5`JSHAPE')t`i`5t`F`"
+"JINPUT'&&@E&&@E`E)t=@E`E();`6!$To^q)t='A';}`2t`9oid`0o`1,^J,p,c,n`i,x=0`5t@S^6$Wo`g;c=o.`o`5o^q^w`JA$s`JAREA')^w!c$rp||p`8`4'`s$p0))n@x`6c@r^1s.rep(^1s.rep@oc,\"\\r@s\"\\n@s\"\\t@s' `H^Yx=2}`6$g^w`"
+"JINPUT$s`JSUBMIT')@r$g;x=3}`6o@O&&`JIMAGE')n=o@O`5$S^6=^sn$6;^6t=x}}`2^6`9rqf`0t,un`1,e=t`4@q,u=e>=0?`H+t`30,e)+`H:'';`2u&&u`4`H+un+`H)>=0?@it`3e$a:''`9rq`0un`1,c#6`4`H),v=^d@Usq'),q`i`5c<0)`2`Mv,'"
+"&`Hrq@0un);`2`M#7`H,'rq',0)`9sqp`0t,a`1,e=t`4@q,q=e<0?'':@it`3e+1)`Usqq[q]`i`5e>=0)`Mt`30,e),`H@m`20`9sqs`0#7q`1;^Gu[u@zq;`20`9sq`0q`1,k=@Usq',v=^dk),x,c=0;^Gq`C;^Gu`C;^Gq[q]`i;`Mv,'&`Hsqp',0);`M^4"
+",`H@mv`i;`vx$4^Gu`V)^Gq[^Gu[x]]+=(^Gq[^Gu[x]]?`H`n+x;`vx$4^Gq`V^5sqq[x]^wx==q||c<2)){v+=(v#R'`n+^Gq[x]+'`ax);c++}`2^ek,v,0)`9wdl`7'e`H`Ls=`B,r=$0,b=^f(`I,\"o^L\"),i,o,oc`5b)r=^Z$u`vi=0;i<s.d.`Ws`A^"
+"X{o=s.d.`Ws[i];oc=o.`o?\"\"+o.`o:\"\"`5(oc`4$M<0||oc`4\"^zoc(\")>=0)#Tc`4$k<0)^f(o,\"`o\",0,s.lc);}`2r^Y`Is`0`1`5`S>3^w!^g$rs.^o||`S#Z`Fs.b^5$O^W)s.$O^W('`o#M);`6s.b^5b.add^W$L)s.b.add^W$L('click#M"
+",false);`c^f(`I,'o^L',0,`Il)}`9vs`0x`1,v=s.`Z^U,g=s.`Z^U#Ok=@Uvsn^x^4+(g?'^xg`n,n=^dk),e`h,y=e.g@Q);e.s@Qy+10@y1900:0))`5v){v*=100`5!n`F!^ek,x,e))`20;n=x`pn%10000>v)`20}`21`9dyasmf`0t,m`F$Tm&&m`4t)"
+">=0)`21;`20`9dyasf`0t,m`1,i=t?t`4@q:-1,n,x`5i>=0&&m){`Ln=t`30,i),x=t`3i+1)`5`Mx,`H,'dyasm@0m))`2n}`20`9uns`0`1,x=s.`RSele^I,l=s.`RList,m=s.`RM#D,n,i;^4=^4`8`5x&&l`F!m)m=`I`P^D`5!m.toLowerCase)m`i+m"
+";l=l`8;m=m`8;n=`Ml,';`Hdyas@0m)`5n)^4=n}i=^4`4`H`Ufun=i<0?^4:^4`30,i)`9sa`0un`1;^4#6`5!@B)@B#6;`6(`H+@B+`H)`4un)<0)@B+=`H+un;^4s()`9m_i`0n,a`1,m,f=n`30,1),r,l,i`5!`Xl)`Xl`C`5!`Xnl)`Xnl`N;m=`Xl[n]`5"
+"!a&&m&&#G@Sm@4)`Xa(n)`5!m){m`C,m._c=@Um';m@4n=`I`mn;m@4l=s@4l;m@4l[m@4@zm;`I`mn++;m.s=s;m._n=n;$B`N('_c`H_in`H_il`H_i`H_e`H_d`H_dl`Hs`Hn`H_r`H_g`H_g1`H_t`H_t1`H_x`H_x1`H_rs`H_rr`H_l'`Um_l[@zm;`Xnl["
+"`Xnl`A]=n}`6m._r@Sm._m){r=m._r;r._m=m;l=$B;`vi=0;i<l`A^X@um[l[i]])r[l[i]]=m[l[i]];r@4l[r@4@zr;m=`Xl[@zr`pf==f`E())s[@zm;`2m`9m_a`7'n`Hg`H@u!g)g=@F;`Ls=`B,c=s[g@g,m,x,f=0`5!c)c=`I$h@g`5c&&s_d)s[g]`7"
+"\"s\",s_ft(s_d(c)));x=s[g]`5!x)x=s[g]=`I$h];m=`Xi(n,1)`5x){m@4=f=1`5(\"\"+x)`4\"fun^I\")>=0)x(s);`c`Xm(\"x\",n,x)}m=`Xi(n,1)`5@kl)@kl=@k=0;`tt();`2f'`Um_m`0t,n,d#W'^xt;`Ls=^Z,i,x,m,f='^xt`5`Xl&&`Xn"
+"l)`vi=0;i<`Xnl`A^X{x=`Xnl[i]`5!n||x==$Sm=`Xi(x)`5m[t]`F`J_d')`21`5d)m#ad);`cm#a)`pm[t+1]@Sm[f]`Fd)$xd);`c$x)}m[f]=1}}`20`9@f`0n,u,d,l`1,m,i=n`4':'),g=i<0?@F:n`3i+1),o=0,f,c=s.h?s.h:s.b,^m`5i>=0)n=n"
+"`30,i);m=`Xi(n)`5(l$r`Xa(n,g))&&u^5d&&c^5$Q`Y`Fd){@k=1;@kl=1`p@A)u=^1u,$5:`Hhttps:^Yf`7'e`H`B.m_a(\"@e\",$tg+'\")^Y^m`7's`Hf`Hu`Hc`H`Le,o=0@9o=s.$Q`Y(\"script\")`5o){@E=\"text/`s\"`5f)o.o^L=f;o@O=u"
+";c.appendChild(o)}`eo=0}`2o^Yo=^m(s,f,u,c)}`cm=`Xi(n);#G=1;`2m`9vo1`0t,a`Fa[t]||$b)^Z#Xa[t]`9vo2`0t,a`F#b{a#X^Z[t]`5#b$b=1}`9dlt`7'`Ls=`B,d`h,i,vo,f=0`5`tl)`vi=0;i<`tl`A^X{vo=`tl[i]`5vo`F!`Xm(\"d\""
+")||d`d-$N>=^E){`tl[i]=0;s.t(@v}`cf=1}`p`ti)clear#Bout(`ti`Udli=0`5f`F!`ti)`ti=^T`tt,^E)}`c`tl=0'`Udl`0vo`1,d`h`5!@vvo`C;`M^2,`H$I2',@v;$N=d`d`5!`tl)`tl`N;`tl[`tl`A]=vo`5!^E)^E=250;`tt()`9t`0vo,id`1"
+",trk=1,tm`h,sed=Math&&@V#3?@V#C@V#3()*10000000000000):tm`d,$1='s'+@V#Ctm`d/10800000)%10+sed,y=tm.g@Q),vt=tm.getDate($U`xMonth($U'@yy+1900:y)+' `xHour$V:`xMinute$V:`xSecond$V `xDay()+' `x#BzoneO$8()"
+",^m,^3=s.g^3(),ta`i,q`i,qs`i,#4`i,vb`C#L^2`Uuns()`5!s.td){`Ltl=^3`P,a,o,i,x`i,c`i,v`i,p`i,bw`i,bh`i,^M0',k=^e@Ucc`H$0',0@2,hp`i,ct`i,pn=0,ps`5^B&&^B.prototype){^M1'`5j.m#D){^M2'`5tm.setUTCDate){^M3"
+"'`5^g^5^o&&`S#Z^M4'`5pn.toPrecisio$S^M5';a`N`5a.forEach){^M6';i=0;o`C;^m`7'o`H`Le,i=0@9i=new Iterator(o)`e}`2i^Yi=^m(o)`5i&&i.next)^M7'#V`p`S>=4)x=^rwidth+'x'+^r#1`5s.isns||s.^n`F`S>=3$c`k(@2`5`S>="
+"4){c=^rpixelDepth;bw=`I#J@D;bh=`I#J^k}}$J=s.n.p^P}`6^g`F`S>=4$c`k(@2;c=^r^A`5`S#Z{bw=s.d.^9`Y.o$8@D;bh=s.d.^9`Y.o$8^k`5!s.^o^5b){^m`7's`Htl`H`Le,hp=0`uh$n\");hp=s.b.isH$n(tl)?\"Y\":\"N\"`e}`2hp^Yhp"
+"=^m(s,tl);^m`7's`H`Le,ct=0`uclientCaps\");ct=s.b.`l`e}`2ct^Yct=^m(s)}}}`cr`i`p$J)^Cpn<$J`A&&pn<30){ps=^s$J[pn].@7$6#S`5p`4ps)<0)p+=ps;pn++}s.^a=x;s.^A=c;s.`s^u=j;s.`k=v;s.^0@I=k;s.^7@D=bw;s.^7^k=bh"
+";s.`l=ct;s.@8=hp;s.p^P=p;s.td=1`p@v{`M^2,`H$I2',vb);`M^2,`H$I1',@v`ps.useP^P)s.doP^P(s);`Ll=`I`P,r=^3.^9.`b`5!s.^N)s.^N=l^q?l^q:l`5!s.`b@Ss._1_`b@1`b=r;s._1_`b=1}`Xm('g')`5(vo&&$N)$r`Xm('d')`F@X||^"
+"O){`Lo=^O?^O:@X`5!o)`2'';`Lp=$F'#N`q'),w=1,^J,@l,x=^6t,h,l,i,oc`5^O#T==^O){^Co@Sn@tBODY'){o=o^8`Y?o^8`Y:o^8Node`5!o)`2'';^J;@l;x=^6t}oc=o.`o?''+o.`o:''`5(oc`4$M>=0#Tc`4\"^zoc(\")<0)||oc`4$k>=0)`2''"
+"}ta=n?o$e:1;h@xi=h`4'?^Yh=s.`W@n^B||i<0?h:h`30,i);l=s.`W`q?s.`W`q:s.ln(h);t=s.`W^c?s.`W^c`8:s.lt(h)`5t^wh||l))q+=$A=$z^x(`Jd$s`Je'?@ht):'o')+(h?$Av1`ah)`n+(l?$Av2`al)`n;`ctrk=0`5s.^K@Z`F!p$W$F'^N^Y"
+"w=0}^J;i=o.sourceIndex`5@H')@r@H^Yx=1;i=1`pp&&n&&t)qs='&pid`a^sp,255))+(w#Rpidt$qw`n+'&oid`a^sn$6)+(x#Roidt$qx`n+'&ot`at)+(i#Roi$qi`n}`p!trk@Sqs)`2'';@w=s.vs(sed)`5trk`F@w)#4=s.mr($1,(vt#Rt`avt)`n+"
+"s.hav()+q+(qs?qs:s.rq(^4)),0,id,ta);qs`i;`Xm('t')`5s.p_r)s.p_r(`U`b`i}^G(qs);^b`t(@v;`p@v`M^2,`H$I1',vb`G''`5#F)`I^z$z=`I^zeo=`I^z`W`q=`I^z`W^c`i`5!id@Ss.tc@1tc=1;s.flush`T()}`2#4`9tl`0o,t,n,vo`1;@"
+"X=$7o`U`W^c=t;s.`W`q=n;s.t(@v}`5pg){`I^zco`0o){`L^t\"_\",1,#U`2$7o)`9wd^zgs`0u$S`L^t#71,#U`2s.t()`9wd^zdc`0u$S`L^t#7#U`2s.t()}}@A=(`I`P`g`8`4$5s@p0`Ud=^9;s.b=s.d.body`5$o`Y#Q`q@1h=$o`Y#Q`q('HEAD')`"
+"5s.h)s.h=s.h[0]}s.n=navigator;s.u=s.n.userAgent;@Y=s.u`4'N$l6/^Y`Lapn$R`q,v$R^u,ie=v`4#9'),o=s.u`4'@T '),i`5v`4'@T@p0||o>0)apn='@T';^g$K^SMicrosoft Internet Explorer'`Uisns$K^SN$l'`U^n$K^S@T'`U^o=("
+"s.u`4'Mac@p0)`5o>0)`S`ws.u`3o+6));`6ie>0){`S=^Hi=v`3ie+5))`5`S>3)`S`wi)}`6@Y>0)`S`ws.u`3@Y+10));`c`S`wv`Uem=0`5^B#P^v){i=^p^B#P^v(256))`E(`Uem=(i^S%C4%80'?2:(i^S%U0100'?1:0))}s.sa(un`Uvl_l='^R,`ZID"
+",vmk,`Z@R,`D,`D^j,ppu,@L,`Z`qspace,c`O,^0@G,#N`q,^N,`b,@N';^i=^h+',^y,$d,server,#N^c,#K^IID,purchaseID,$2,state,zip,#2,products,`W`q,`W^c';`v`Ln=1;n<51;n++)^i+=',prop@e,eVar@e,hier@e,list$Z^h2=',tn"
+"t,pe#81#82#83,^a,^A,`s^u,`k,^0@I,^7@D,^7^k,`l,@8,p^P';^i+=^h2;^2=^i+',`Q,`Q^j,`QBase,fpC`O,@P`T,#0,`Z^U,`Z^U#O`RSele^I,`RList,`RM#D,^KDow^LLinks,^K@K,^K@Z,`W@n^B,`WDow^LFile^cs,`WEx`r,`WIn`r,`W@bVa"
+"#A`W@b^Ws,`W`qs,$z,eo,_1_`b';#F=pg#L^2)`5!ss)`Is()",
w=window,l=w.s_c_il,n=navigator,u=n.userAgent,v=n.appVersion,e=v.indexOf('MSIE '),m=u.indexOf('Netscape6/'),a,i,s;if(un){un=un.toLowerCase();if(l)for(i=0;i<l.length;i++){s=l[i];if(s._c=='s_c'){if(s.oun==un)return s;else if(s.fs&&s.sa&&s.fs(s.oun,un)){s.sa(un);return s}}}}
w.s_r=new Function("x","o","n","var i=x.indexOf(o);if(i>=0&&x.split)x=(x.split(o)).join(n);else while(i>=0){x=x.substring(0,i)+n+x.substring(i+o.length);i=x.indexOf(o)}return x");
w.s_d=new Function("x","var t='`^@$#',l='0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz',d,n=0,b,k,w,i=x.lastIndexOf('~~');if(i>0){d=x.substring(0,i);x=x.substring(i+2);while(d){w=d;i"
+"=d.indexOf('~');if(i>0){w=d.substring(0,i);d=d.substring(i+1)}else d='';b=(n-n%62)/62;k=n-b*62;k=t.substring(b,b+1)+l.substring(k,k+1);x=s_r(x,k,w);n++}for(i=0;i<5;i++){w=t.substring(i,i+1);x=s_r(x"
+",w+' ',w)}}return x");
w.s_fe=new Function("c","return s_r(s_r(s_r(c,'\\\\','\\\\\\\\'),'\"','\\\\\"'),\"\\n\",\"\\\\n\")");
w.s_fa=new Function("f","var s=f.indexOf('(')+1,e=f.indexOf(')'),a='',c;while(s>=0&&s<e){c=f.substring(s,s+1);if(c==',')a+='\",\"';else if((\"\\n\\r\\t \").indexOf(c)<0)a+=c;s++}return a?'\"'+a+'\"':"
+"a");
w.s_ft=new Function("c","c+='';var s,e,o,a,d,q,f,h,x;s=c.indexOf('=function(');while(s>=0){s++;d=1;q='';x=0;f=c.substring(s);a=s_fa(f);e=o=c.indexOf('{',s);e++;while(d>0){h=c.substring(e,e+1);if(q){i"
+"f(h==q&&!x)q='';if(h=='\\\\')x=x?0:1;else x=0}else{if(h=='\"'||h==\"'\")q=h;if(h=='{')d++;if(h=='}')d--}if(d>0)e++}c=c.substring(0,s)+'new Function('+(a?a+',':'')+'\"'+s_fe(c.substring(o+1,e))+'\")"
+"'+c.substring(e+1);s=c.indexOf('=function(')}return c;");
c=s_d(c);if(e>0){a=parseInt(i=v.substring(e+5));if(a>3)a=parseFloat(i)}else if(m>0)a=parseFloat(u.substring(m+10));else a=parseFloat(v);if(a>=5&&v.indexOf('Opera')<0&&u.indexOf('Opera')<0){w.s_c=new Function("un","pg","ss","var s=this;"+c);return new s_c(un,pg,ss)}else s=new Function("un","pg","ss","var s=new Object;"+s_ft(c)+";return s");return s(un,pg,ss)}

(function () {
s.linkInternalFilters="javascript:,codeplex.com";
s.pageName = window.location.hostname.toLowerCase() + window.location.pathname.toLowerCase();
var temphost = window.location.hostname.toLowerCase();
s.server = temphost;
s.channel = temphost.replace(/\.codeplex\.com/,"");
s.prop1 = "codeplex";
s.prop2 = "en-us";  // update as appropriate
s.prop3 = document.title;

var temppath = window.location.pathname.substring(1).toLowerCase();
var temppath2 = getOmniPath(temppath,0,1);
var temppatharr = temppath2.split('\/');
if (temppatharr.length > 0) s.prop4 = s.channel + ": " + temppatharr[0]   
if (temppatharr.length > 1) s.prop5 = s.prop4 + ": " + temppatharr[1]
s.prop10 = window.location.href.toLowerCase();

var tempvar;
tempvar = getOmniMetaTagData("t_omni_title"); if (tempvar) s.prop3 = tempvar;
tempvar = getOmniMetaTagData("wt.pi"); if (tempvar) s.prop6 = tempvar;

s.prop36=s.getQueryParam('ProjectSearchText'); // project search text
s.prop37=s.getQueryParam('SearchText'); // wiki search text
s.prop38=s.getQueryParam('TagName'); // tag search

s.eVar47="D=User-Agent";
s.visitorID = setOmniID();
}
)();



try { var s_code = s.t(); if (s_code) document.write(s_code) } catch (err) { }
/*
 * jQuery JavaScript Library v1.3.2
 * http://jquery.com/
 *
 * Copyright (c) 2009 John Resig
 * Dual licensed under the MIT and GPL licenses.
 * http://docs.jquery.com/License
 *
 * Date: 2009-02-19 17:34:21 -0500 (Thu, 19 Feb 2009)
 * Revision: 6246
 */
(function(){var l=this,g,y=l.jQuery,p=l.$,o=l.jQuery=l.$=function(E,F){return new o.fn.init(E,F)},D=/^[^<]*(<(.|\s)+>)[^>]*$|^#([\w-]+)$/,f=/^.[^:#\[\.,]*$/;o.fn=o.prototype={init:function(E,H){E=E||document;if(E.nodeType){this[0]=E;this.length=1;this.context=E;return this}if(typeof E==="string"){var G=D.exec(E);if(G&&(G[1]||!H)){if(G[1]){E=o.clean([G[1]],H)}else{var I=document.getElementById(G[3]);if(I&&I.id!=G[3]){return o().find(E)}var F=o(I||[]);F.context=document;F.selector=E;return F}}else{return o(H).find(E)}}else{if(o.isFunction(E)){return o(document).ready(E)}}if(E.selector&&E.context){this.selector=E.selector;this.context=E.context}return this.setArray(o.isArray(E)?E:o.makeArray(E))},selector:"",jquery:"1.3.2",size:function(){return this.length},get:function(E){return E===g?Array.prototype.slice.call(this):this[E]},pushStack:function(F,H,E){var G=o(F);G.prevObject=this;G.context=this.context;if(H==="find"){G.selector=this.selector+(this.selector?" ":"")+E}else{if(H){G.selector=this.selector+"."+H+"("+E+")"}}return G},setArray:function(E){this.length=0;Array.prototype.push.apply(this,E);return this},each:function(F,E){return o.each(this,F,E)},index:function(E){return o.inArray(E&&E.jquery?E[0]:E,this)},attr:function(F,H,G){var E=F;if(typeof F==="string"){if(H===g){return this[0]&&o[G||"attr"](this[0],F)}else{E={};E[F]=H}}return this.each(function(I){for(F in E){o.attr(G?this.style:this,F,o.prop(this,E[F],G,I,F))}})},css:function(E,F){if((E=="width"||E=="height")&&parseFloat(F)<0){F=g}return this.attr(E,F,"curCSS")},text:function(F){if(typeof F!=="object"&&F!=null){return this.empty().append((this[0]&&this[0].ownerDocument||document).createTextNode(F))}var E="";o.each(F||this,function(){o.each(this.childNodes,function(){if(this.nodeType!=8){E+=this.nodeType!=1?this.nodeValue:o.fn.text([this])}})});return E},wrapAll:function(E){if(this[0]){var F=o(E,this[0].ownerDocument).clone();if(this[0].parentNode){F.insertBefore(this[0])}F.map(function(){var G=this;while(G.firstChild){G=G.firstChild}return G}).append(this)}return this},wrapInner:function(E){return this.each(function(){o(this).contents().wrapAll(E)})},wrap:function(E){return this.each(function(){o(this).wrapAll(E)})},append:function(){return this.domManip(arguments,true,function(E){if(this.nodeType==1){this.appendChild(E)}})},prepend:function(){return this.domManip(arguments,true,function(E){if(this.nodeType==1){this.insertBefore(E,this.firstChild)}})},before:function(){return this.domManip(arguments,false,function(E){this.parentNode.insertBefore(E,this)})},after:function(){return this.domManip(arguments,false,function(E){this.parentNode.insertBefore(E,this.nextSibling)})},end:function(){return this.prevObject||o([])},push:[].push,sort:[].sort,splice:[].splice,find:function(E){if(this.length===1){var F=this.pushStack([],"find",E);F.length=0;o.find(E,this[0],F);return F}else{return this.pushStack(o.unique(o.map(this,function(G){return o.find(E,G)})),"find",E)}},clone:function(G){var E=this.map(function(){if(!o.support.noCloneEvent&&!o.isXMLDoc(this)){var I=this.outerHTML;if(!I){var J=this.ownerDocument.createElement("div");J.appendChild(this.cloneNode(true));I=J.innerHTML}return o.clean([I.replace(/ jQuery\d+="(?:\d+|null)"/g,"").replace(/^\s*/,"")])[0]}else{return this.cloneNode(true)}});if(G===true){var H=this.find("*").andSelf(),F=0;E.find("*").andSelf().each(function(){if(this.nodeName!==H[F].nodeName){return}var I=o.data(H[F],"events");for(var K in I){for(var J in I[K]){o.event.add(this,K,I[K][J],I[K][J].data)}}F++})}return E},filter:function(E){return this.pushStack(o.isFunction(E)&&o.grep(this,function(G,F){return E.call(G,F)})||o.multiFilter(E,o.grep(this,function(F){return F.nodeType===1})),"filter",E)},closest:function(E){var G=o.expr.match.POS.test(E)?o(E):null,F=0;return this.map(function(){var H=this;while(H&&H.ownerDocument){if(G?G.index(H)>-1:o(H).is(E)){o.data(H,"closest",F);return H}H=H.parentNode;F++}})},not:function(E){if(typeof E==="string"){if(f.test(E)){return this.pushStack(o.multiFilter(E,this,true),"not",E)}else{E=o.multiFilter(E,this)}}var F=E.length&&E[E.length-1]!==g&&!E.nodeType;return this.filter(function(){return F?o.inArray(this,E)<0:this!=E})},add:function(E){return this.pushStack(o.unique(o.merge(this.get(),typeof E==="string"?o(E):o.makeArray(E))))},is:function(E){return !!E&&o.multiFilter(E,this).length>0},hasClass:function(E){return !!E&&this.is("."+E)},val:function(K){if(K===g){var E=this[0];if(E){if(o.nodeName(E,"option")){return(E.attributes.value||{}).specified?E.value:E.text}if(o.nodeName(E,"select")){var I=E.selectedIndex,L=[],M=E.options,H=E.type=="select-one";if(I<0){return null}for(var F=H?I:0,J=H?I+1:M.length;F<J;F++){var G=M[F];if(G.selected){K=o(G).val();if(H){return K}L.push(K)}}return L}return(E.value||"").replace(/\r/g,"")}return g}if(typeof K==="number"){K+=""}return this.each(function(){if(this.nodeType!=1){return}if(o.isArray(K)&&/radio|checkbox/.test(this.type)){this.checked=(o.inArray(this.value,K)>=0||o.inArray(this.name,K)>=0)}else{if(o.nodeName(this,"select")){var N=o.makeArray(K);o("option",this).each(function(){this.selected=(o.inArray(this.value,N)>=0||o.inArray(this.text,N)>=0)});if(!N.length){this.selectedIndex=-1}}else{this.value=K}}})},html:function(E){return E===g?(this[0]?this[0].innerHTML.replace(/ jQuery\d+="(?:\d+|null)"/g,""):null):this.empty().append(E)},replaceWith:function(E){return this.after(E).remove()},eq:function(E){return this.slice(E,+E+1)},slice:function(){return this.pushStack(Array.prototype.slice.apply(this,arguments),"slice",Array.prototype.slice.call(arguments).join(","))},map:function(E){return this.pushStack(o.map(this,function(G,F){return E.call(G,F,G)}))},andSelf:function(){return this.add(this.prevObject)},domManip:function(J,M,L){if(this[0]){var I=(this[0].ownerDocument||this[0]).createDocumentFragment(),F=o.clean(J,(this[0].ownerDocument||this[0]),I),H=I.firstChild;if(H){for(var G=0,E=this.length;G<E;G++){L.call(K(this[G],H),this.length>1||G>0?I.cloneNode(true):I)}}if(F){o.each(F,z)}}return this;function K(N,O){return M&&o.nodeName(N,"table")&&o.nodeName(O,"tr")?(N.getElementsByTagName("tbody")[0]||N.appendChild(N.ownerDocument.createElement("tbody"))):N}}};o.fn.init.prototype=o.fn;function z(E,F){if(F.src){o.ajax({url:F.src,async:false,dataType:"script"})}else{o.globalEval(F.text||F.textContent||F.innerHTML||"")}if(F.parentNode){F.parentNode.removeChild(F)}}function e(){return +new Date}o.extend=o.fn.extend=function(){var J=arguments[0]||{},H=1,I=arguments.length,E=false,G;if(typeof J==="boolean"){E=J;J=arguments[1]||{};H=2}if(typeof J!=="object"&&!o.isFunction(J)){J={}}if(I==H){J=this;--H}for(;H<I;H++){if((G=arguments[H])!=null){for(var F in G){var K=J[F],L=G[F];if(J===L){continue}if(E&&L&&typeof L==="object"&&!L.nodeType){J[F]=o.extend(E,K||(L.length!=null?[]:{}),L)}else{if(L!==g){J[F]=L}}}}}return J};var b=/z-?index|font-?weight|opacity|zoom|line-?height/i,q=document.defaultView||{},s=Object.prototype.toString;o.extend({noConflict:function(E){l.$=p;if(E){l.jQuery=y}return o},isFunction:function(E){return s.call(E)==="[object Function]"},isArray:function(E){return s.call(E)==="[object Array]"},isXMLDoc:function(E){return E.nodeType===9&&E.documentElement.nodeName!=="HTML"||!!E.ownerDocument&&o.isXMLDoc(E.ownerDocument)},globalEval:function(G){if(G&&/\S/.test(G)){var F=document.getElementsByTagName("head")[0]||document.documentElement,E=document.createElement("script");E.type="text/javascript";if(o.support.scriptEval){E.appendChild(document.createTextNode(G))}else{E.text=G}F.insertBefore(E,F.firstChild);F.removeChild(E)}},nodeName:function(F,E){return F.nodeName&&F.nodeName.toUpperCase()==E.toUpperCase()},each:function(G,K,F){var E,H=0,I=G.length;if(F){if(I===g){for(E in G){if(K.apply(G[E],F)===false){break}}}else{for(;H<I;){if(K.apply(G[H++],F)===false){break}}}}else{if(I===g){for(E in G){if(K.call(G[E],E,G[E])===false){break}}}else{for(var J=G[0];H<I&&K.call(J,H,J)!==false;J=G[++H]){}}}return G},prop:function(H,I,G,F,E){if(o.isFunction(I)){I=I.call(H,F)}return typeof I==="number"&&G=="curCSS"&&!b.test(E)?I+"px":I},className:{add:function(E,F){o.each((F||"").split(/\s+/),function(G,H){if(E.nodeType==1&&!o.className.has(E.className,H)){E.className+=(E.className?" ":"")+H}})},remove:function(E,F){if(E.nodeType==1){E.className=F!==g?o.grep(E.className.split(/\s+/),function(G){return !o.className.has(F,G)}).join(" "):""}},has:function(F,E){return F&&o.inArray(E,(F.className||F).toString().split(/\s+/))>-1}},swap:function(H,G,I){var E={};for(var F in G){E[F]=H.style[F];H.style[F]=G[F]}I.call(H);for(var F in G){H.style[F]=E[F]}},css:function(H,F,J,E){if(F=="width"||F=="height"){var L,G={position:"absolute",visibility:"hidden",display:"block"},K=F=="width"?["Left","Right"]:["Top","Bottom"];function I(){L=F=="width"?H.offsetWidth:H.offsetHeight;if(E==="border"){return}o.each(K,function(){if(!E){L-=parseFloat(o.curCSS(H,"padding"+this,true))||0}if(E==="margin"){L+=parseFloat(o.curCSS(H,"margin"+this,true))||0}else{L-=parseFloat(o.curCSS(H,"border"+this+"Width",true))||0}})}if(H.offsetWidth!==0){I()}else{o.swap(H,G,I)}return Math.max(0,Math.round(L))}return o.curCSS(H,F,J)},curCSS:function(I,F,G){var L,E=I.style;if(F=="opacity"&&!o.support.opacity){L=o.attr(E,"opacity");return L==""?"1":L}if(F.match(/float/i)){F=w}if(!G&&E&&E[F]){L=E[F]}else{if(q.getComputedStyle){if(F.match(/float/i)){F="float"}F=F.replace(/([A-Z])/g,"-$1").toLowerCase();var M=q.getComputedStyle(I,null);if(M){L=M.getPropertyValue(F)}if(F=="opacity"&&L==""){L="1"}}else{if(I.currentStyle){var J=F.replace(/\-(\w)/g,function(N,O){return O.toUpperCase()});L=I.currentStyle[F]||I.currentStyle[J];if(!/^\d+(px)?$/i.test(L)&&/^\d/.test(L)){var H=E.left,K=I.runtimeStyle.left;I.runtimeStyle.left=I.currentStyle.left;E.left=L||0;L=E.pixelLeft+"px";E.left=H;I.runtimeStyle.left=K}}}}return L},clean:function(F,K,I){K=K||document;if(typeof K.createElement==="undefined"){K=K.ownerDocument||K[0]&&K[0].ownerDocument||document}if(!I&&F.length===1&&typeof F[0]==="string"){var H=/^<(\w+)\s*\/?>$/.exec(F[0]);if(H){return[K.createElement(H[1])]}}var G=[],E=[],L=K.createElement("div");o.each(F,function(P,S){if(typeof S==="number"){S+=""}if(!S){return}if(typeof S==="string"){S=S.replace(/(<(\w+)[^>]*?)\/>/g,function(U,V,T){return T.match(/^(abbr|br|col|img|input|link|meta|param|hr|area|embed)$/i)?U:V+"></"+T+">"});var O=S.replace(/^\s+/,"").substring(0,10).toLowerCase();var Q=!O.indexOf("<opt")&&[1,"<select multiple='multiple'>","</select>"]||!O.indexOf("<leg")&&[1,"<fieldset>","</fieldset>"]||O.match(/^<(thead|tbody|tfoot|colg|cap)/)&&[1,"<table>","</table>"]||!O.indexOf("<tr")&&[2,"<table><tbody>","</tbody></table>"]||(!O.indexOf("<td")||!O.indexOf("<th"))&&[3,"<table><tbody><tr>","</tr></tbody></table>"]||!O.indexOf("<col")&&[2,"<table><tbody></tbody><colgroup>","</colgroup></table>"]||!o.support.htmlSerialize&&[1,"div<div>","</div>"]||[0,"",""];L.innerHTML=Q[1]+S+Q[2];while(Q[0]--){L=L.lastChild}if(!o.support.tbody){var R=/<tbody/i.test(S),N=!O.indexOf("<table")&&!R?L.firstChild&&L.firstChild.childNodes:Q[1]=="<table>"&&!R?L.childNodes:[];for(var M=N.length-1;M>=0;--M){if(o.nodeName(N[M],"tbody")&&!N[M].childNodes.length){N[M].parentNode.removeChild(N[M])}}}if(!o.support.leadingWhitespace&&/^\s/.test(S)){L.insertBefore(K.createTextNode(S.match(/^\s*/)[0]),L.firstChild)}S=o.makeArray(L.childNodes)}if(S.nodeType){G.push(S)}else{G=o.merge(G,S)}});if(I){for(var J=0;G[J];J++){if(o.nodeName(G[J],"script")&&(!G[J].type||G[J].type.toLowerCase()==="text/javascript")){E.push(G[J].parentNode?G[J].parentNode.removeChild(G[J]):G[J])}else{if(G[J].nodeType===1){G.splice.apply(G,[J+1,0].concat(o.makeArray(G[J].getElementsByTagName("script"))))}I.appendChild(G[J])}}return E}return G},attr:function(J,G,K){if(!J||J.nodeType==3||J.nodeType==8){return g}var H=!o.isXMLDoc(J),L=K!==g;G=H&&o.props[G]||G;if(J.tagName){var F=/href|src|style/.test(G);if(G=="selected"&&J.parentNode){J.parentNode.selectedIndex}if(G in J&&H&&!F){if(L){if(G=="type"&&o.nodeName(J,"input")&&J.parentNode){throw"type property can't be changed"}J[G]=K}if(o.nodeName(J,"form")&&J.getAttributeNode(G)){return J.getAttributeNode(G).nodeValue}if(G=="tabIndex"){var I=J.getAttributeNode("tabIndex");return I&&I.specified?I.value:J.nodeName.match(/(button|input|object|select|textarea)/i)?0:J.nodeName.match(/^(a|area)$/i)&&J.href?0:g}return J[G]}if(!o.support.style&&H&&G=="style"){return o.attr(J.style,"cssText",K)}if(L){J.setAttribute(G,""+K)}var E=!o.support.hrefNormalized&&H&&F?J.getAttribute(G,2):J.getAttribute(G);return E===null?g:E}if(!o.support.opacity&&G=="opacity"){if(L){J.zoom=1;J.filter=(J.filter||"").replace(/alpha\([^)]*\)/,"")+(parseInt(K)+""=="NaN"?"":"alpha(opacity="+K*100+")")}return J.filter&&J.filter.indexOf("opacity=")>=0?(parseFloat(J.filter.match(/opacity=([^)]*)/)[1])/100)+"":""}G=G.replace(/-([a-z])/ig,function(M,N){return N.toUpperCase()});if(L){J[G]=K}return J[G]},trim:function(E){return(E||"").replace(/^\s+|\s+$/g,"")},makeArray:function(G){var E=[];if(G!=null){var F=G.length;if(F==null||typeof G==="string"||o.isFunction(G)||G.setInterval){E[0]=G}else{while(F){E[--F]=G[F]}}}return E},inArray:function(G,H){for(var E=0,F=H.length;E<F;E++){if(H[E]===G){return E}}return -1},merge:function(H,E){var F=0,G,I=H.length;if(!o.support.getAll){while((G=E[F++])!=null){if(G.nodeType!=8){H[I++]=G}}}else{while((G=E[F++])!=null){H[I++]=G}}return H},unique:function(K){var F=[],E={};try{for(var G=0,H=K.length;G<H;G++){var J=o.data(K[G]);if(!E[J]){E[J]=true;F.push(K[G])}}}catch(I){F=K}return F},grep:function(F,J,E){var G=[];for(var H=0,I=F.length;H<I;H++){if(!E!=!J(F[H],H)){G.push(F[H])}}return G},map:function(E,J){var F=[];for(var G=0,H=E.length;G<H;G++){var I=J(E[G],G);if(I!=null){F[F.length]=I}}return F.concat.apply([],F)}});var C=navigator.userAgent.toLowerCase();o.browser={version:(C.match(/.+(?:rv|it|ra|ie)[\/: ]([\d.]+)/)||[0,"0"])[1],safari:/webkit/.test(C),opera:/opera/.test(C),msie:/msie/.test(C)&&!/opera/.test(C),mozilla:/mozilla/.test(C)&&!/(compatible|webkit)/.test(C)};o.each({parent:function(E){return E.parentNode},parents:function(E){return o.dir(E,"parentNode")},next:function(E){return o.nth(E,2,"nextSibling")},prev:function(E){return o.nth(E,2,"previousSibling")},nextAll:function(E){return o.dir(E,"nextSibling")},prevAll:function(E){return o.dir(E,"previousSibling")},siblings:function(E){return o.sibling(E.parentNode.firstChild,E)},children:function(E){return o.sibling(E.firstChild)},contents:function(E){return o.nodeName(E,"iframe")?E.contentDocument||E.contentWindow.document:o.makeArray(E.childNodes)}},function(E,F){o.fn[E]=function(G){var H=o.map(this,F);if(G&&typeof G=="string"){H=o.multiFilter(G,H)}return this.pushStack(o.unique(H),E,G)}});o.each({appendTo:"append",prependTo:"prepend",insertBefore:"before",insertAfter:"after",replaceAll:"replaceWith"},function(E,F){o.fn[E]=function(G){var J=[],L=o(G);for(var K=0,H=L.length;K<H;K++){var I=(K>0?this.clone(true):this).get();o.fn[F].apply(o(L[K]),I);J=J.concat(I)}return this.pushStack(J,E,G)}});o.each({removeAttr:function(E){o.attr(this,E,"");if(this.nodeType==1){this.removeAttribute(E)}},addClass:function(E){o.className.add(this,E)},removeClass:function(E){o.className.remove(this,E)},toggleClass:function(F,E){if(typeof E!=="boolean"){E=!o.className.has(this,F)}o.className[E?"add":"remove"](this,F)},remove:function(E){if(!E||o.filter(E,[this]).length){o("*",this).add([this]).each(function(){o.event.remove(this);o.removeData(this)});if(this.parentNode){this.parentNode.removeChild(this)}}},empty:function(){o(this).children().remove();while(this.firstChild){this.removeChild(this.firstChild)}}},function(E,F){o.fn[E]=function(){return this.each(F,arguments)}});function j(E,F){return E[0]&&parseInt(o.curCSS(E[0],F,true),10)||0}var h="jQuery"+e(),v=0,A={};o.extend({cache:{},data:function(F,E,G){F=F==l?A:F;var H=F[h];if(!H){H=F[h]=++v}if(E&&!o.cache[H]){o.cache[H]={}}if(G!==g){o.cache[H][E]=G}return E?o.cache[H][E]:H},removeData:function(F,E){F=F==l?A:F;var H=F[h];if(E){if(o.cache[H]){delete o.cache[H][E];E="";for(E in o.cache[H]){break}if(!E){o.removeData(F)}}}else{try{delete F[h]}catch(G){if(F.removeAttribute){F.removeAttribute(h)}}delete o.cache[H]}},queue:function(F,E,H){if(F){E=(E||"fx")+"queue";var G=o.data(F,E);if(!G||o.isArray(H)){G=o.data(F,E,o.makeArray(H))}else{if(H){G.push(H)}}}return G},dequeue:function(H,G){var E=o.queue(H,G),F=E.shift();if(!G||G==="fx"){F=E[0]}if(F!==g){F.call(H)}}});o.fn.extend({data:function(E,G){var H=E.split(".");H[1]=H[1]?"."+H[1]:"";if(G===g){var F=this.triggerHandler("getData"+H[1]+"!",[H[0]]);if(F===g&&this.length){F=o.data(this[0],E)}return F===g&&H[1]?this.data(H[0]):F}else{return this.trigger("setData"+H[1]+"!",[H[0],G]).each(function(){o.data(this,E,G)})}},removeData:function(E){return this.each(function(){o.removeData(this,E)})},queue:function(E,F){if(typeof E!=="string"){F=E;E="fx"}if(F===g){return o.queue(this[0],E)}return this.each(function(){var G=o.queue(this,E,F);if(E=="fx"&&G.length==1){G[0].call(this)}})},dequeue:function(E){return this.each(function(){o.dequeue(this,E)})}});
/*
 * Sizzle CSS Selector Engine - v0.9.3
 *  Copyright 2009, The Dojo Foundation
 *  Released under the MIT, BSD, and GPL Licenses.
 *  More information: http://sizzlejs.com/
 */
(function(){var R=/((?:\((?:\([^()]+\)|[^()]+)+\)|\[(?:\[[^[\]]*\]|['"][^'"]*['"]|[^[\]'"]+)+\]|\\.|[^ >+~,(\[\\]+)+|[>+~])(\s*,\s*)?/g,L=0,H=Object.prototype.toString;var F=function(Y,U,ab,ac){ab=ab||[];U=U||document;if(U.nodeType!==1&&U.nodeType!==9){return[]}if(!Y||typeof Y!=="string"){return ab}var Z=[],W,af,ai,T,ad,V,X=true;R.lastIndex=0;while((W=R.exec(Y))!==null){Z.push(W[1]);if(W[2]){V=RegExp.rightContext;break}}if(Z.length>1&&M.exec(Y)){if(Z.length===2&&I.relative[Z[0]]){af=J(Z[0]+Z[1],U)}else{af=I.relative[Z[0]]?[U]:F(Z.shift(),U);while(Z.length){Y=Z.shift();if(I.relative[Y]){Y+=Z.shift()}af=J(Y,af)}}}else{var ae=ac?{expr:Z.pop(),set:E(ac)}:F.find(Z.pop(),Z.length===1&&U.parentNode?U.parentNode:U,Q(U));af=F.filter(ae.expr,ae.set);if(Z.length>0){ai=E(af)}else{X=false}while(Z.length){var ah=Z.pop(),ag=ah;if(!I.relative[ah]){ah=""}else{ag=Z.pop()}if(ag==null){ag=U}I.relative[ah](ai,ag,Q(U))}}if(!ai){ai=af}if(!ai){throw"Syntax error, unrecognized expression: "+(ah||Y)}if(H.call(ai)==="[object Array]"){if(!X){ab.push.apply(ab,ai)}else{if(U.nodeType===1){for(var aa=0;ai[aa]!=null;aa++){if(ai[aa]&&(ai[aa]===true||ai[aa].nodeType===1&&K(U,ai[aa]))){ab.push(af[aa])}}}else{for(var aa=0;ai[aa]!=null;aa++){if(ai[aa]&&ai[aa].nodeType===1){ab.push(af[aa])}}}}}else{E(ai,ab)}if(V){F(V,U,ab,ac);if(G){hasDuplicate=false;ab.sort(G);if(hasDuplicate){for(var aa=1;aa<ab.length;aa++){if(ab[aa]===ab[aa-1]){ab.splice(aa--,1)}}}}}return ab};F.matches=function(T,U){return F(T,null,null,U)};F.find=function(aa,T,ab){var Z,X;if(!aa){return[]}for(var W=0,V=I.order.length;W<V;W++){var Y=I.order[W],X;if((X=I.match[Y].exec(aa))){var U=RegExp.leftContext;if(U.substr(U.length-1)!=="\\"){X[1]=(X[1]||"").replace(/\\/g,"");Z=I.find[Y](X,T,ab);if(Z!=null){aa=aa.replace(I.match[Y],"");break}}}}if(!Z){Z=T.getElementsByTagName("*")}return{set:Z,expr:aa}};F.filter=function(ad,ac,ag,W){var V=ad,ai=[],aa=ac,Y,T,Z=ac&&ac[0]&&Q(ac[0]);while(ad&&ac.length){for(var ab in I.filter){if((Y=I.match[ab].exec(ad))!=null){var U=I.filter[ab],ah,af;T=false;if(aa==ai){ai=[]}if(I.preFilter[ab]){Y=I.preFilter[ab](Y,aa,ag,ai,W,Z);if(!Y){T=ah=true}else{if(Y===true){continue}}}if(Y){for(var X=0;(af=aa[X])!=null;X++){if(af){ah=U(af,Y,X,aa);var ae=W^!!ah;if(ag&&ah!=null){if(ae){T=true}else{aa[X]=false}}else{if(ae){ai.push(af);T=true}}}}}if(ah!==g){if(!ag){aa=ai}ad=ad.replace(I.match[ab],"");if(!T){return[]}break}}}if(ad==V){if(T==null){throw"Syntax error, unrecognized expression: "+ad}else{break}}V=ad}return aa};var I=F.selectors={order:["ID","NAME","TAG"],match:{ID:/#((?:[\w\u00c0-\uFFFF_-]|\\.)+)/,CLASS:/\.((?:[\w\u00c0-\uFFFF_-]|\\.)+)/,NAME:/\[name=['"]*((?:[\w\u00c0-\uFFFF_-]|\\.)+)['"]*\]/,ATTR:/\[\s*((?:[\w\u00c0-\uFFFF_-]|\\.)+)\s*(?:(\S?=)\s*(['"]*)(.*?)\3|)\s*\]/,TAG:/^((?:[\w\u00c0-\uFFFF\*_-]|\\.)+)/,CHILD:/:(only|nth|last|first)-child(?:\((even|odd|[\dn+-]*)\))?/,POS:/:(nth|eq|gt|lt|first|last|even|odd)(?:\((\d*)\))?(?=[^-]|$)/,PSEUDO:/:((?:[\w\u00c0-\uFFFF_-]|\\.)+)(?:\((['"]*)((?:\([^\)]+\)|[^\2\(\)]*)+)\2\))?/},attrMap:{"class":"className","for":"htmlFor"},attrHandle:{href:function(T){return T.getAttribute("href")}},relative:{"+":function(aa,T,Z){var X=typeof T==="string",ab=X&&!/\W/.test(T),Y=X&&!ab;if(ab&&!Z){T=T.toUpperCase()}for(var W=0,V=aa.length,U;W<V;W++){if((U=aa[W])){while((U=U.previousSibling)&&U.nodeType!==1){}aa[W]=Y||U&&U.nodeName===T?U||false:U===T}}if(Y){F.filter(T,aa,true)}},">":function(Z,U,aa){var X=typeof U==="string";if(X&&!/\W/.test(U)){U=aa?U:U.toUpperCase();for(var V=0,T=Z.length;V<T;V++){var Y=Z[V];if(Y){var W=Y.parentNode;Z[V]=W.nodeName===U?W:false}}}else{for(var V=0,T=Z.length;V<T;V++){var Y=Z[V];if(Y){Z[V]=X?Y.parentNode:Y.parentNode===U}}if(X){F.filter(U,Z,true)}}},"":function(W,U,Y){var V=L++,T=S;if(!U.match(/\W/)){var X=U=Y?U:U.toUpperCase();T=P}T("parentNode",U,V,W,X,Y)},"~":function(W,U,Y){var V=L++,T=S;if(typeof U==="string"&&!U.match(/\W/)){var X=U=Y?U:U.toUpperCase();T=P}T("previousSibling",U,V,W,X,Y)}},find:{ID:function(U,V,W){if(typeof V.getElementById!=="undefined"&&!W){var T=V.getElementById(U[1]);return T?[T]:[]}},NAME:function(V,Y,Z){if(typeof Y.getElementsByName!=="undefined"){var U=[],X=Y.getElementsByName(V[1]);for(var W=0,T=X.length;W<T;W++){if(X[W].getAttribute("name")===V[1]){U.push(X[W])}}return U.length===0?null:U}},TAG:function(T,U){return U.getElementsByTagName(T[1])}},preFilter:{CLASS:function(W,U,V,T,Z,aa){W=" "+W[1].replace(/\\/g,"")+" ";if(aa){return W}for(var X=0,Y;(Y=U[X])!=null;X++){if(Y){if(Z^(Y.className&&(" "+Y.className+" ").indexOf(W)>=0)){if(!V){T.push(Y)}}else{if(V){U[X]=false}}}}return false},ID:function(T){return T[1].replace(/\\/g,"")},TAG:function(U,T){for(var V=0;T[V]===false;V++){}return T[V]&&Q(T[V])?U[1]:U[1].toUpperCase()},CHILD:function(T){if(T[1]=="nth"){var U=/(-?)(\d*)n((?:\+|-)?\d*)/.exec(T[2]=="even"&&"2n"||T[2]=="odd"&&"2n+1"||!/\D/.test(T[2])&&"0n+"+T[2]||T[2]);T[2]=(U[1]+(U[2]||1))-0;T[3]=U[3]-0}T[0]=L++;return T},ATTR:function(X,U,V,T,Y,Z){var W=X[1].replace(/\\/g,"");if(!Z&&I.attrMap[W]){X[1]=I.attrMap[W]}if(X[2]==="~="){X[4]=" "+X[4]+" "}return X},PSEUDO:function(X,U,V,T,Y){if(X[1]==="not"){if(X[3].match(R).length>1||/^\w/.test(X[3])){X[3]=F(X[3],null,null,U)}else{var W=F.filter(X[3],U,V,true^Y);if(!V){T.push.apply(T,W)}return false}}else{if(I.match.POS.test(X[0])||I.match.CHILD.test(X[0])){return true}}return X},POS:function(T){T.unshift(true);return T}},filters:{enabled:function(T){return T.disabled===false&&T.type!=="hidden"},disabled:function(T){return T.disabled===true},checked:function(T){return T.checked===true},selected:function(T){T.parentNode.selectedIndex;return T.selected===true},parent:function(T){return !!T.firstChild},empty:function(T){return !T.firstChild},has:function(V,U,T){return !!F(T[3],V).length},header:function(T){return/h\d/i.test(T.nodeName)},text:function(T){return"text"===T.type},radio:function(T){return"radio"===T.type},checkbox:function(T){return"checkbox"===T.type},file:function(T){return"file"===T.type},password:function(T){return"password"===T.type},submit:function(T){return"submit"===T.type},image:function(T){return"image"===T.type},reset:function(T){return"reset"===T.type},button:function(T){return"button"===T.type||T.nodeName.toUpperCase()==="BUTTON"},input:function(T){return/input|select|textarea|button/i.test(T.nodeName)}},setFilters:{first:function(U,T){return T===0},last:function(V,U,T,W){return U===W.length-1},even:function(U,T){return T%2===0},odd:function(U,T){return T%2===1},lt:function(V,U,T){return U<T[3]-0},gt:function(V,U,T){return U>T[3]-0},nth:function(V,U,T){return T[3]-0==U},eq:function(V,U,T){return T[3]-0==U}},filter:{PSEUDO:function(Z,V,W,aa){var U=V[1],X=I.filters[U];if(X){return X(Z,W,V,aa)}else{if(U==="contains"){return(Z.textContent||Z.innerText||"").indexOf(V[3])>=0}else{if(U==="not"){var Y=V[3];for(var W=0,T=Y.length;W<T;W++){if(Y[W]===Z){return false}}return true}}}},CHILD:function(T,W){var Z=W[1],U=T;switch(Z){case"only":case"first":while(U=U.previousSibling){if(U.nodeType===1){return false}}if(Z=="first"){return true}U=T;case"last":while(U=U.nextSibling){if(U.nodeType===1){return false}}return true;case"nth":var V=W[2],ac=W[3];if(V==1&&ac==0){return true}var Y=W[0],ab=T.parentNode;if(ab&&(ab.sizcache!==Y||!T.nodeIndex)){var X=0;for(U=ab.firstChild;U;U=U.nextSibling){if(U.nodeType===1){U.nodeIndex=++X}}ab.sizcache=Y}var aa=T.nodeIndex-ac;if(V==0){return aa==0}else{return(aa%V==0&&aa/V>=0)}}},ID:function(U,T){return U.nodeType===1&&U.getAttribute("id")===T},TAG:function(U,T){return(T==="*"&&U.nodeType===1)||U.nodeName===T},CLASS:function(U,T){return(" "+(U.className||U.getAttribute("class"))+" ").indexOf(T)>-1},ATTR:function(Y,W){var V=W[1],T=I.attrHandle[V]?I.attrHandle[V](Y):Y[V]!=null?Y[V]:Y.getAttribute(V),Z=T+"",X=W[2],U=W[4];return T==null?X==="!=":X==="="?Z===U:X==="*="?Z.indexOf(U)>=0:X==="~="?(" "+Z+" ").indexOf(U)>=0:!U?Z&&T!==false:X==="!="?Z!=U:X==="^="?Z.indexOf(U)===0:X==="$="?Z.substr(Z.length-U.length)===U:X==="|="?Z===U||Z.substr(0,U.length+1)===U+"-":false},POS:function(X,U,V,Y){var T=U[2],W=I.setFilters[T];if(W){return W(X,V,U,Y)}}}};var M=I.match.POS;for(var O in I.match){I.match[O]=RegExp(I.match[O].source+/(?![^\[]*\])(?![^\(]*\))/.source)}var E=function(U,T){U=Array.prototype.slice.call(U);if(T){T.push.apply(T,U);return T}return U};try{Array.prototype.slice.call(document.documentElement.childNodes)}catch(N){E=function(X,W){var U=W||[];if(H.call(X)==="[object Array]"){Array.prototype.push.apply(U,X)}else{if(typeof X.length==="number"){for(var V=0,T=X.length;V<T;V++){U.push(X[V])}}else{for(var V=0;X[V];V++){U.push(X[V])}}}return U}}var G;if(document.documentElement.compareDocumentPosition){G=function(U,T){var V=U.compareDocumentPosition(T)&4?-1:U===T?0:1;if(V===0){hasDuplicate=true}return V}}else{if("sourceIndex" in document.documentElement){G=function(U,T){var V=U.sourceIndex-T.sourceIndex;if(V===0){hasDuplicate=true}return V}}else{if(document.createRange){G=function(W,U){var V=W.ownerDocument.createRange(),T=U.ownerDocument.createRange();V.selectNode(W);V.collapse(true);T.selectNode(U);T.collapse(true);var X=V.compareBoundaryPoints(Range.START_TO_END,T);if(X===0){hasDuplicate=true}return X}}}}(function(){var U=document.createElement("form"),V="script"+(new Date).getTime();U.innerHTML="<input name='"+V+"'/>";var T=document.documentElement;T.insertBefore(U,T.firstChild);if(!!document.getElementById(V)){I.find.ID=function(X,Y,Z){if(typeof Y.getElementById!=="undefined"&&!Z){var W=Y.getElementById(X[1]);return W?W.id===X[1]||typeof W.getAttributeNode!=="undefined"&&W.getAttributeNode("id").nodeValue===X[1]?[W]:g:[]}};I.filter.ID=function(Y,W){var X=typeof Y.getAttributeNode!=="undefined"&&Y.getAttributeNode("id");return Y.nodeType===1&&X&&X.nodeValue===W}}T.removeChild(U)})();(function(){var T=document.createElement("div");T.appendChild(document.createComment(""));if(T.getElementsByTagName("*").length>0){I.find.TAG=function(U,Y){var X=Y.getElementsByTagName(U[1]);if(U[1]==="*"){var W=[];for(var V=0;X[V];V++){if(X[V].nodeType===1){W.push(X[V])}}X=W}return X}}T.innerHTML="<a href='#'></a>";if(T.firstChild&&typeof T.firstChild.getAttribute!=="undefined"&&T.firstChild.getAttribute("href")!=="#"){I.attrHandle.href=function(U){return U.getAttribute("href",2)}}})();if(document.querySelectorAll){(function(){var T=F,U=document.createElement("div");U.innerHTML="<p class='TEST'></p>";if(U.querySelectorAll&&U.querySelectorAll(".TEST").length===0){return}F=function(Y,X,V,W){X=X||document;if(!W&&X.nodeType===9&&!Q(X)){try{return E(X.querySelectorAll(Y),V)}catch(Z){}}return T(Y,X,V,W)};F.find=T.find;F.filter=T.filter;F.selectors=T.selectors;F.matches=T.matches})()}if(document.getElementsByClassName&&document.documentElement.getElementsByClassName){(function(){var T=document.createElement("div");T.innerHTML="<div class='test e'></div><div class='test'></div>";if(T.getElementsByClassName("e").length===0){return}T.lastChild.className="e";if(T.getElementsByClassName("e").length===1){return}I.order.splice(1,0,"CLASS");I.find.CLASS=function(U,V,W){if(typeof V.getElementsByClassName!=="undefined"&&!W){return V.getElementsByClassName(U[1])}}})()}function P(U,Z,Y,ad,aa,ac){var ab=U=="previousSibling"&&!ac;for(var W=0,V=ad.length;W<V;W++){var T=ad[W];if(T){if(ab&&T.nodeType===1){T.sizcache=Y;T.sizset=W}T=T[U];var X=false;while(T){if(T.sizcache===Y){X=ad[T.sizset];break}if(T.nodeType===1&&!ac){T.sizcache=Y;T.sizset=W}if(T.nodeName===Z){X=T;break}T=T[U]}ad[W]=X}}}function S(U,Z,Y,ad,aa,ac){var ab=U=="previousSibling"&&!ac;for(var W=0,V=ad.length;W<V;W++){var T=ad[W];if(T){if(ab&&T.nodeType===1){T.sizcache=Y;T.sizset=W}T=T[U];var X=false;while(T){if(T.sizcache===Y){X=ad[T.sizset];break}if(T.nodeType===1){if(!ac){T.sizcache=Y;T.sizset=W}if(typeof Z!=="string"){if(T===Z){X=true;break}}else{if(F.filter(Z,[T]).length>0){X=T;break}}}T=T[U]}ad[W]=X}}}var K=document.compareDocumentPosition?function(U,T){return U.compareDocumentPosition(T)&16}:function(U,T){return U!==T&&(U.contains?U.contains(T):true)};var Q=function(T){return T.nodeType===9&&T.documentElement.nodeName!=="HTML"||!!T.ownerDocument&&Q(T.ownerDocument)};var J=function(T,aa){var W=[],X="",Y,V=aa.nodeType?[aa]:aa;while((Y=I.match.PSEUDO.exec(T))){X+=Y[0];T=T.replace(I.match.PSEUDO,"")}T=I.relative[T]?T+"*":T;for(var Z=0,U=V.length;Z<U;Z++){F(T,V[Z],W)}return F.filter(X,W)};o.find=F;o.filter=F.filter;o.expr=F.selectors;o.expr[":"]=o.expr.filters;F.selectors.filters.hidden=function(T){return T.offsetWidth===0||T.offsetHeight===0};F.selectors.filters.visible=function(T){return T.offsetWidth>0||T.offsetHeight>0};F.selectors.filters.animated=function(T){return o.grep(o.timers,function(U){return T===U.elem}).length};o.multiFilter=function(V,T,U){if(U){V=":not("+V+")"}return F.matches(V,T)};o.dir=function(V,U){var T=[],W=V[U];while(W&&W!=document){if(W.nodeType==1){T.push(W)}W=W[U]}return T};o.nth=function(X,T,V,W){T=T||1;var U=0;for(;X;X=X[V]){if(X.nodeType==1&&++U==T){break}}return X};o.sibling=function(V,U){var T=[];for(;V;V=V.nextSibling){if(V.nodeType==1&&V!=U){T.push(V)}}return T};return;l.Sizzle=F})();o.event={add:function(I,F,H,K){if(I.nodeType==3||I.nodeType==8){return}if(I.setInterval&&I!=l){I=l}if(!H.guid){H.guid=this.guid++}if(K!==g){var G=H;H=this.proxy(G);H.data=K}var E=o.data(I,"events")||o.data(I,"events",{}),J=o.data(I,"handle")||o.data(I,"handle",function(){return typeof o!=="undefined"&&!o.event.triggered?o.event.handle.apply(arguments.callee.elem,arguments):g});J.elem=I;o.each(F.split(/\s+/),function(M,N){var O=N.split(".");N=O.shift();H.type=O.slice().sort().join(".");var L=E[N];if(o.event.specialAll[N]){o.event.specialAll[N].setup.call(I,K,O)}if(!L){L=E[N]={};if(!o.event.special[N]||o.event.special[N].setup.call(I,K,O)===false){if(I.addEventListener){I.addEventListener(N,J,false)}else{if(I.attachEvent){I.attachEvent("on"+N,J)}}}}L[H.guid]=H;o.event.global[N]=true});I=null},guid:1,global:{},remove:function(K,H,J){if(K.nodeType==3||K.nodeType==8){return}var G=o.data(K,"events"),F,E;if(G){if(H===g||(typeof H==="string"&&H.charAt(0)==".")){for(var I in G){this.remove(K,I+(H||""))}}else{if(H.type){J=H.handler;H=H.type}o.each(H.split(/\s+/),function(M,O){var Q=O.split(".");O=Q.shift();var N=RegExp("(^|\\.)"+Q.slice().sort().join(".*\\.")+"(\\.|$)");if(G[O]){if(J){delete G[O][J.guid]}else{for(var P in G[O]){if(N.test(G[O][P].type)){delete G[O][P]}}}if(o.event.specialAll[O]){o.event.specialAll[O].teardown.call(K,Q)}for(F in G[O]){break}if(!F){if(!o.event.special[O]||o.event.special[O].teardown.call(K,Q)===false){if(K.removeEventListener){K.removeEventListener(O,o.data(K,"handle"),false)}else{if(K.detachEvent){K.detachEvent("on"+O,o.data(K,"handle"))}}}F=null;delete G[O]}}})}for(F in G){break}if(!F){var L=o.data(K,"handle");if(L){L.elem=null}o.removeData(K,"events");o.removeData(K,"handle")}}},trigger:function(I,K,H,E){var G=I.type||I;if(!E){I=typeof I==="object"?I[h]?I:o.extend(o.Event(G),I):o.Event(G);if(G.indexOf("!")>=0){I.type=G=G.slice(0,-1);I.exclusive=true}if(!H){I.stopPropagation();if(this.global[G]){o.each(o.cache,function(){if(this.events&&this.events[G]){o.event.trigger(I,K,this.handle.elem)}})}}if(!H||H.nodeType==3||H.nodeType==8){return g}I.result=g;I.target=H;K=o.makeArray(K);K.unshift(I)}I.currentTarget=H;var J=o.data(H,"handle");if(J){J.apply(H,K)}if((!H[G]||(o.nodeName(H,"a")&&G=="click"))&&H["on"+G]&&H["on"+G].apply(H,K)===false){I.result=false}if(!E&&H[G]&&!I.isDefaultPrevented()&&!(o.nodeName(H,"a")&&G=="click")){this.triggered=true;try{H[G]()}catch(L){}}this.triggered=false;if(!I.isPropagationStopped()){var F=H.parentNode||H.ownerDocument;if(F){o.event.trigger(I,K,F,true)}}},handle:function(K){var J,E;K=arguments[0]=o.event.fix(K||l.event);K.currentTarget=this;var L=K.type.split(".");K.type=L.shift();J=!L.length&&!K.exclusive;var I=RegExp("(^|\\.)"+L.slice().sort().join(".*\\.")+"(\\.|$)");E=(o.data(this,"events")||{})[K.type];for(var G in E){var H=E[G];if(J||I.test(H.type)){K.handler=H;K.data=H.data;var F=H.apply(this,arguments);if(F!==g){K.result=F;if(F===false){K.preventDefault();K.stopPropagation()}}if(K.isImmediatePropagationStopped()){break}}}},props:"altKey attrChange attrName bubbles button cancelable charCode clientX clientY ctrlKey currentTarget data detail eventPhase fromElement handler keyCode metaKey newValue originalTarget pageX pageY prevValue relatedNode relatedTarget screenX screenY shiftKey srcElement target toElement view wheelDelta which".split(" "),fix:function(H){if(H[h]){return H}var F=H;H=o.Event(F);for(var G=this.props.length,J;G;){J=this.props[--G];H[J]=F[J]}if(!H.target){H.target=H.srcElement||document}if(H.target.nodeType==3){H.target=H.target.parentNode}if(!H.relatedTarget&&H.fromElement){H.relatedTarget=H.fromElement==H.target?H.toElement:H.fromElement}if(H.pageX==null&&H.clientX!=null){var I=document.documentElement,E=document.body;H.pageX=H.clientX+(I&&I.scrollLeft||E&&E.scrollLeft||0)-(I.clientLeft||0);H.pageY=H.clientY+(I&&I.scrollTop||E&&E.scrollTop||0)-(I.clientTop||0)}if(!H.which&&((H.charCode||H.charCode===0)?H.charCode:H.keyCode)){H.which=H.charCode||H.keyCode}if(!H.metaKey&&H.ctrlKey){H.metaKey=H.ctrlKey}if(!H.which&&H.button){H.which=(H.button&1?1:(H.button&2?3:(H.button&4?2:0)))}return H},proxy:function(F,E){E=E||function(){return F.apply(this,arguments)};E.guid=F.guid=F.guid||E.guid||this.guid++;return E},special:{ready:{setup:B,teardown:function(){}}},specialAll:{live:{setup:function(E,F){o.event.add(this,F[0],c)},teardown:function(G){if(G.length){var E=0,F=RegExp("(^|\\.)"+G[0]+"(\\.|$)");o.each((o.data(this,"events").live||{}),function(){if(F.test(this.type)){E++}});if(E<1){o.event.remove(this,G[0],c)}}}}}};o.Event=function(E){if(!this.preventDefault){return new o.Event(E)}if(E&&E.type){this.originalEvent=E;this.type=E.type}else{this.type=E}this.timeStamp=e();this[h]=true};function k(){return false}function u(){return true}o.Event.prototype={preventDefault:function(){this.isDefaultPrevented=u;var E=this.originalEvent;if(!E){return}if(E.preventDefault){E.preventDefault()}E.returnValue=false},stopPropagation:function(){this.isPropagationStopped=u;var E=this.originalEvent;if(!E){return}if(E.stopPropagation){E.stopPropagation()}E.cancelBubble=true},stopImmediatePropagation:function(){this.isImmediatePropagationStopped=u;this.stopPropagation()},isDefaultPrevented:k,isPropagationStopped:k,isImmediatePropagationStopped:k};var a=function(F){var E=F.relatedTarget;while(E&&E!=this){try{E=E.parentNode}catch(G){E=this}}if(E!=this){F.type=F.data;o.event.handle.apply(this,arguments)}};o.each({mouseover:"mouseenter",mouseout:"mouseleave"},function(F,E){o.event.special[E]={setup:function(){o.event.add(this,F,a,E)},teardown:function(){o.event.remove(this,F,a)}}});o.fn.extend({bind:function(F,G,E){return F=="unload"?this.one(F,G,E):this.each(function(){o.event.add(this,F,E||G,E&&G)})},one:function(G,H,F){var E=o.event.proxy(F||H,function(I){o(this).unbind(I,E);return(F||H).apply(this,arguments)});return this.each(function(){o.event.add(this,G,E,F&&H)})},unbind:function(F,E){return this.each(function(){o.event.remove(this,F,E)})},trigger:function(E,F){return this.each(function(){o.event.trigger(E,F,this)})},triggerHandler:function(E,G){if(this[0]){var F=o.Event(E);F.preventDefault();F.stopPropagation();o.event.trigger(F,G,this[0]);return F.result}},toggle:function(G){var E=arguments,F=1;while(F<E.length){o.event.proxy(G,E[F++])}return this.click(o.event.proxy(G,function(H){this.lastToggle=(this.lastToggle||0)%F;H.preventDefault();return E[this.lastToggle++].apply(this,arguments)||false}))},hover:function(E,F){return this.mouseenter(E).mouseleave(F)},ready:function(E){B();if(o.isReady){E.call(document,o)}else{o.readyList.push(E)}return this},live:function(G,F){var E=o.event.proxy(F);E.guid+=this.selector+G;o(document).bind(i(G,this.selector),this.selector,E);return this},die:function(F,E){o(document).unbind(i(F,this.selector),E?{guid:E.guid+this.selector+F}:null);return this}});function c(H){var E=RegExp("(^|\\.)"+H.type+"(\\.|$)"),G=true,F=[];o.each(o.data(this,"events").live||[],function(I,J){if(E.test(J.type)){var K=o(H.target).closest(J.data)[0];if(K){F.push({elem:K,fn:J})}}});F.sort(function(J,I){return o.data(J.elem,"closest")-o.data(I.elem,"closest")});o.each(F,function(){if(this.fn.call(this.elem,H,this.fn.data)===false){return(G=false)}});return G}function i(F,E){return["live",F,E.replace(/\./g,"`").replace(/ /g,"|")].join(".")}o.extend({isReady:false,readyList:[],ready:function(){if(!o.isReady){o.isReady=true;if(o.readyList){o.each(o.readyList,function(){this.call(document,o)});o.readyList=null}o(document).triggerHandler("ready")}}});var x=false;function B(){if(x){return}x=true;if(document.addEventListener){document.addEventListener("DOMContentLoaded",function(){document.removeEventListener("DOMContentLoaded",arguments.callee,false);o.ready()},false)}else{if(document.attachEvent){document.attachEvent("onreadystatechange",function(){if(document.readyState==="complete"){document.detachEvent("onreadystatechange",arguments.callee);o.ready()}});if(document.documentElement.doScroll&&l==l.top){(function(){if(o.isReady){return}try{document.documentElement.doScroll("left")}catch(E){setTimeout(arguments.callee,0);return}o.ready()})()}}}o.event.add(l,"load",o.ready)}o.each(("blur,focus,load,resize,scroll,unload,click,dblclick,mousedown,mouseup,mousemove,mouseover,mouseout,mouseenter,mouseleave,change,select,submit,keydown,keypress,keyup,error").split(","),function(F,E){o.fn[E]=function(G){return G?this.bind(E,G):this.trigger(E)}});o(l).bind("unload",function(){for(var E in o.cache){if(E!=1&&o.cache[E].handle){o.event.remove(o.cache[E].handle.elem)}}});(function(){o.support={};var F=document.documentElement,G=document.createElement("script"),K=document.createElement("div"),J="script"+(new Date).getTime();K.style.display="none";K.innerHTML='   <link/><table></table><a href="/a" style="color:red;float:left;opacity:.5;">a</a><select><option>text</option></select><object><param/></object>';var H=K.getElementsByTagName("*"),E=K.getElementsByTagName("a")[0];if(!H||!H.length||!E){return}o.support={leadingWhitespace:K.firstChild.nodeType==3,tbody:!K.getElementsByTagName("tbody").length,objectAll:!!K.getElementsByTagName("object")[0].getElementsByTagName("*").length,htmlSerialize:!!K.getElementsByTagName("link").length,style:/red/.test(E.getAttribute("style")),hrefNormalized:E.getAttribute("href")==="/a",opacity:E.style.opacity==="0.5",cssFloat:!!E.style.cssFloat,scriptEval:false,noCloneEvent:true,boxModel:null};G.type="text/javascript";try{G.appendChild(document.createTextNode("window."+J+"=1;"))}catch(I){}F.insertBefore(G,F.firstChild);if(l[J]){o.support.scriptEval=true;delete l[J]}F.removeChild(G);if(K.attachEvent&&K.fireEvent){K.attachEvent("onclick",function(){o.support.noCloneEvent=false;K.detachEvent("onclick",arguments.callee)});K.cloneNode(true).fireEvent("onclick")}o(function(){var L=document.createElement("div");L.style.width=L.style.paddingLeft="1px";document.body.appendChild(L);o.boxModel=o.support.boxModel=L.offsetWidth===2;document.body.removeChild(L).style.display="none"})})();var w=o.support.cssFloat?"cssFloat":"styleFloat";o.props={"for":"htmlFor","class":"className","float":w,cssFloat:w,styleFloat:w,readonly:"readOnly",maxlength:"maxLength",cellspacing:"cellSpacing",rowspan:"rowSpan",tabindex:"tabIndex"};o.fn.extend({_load:o.fn.load,load:function(G,J,K){if(typeof G!=="string"){return this._load(G)}var I=G.indexOf(" ");if(I>=0){var E=G.slice(I,G.length);G=G.slice(0,I)}var H="GET";if(J){if(o.isFunction(J)){K=J;J=null}else{if(typeof J==="object"){J=o.param(J);H="POST"}}}var F=this;o.ajax({url:G,type:H,dataType:"html",data:J,complete:function(M,L){if(L=="success"||L=="notmodified"){F.html(E?o("<div/>").append(M.responseText.replace(/<script(.|\s)*?\/script>/g,"")).find(E):M.responseText)}if(K){F.each(K,[M.responseText,L,M])}}});return this},serialize:function(){return o.param(this.serializeArray())},serializeArray:function(){return this.map(function(){return this.elements?o.makeArray(this.elements):this}).filter(function(){return this.name&&!this.disabled&&(this.checked||/select|textarea/i.test(this.nodeName)||/text|hidden|password|search/i.test(this.type))}).map(function(E,F){var G=o(this).val();return G==null?null:o.isArray(G)?o.map(G,function(I,H){return{name:F.name,value:I}}):{name:F.name,value:G}}).get()}});o.each("ajaxStart,ajaxStop,ajaxComplete,ajaxError,ajaxSuccess,ajaxSend".split(","),function(E,F){o.fn[F]=function(G){return this.bind(F,G)}});var r=e();o.extend({get:function(E,G,H,F){if(o.isFunction(G)){H=G;G=null}return o.ajax({type:"GET",url:E,data:G,success:H,dataType:F})},getScript:function(E,F){return o.get(E,null,F,"script")},getJSON:function(E,F,G){return o.get(E,F,G,"json")},post:function(E,G,H,F){if(o.isFunction(G)){H=G;G={}}return o.ajax({type:"POST",url:E,data:G,success:H,dataType:F})},ajaxSetup:function(E){o.extend(o.ajaxSettings,E)},ajaxSettings:{url:location.href,global:true,type:"GET",contentType:"application/x-www-form-urlencoded",processData:true,async:true,xhr:function(){return l.ActiveXObject?new ActiveXObject("Microsoft.XMLHTTP"):new XMLHttpRequest()},accepts:{xml:"application/xml, text/xml",html:"text/html",script:"text/javascript, application/javascript",json:"application/json, text/javascript",text:"text/plain",_default:"*/*"}},lastModified:{},ajax:function(M){M=o.extend(true,M,o.extend(true,{},o.ajaxSettings,M));var W,F=/=\?(&|$)/g,R,V,G=M.type.toUpperCase();if(M.data&&M.processData&&typeof M.data!=="string"){M.data=o.param(M.data)}if(M.dataType=="jsonp"){if(G=="GET"){if(!M.url.match(F)){M.url+=(M.url.match(/\?/)?"&":"?")+(M.jsonp||"callback")+"=?"}}else{if(!M.data||!M.data.match(F)){M.data=(M.data?M.data+"&":"")+(M.jsonp||"callback")+"=?"}}M.dataType="json"}if(M.dataType=="json"&&(M.data&&M.data.match(F)||M.url.match(F))){W="jsonp"+r++;if(M.data){M.data=(M.data+"").replace(F,"="+W+"$1")}M.url=M.url.replace(F,"="+W+"$1");M.dataType="script";l[W]=function(X){V=X;I();L();l[W]=g;try{delete l[W]}catch(Y){}if(H){H.removeChild(T)}}}if(M.dataType=="script"&&M.cache==null){M.cache=false}if(M.cache===false&&G=="GET"){var E=e();var U=M.url.replace(/(\?|&)_=.*?(&|$)/,"$1_="+E+"$2");M.url=U+((U==M.url)?(M.url.match(/\?/)?"&":"?")+"_="+E:"")}if(M.data&&G=="GET"){M.url+=(M.url.match(/\?/)?"&":"?")+M.data;M.data=null}if(M.global&&!o.active++){o.event.trigger("ajaxStart")}var Q=/^(\w+:)?\/\/([^\/?#]+)/.exec(M.url);if(M.dataType=="script"&&G=="GET"&&Q&&(Q[1]&&Q[1]!=location.protocol||Q[2]!=location.host)){var H=document.getElementsByTagName("head")[0];var T=document.createElement("script");T.src=M.url;if(M.scriptCharset){T.charset=M.scriptCharset}if(!W){var O=false;T.onload=T.onreadystatechange=function(){if(!O&&(!this.readyState||this.readyState=="loaded"||this.readyState=="complete")){O=true;I();L();T.onload=T.onreadystatechange=null;H.removeChild(T)}}}H.appendChild(T);return g}var K=false;var J=M.xhr();if(M.username){J.open(G,M.url,M.async,M.username,M.password)}else{J.open(G,M.url,M.async)}try{if(M.data){J.setRequestHeader("Content-Type",M.contentType)}if(M.ifModified){J.setRequestHeader("If-Modified-Since",o.lastModified[M.url]||"Thu, 01 Jan 1970 00:00:00 GMT")}J.setRequestHeader("X-Requested-With","XMLHttpRequest");J.setRequestHeader("Accept",M.dataType&&M.accepts[M.dataType]?M.accepts[M.dataType]+", */*":M.accepts._default)}catch(S){}if(M.beforeSend&&M.beforeSend(J,M)===false){if(M.global&&!--o.active){o.event.trigger("ajaxStop")}J.abort();return false}if(M.global){o.event.trigger("ajaxSend",[J,M])}var N=function(X){if(J.readyState==0){if(P){clearInterval(P);P=null;if(M.global&&!--o.active){o.event.trigger("ajaxStop")}}}else{if(!K&&J&&(J.readyState==4||X=="timeout")){K=true;if(P){clearInterval(P);P=null}R=X=="timeout"?"timeout":!o.httpSuccess(J)?"error":M.ifModified&&o.httpNotModified(J,M.url)?"notmodified":"success";if(R=="success"){try{V=o.httpData(J,M.dataType,M)}catch(Z){R="parsererror"}}if(R=="success"){var Y;try{Y=J.getResponseHeader("Last-Modified")}catch(Z){}if(M.ifModified&&Y){o.lastModified[M.url]=Y}if(!W){I()}}else{o.handleError(M,J,R)}L();if(X){J.abort()}if(M.async){J=null}}}};if(M.async){var P=setInterval(N,13);if(M.timeout>0){setTimeout(function(){if(J&&!K){N("timeout")}},M.timeout)}}try{J.send(M.data)}catch(S){o.handleError(M,J,null,S)}if(!M.async){N()}function I(){if(M.success){M.success(V,R)}if(M.global){o.event.trigger("ajaxSuccess",[J,M])}}function L(){if(M.complete){M.complete(J,R)}if(M.global){o.event.trigger("ajaxComplete",[J,M])}if(M.global&&!--o.active){o.event.trigger("ajaxStop")}}return J},handleError:function(F,H,E,G){if(F.error){F.error(H,E,G)}if(F.global){o.event.trigger("ajaxError",[H,F,G])}},active:0,httpSuccess:function(F){try{return !F.status&&location.protocol=="file:"||(F.status>=200&&F.status<300)||F.status==304||F.status==1223}catch(E){}return false},httpNotModified:function(G,E){try{var H=G.getResponseHeader("Last-Modified");return G.status==304||H==o.lastModified[E]}catch(F){}return false},httpData:function(J,H,G){var F=J.getResponseHeader("content-type"),E=H=="xml"||!H&&F&&F.indexOf("xml")>=0,I=E?J.responseXML:J.responseText;if(E&&I.documentElement.tagName=="parsererror"){throw"parsererror"}if(G&&G.dataFilter){I=G.dataFilter(I,H)}if(typeof I==="string"){if(H=="script"){o.globalEval(I)}if(H=="json"){I=l["eval"]("("+I+")")}}return I},param:function(E){var G=[];function H(I,J){G[G.length]=encodeURIComponent(I)+"="+encodeURIComponent(J)}if(o.isArray(E)||E.jquery){o.each(E,function(){H(this.name,this.value)})}else{for(var F in E){if(o.isArray(E[F])){o.each(E[F],function(){H(F,this)})}else{H(F,o.isFunction(E[F])?E[F]():E[F])}}}return G.join("&").replace(/%20/g,"+")}});var m={},n,d=[["height","marginTop","marginBottom","paddingTop","paddingBottom"],["width","marginLeft","marginRight","paddingLeft","paddingRight"],["opacity"]];function t(F,E){var G={};o.each(d.concat.apply([],d.slice(0,E)),function(){G[this]=F});return G}o.fn.extend({show:function(J,L){if(J){return this.animate(t("show",3),J,L)}else{for(var H=0,F=this.length;H<F;H++){var E=o.data(this[H],"olddisplay");this[H].style.display=E||"";if(o.css(this[H],"display")==="none"){var G=this[H].tagName,K;if(m[G]){K=m[G]}else{var I=o("<"+G+" />").appendTo("body");K=I.css("display");if(K==="none"){K="block"}I.remove();m[G]=K}o.data(this[H],"olddisplay",K)}}for(var H=0,F=this.length;H<F;H++){this[H].style.display=o.data(this[H],"olddisplay")||""}return this}},hide:function(H,I){if(H){return this.animate(t("hide",3),H,I)}else{for(var G=0,F=this.length;G<F;G++){var E=o.data(this[G],"olddisplay");if(!E&&E!=="none"){o.data(this[G],"olddisplay",o.css(this[G],"display"))}}for(var G=0,F=this.length;G<F;G++){this[G].style.display="none"}return this}},_toggle:o.fn.toggle,toggle:function(G,F){var E=typeof G==="boolean";return o.isFunction(G)&&o.isFunction(F)?this._toggle.apply(this,arguments):G==null||E?this.each(function(){var H=E?G:o(this).is(":hidden");o(this)[H?"show":"hide"]()}):this.animate(t("toggle",3),G,F)},fadeTo:function(E,G,F){return this.animate({opacity:G},E,F)},animate:function(I,F,H,G){var E=o.speed(F,H,G);return this[E.queue===false?"each":"queue"](function(){var K=o.extend({},E),M,L=this.nodeType==1&&o(this).is(":hidden"),J=this;for(M in I){if(I[M]=="hide"&&L||I[M]=="show"&&!L){return K.complete.call(this)}if((M=="height"||M=="width")&&this.style){K.display=o.css(this,"display");K.overflow=this.style.overflow}}if(K.overflow!=null){this.style.overflow="hidden"}K.curAnim=o.extend({},I);o.each(I,function(O,S){var R=new o.fx(J,K,O);if(/toggle|show|hide/.test(S)){R[S=="toggle"?L?"show":"hide":S](I)}else{var Q=S.toString().match(/^([+-]=)?([\d+-.]+)(.*)$/),T=R.cur(true)||0;if(Q){var N=parseFloat(Q[2]),P=Q[3]||"px";if(P!="px"){J.style[O]=(N||1)+P;T=((N||1)/R.cur(true))*T;J.style[O]=T+P}if(Q[1]){N=((Q[1]=="-="?-1:1)*N)+T}R.custom(T,N,P)}else{R.custom(T,S,"")}}});return true})},stop:function(F,E){var G=o.timers;if(F){this.queue([])}this.each(function(){for(var H=G.length-1;H>=0;H--){if(G[H].elem==this){if(E){G[H](true)}G.splice(H,1)}}});if(!E){this.dequeue()}return this}});o.each({slideDown:t("show",1),slideUp:t("hide",1),slideToggle:t("toggle",1),fadeIn:{opacity:"show"},fadeOut:{opacity:"hide"}},function(E,F){o.fn[E]=function(G,H){return this.animate(F,G,H)}});o.extend({speed:function(G,H,F){var E=typeof G==="object"?G:{complete:F||!F&&H||o.isFunction(G)&&G,duration:G,easing:F&&H||H&&!o.isFunction(H)&&H};E.duration=o.fx.off?0:typeof E.duration==="number"?E.duration:o.fx.speeds[E.duration]||o.fx.speeds._default;E.old=E.complete;E.complete=function(){if(E.queue!==false){o(this).dequeue()}if(o.isFunction(E.old)){E.old.call(this)}};return E},easing:{linear:function(G,H,E,F){return E+F*G},swing:function(G,H,E,F){return((-Math.cos(G*Math.PI)/2)+0.5)*F+E}},timers:[],fx:function(F,E,G){this.options=E;this.elem=F;this.prop=G;if(!E.orig){E.orig={}}}});o.fx.prototype={update:function(){if(this.options.step){this.options.step.call(this.elem,this.now,this)}(o.fx.step[this.prop]||o.fx.step._default)(this);if((this.prop=="height"||this.prop=="width")&&this.elem.style){this.elem.style.display="block"}},cur:function(F){if(this.elem[this.prop]!=null&&(!this.elem.style||this.elem.style[this.prop]==null)){return this.elem[this.prop]}var E=parseFloat(o.css(this.elem,this.prop,F));return E&&E>-10000?E:parseFloat(o.curCSS(this.elem,this.prop))||0},custom:function(I,H,G){this.startTime=e();this.start=I;this.end=H;this.unit=G||this.unit||"px";this.now=this.start;this.pos=this.state=0;var E=this;function F(J){return E.step(J)}F.elem=this.elem;if(F()&&o.timers.push(F)&&!n){n=setInterval(function(){var K=o.timers;for(var J=0;J<K.length;J++){if(!K[J]()){K.splice(J--,1)}}if(!K.length){clearInterval(n);n=g}},13)}},show:function(){this.options.orig[this.prop]=o.attr(this.elem.style,this.prop);this.options.show=true;this.custom(this.prop=="width"||this.prop=="height"?1:0,this.cur());o(this.elem).show()},hide:function(){this.options.orig[this.prop]=o.attr(this.elem.style,this.prop);this.options.hide=true;this.custom(this.cur(),0)},step:function(H){var G=e();if(H||G>=this.options.duration+this.startTime){this.now=this.end;this.pos=this.state=1;this.update();this.options.curAnim[this.prop]=true;var E=true;for(var F in this.options.curAnim){if(this.options.curAnim[F]!==true){E=false}}if(E){if(this.options.display!=null){this.elem.style.overflow=this.options.overflow;this.elem.style.display=this.options.display;if(o.css(this.elem,"display")=="none"){this.elem.style.display="block"}}if(this.options.hide){o(this.elem).hide()}if(this.options.hide||this.options.show){for(var I in this.options.curAnim){o.attr(this.elem.style,I,this.options.orig[I])}}this.options.complete.call(this.elem)}return false}else{var J=G-this.startTime;this.state=J/this.options.duration;this.pos=o.easing[this.options.easing||(o.easing.swing?"swing":"linear")](this.state,J,0,1,this.options.duration);this.now=this.start+((this.end-this.start)*this.pos);this.update()}return true}};o.extend(o.fx,{speeds:{slow:600,fast:200,_default:400},step:{opacity:function(E){o.attr(E.elem.style,"opacity",E.now)},_default:function(E){if(E.elem.style&&E.elem.style[E.prop]!=null){E.elem.style[E.prop]=E.now+E.unit}else{E.elem[E.prop]=E.now}}}});if(document.documentElement.getBoundingClientRect){o.fn.offset=function(){if(!this[0]){return{top:0,left:0}}if(this[0]===this[0].ownerDocument.body){return o.offset.bodyOffset(this[0])}var G=this[0].getBoundingClientRect(),J=this[0].ownerDocument,F=J.body,E=J.documentElement,L=E.clientTop||F.clientTop||0,K=E.clientLeft||F.clientLeft||0,I=G.top+(self.pageYOffset||o.boxModel&&E.scrollTop||F.scrollTop)-L,H=G.left+(self.pageXOffset||o.boxModel&&E.scrollLeft||F.scrollLeft)-K;return{top:I,left:H}}}else{o.fn.offset=function(){if(!this[0]){return{top:0,left:0}}if(this[0]===this[0].ownerDocument.body){return o.offset.bodyOffset(this[0])}o.offset.initialized||o.offset.initialize();var J=this[0],G=J.offsetParent,F=J,O=J.ownerDocument,M,H=O.documentElement,K=O.body,L=O.defaultView,E=L.getComputedStyle(J,null),N=J.offsetTop,I=J.offsetLeft;while((J=J.parentNode)&&J!==K&&J!==H){M=L.getComputedStyle(J,null);N-=J.scrollTop,I-=J.scrollLeft;if(J===G){N+=J.offsetTop,I+=J.offsetLeft;if(o.offset.doesNotAddBorder&&!(o.offset.doesAddBorderForTableAndCells&&/^t(able|d|h)$/i.test(J.tagName))){N+=parseInt(M.borderTopWidth,10)||0,I+=parseInt(M.borderLeftWidth,10)||0}F=G,G=J.offsetParent}if(o.offset.subtractsBorderForOverflowNotVisible&&M.overflow!=="visible"){N+=parseInt(M.borderTopWidth,10)||0,I+=parseInt(M.borderLeftWidth,10)||0}E=M}if(E.position==="relative"||E.position==="static"){N+=K.offsetTop,I+=K.offsetLeft}if(E.position==="fixed"){N+=Math.max(H.scrollTop,K.scrollTop),I+=Math.max(H.scrollLeft,K.scrollLeft)}return{top:N,left:I}}}o.offset={initialize:function(){if(this.initialized){return}var L=document.body,F=document.createElement("div"),H,G,N,I,M,E,J=L.style.marginTop,K='<div style="position:absolute;top:0;left:0;margin:0;border:5px solid #000;padding:0;width:1px;height:1px;"><div></div></div><table style="position:absolute;top:0;left:0;margin:0;border:5px solid #000;padding:0;width:1px;height:1px;" cellpadding="0" cellspacing="0"><tr><td></td></tr></table>';M={position:"absolute",top:0,left:0,margin:0,border:0,width:"1px",height:"1px",visibility:"hidden"};for(E in M){F.style[E]=M[E]}F.innerHTML=K;L.insertBefore(F,L.firstChild);H=F.firstChild,G=H.firstChild,I=H.nextSibling.firstChild.firstChild;this.doesNotAddBorder=(G.offsetTop!==5);this.doesAddBorderForTableAndCells=(I.offsetTop===5);H.style.overflow="hidden",H.style.position="relative";this.subtractsBorderForOverflowNotVisible=(G.offsetTop===-5);L.style.marginTop="1px";this.doesNotIncludeMarginInBodyOffset=(L.offsetTop===0);L.style.marginTop=J;L.removeChild(F);this.initialized=true},bodyOffset:function(E){o.offset.initialized||o.offset.initialize();var G=E.offsetTop,F=E.offsetLeft;if(o.offset.doesNotIncludeMarginInBodyOffset){G+=parseInt(o.curCSS(E,"marginTop",true),10)||0,F+=parseInt(o.curCSS(E,"marginLeft",true),10)||0}return{top:G,left:F}}};o.fn.extend({position:function(){var I=0,H=0,F;if(this[0]){var G=this.offsetParent(),J=this.offset(),E=/^body|html$/i.test(G[0].tagName)?{top:0,left:0}:G.offset();J.top-=j(this,"marginTop");J.left-=j(this,"marginLeft");E.top+=j(G,"borderTopWidth");E.left+=j(G,"borderLeftWidth");F={top:J.top-E.top,left:J.left-E.left}}return F},offsetParent:function(){var E=this[0].offsetParent||document.body;while(E&&(!/^body|html$/i.test(E.tagName)&&o.css(E,"position")=="static")){E=E.offsetParent}return o(E)}});o.each(["Left","Top"],function(F,E){var G="scroll"+E;o.fn[G]=function(H){if(!this[0]){return null}return H!==g?this.each(function(){this==l||this==document?l.scrollTo(!F?H:o(l).scrollLeft(),F?H:o(l).scrollTop()):this[G]=H}):this[0]==l||this[0]==document?self[F?"pageYOffset":"pageXOffset"]||o.boxModel&&document.documentElement[G]||document.body[G]:this[0][G]}});o.each(["Height","Width"],function(I,G){var E=I?"Left":"Top",H=I?"Right":"Bottom",F=G.toLowerCase();o.fn["inner"+G]=function(){return this[0]?o.css(this[0],F,false,"padding"):null};o.fn["outer"+G]=function(K){return this[0]?o.css(this[0],F,false,K?"margin":"border"):null};var J=G.toLowerCase();o.fn[J]=function(K){return this[0]==l?document.compatMode=="CSS1Compat"&&document.documentElement["client"+G]||document.body["client"+G]:this[0]==document?Math.max(document.documentElement["client"+G],document.body["scroll"+G],document.documentElement["scroll"+G],document.body["offset"+G],document.documentElement["offset"+G]):K===g?(this.length?o.css(this[0],J):null):this.css(J,typeof K==="string"?K:K+"px")}})})();(function(jQuery) {

    jQuery.fn.autocomplete = function() {
        return this.each(function() {
            new jQuery.autocomplete(this);
        });
    }

    jQuery.autocomplete = function(element) {
        this.input = jQuery(element);
        this.results = jQuery('<div style="position:absolute;z-index:100;display:none;width:200px;"></div>').addClass('AutoCompletePanel').hide();
        this.isVisible = false;
        this.selectedIndex = -1;
        this.data = [];
        this.init();
    };

    jQuery.autocomplete.fn = jQuery.autocomplete.prototype = {};
    jQuery.autocomplete.fn.extend = jQuery.autocomplete.extend = jQuery.extend;
    jQuery.autocomplete.fn.extend({
        init: function() {
            var self = this;
            this.input
                .bind('keyup', function(e) { self.handleKeyUp(e) })
                .bind('keydown', function(e) { self.handleKeyDown(e) })
                .bind('keypress', function(e) { self.handleKeyPress(e) })
                .bind('blur', function(e) { self.handleBlur(e) });

            jQuery(window).bind('resize', function() { self.reposition() });

            this.results.appendTo('body');
        },

        handleKeyUp: function(e) {
            if (!e) e = window.event;
            switch (e.keyCode) {
                case 38:
                    this.handleUpArrowKey();
                    return;
                case 40:
                    this.handleDownArrowKey();
                    return;
                case 13:
                    return;
            }
            if (e.keyCode == 9 || this.input.val().length == 0) this.reset();
            else {
                var self = this;
                $.ajax({
                    type: "POST",
                    url: "/Services/TaggingService.asmx/GetTagAutoCompleteInfo",
                    data: "{'prefixText': '" + this.input.val() + "', 'count': '10'}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function(result) {
                        self.dataCallback(result.d, self);
                    }
                });
            }
        },

        handleKeyDown: function(e) {
            if (!e) e = window.event;
            switch (e.keyCode) {
                case 9:
                    if (this.selectedIndex != -1) {
                        this.handleTabKey();
                        return false;
                    }
                case 13:
                    if (this.isVisible && this.selectedIndex > -1) {
                        this.handleEnterKey();
                        this.preventFurtherAction(e);
                    }
            }
            return true;
        },

        handleKeyPress: function(e) {
            if (!e) e = window.event;
            if (e.keyCode == 13 && this.isVisible) {
                return this.preventFurtherAction(e);
            }
        },

        handleUpArrowKey: function() {
            if (this.selectedIndex > 0) this.selectedIndex--;
            this.updateSelectedStyles();
        },

        handleDownArrowKey: function() {
            if (this.selectedIndex < this.results.children().length - 1) this.selectedIndex++;
            this.updateSelectedStyles();
        },

        handleTabKey: function() {
            if (this.selectedIndex != -1) this.input.val(this.data[this.selectedIndex].TagName);
            this.input.focus();
            this.reset();
        },

        handleBlur: function(e) {
            this.reset();
        },

        handleEnterKey: function() {
            this.selectTag(jQuery.browser.mozilla);
        },

        handleMouseOver: function(e) {
            var row = jQuery(e.target);
            if (!row.hasClass('Row')) row = row.parent();
            e.data.selectedIndex = row.attr('index');
            e.data.updateSelectedStyles();
        },

        handleMouseDown: function(e) {
            var row = jQuery(e.target);
            if (!row.hasClass('Row')) row = row.parent();
            e.data.selectTag(false);
        },

        reset: function() {
            this.data = [];
            this.selectedIndex = -1;
            this.results.hide();
            this.isVisible = false;
        },

        dataCallback: function(result, control) {
            control.data = result ? result : [];
            if (control.data.length > 0) {
                control.dataBind();
                control.reposition();
                control.results.show();
                control.isVisible = true;
            } else control.reset();
        },

        dataBind: function() {
            this.results.empty();
            if (this.data.length > 0) {
                for (var i = 0; i < this.data.length; i++) {
                    var dataItem = this.data[i];
                    var popularity = dataItem.Popularity;
                    popularity += (popularity > 1) ? " usages" : " usage";

                    var itemDiv = this._bindMouseEvents(jQuery('<div></div>').addClass('Row').attr('index', i).appendTo(this.results));
                    var popularityDiv = this._bindMouseEvents(jQuery('<div></div>').text(popularity).css('float', 'right').appendTo(itemDiv));
                    var tagNameDiv = this._bindMouseEvents(jQuery('<span></span>').text(dataItem.TagName).appendTo(itemDiv));
                }
            }
        },

        _bindMouseEvents: function(el) {
            var self = this;
            return el.bind('mouseover', self, self.handleMouseOver).bind('mousedown', self, self.handleMouseDown);
        },

        selectTag: function(skipReset) {
            if (this.data == null) return;
            var item = this.data[this.selectedIndex];
            if (item) {
                this.input.val(item.TagName);
                if (!skipReset) this.reset();
                this.input.focus();
            }
        },

        updateSelectedStyles: function() {
            var self = this;
            this.results.children().each(function() {
                if ($(this).attr('index') == self.selectedIndex) $(this).addClass('Selected');
                else $(this).removeClass('Selected');
            });
        },

        preventFurtherAction: function(e) {
            var evt = e.rawEvent ? e.rawEvent : e;
            if (evt.preventDefault) evt.preventDefault();
            if (evt.stopPropagation) evt.stopPropagation();
            this.reset();
            return false;
        },

        encodeHTML: function(value, isAttribute) {
            var babel = document.createElement("div");
            babel.appendChild(document.createTextNode(value));
            return (isAttribute ? babel.innerHTML.split('"').join("&quot;") : babel.innerHTML).split(' ').join("&nbsp;");
        },

        reposition: function() {
            var position = this.input.position();
            this.results.css('left', position.left).css('top', position.top + this.input.height() + 5);
        }
    });
})(jQuery);/*
 * jQuery UI 1.7.1
 *
 * Copyright (c) 2009 AUTHORS.txt (http://jqueryui.com/about)
 * Dual licensed under the MIT (MIT-LICENSE.txt)
 * and GPL (GPL-LICENSE.txt) licenses.
 *
 * http://docs.jquery.com/UI
 */jQuery.ui||(function(c){var i=c.fn.remove,d=c.browser.mozilla&&(parseFloat(c.browser.version)<1.9);c.ui={version:"1.7.1",plugin:{add:function(k,l,n){var m=c.ui[k].prototype;for(var j in n){m.plugins[j]=m.plugins[j]||[];m.plugins[j].push([l,n[j]])}},call:function(j,l,k){var n=j.plugins[l];if(!n||!j.element[0].parentNode){return}for(var m=0;m<n.length;m++){if(j.options[n[m][0]]){n[m][1].apply(j.element,k)}}}},contains:function(k,j){return document.compareDocumentPosition?k.compareDocumentPosition(j)&16:k!==j&&k.contains(j)},hasScroll:function(m,k){if(c(m).css("overflow")=="hidden"){return false}var j=(k&&k=="left")?"scrollLeft":"scrollTop",l=false;if(m[j]>0){return true}m[j]=1;l=(m[j]>0);m[j]=0;return l},isOverAxis:function(k,j,l){return(k>j)&&(k<(j+l))},isOver:function(o,k,n,m,j,l){return c.ui.isOverAxis(o,n,j)&&c.ui.isOverAxis(k,m,l)},keyCode:{BACKSPACE:8,CAPS_LOCK:20,COMMA:188,CONTROL:17,DELETE:46,DOWN:40,END:35,ENTER:13,ESCAPE:27,HOME:36,INSERT:45,LEFT:37,NUMPAD_ADD:107,NUMPAD_DECIMAL:110,NUMPAD_DIVIDE:111,NUMPAD_ENTER:108,NUMPAD_MULTIPLY:106,NUMPAD_SUBTRACT:109,PAGE_DOWN:34,PAGE_UP:33,PERIOD:190,RIGHT:39,SHIFT:16,SPACE:32,TAB:9,UP:38}};if(d){var f=c.attr,e=c.fn.removeAttr,h="http://www.w3.org/2005/07/aaa",a=/^aria-/,b=/^wairole:/;c.attr=function(k,j,l){var m=l!==undefined;return(j=="role"?(m?f.call(this,k,j,"wairole:"+l):(f.apply(this,arguments)||"").replace(b,"")):(a.test(j)?(m?k.setAttributeNS(h,j.replace(a,"aaa:"),l):f.call(this,k,j.replace(a,"aaa:"))):f.apply(this,arguments)))};c.fn.removeAttr=function(j){return(a.test(j)?this.each(function(){this.removeAttributeNS(h,j.replace(a,""))}):e.call(this,j))}}c.fn.extend({remove:function(){c("*",this).add(this).each(function(){c(this).triggerHandler("remove")});return i.apply(this,arguments)},enableSelection:function(){return this.attr("unselectable","off").css("MozUserSelect","").unbind("selectstart.ui")},disableSelection:function(){return this.attr("unselectable","on").css("MozUserSelect","none").bind("selectstart.ui",function(){return false})},scrollParent:function(){var j;if((c.browser.msie&&(/(static|relative)/).test(this.css("position")))||(/absolute/).test(this.css("position"))){j=this.parents().filter(function(){return(/(relative|absolute|fixed)/).test(c.curCSS(this,"position",1))&&(/(auto|scroll)/).test(c.curCSS(this,"overflow",1)+c.curCSS(this,"overflow-y",1)+c.curCSS(this,"overflow-x",1))}).eq(0)}else{j=this.parents().filter(function(){return(/(auto|scroll)/).test(c.curCSS(this,"overflow",1)+c.curCSS(this,"overflow-y",1)+c.curCSS(this,"overflow-x",1))}).eq(0)}return(/fixed/).test(this.css("position"))||!j.length?c(document):j}});c.extend(c.expr[":"],{data:function(l,k,j){return !!c.data(l,j[3])},focusable:function(k){var l=k.nodeName.toLowerCase(),j=c.attr(k,"tabindex");return(/input|select|textarea|button|object/.test(l)?!k.disabled:"a"==l||"area"==l?k.href||!isNaN(j):!isNaN(j))&&!c(k)["area"==l?"parents":"closest"](":hidden").length},tabbable:function(k){var j=c.attr(k,"tabindex");return(isNaN(j)||j>=0)&&c(k).is(":focusable")}});function g(m,n,o,l){function k(q){var p=c[m][n][q]||[];return(typeof p=="string"?p.split(/,?\s+/):p)}var j=k("getter");if(l.length==1&&typeof l[0]=="string"){j=j.concat(k("getterSetter"))}return(c.inArray(o,j)!=-1)}c.widget=function(k,j){var l=k.split(".")[0];k=k.split(".")[1];c.fn[k]=function(p){var n=(typeof p=="string"),o=Array.prototype.slice.call(arguments,1);if(n&&p.substring(0,1)=="_"){return this}if(n&&g(l,k,p,o)){var m=c.data(this[0],k);return(m?m[p].apply(m,o):undefined)}return this.each(function(){var q=c.data(this,k);(!q&&!n&&c.data(this,k,new c[l][k](this,p))._init());(q&&n&&c.isFunction(q[p])&&q[p].apply(q,o))})};c[l]=c[l]||{};c[l][k]=function(o,n){var m=this;this.namespace=l;this.widgetName=k;this.widgetEventPrefix=c[l][k].eventPrefix||k;this.widgetBaseClass=l+"-"+k;this.options=c.extend({},c.widget.defaults,c[l][k].defaults,c.metadata&&c.metadata.get(o)[k],n);this.element=c(o).bind("setData."+k,function(q,p,r){if(q.target==o){return m._setData(p,r)}}).bind("getData."+k,function(q,p){if(q.target==o){return m._getData(p)}}).bind("remove",function(){return m.destroy()})};c[l][k].prototype=c.extend({},c.widget.prototype,j);c[l][k].getterSetter="option"};c.widget.prototype={_init:function(){},destroy:function(){this.element.removeData(this.widgetName).removeClass(this.widgetBaseClass+"-disabled "+this.namespace+"-state-disabled").removeAttr("aria-disabled")},option:function(l,m){var k=l,j=this;if(typeof l=="string"){if(m===undefined){return this._getData(l)}k={};k[l]=m}c.each(k,function(n,o){j._setData(n,o)})},_getData:function(j){return this.options[j]},_setData:function(j,k){this.options[j]=k;if(j=="disabled"){this.element[k?"addClass":"removeClass"](this.widgetBaseClass+"-disabled "+this.namespace+"-state-disabled").attr("aria-disabled",k)}},enable:function(){this._setData("disabled",false)},disable:function(){this._setData("disabled",true)},_trigger:function(l,m,n){var p=this.options[l],j=(l==this.widgetEventPrefix?l:this.widgetEventPrefix+l);m=c.Event(m);m.type=j;if(m.originalEvent){for(var k=c.event.props.length,o;k;){o=c.event.props[--k];m[o]=m.originalEvent[o]}}this.element.trigger(m,n);return !(c.isFunction(p)&&p.call(this.element[0],m,n)===false||m.isDefaultPrevented())}};c.widget.defaults={disabled:false};c.ui.mouse={_mouseInit:function(){var j=this;this.element.bind("mousedown."+this.widgetName,function(k){return j._mouseDown(k)}).bind("click."+this.widgetName,function(k){if(j._preventClickEvent){j._preventClickEvent=false;k.stopImmediatePropagation();return false}});if(c.browser.msie){this._mouseUnselectable=this.element.attr("unselectable");this.element.attr("unselectable","on")}this.started=false},_mouseDestroy:function(){this.element.unbind("."+this.widgetName);(c.browser.msie&&this.element.attr("unselectable",this._mouseUnselectable))},_mouseDown:function(l){l.originalEvent=l.originalEvent||{};if(l.originalEvent.mouseHandled){return}(this._mouseStarted&&this._mouseUp(l));this._mouseDownEvent=l;var k=this,m=(l.which==1),j=(typeof this.options.cancel=="string"?c(l.target).parents().add(l.target).filter(this.options.cancel).length:false);if(!m||j||!this._mouseCapture(l)){return true}this.mouseDelayMet=!this.options.delay;if(!this.mouseDelayMet){this._mouseDelayTimer=setTimeout(function(){k.mouseDelayMet=true},this.options.delay)}if(this._mouseDistanceMet(l)&&this._mouseDelayMet(l)){this._mouseStarted=(this._mouseStart(l)!==false);if(!this._mouseStarted){l.preventDefault();return true}}this._mouseMoveDelegate=function(n){return k._mouseMove(n)};this._mouseUpDelegate=function(n){return k._mouseUp(n)};c(document).bind("mousemove."+this.widgetName,this._mouseMoveDelegate).bind("mouseup."+this.widgetName,this._mouseUpDelegate);(c.browser.safari||l.preventDefault());l.originalEvent.mouseHandled=true;return true},_mouseMove:function(j){if(c.browser.msie&&!j.button){return this._mouseUp(j)}if(this._mouseStarted){this._mouseDrag(j);return j.preventDefault()}if(this._mouseDistanceMet(j)&&this._mouseDelayMet(j)){this._mouseStarted=(this._mouseStart(this._mouseDownEvent,j)!==false);(this._mouseStarted?this._mouseDrag(j):this._mouseUp(j))}return !this._mouseStarted},_mouseUp:function(j){c(document).unbind("mousemove."+this.widgetName,this._mouseMoveDelegate).unbind("mouseup."+this.widgetName,this._mouseUpDelegate);if(this._mouseStarted){this._mouseStarted=false;this._preventClickEvent=(j.target==this._mouseDownEvent.target);this._mouseStop(j)}return false},_mouseDistanceMet:function(j){return(Math.max(Math.abs(this._mouseDownEvent.pageX-j.pageX),Math.abs(this._mouseDownEvent.pageY-j.pageY))>=this.options.distance)},_mouseDelayMet:function(j){return this.mouseDelayMet},_mouseStart:function(j){},_mouseDrag:function(j){},_mouseStop:function(j){},_mouseCapture:function(j){return true}};c.ui.mouse.defaults={cancel:null,distance:1,delay:0}})(jQuery);;/*
 * jQuery UI Accordion 1.7.1
 *
 * Copyright (c) 2009 AUTHORS.txt (http://jqueryui.com/about)
 * Dual licensed under the MIT (MIT-LICENSE.txt)
 * and GPL (GPL-LICENSE.txt) licenses.
 *
 * http://docs.jquery.com/UI/Accordion
 *
 * Depends:
 *	ui.core.js
 */(function(a){a.widget("ui.accordion",{_init:function(){var d=this.options,b=this;this.running=0;if(d.collapsible==a.ui.accordion.defaults.collapsible&&d.alwaysOpen!=a.ui.accordion.defaults.alwaysOpen){d.collapsible=!d.alwaysOpen}if(d.navigation){var c=this.element.find("a").filter(d.navigationFilter);if(c.length){if(c.filter(d.header).length){this.active=c}else{this.active=c.parent().parent().prev();c.addClass("ui-accordion-content-active")}}}this.element.addClass("ui-accordion ui-widget ui-helper-reset");if(this.element[0].nodeName=="UL"){this.element.children("li").addClass("ui-accordion-li-fix")}this.headers=this.element.find(d.header).addClass("ui-accordion-header ui-helper-reset ui-state-default ui-corner-all").bind("mouseenter.accordion",function(){a(this).addClass("ui-state-hover")}).bind("mouseleave.accordion",function(){a(this).removeClass("ui-state-hover")}).bind("focus.accordion",function(){a(this).addClass("ui-state-focus")}).bind("blur.accordion",function(){a(this).removeClass("ui-state-focus")});this.headers.next().addClass("ui-accordion-content ui-helper-reset ui-widget-content ui-corner-bottom");this.active=this._findActive(this.active||d.active).toggleClass("ui-state-default").toggleClass("ui-state-active").toggleClass("ui-corner-all").toggleClass("ui-corner-top");this.active.next().addClass("ui-accordion-content-active");a("<span/>").addClass("ui-icon "+d.icons.header).prependTo(this.headers);this.active.find(".ui-icon").toggleClass(d.icons.header).toggleClass(d.icons.headerSelected);if(a.browser.msie){this.element.find("a").css("zoom","1")}this.resize();this.element.attr("role","tablist");this.headers.attr("role","tab").bind("keydown",function(e){return b._keydown(e)}).next().attr("role","tabpanel");this.headers.not(this.active||"").attr("aria-expanded","false").attr("tabIndex","-1").next().hide();if(!this.active.length){this.headers.eq(0).attr("tabIndex","0")}else{this.active.attr("aria-expanded","true").attr("tabIndex","0")}if(!a.browser.safari){this.headers.find("a").attr("tabIndex","-1")}if(d.event){this.headers.bind((d.event)+".accordion",function(e){return b._clickHandler.call(b,e,this)})}},destroy:function(){var c=this.options;this.element.removeClass("ui-accordion ui-widget ui-helper-reset").removeAttr("role").unbind(".accordion").removeData("accordion");this.headers.unbind(".accordion").removeClass("ui-accordion-header ui-helper-reset ui-state-default ui-corner-all ui-state-active ui-corner-top").removeAttr("role").removeAttr("aria-expanded").removeAttr("tabindex");this.headers.find("a").removeAttr("tabindex");this.headers.children(".ui-icon").remove();var b=this.headers.next().css("display","").removeAttr("role").removeClass("ui-helper-reset ui-widget-content ui-corner-bottom ui-accordion-content ui-accordion-content-active");if(c.autoHeight||c.fillHeight){b.css("height","")}},_setData:function(b,c){if(b=="alwaysOpen"){b="collapsible";c=!c}a.widget.prototype._setData.apply(this,arguments)},_keydown:function(e){var g=this.options,f=a.ui.keyCode;if(g.disabled||e.altKey||e.ctrlKey){return}var d=this.headers.length;var b=this.headers.index(e.target);var c=false;switch(e.keyCode){case f.RIGHT:case f.DOWN:c=this.headers[(b+1)%d];break;case f.LEFT:case f.UP:c=this.headers[(b-1+d)%d];break;case f.SPACE:case f.ENTER:return this._clickHandler({target:e.target},e.target)}if(c){a(e.target).attr("tabIndex","-1");a(c).attr("tabIndex","0");c.focus();return false}return true},resize:function(){var e=this.options,d;if(e.fillSpace){if(a.browser.msie){var b=this.element.parent().css("overflow");this.element.parent().css("overflow","hidden")}d=this.element.parent().height();if(a.browser.msie){this.element.parent().css("overflow",b)}this.headers.each(function(){d-=a(this).outerHeight()});var c=0;this.headers.next().each(function(){c=Math.max(c,a(this).innerHeight()-a(this).height())}).height(Math.max(0,d-c)).css("overflow","auto")}else{if(e.autoHeight){d=0;this.headers.next().each(function(){d=Math.max(d,a(this).outerHeight())}).height(d)}}},activate:function(b){var c=this._findActive(b)[0];this._clickHandler({target:c},c)},_findActive:function(b){return b?typeof b=="number"?this.headers.filter(":eq("+b+")"):this.headers.not(this.headers.not(b)):b===false?a([]):this.headers.filter(":eq(0)")},_clickHandler:function(b,f){var d=this.options;if(d.disabled){return false}if(!b.target&&d.collapsible){this.active.removeClass("ui-state-active ui-corner-top").addClass("ui-state-default ui-corner-all").find(".ui-icon").removeClass(d.icons.headerSelected).addClass(d.icons.header);this.active.next().addClass("ui-accordion-content-active");var h=this.active.next(),e={options:d,newHeader:a([]),oldHeader:d.active,newContent:a([]),oldContent:h},c=(this.active=a([]));this._toggle(c,h,e);return false}var g=a(b.currentTarget||f);var i=g[0]==this.active[0];if(this.running||(!d.collapsible&&i)){return false}this.active.removeClass("ui-state-active ui-corner-top").addClass("ui-state-default ui-corner-all").find(".ui-icon").removeClass(d.icons.headerSelected).addClass(d.icons.header);this.active.next().addClass("ui-accordion-content-active");if(!i){g.removeClass("ui-state-default ui-corner-all").addClass("ui-state-active ui-corner-top").find(".ui-icon").removeClass(d.icons.header).addClass(d.icons.headerSelected);g.next().addClass("ui-accordion-content-active")}var c=g.next(),h=this.active.next(),e={options:d,newHeader:i&&d.collapsible?a([]):g,oldHeader:this.active,newContent:i&&d.collapsible?a([]):c.find("> *"),oldContent:h.find("> *")},j=this.headers.index(this.active[0])>this.headers.index(g[0]);this.active=i?a([]):g;this._toggle(c,h,e,i,j);return false},_toggle:function(b,i,g,j,k){var d=this.options,m=this;this.toShow=b;this.toHide=i;this.data=g;var c=function(){if(!m){return}return m._completed.apply(m,arguments)};this._trigger("changestart",null,this.data);this.running=i.size()===0?b.size():i.size();if(d.animated){var f={};if(d.collapsible&&j){f={toShow:a([]),toHide:i,complete:c,down:k,autoHeight:d.autoHeight||d.fillSpace}}else{f={toShow:b,toHide:i,complete:c,down:k,autoHeight:d.autoHeight||d.fillSpace}}if(!d.proxied){d.proxied=d.animated}if(!d.proxiedDuration){d.proxiedDuration=d.duration}d.animated=a.isFunction(d.proxied)?d.proxied(f):d.proxied;d.duration=a.isFunction(d.proxiedDuration)?d.proxiedDuration(f):d.proxiedDuration;var l=a.ui.accordion.animations,e=d.duration,h=d.animated;if(!l[h]){l[h]=function(n){this.slide(n,{easing:h,duration:e||700})}}l[h](f)}else{if(d.collapsible&&j){b.toggle()}else{i.hide();b.show()}c(true)}i.prev().attr("aria-expanded","false").attr("tabIndex","-1").blur();b.prev().attr("aria-expanded","true").attr("tabIndex","0").focus()},_completed:function(b){var c=this.options;this.running=b?0:--this.running;if(this.running){return}if(c.clearStyle){this.toShow.add(this.toHide).css({height:"",overflow:""})}this._trigger("change",null,this.data)}});a.extend(a.ui.accordion,{version:"1.7.1",defaults:{active:null,alwaysOpen:true,animated:"slide",autoHeight:true,clearStyle:false,collapsible:false,event:"click",fillSpace:false,header:"> li > :first-child,> :not(li):even",icons:{header:"ui-icon-triangle-1-e",headerSelected:"ui-icon-triangle-1-s"},navigation:false,navigationFilter:function(){return this.href.toLowerCase()==location.href.toLowerCase()}},animations:{slide:function(j,h){j=a.extend({easing:"swing",duration:300},j,h);if(!j.toHide.size()){j.toShow.animate({height:"show"},j);return}if(!j.toShow.size()){j.toHide.animate({height:"hide"},j);return}var c=j.toShow.css("overflow"),g,d={},f={},e=["height","paddingTop","paddingBottom"],b;var i=j.toShow;b=i[0].style.width;i.width(parseInt(i.parent().width(),10)-parseInt(i.css("paddingLeft"),10)-parseInt(i.css("paddingRight"),10)-(parseInt(i.css("borderLeftWidth"),10)||0)-(parseInt(i.css("borderRightWidth"),10)||0));a.each(e,function(k,m){f[m]="hide";var l=(""+a.css(j.toShow[0],m)).match(/^([\d+-.]+)(.*)$/);d[m]={value:l[1],unit:l[2]||"px"}});j.toShow.css({height:0,overflow:"hidden"}).show();j.toHide.filter(":hidden").each(j.complete).end().filter(":visible").animate(f,{step:function(k,l){if(l.prop=="height"){g=(l.now-l.start)/(l.end-l.start)}j.toShow[0].style[l.prop]=(g*d[l.prop].value)+d[l.prop].unit},duration:j.duration,easing:j.easing,complete:function(){if(!j.autoHeight){j.toShow.css("height","")}j.toShow.css("width",b);j.toShow.css({overflow:c});j.complete()}})},bounceslide:function(b){this.slide(b,{easing:b.down?"easeOutBounce":"swing",duration:b.down?1000:200})},easeslide:function(b){this.slide(b,{easing:"easeinout",duration:700})}}})})(jQuery);;/*
 * jQuery UI Dialog 1.7.1
 *
 * Copyright (c) 2009 AUTHORS.txt (http://jqueryui.com/about)
 * Dual licensed under the MIT (MIT-LICENSE.txt)
 * and GPL (GPL-LICENSE.txt) licenses.
 *
 * http://docs.jquery.com/UI/Dialog
 *
 * Depends:
 *	ui.core.js
 *	ui.draggable.js
 *	ui.resizable.js
 */(function(c){var b={dragStart:"start.draggable",drag:"drag.draggable",dragStop:"stop.draggable",maxHeight:"maxHeight.resizable",minHeight:"minHeight.resizable",maxWidth:"maxWidth.resizable",minWidth:"minWidth.resizable",resizeStart:"start.resizable",resize:"drag.resizable",resizeStop:"stop.resizable"},a="ui-dialog ui-widget ui-widget-content ui-corner-all ";c.widget("ui.dialog",{_init:function(){this.originalTitle=this.element.attr("title");var l=this,m=this.options,j=m.title||this.originalTitle||"&nbsp;",e=c.ui.dialog.getTitleId(this.element),k=(this.uiDialog=c("<div/>")).appendTo(document.body).hide().addClass(a+m.dialogClass).css({position:"absolute",overflow:"hidden",zIndex:m.zIndex}).attr("tabIndex",-1).css("outline",0).keydown(function(n){(m.closeOnEscape&&n.keyCode&&n.keyCode==c.ui.keyCode.ESCAPE&&l.close(n))}).attr({role:"dialog","aria-labelledby":e}).mousedown(function(n){l.moveToTop(false,n)}),g=this.element.show().removeAttr("title").addClass("ui-dialog-content ui-widget-content").appendTo(k),f=(this.uiDialogTitlebar=c("<div></div>")).addClass("ui-dialog-titlebar ui-widget-header ui-corner-all ui-helper-clearfix").prependTo(k),i=c('<a href="#"/>').addClass("ui-dialog-titlebar-close ui-corner-all").attr("role","button").hover(function(){i.addClass("ui-state-hover")},function(){i.removeClass("ui-state-hover")}).focus(function(){i.addClass("ui-state-focus")}).blur(function(){i.removeClass("ui-state-focus")}).mousedown(function(n){n.stopPropagation()}).click(function(n){l.close(n);return false}).appendTo(f),h=(this.uiDialogTitlebarCloseText=c("<span/>")).addClass("ui-icon ui-icon-closethick").text(m.closeText).appendTo(i),d=c("<span/>").addClass("ui-dialog-title").attr("id",e).html(j).prependTo(f);f.find("*").add(f).disableSelection();(m.draggable&&c.fn.draggable&&this._makeDraggable());(m.resizable&&c.fn.resizable&&this._makeResizable());this._createButtons(m.buttons);this._isOpen=false;(m.bgiframe&&c.fn.bgiframe&&k.bgiframe());(m.autoOpen&&this.open())},destroy:function(){(this.overlay&&this.overlay.destroy());this.uiDialog.hide();this.element.unbind(".dialog").removeData("dialog").removeClass("ui-dialog-content ui-widget-content").hide().appendTo("body");this.uiDialog.remove();(this.originalTitle&&this.element.attr("title",this.originalTitle))},close:function(e){var d=this;if(false===d._trigger("beforeclose",e)){return}(d.overlay&&d.overlay.destroy());d.uiDialog.unbind("keypress.ui-dialog");(d.options.hide?d.uiDialog.hide(d.options.hide,function(){d._trigger("close",e)}):d.uiDialog.hide()&&d._trigger("close",e));c.ui.dialog.overlay.resize();d._isOpen=false},isOpen:function(){return this._isOpen},moveToTop:function(f,e){if((this.options.modal&&!f)||(!this.options.stack&&!this.options.modal)){return this._trigger("focus",e)}if(this.options.zIndex>c.ui.dialog.maxZ){c.ui.dialog.maxZ=this.options.zIndex}(this.overlay&&this.overlay.$el.css("z-index",c.ui.dialog.overlay.maxZ=++c.ui.dialog.maxZ));var d={scrollTop:this.element.attr("scrollTop"),scrollLeft:this.element.attr("scrollLeft")};this.uiDialog.css("z-index",++c.ui.dialog.maxZ);this.element.attr(d);this._trigger("focus",e)},open:function(){if(this._isOpen){return}var e=this.options,d=this.uiDialog;this.overlay=e.modal?new c.ui.dialog.overlay(this):null;(d.next().length&&d.appendTo("body"));this._size();this._position(e.position);d.show(e.show);this.moveToTop(true);(e.modal&&d.bind("keypress.ui-dialog",function(h){if(h.keyCode!=c.ui.keyCode.TAB){return}var g=c(":tabbable",this),i=g.filter(":first")[0],f=g.filter(":last")[0];if(h.target==f&&!h.shiftKey){setTimeout(function(){i.focus()},1)}else{if(h.target==i&&h.shiftKey){setTimeout(function(){f.focus()},1)}}}));c([]).add(d.find(".ui-dialog-content :tabbable:first")).add(d.find(".ui-dialog-buttonpane :tabbable:first")).add(d).filter(":first").focus();this._trigger("open");this._isOpen=true},_createButtons:function(g){var f=this,d=false,e=c("<div></div>").addClass("ui-dialog-buttonpane ui-widget-content ui-helper-clearfix");this.uiDialog.find(".ui-dialog-buttonpane").remove();(typeof g=="object"&&g!==null&&c.each(g,function(){return !(d=true)}));if(d){c.each(g,function(h,i){c('<button type="button"></button>').addClass("ui-state-default ui-corner-all").text(h).click(function(){i.apply(f.element[0],arguments)}).hover(function(){c(this).addClass("ui-state-hover")},function(){c(this).removeClass("ui-state-hover")}).focus(function(){c(this).addClass("ui-state-focus")}).blur(function(){c(this).removeClass("ui-state-focus")}).appendTo(e)});e.appendTo(this.uiDialog)}},_makeDraggable:function(){var d=this,f=this.options,e;this.uiDialog.draggable({cancel:".ui-dialog-content",handle:".ui-dialog-titlebar",containment:"document",start:function(){e=f.height;c(this).height(c(this).height()).addClass("ui-dialog-dragging");(f.dragStart&&f.dragStart.apply(d.element[0],arguments))},drag:function(){(f.drag&&f.drag.apply(d.element[0],arguments))},stop:function(){c(this).removeClass("ui-dialog-dragging").height(e);(f.dragStop&&f.dragStop.apply(d.element[0],arguments));c.ui.dialog.overlay.resize()}})},_makeResizable:function(g){g=(g===undefined?this.options.resizable:g);var d=this,f=this.options,e=typeof g=="string"?g:"n,e,s,w,se,sw,ne,nw";this.uiDialog.resizable({cancel:".ui-dialog-content",alsoResize:this.element,maxWidth:f.maxWidth,maxHeight:f.maxHeight,minWidth:f.minWidth,minHeight:f.minHeight,start:function(){c(this).addClass("ui-dialog-resizing");(f.resizeStart&&f.resizeStart.apply(d.element[0],arguments))},resize:function(){(f.resize&&f.resize.apply(d.element[0],arguments))},handles:e,stop:function(){c(this).removeClass("ui-dialog-resizing");f.height=c(this).height();f.width=c(this).width();(f.resizeStop&&f.resizeStop.apply(d.element[0],arguments));c.ui.dialog.overlay.resize()}}).find(".ui-resizable-se").addClass("ui-icon ui-icon-grip-diagonal-se")},_position:function(i){var e=c(window),f=c(document),g=f.scrollTop(),d=f.scrollLeft(),h=g;if(c.inArray(i,["center","top","right","bottom","left"])>=0){i=[i=="right"||i=="left"?i:"center",i=="top"||i=="bottom"?i:"middle"]}if(i.constructor!=Array){i=["center","middle"]}if(i[0].constructor==Number){d+=i[0]}else{switch(i[0]){case"left":d+=0;break;case"right":d+=e.width()-this.uiDialog.outerWidth();break;default:case"center":d+=(e.width()-this.uiDialog.outerWidth())/2}}if(i[1].constructor==Number){g+=i[1]}else{switch(i[1]){case"top":g+=0;break;case"bottom":g+=e.height()-this.uiDialog.outerHeight();break;default:case"middle":g+=(e.height()-this.uiDialog.outerHeight())/2}}g=Math.max(g,h);this.uiDialog.css({top:g,left:d})},_setData:function(e,f){(b[e]&&this.uiDialog.data(b[e],f));switch(e){case"buttons":this._createButtons(f);break;case"closeText":this.uiDialogTitlebarCloseText.text(f);break;case"dialogClass":this.uiDialog.removeClass(this.options.dialogClass).addClass(a+f);break;case"draggable":(f?this._makeDraggable():this.uiDialog.draggable("destroy"));break;case"height":this.uiDialog.height(f);break;case"position":this._position(f);break;case"resizable":var d=this.uiDialog,g=this.uiDialog.is(":data(resizable)");(g&&!f&&d.resizable("destroy"));(g&&typeof f=="string"&&d.resizable("option","handles",f));(g||this._makeResizable(f));break;case"title":c(".ui-dialog-title",this.uiDialogTitlebar).html(f||"&nbsp;");break;case"width":this.uiDialog.width(f);break}c.widget.prototype._setData.apply(this,arguments)},_size:function(){var e=this.options;this.element.css({height:0,minHeight:0,width:"auto"});var d=this.uiDialog.css({height:"auto",width:e.width}).height();this.element.css({minHeight:Math.max(e.minHeight-d,0),height:e.height=="auto"?"auto":Math.max(e.height-d,0)})}});c.extend(c.ui.dialog,{version:"1.7.1",defaults:{autoOpen:true,bgiframe:false,buttons:{},closeOnEscape:true,closeText:"close",dialogClass:"",draggable:true,hide:null,height:"auto",maxHeight:false,maxWidth:false,minHeight:150,minWidth:150,modal:false,position:"center",resizable:true,show:null,stack:true,title:"",width:300,zIndex:1000},getter:"isOpen",uuid:0,maxZ:0,getTitleId:function(d){return"ui-dialog-title-"+(d.attr("id")||++this.uuid)},overlay:function(d){this.$el=c.ui.dialog.overlay.create(d)}});c.extend(c.ui.dialog.overlay,{instances:[],maxZ:0,events:c.map("focus,mousedown,mouseup,keydown,keypress,click".split(","),function(d){return d+".dialog-overlay"}).join(" "),create:function(e){if(this.instances.length===0){setTimeout(function(){c(document).bind(c.ui.dialog.overlay.events,function(f){var g=c(f.target).parents(".ui-dialog").css("zIndex")||0;return(g>c.ui.dialog.overlay.maxZ)})},1);c(document).bind("keydown.dialog-overlay",function(f){(e.options.closeOnEscape&&f.keyCode&&f.keyCode==c.ui.keyCode.ESCAPE&&e.close(f))});c(window).bind("resize.dialog-overlay",c.ui.dialog.overlay.resize)}var d=c("<div></div>").appendTo(document.body).addClass("ui-widget-overlay").css({width:this.width(),height:this.height()});(e.options.bgiframe&&c.fn.bgiframe&&d.bgiframe());this.instances.push(d);return d},destroy:function(d){this.instances.splice(c.inArray(this.instances,d),1);if(this.instances.length===0){c([document,window]).unbind(".dialog-overlay")}d.remove()},height:function(){if(c.browser.msie&&c.browser.version<7){var e=Math.max(document.documentElement.scrollHeight,document.body.scrollHeight);var d=Math.max(document.documentElement.offsetHeight,document.body.offsetHeight);if(e<d){return c(window).height()+"px"}else{return e+"px"}}else{return c(document).height()+"px"}},width:function(){if(c.browser.msie&&c.browser.version<7){var d=Math.max(document.documentElement.scrollWidth,document.body.scrollWidth);var e=Math.max(document.documentElement.offsetWidth,document.body.offsetWidth);if(d<e){return c(window).width()+"px"}else{return d+"px"}}else{return c(document).width()+"px"}},resize:function(){var d=c([]);c.each(c.ui.dialog.overlay.instances,function(){d=d.add(this)});d.css({width:0,height:0}).css({width:c.ui.dialog.overlay.width(),height:c.ui.dialog.overlay.height()})}});c.extend(c.ui.dialog.overlay.prototype,{destroy:function(){c.ui.dialog.overlay.destroy(this.$el)}})})(jQuery);;/*
 * jQuery UI Slider 1.7.1
 *
 * Copyright (c) 2009 AUTHORS.txt (http://jqueryui.com/about)
 * Dual licensed under the MIT (MIT-LICENSE.txt)
 * and GPL (GPL-LICENSE.txt) licenses.
 *
 * http://docs.jquery.com/UI/Slider
 *
 * Depends:
 *	ui.core.js
 */(function(a){a.widget("ui.slider",a.extend({},a.ui.mouse,{_init:function(){var b=this,c=this.options;this._keySliding=false;this._handleIndex=null;this._detectOrientation();this._mouseInit();this.element.addClass("ui-slider ui-slider-"+this.orientation+" ui-widget ui-widget-content ui-corner-all");this.range=a([]);if(c.range){if(c.range===true){this.range=a("<div></div>");if(!c.values){c.values=[this._valueMin(),this._valueMin()]}if(c.values.length&&c.values.length!=2){c.values=[c.values[0],c.values[0]]}}else{this.range=a("<div></div>")}this.range.appendTo(this.element).addClass("ui-slider-range");if(c.range=="min"||c.range=="max"){this.range.addClass("ui-slider-range-"+c.range)}this.range.addClass("ui-widget-header")}if(a(".ui-slider-handle",this.element).length==0){a('<a href="#"></a>').appendTo(this.element).addClass("ui-slider-handle")}if(c.values&&c.values.length){while(a(".ui-slider-handle",this.element).length<c.values.length){a('<a href="#"></a>').appendTo(this.element).addClass("ui-slider-handle")}}this.handles=a(".ui-slider-handle",this.element).addClass("ui-state-default ui-corner-all");this.handle=this.handles.eq(0);this.handles.add(this.range).filter("a").click(function(d){d.preventDefault()}).hover(function(){a(this).addClass("ui-state-hover")},function(){a(this).removeClass("ui-state-hover")}).focus(function(){a(".ui-slider .ui-state-focus").removeClass("ui-state-focus");a(this).addClass("ui-state-focus")}).blur(function(){a(this).removeClass("ui-state-focus")});this.handles.each(function(d){a(this).data("index.ui-slider-handle",d)});this.handles.keydown(function(i){var f=true;var e=a(this).data("index.ui-slider-handle");if(b.options.disabled){return}switch(i.keyCode){case a.ui.keyCode.HOME:case a.ui.keyCode.END:case a.ui.keyCode.UP:case a.ui.keyCode.RIGHT:case a.ui.keyCode.DOWN:case a.ui.keyCode.LEFT:f=false;if(!b._keySliding){b._keySliding=true;a(this).addClass("ui-state-active");b._start(i,e)}break}var g,d,h=b._step();if(b.options.values&&b.options.values.length){g=d=b.values(e)}else{g=d=b.value()}switch(i.keyCode){case a.ui.keyCode.HOME:d=b._valueMin();break;case a.ui.keyCode.END:d=b._valueMax();break;case a.ui.keyCode.UP:case a.ui.keyCode.RIGHT:if(g==b._valueMax()){return}d=g+h;break;case a.ui.keyCode.DOWN:case a.ui.keyCode.LEFT:if(g==b._valueMin()){return}d=g-h;break}b._slide(i,e,d);return f}).keyup(function(e){var d=a(this).data("index.ui-slider-handle");if(b._keySliding){b._stop(e,d);b._change(e,d);b._keySliding=false;a(this).removeClass("ui-state-active")}});this._refreshValue()},destroy:function(){this.handles.remove();this.range.remove();this.element.removeClass("ui-slider ui-slider-horizontal ui-slider-vertical ui-slider-disabled ui-widget ui-widget-content ui-corner-all").removeData("slider").unbind(".slider");this._mouseDestroy()},_mouseCapture:function(d){var e=this.options;if(e.disabled){return false}this.elementSize={width:this.element.outerWidth(),height:this.element.outerHeight()};this.elementOffset=this.element.offset();var h={x:d.pageX,y:d.pageY};var j=this._normValueFromMouse(h);var c=this._valueMax()-this._valueMin()+1,f;var k=this,i;this.handles.each(function(l){var m=Math.abs(j-k.values(l));if(c>m){c=m;f=a(this);i=l}});if(e.range==true&&this.values(1)==e.min){f=a(this.handles[++i])}this._start(d,i);k._handleIndex=i;f.addClass("ui-state-active").focus();var g=f.offset();var b=!a(d.target).parents().andSelf().is(".ui-slider-handle");this._clickOffset=b?{left:0,top:0}:{left:d.pageX-g.left-(f.width()/2),top:d.pageY-g.top-(f.height()/2)-(parseInt(f.css("borderTopWidth"),10)||0)-(parseInt(f.css("borderBottomWidth"),10)||0)+(parseInt(f.css("marginTop"),10)||0)};j=this._normValueFromMouse(h);this._slide(d,i,j);return true},_mouseStart:function(b){return true},_mouseDrag:function(d){var b={x:d.pageX,y:d.pageY};var c=this._normValueFromMouse(b);this._slide(d,this._handleIndex,c);return false},_mouseStop:function(b){this.handles.removeClass("ui-state-active");this._stop(b,this._handleIndex);this._change(b,this._handleIndex);this._handleIndex=null;this._clickOffset=null;return false},_detectOrientation:function(){this.orientation=this.options.orientation=="vertical"?"vertical":"horizontal"},_normValueFromMouse:function(d){var c,h;if("horizontal"==this.orientation){c=this.elementSize.width;h=d.x-this.elementOffset.left-(this._clickOffset?this._clickOffset.left:0)}else{c=this.elementSize.height;h=d.y-this.elementOffset.top-(this._clickOffset?this._clickOffset.top:0)}var f=(h/c);if(f>1){f=1}if(f<0){f=0}if("vertical"==this.orientation){f=1-f}var e=this._valueMax()-this._valueMin(),i=f*e,b=i%this.options.step,g=this._valueMin()+i-b;if(b>(this.options.step/2)){g+=this.options.step}return parseFloat(g.toFixed(5))},_start:function(d,c){var b={handle:this.handles[c],value:this.value()};if(this.options.values&&this.options.values.length){b.value=this.values(c);b.values=this.values()}this._trigger("start",d,b)},_slide:function(f,e,d){var g=this.handles[e];if(this.options.values&&this.options.values.length){var b=this.values(e?0:1);if((e==0&&d>=b)||(e==1&&d<=b)){d=b}if(d!=this.values(e)){var c=this.values();c[e]=d;var h=this._trigger("slide",f,{handle:this.handles[e],value:d,values:c});var b=this.values(e?0:1);if(h!==false){this.values(e,d,(f.type=="mousedown"&&this.options.animate),true)}}}else{if(d!=this.value()){var h=this._trigger("slide",f,{handle:this.handles[e],value:d});if(h!==false){this._setData("value",d,(f.type=="mousedown"&&this.options.animate))}}}},_stop:function(d,c){var b={handle:this.handles[c],value:this.value()};if(this.options.values&&this.options.values.length){b.value=this.values(c);b.values=this.values()}this._trigger("stop",d,b)},_change:function(d,c){var b={handle:this.handles[c],value:this.value()};if(this.options.values&&this.options.values.length){b.value=this.values(c);b.values=this.values()}this._trigger("change",d,b)},value:function(b){if(arguments.length){this._setData("value",b);this._change(null,0)}return this._value()},values:function(b,e,c,d){if(arguments.length>1){this.options.values[b]=e;this._refreshValue(c);if(!d){this._change(null,b)}}if(arguments.length){if(this.options.values&&this.options.values.length){return this._values(b)}else{return this.value()}}else{return this._values()}},_setData:function(b,d,c){a.widget.prototype._setData.apply(this,arguments);switch(b){case"orientation":this._detectOrientation();this.element.removeClass("ui-slider-horizontal ui-slider-vertical").addClass("ui-slider-"+this.orientation);this._refreshValue(c);break;case"value":this._refreshValue(c);break}},_step:function(){var b=this.options.step;return b},_value:function(){var b=this.options.value;if(b<this._valueMin()){b=this._valueMin()}if(b>this._valueMax()){b=this._valueMax()}return b},_values:function(b){if(arguments.length){var c=this.options.values[b];if(c<this._valueMin()){c=this._valueMin()}if(c>this._valueMax()){c=this._valueMax()}return c}else{return this.options.values}},_valueMin:function(){var b=this.options.min;return b},_valueMax:function(){var b=this.options.max;return b},_refreshValue:function(c){var f=this.options.range,d=this.options,l=this;if(this.options.values&&this.options.values.length){var i,h;this.handles.each(function(p,n){var o=(l.values(p)-l._valueMin())/(l._valueMax()-l._valueMin())*100;var m={};m[l.orientation=="horizontal"?"left":"bottom"]=o+"%";a(this).stop(1,1)[c?"animate":"css"](m,d.animate);if(l.options.range===true){if(l.orientation=="horizontal"){(p==0)&&l.range.stop(1,1)[c?"animate":"css"]({left:o+"%"},d.animate);(p==1)&&l.range[c?"animate":"css"]({width:(o-lastValPercent)+"%"},{queue:false,duration:d.animate})}else{(p==0)&&l.range.stop(1,1)[c?"animate":"css"]({bottom:(o)+"%"},d.animate);(p==1)&&l.range[c?"animate":"css"]({height:(o-lastValPercent)+"%"},{queue:false,duration:d.animate})}}lastValPercent=o})}else{var j=this.value(),g=this._valueMin(),k=this._valueMax(),e=k!=g?(j-g)/(k-g)*100:0;var b={};b[l.orientation=="horizontal"?"left":"bottom"]=e+"%";this.handle.stop(1,1)[c?"animate":"css"](b,d.animate);(f=="min")&&(this.orientation=="horizontal")&&this.range.stop(1,1)[c?"animate":"css"]({width:e+"%"},d.animate);(f=="max")&&(this.orientation=="horizontal")&&this.range[c?"animate":"css"]({width:(100-e)+"%"},{queue:false,duration:d.animate});(f=="min")&&(this.orientation=="vertical")&&this.range.stop(1,1)[c?"animate":"css"]({height:e+"%"},d.animate);(f=="max")&&(this.orientation=="vertical")&&this.range[c?"animate":"css"]({height:(100-e)+"%"},{queue:false,duration:d.animate})}}}));a.extend(a.ui.slider,{getter:"value values",version:"1.7.1",eventPrefix:"slide",defaults:{animate:false,delay:0,distance:0,max:100,min:0,orientation:"horizontal",range:false,step:1,value:0,values:null}})})(jQuery);;/*
 * jQuery UI Tabs 1.7.1
 *
 * Copyright (c) 2009 AUTHORS.txt (http://jqueryui.com/about)
 * Dual licensed under the MIT (MIT-LICENSE.txt)
 * and GPL (GPL-LICENSE.txt) licenses.
 *
 * http://docs.jquery.com/UI/Tabs
 *
 * Depends:
 *	ui.core.js
 */(function(a){a.widget("ui.tabs",{_init:function(){if(this.options.deselectable!==undefined){this.options.collapsible=this.options.deselectable}this._tabify(true)},_setData:function(b,c){if(b=="selected"){if(this.options.collapsible&&c==this.options.selected){return}this.select(c)}else{this.options[b]=c;if(b=="deselectable"){this.options.collapsible=c}this._tabify()}},_tabId:function(b){return b.title&&b.title.replace(/\s/g,"_").replace(/[^A-Za-z0-9\-_:\.]/g,"")||this.options.idPrefix+a.data(b)},_sanitizeSelector:function(b){return b.replace(/:/g,"\\:")},_cookie:function(){var b=this.cookie||(this.cookie=this.options.cookie.name||"ui-tabs-"+a.data(this.list[0]));return a.cookie.apply(null,[b].concat(a.makeArray(arguments)))},_ui:function(c,b){return{tab:c,panel:b,index:this.anchors.index(c)}},_cleanup:function(){this.lis.filter(".ui-state-processing").removeClass("ui-state-processing").find("span:data(label.tabs)").each(function(){var b=a(this);b.html(b.data("label.tabs")).removeData("label.tabs")})},_tabify:function(n){this.list=this.element.children("ul:first");this.lis=a("li:has(a[href])",this.list);this.anchors=this.lis.map(function(){return a("a",this)[0]});this.panels=a([]);var p=this,d=this.options;var c=/^#.+/;this.anchors.each(function(r,o){var q=a(o).attr("href");var s=q.split("#")[0],u;if(s&&(s===location.toString().split("#")[0]||(u=a("base")[0])&&s===u.href)){q=o.hash;o.href=q}if(c.test(q)){p.panels=p.panels.add(p._sanitizeSelector(q))}else{if(q!="#"){a.data(o,"href.tabs",q);a.data(o,"load.tabs",q.replace(/#.*$/,""));var w=p._tabId(o);o.href="#"+w;var v=a("#"+w);if(!v.length){v=a(d.panelTemplate).attr("id",w).addClass("ui-tabs-panel ui-widget-content ui-corner-bottom").insertAfter(p.panels[r-1]||p.list);v.data("destroy.tabs",true)}p.panels=p.panels.add(v)}else{d.disabled.push(r)}}});if(n){this.element.addClass("ui-tabs ui-widget ui-widget-content ui-corner-all");this.list.addClass("ui-tabs-nav ui-helper-reset ui-helper-clearfix ui-widget-header ui-corner-all");this.lis.addClass("ui-state-default ui-corner-top");this.panels.addClass("ui-tabs-panel ui-widget-content ui-corner-bottom");if(d.selected===undefined){if(location.hash){this.anchors.each(function(q,o){if(o.hash==location.hash){d.selected=q;return false}})}if(typeof d.selected!="number"&&d.cookie){d.selected=parseInt(p._cookie(),10)}if(typeof d.selected!="number"&&this.lis.filter(".ui-tabs-selected").length){d.selected=this.lis.index(this.lis.filter(".ui-tabs-selected"))}d.selected=d.selected||0}else{if(d.selected===null){d.selected=-1}}d.selected=((d.selected>=0&&this.anchors[d.selected])||d.selected<0)?d.selected:0;d.disabled=a.unique(d.disabled.concat(a.map(this.lis.filter(".ui-state-disabled"),function(q,o){return p.lis.index(q)}))).sort();if(a.inArray(d.selected,d.disabled)!=-1){d.disabled.splice(a.inArray(d.selected,d.disabled),1)}this.panels.addClass("ui-tabs-hide");this.lis.removeClass("ui-tabs-selected ui-state-active");if(d.selected>=0&&this.anchors.length){this.panels.eq(d.selected).removeClass("ui-tabs-hide");this.lis.eq(d.selected).addClass("ui-tabs-selected ui-state-active");p.element.queue("tabs",function(){p._trigger("show",null,p._ui(p.anchors[d.selected],p.panels[d.selected]))});this.load(d.selected)}a(window).bind("unload",function(){p.lis.add(p.anchors).unbind(".tabs");p.lis=p.anchors=p.panels=null})}else{d.selected=this.lis.index(this.lis.filter(".ui-tabs-selected"))}this.element[d.collapsible?"addClass":"removeClass"]("ui-tabs-collapsible");if(d.cookie){this._cookie(d.selected,d.cookie)}for(var g=0,m;(m=this.lis[g]);g++){a(m)[a.inArray(g,d.disabled)!=-1&&!a(m).hasClass("ui-tabs-selected")?"addClass":"removeClass"]("ui-state-disabled")}if(d.cache===false){this.anchors.removeData("cache.tabs")}this.lis.add(this.anchors).unbind(".tabs");if(d.event!="mouseover"){var f=function(o,i){if(i.is(":not(.ui-state-disabled)")){i.addClass("ui-state-"+o)}};var j=function(o,i){i.removeClass("ui-state-"+o)};this.lis.bind("mouseover.tabs",function(){f("hover",a(this))});this.lis.bind("mouseout.tabs",function(){j("hover",a(this))});this.anchors.bind("focus.tabs",function(){f("focus",a(this).closest("li"))});this.anchors.bind("blur.tabs",function(){j("focus",a(this).closest("li"))})}var b,h;if(d.fx){if(a.isArray(d.fx)){b=d.fx[0];h=d.fx[1]}else{b=h=d.fx}}function e(i,o){i.css({display:""});if(a.browser.msie&&o.opacity){i[0].style.removeAttribute("filter")}}var k=h?function(i,o){a(i).closest("li").removeClass("ui-state-default").addClass("ui-tabs-selected ui-state-active");o.hide().removeClass("ui-tabs-hide").animate(h,h.duration||"normal",function(){e(o,h);p._trigger("show",null,p._ui(i,o[0]))})}:function(i,o){a(i).closest("li").removeClass("ui-state-default").addClass("ui-tabs-selected ui-state-active");o.removeClass("ui-tabs-hide");p._trigger("show",null,p._ui(i,o[0]))};var l=b?function(o,i){i.animate(b,b.duration||"normal",function(){p.lis.removeClass("ui-tabs-selected ui-state-active").addClass("ui-state-default");i.addClass("ui-tabs-hide");e(i,b);p.element.dequeue("tabs")})}:function(o,i,q){p.lis.removeClass("ui-tabs-selected ui-state-active").addClass("ui-state-default");i.addClass("ui-tabs-hide");p.element.dequeue("tabs")};this.anchors.bind(d.event+".tabs",function(){var o=this,r=a(this).closest("li"),i=p.panels.filter(":not(.ui-tabs-hide)"),q=a(p._sanitizeSelector(this.hash));if((r.hasClass("ui-tabs-selected")&&!d.collapsible)||r.hasClass("ui-state-disabled")||r.hasClass("ui-state-processing")||p._trigger("select",null,p._ui(this,q[0]))===false){this.blur();return false}d.selected=p.anchors.index(this);p.abort();if(d.collapsible){if(r.hasClass("ui-tabs-selected")){d.selected=-1;if(d.cookie){p._cookie(d.selected,d.cookie)}p.element.queue("tabs",function(){l(o,i)}).dequeue("tabs");this.blur();return false}else{if(!i.length){if(d.cookie){p._cookie(d.selected,d.cookie)}p.element.queue("tabs",function(){k(o,q)});p.load(p.anchors.index(this));this.blur();return false}}}if(d.cookie){p._cookie(d.selected,d.cookie)}if(q.length){if(i.length){p.element.queue("tabs",function(){l(o,i)})}p.element.queue("tabs",function(){k(o,q)});p.load(p.anchors.index(this))}else{throw"jQuery UI Tabs: Mismatching fragment identifier."}if(a.browser.msie){this.blur()}});this.anchors.bind("click.tabs",function(){return false})},destroy:function(){var b=this.options;this.abort();this.element.unbind(".tabs").removeClass("ui-tabs ui-widget ui-widget-content ui-corner-all ui-tabs-collapsible").removeData("tabs");this.list.removeClass("ui-tabs-nav ui-helper-reset ui-helper-clearfix ui-widget-header ui-corner-all");this.anchors.each(function(){var c=a.data(this,"href.tabs");if(c){this.href=c}var d=a(this).unbind(".tabs");a.each(["href","load","cache"],function(e,f){d.removeData(f+".tabs")})});this.lis.unbind(".tabs").add(this.panels).each(function(){if(a.data(this,"destroy.tabs")){a(this).remove()}else{a(this).removeClass(["ui-state-default","ui-corner-top","ui-tabs-selected","ui-state-active","ui-state-hover","ui-state-focus","ui-state-disabled","ui-tabs-panel","ui-widget-content","ui-corner-bottom","ui-tabs-hide"].join(" "))}});if(b.cookie){this._cookie(null,b.cookie)}},add:function(e,d,c){if(c===undefined){c=this.anchors.length}var b=this,g=this.options,i=a(g.tabTemplate.replace(/#\{href\}/g,e).replace(/#\{label\}/g,d)),h=!e.indexOf("#")?e.replace("#",""):this._tabId(a("a",i)[0]);i.addClass("ui-state-default ui-corner-top").data("destroy.tabs",true);var f=a("#"+h);if(!f.length){f=a(g.panelTemplate).attr("id",h).data("destroy.tabs",true)}f.addClass("ui-tabs-panel ui-widget-content ui-corner-bottom ui-tabs-hide");if(c>=this.lis.length){i.appendTo(this.list);f.appendTo(this.list[0].parentNode)}else{i.insertBefore(this.lis[c]);f.insertBefore(this.panels[c])}g.disabled=a.map(g.disabled,function(k,j){return k>=c?++k:k});this._tabify();if(this.anchors.length==1){i.addClass("ui-tabs-selected ui-state-active");f.removeClass("ui-tabs-hide");this.element.queue("tabs",function(){b._trigger("show",null,b._ui(b.anchors[0],b.panels[0]))});this.load(0)}this._trigger("add",null,this._ui(this.anchors[c],this.panels[c]))},remove:function(b){var d=this.options,e=this.lis.eq(b).remove(),c=this.panels.eq(b).remove();if(e.hasClass("ui-tabs-selected")&&this.anchors.length>1){this.select(b+(b+1<this.anchors.length?1:-1))}d.disabled=a.map(a.grep(d.disabled,function(g,f){return g!=b}),function(g,f){return g>=b?--g:g});this._tabify();this._trigger("remove",null,this._ui(e.find("a")[0],c[0]))},enable:function(b){var c=this.options;if(a.inArray(b,c.disabled)==-1){return}this.lis.eq(b).removeClass("ui-state-disabled");c.disabled=a.grep(c.disabled,function(e,d){return e!=b});this._trigger("enable",null,this._ui(this.anchors[b],this.panels[b]))},disable:function(c){var b=this,d=this.options;if(c!=d.selected){this.lis.eq(c).addClass("ui-state-disabled");d.disabled.push(c);d.disabled.sort();this._trigger("disable",null,this._ui(this.anchors[c],this.panels[c]))}},select:function(b){if(typeof b=="string"){b=this.anchors.index(this.anchors.filter("[href$="+b+"]"))}else{if(b===null){b=-1}}if(b==-1&&this.options.collapsible){b=this.options.selected}this.anchors.eq(b).trigger(this.options.event+".tabs")},load:function(e){var c=this,g=this.options,b=this.anchors.eq(e)[0],d=a.data(b,"load.tabs");this.abort();if(!d||this.element.queue("tabs").length!==0&&a.data(b,"cache.tabs")){this.element.dequeue("tabs");return}this.lis.eq(e).addClass("ui-state-processing");if(g.spinner){var f=a("span",b);f.data("label.tabs",f.html()).html(g.spinner)}this.xhr=a.ajax(a.extend({},g.ajaxOptions,{url:d,success:function(i,h){a(c._sanitizeSelector(b.hash)).html(i);c._cleanup();if(g.cache){a.data(b,"cache.tabs",true)}c._trigger("load",null,c._ui(c.anchors[e],c.panels[e]));try{g.ajaxOptions.success(i,h)}catch(j){}c.element.dequeue("tabs")}}))},abort:function(){this.element.queue([]);this.panels.stop(false,true);if(this.xhr){this.xhr.abort();delete this.xhr}this._cleanup()},url:function(c,b){this.anchors.eq(c).removeData("cache.tabs").data("load.tabs",b)},length:function(){return this.anchors.length}});a.extend(a.ui.tabs,{version:"1.7.1",getter:"length",defaults:{ajaxOptions:null,cache:false,cookie:null,collapsible:false,disabled:[],event:"click",fx:null,idPrefix:"ui-tabs-",panelTemplate:"<div></div>",spinner:"<em>Loading&#8230;</em>",tabTemplate:'<li><a href="#{href}"><span>#{label}</span></a></li>'}});a.extend(a.ui.tabs.prototype,{rotation:null,rotate:function(d,f){var b=this,g=this.options;var c=b._rotate||(b._rotate=function(h){clearTimeout(b.rotation);b.rotation=setTimeout(function(){var i=g.selected;b.select(++i<b.anchors.length?i:0)},d);if(h){h.stopPropagation()}});var e=b._unrotate||(b._unrotate=!f?function(h){if(h.clientX){b.rotate(null)}}:function(h){t=g.selected;c()});if(d){this.element.bind("tabsshow",c);this.anchors.bind(g.event+".tabs",e);c()}else{clearTimeout(b.rotation);this.element.unbind("tabsshow",c);this.anchors.unbind(g.event+".tabs",e);delete this._rotate;delete this._unrotate}}})})(jQuery);;/*
 * jQuery UI Datepicker 1.7.1
 *
 * Copyright (c) 2009 AUTHORS.txt (http://jqueryui.com/about)
 * Dual licensed under the MIT (MIT-LICENSE.txt)
 * and GPL (GPL-LICENSE.txt) licenses.
 *
 * http://docs.jquery.com/UI/Datepicker
 *
 * Depends:
 *	ui.core.js
 */(function($){$.extend($.ui,{datepicker:{version:"1.7.1"}});var PROP_NAME="datepicker";function Datepicker(){this.debug=false;this._curInst=null;this._keyEvent=false;this._disabledInputs=[];this._datepickerShowing=false;this._inDialog=false;this._mainDivId="ui-datepicker-div";this._inlineClass="ui-datepicker-inline";this._appendClass="ui-datepicker-append";this._triggerClass="ui-datepicker-trigger";this._dialogClass="ui-datepicker-dialog";this._disableClass="ui-datepicker-disabled";this._unselectableClass="ui-datepicker-unselectable";this._currentClass="ui-datepicker-current-day";this._dayOverClass="ui-datepicker-days-cell-over";this.regional=[];this.regional[""]={closeText:"Done",prevText:"Prev",nextText:"Next",currentText:"Today",monthNames:["January","February","March","April","May","June","July","August","September","October","November","December"],monthNamesShort:["Jan","Feb","Mar","Apr","May","Jun","Jul","Aug","Sep","Oct","Nov","Dec"],dayNames:["Sunday","Monday","Tuesday","Wednesday","Thursday","Friday","Saturday"],dayNamesShort:["Sun","Mon","Tue","Wed","Thu","Fri","Sat"],dayNamesMin:["Su","Mo","Tu","We","Th","Fr","Sa"],dateFormat:"mm/dd/yy",firstDay:0,isRTL:false};this._defaults={showOn:"focus",showAnim:"show",showOptions:{},defaultDate:null,appendText:"",buttonText:"...",buttonImage:"",buttonImageOnly:false,hideIfNoPrevNext:false,navigationAsDateFormat:false,gotoCurrent:false,changeMonth:false,changeYear:false,showMonthAfterYear:false,yearRange:"-10:+10",showOtherMonths:false,calculateWeek:this.iso8601Week,shortYearCutoff:"+10",minDate:null,maxDate:null,duration:"normal",beforeShowDay:null,beforeShow:null,onSelect:null,onChangeMonthYear:null,onClose:null,numberOfMonths:1,showCurrentAtPos:0,stepMonths:1,stepBigMonths:12,altField:"",altFormat:"",constrainInput:true,showButtonPanel:false};$.extend(this._defaults,this.regional[""]);this.dpDiv=$('<div id="'+this._mainDivId+'" class="ui-datepicker ui-widget ui-widget-content ui-helper-clearfix ui-corner-all ui-helper-hidden-accessible"></div>')}$.extend(Datepicker.prototype,{markerClassName:"hasDatepicker",log:function(){if(this.debug){console.log.apply("",arguments)}},setDefaults:function(settings){extendRemove(this._defaults,settings||{});return this},_attachDatepicker:function(target,settings){var inlineSettings=null;for(var attrName in this._defaults){var attrValue=target.getAttribute("date:"+attrName);if(attrValue){inlineSettings=inlineSettings||{};try{inlineSettings[attrName]=eval(attrValue)}catch(err){inlineSettings[attrName]=attrValue}}}var nodeName=target.nodeName.toLowerCase();var inline=(nodeName=="div"||nodeName=="span");if(!target.id){target.id="dp"+(++this.uuid)}var inst=this._newInst($(target),inline);inst.settings=$.extend({},settings||{},inlineSettings||{});if(nodeName=="input"){this._connectDatepicker(target,inst)}else{if(inline){this._inlineDatepicker(target,inst)}}},_newInst:function(target,inline){var id=target[0].id.replace(/([:\[\]\.])/g,"\\\\$1");return{id:id,input:target,selectedDay:0,selectedMonth:0,selectedYear:0,drawMonth:0,drawYear:0,inline:inline,dpDiv:(!inline?this.dpDiv:$('<div class="'+this._inlineClass+' ui-datepicker ui-widget ui-widget-content ui-helper-clearfix ui-corner-all"></div>'))}},_connectDatepicker:function(target,inst){var input=$(target);inst.trigger=$([]);if(input.hasClass(this.markerClassName)){return}var appendText=this._get(inst,"appendText");var isRTL=this._get(inst,"isRTL");if(appendText){input[isRTL?"before":"after"]('<span class="'+this._appendClass+'">'+appendText+"</span>")}var showOn=this._get(inst,"showOn");if(showOn=="focus"||showOn=="both"){input.focus(this._showDatepicker)}if(showOn=="button"||showOn=="both"){var buttonText=this._get(inst,"buttonText");var buttonImage=this._get(inst,"buttonImage");inst.trigger=$(this._get(inst,"buttonImageOnly")?$("<img/>").addClass(this._triggerClass).attr({src:buttonImage,alt:buttonText,title:buttonText}):$('<button type="button"></button>').addClass(this._triggerClass).html(buttonImage==""?buttonText:$("<img/>").attr({src:buttonImage,alt:buttonText,title:buttonText})));input[isRTL?"before":"after"](inst.trigger);inst.trigger.click(function(){if($.datepicker._datepickerShowing&&$.datepicker._lastInput==target){$.datepicker._hideDatepicker()}else{$.datepicker._showDatepicker(target)}return false})}input.addClass(this.markerClassName).keydown(this._doKeyDown).keypress(this._doKeyPress).bind("setData.datepicker",function(event,key,value){inst.settings[key]=value}).bind("getData.datepicker",function(event,key){return this._get(inst,key)});$.data(target,PROP_NAME,inst)},_inlineDatepicker:function(target,inst){var divSpan=$(target);if(divSpan.hasClass(this.markerClassName)){return}divSpan.addClass(this.markerClassName).append(inst.dpDiv).bind("setData.datepicker",function(event,key,value){inst.settings[key]=value}).bind("getData.datepicker",function(event,key){return this._get(inst,key)});$.data(target,PROP_NAME,inst);this._setDate(inst,this._getDefaultDate(inst));this._updateDatepicker(inst);this._updateAlternate(inst)},_dialogDatepicker:function(input,dateText,onSelect,settings,pos){var inst=this._dialogInst;if(!inst){var id="dp"+(++this.uuid);this._dialogInput=$('<input type="text" id="'+id+'" size="1" style="position: absolute; top: -100px;"/>');this._dialogInput.keydown(this._doKeyDown);$("body").append(this._dialogInput);inst=this._dialogInst=this._newInst(this._dialogInput,false);inst.settings={};$.data(this._dialogInput[0],PROP_NAME,inst)}extendRemove(inst.settings,settings||{});this._dialogInput.val(dateText);this._pos=(pos?(pos.length?pos:[pos.pageX,pos.pageY]):null);if(!this._pos){var browserWidth=window.innerWidth||document.documentElement.clientWidth||document.body.clientWidth;var browserHeight=window.innerHeight||document.documentElement.clientHeight||document.body.clientHeight;var scrollX=document.documentElement.scrollLeft||document.body.scrollLeft;var scrollY=document.documentElement.scrollTop||document.body.scrollTop;this._pos=[(browserWidth/2)-100+scrollX,(browserHeight/2)-150+scrollY]}this._dialogInput.css("left",this._pos[0]+"px").css("top",this._pos[1]+"px");inst.settings.onSelect=onSelect;this._inDialog=true;this.dpDiv.addClass(this._dialogClass);this._showDatepicker(this._dialogInput[0]);if($.blockUI){$.blockUI(this.dpDiv)}$.data(this._dialogInput[0],PROP_NAME,inst);return this},_destroyDatepicker:function(target){var $target=$(target);var inst=$.data(target,PROP_NAME);if(!$target.hasClass(this.markerClassName)){return}var nodeName=target.nodeName.toLowerCase();$.removeData(target,PROP_NAME);if(nodeName=="input"){inst.trigger.remove();$target.siblings("."+this._appendClass).remove().end().removeClass(this.markerClassName).unbind("focus",this._showDatepicker).unbind("keydown",this._doKeyDown).unbind("keypress",this._doKeyPress)}else{if(nodeName=="div"||nodeName=="span"){$target.removeClass(this.markerClassName).empty()}}},_enableDatepicker:function(target){var $target=$(target);var inst=$.data(target,PROP_NAME);if(!$target.hasClass(this.markerClassName)){return}var nodeName=target.nodeName.toLowerCase();if(nodeName=="input"){target.disabled=false;inst.trigger.filter("button").each(function(){this.disabled=false}).end().filter("img").css({opacity:"1.0",cursor:""})}else{if(nodeName=="div"||nodeName=="span"){var inline=$target.children("."+this._inlineClass);inline.children().removeClass("ui-state-disabled")}}this._disabledInputs=$.map(this._disabledInputs,function(value){return(value==target?null:value)})},_disableDatepicker:function(target){var $target=$(target);var inst=$.data(target,PROP_NAME);if(!$target.hasClass(this.markerClassName)){return}var nodeName=target.nodeName.toLowerCase();if(nodeName=="input"){target.disabled=true;inst.trigger.filter("button").each(function(){this.disabled=true}).end().filter("img").css({opacity:"0.5",cursor:"default"})}else{if(nodeName=="div"||nodeName=="span"){var inline=$target.children("."+this._inlineClass);inline.children().addClass("ui-state-disabled")}}this._disabledInputs=$.map(this._disabledInputs,function(value){return(value==target?null:value)});this._disabledInputs[this._disabledInputs.length]=target},_isDisabledDatepicker:function(target){if(!target){return false}for(var i=0;i<this._disabledInputs.length;i++){if(this._disabledInputs[i]==target){return true}}return false},_getInst:function(target){try{return $.data(target,PROP_NAME)}catch(err){throw"Missing instance data for this datepicker"}},_optionDatepicker:function(target,name,value){var settings=name||{};if(typeof name=="string"){settings={};settings[name]=value}var inst=this._getInst(target);if(inst){if(this._curInst==inst){this._hideDatepicker(null)}extendRemove(inst.settings,settings);var date=new Date();extendRemove(inst,{rangeStart:null,endDay:null,endMonth:null,endYear:null,selectedDay:date.getDate(),selectedMonth:date.getMonth(),selectedYear:date.getFullYear(),currentDay:date.getDate(),currentMonth:date.getMonth(),currentYear:date.getFullYear(),drawMonth:date.getMonth(),drawYear:date.getFullYear()});this._updateDatepicker(inst)}},_changeDatepicker:function(target,name,value){this._optionDatepicker(target,name,value)},_refreshDatepicker:function(target){var inst=this._getInst(target);if(inst){this._updateDatepicker(inst)}},_setDateDatepicker:function(target,date,endDate){var inst=this._getInst(target);if(inst){this._setDate(inst,date,endDate);this._updateDatepicker(inst);this._updateAlternate(inst)}},_getDateDatepicker:function(target){var inst=this._getInst(target);if(inst&&!inst.inline){this._setDateFromField(inst)}return(inst?this._getDate(inst):null)},_doKeyDown:function(event){var inst=$.datepicker._getInst(event.target);var handled=true;var isRTL=inst.dpDiv.is(".ui-datepicker-rtl");inst._keyEvent=true;if($.datepicker._datepickerShowing){switch(event.keyCode){case 9:$.datepicker._hideDatepicker(null,"");break;case 13:var sel=$("td."+$.datepicker._dayOverClass+", td."+$.datepicker._currentClass,inst.dpDiv);if(sel[0]){$.datepicker._selectDay(event.target,inst.selectedMonth,inst.selectedYear,sel[0])}else{$.datepicker._hideDatepicker(null,$.datepicker._get(inst,"duration"))}return false;break;case 27:$.datepicker._hideDatepicker(null,$.datepicker._get(inst,"duration"));break;case 33:$.datepicker._adjustDate(event.target,(event.ctrlKey?-$.datepicker._get(inst,"stepBigMonths"):-$.datepicker._get(inst,"stepMonths")),"M");break;case 34:$.datepicker._adjustDate(event.target,(event.ctrlKey?+$.datepicker._get(inst,"stepBigMonths"):+$.datepicker._get(inst,"stepMonths")),"M");break;case 35:if(event.ctrlKey||event.metaKey){$.datepicker._clearDate(event.target)}handled=event.ctrlKey||event.metaKey;break;case 36:if(event.ctrlKey||event.metaKey){$.datepicker._gotoToday(event.target)}handled=event.ctrlKey||event.metaKey;break;case 37:if(event.ctrlKey||event.metaKey){$.datepicker._adjustDate(event.target,(isRTL?+1:-1),"D")}handled=event.ctrlKey||event.metaKey;if(event.originalEvent.altKey){$.datepicker._adjustDate(event.target,(event.ctrlKey?-$.datepicker._get(inst,"stepBigMonths"):-$.datepicker._get(inst,"stepMonths")),"M")}break;case 38:if(event.ctrlKey||event.metaKey){$.datepicker._adjustDate(event.target,-7,"D")}handled=event.ctrlKey||event.metaKey;break;case 39:if(event.ctrlKey||event.metaKey){$.datepicker._adjustDate(event.target,(isRTL?-1:+1),"D")}handled=event.ctrlKey||event.metaKey;if(event.originalEvent.altKey){$.datepicker._adjustDate(event.target,(event.ctrlKey?+$.datepicker._get(inst,"stepBigMonths"):+$.datepicker._get(inst,"stepMonths")),"M")}break;case 40:if(event.ctrlKey||event.metaKey){$.datepicker._adjustDate(event.target,+7,"D")}handled=event.ctrlKey||event.metaKey;break;default:handled=false}}else{if(event.keyCode==36&&event.ctrlKey){$.datepicker._showDatepicker(this)}else{handled=false}}if(handled){event.preventDefault();event.stopPropagation()}},_doKeyPress:function(event){var inst=$.datepicker._getInst(event.target);if($.datepicker._get(inst,"constrainInput")){var chars=$.datepicker._possibleChars($.datepicker._get(inst,"dateFormat"));var chr=String.fromCharCode(event.charCode==undefined?event.keyCode:event.charCode);return event.ctrlKey||(chr<" "||!chars||chars.indexOf(chr)>-1)}},_showDatepicker:function(input){input=input.target||input;if(input.nodeName.toLowerCase()!="input"){input=$("input",input.parentNode)[0]}if($.datepicker._isDisabledDatepicker(input)||$.datepicker._lastInput==input){return}var inst=$.datepicker._getInst(input);var beforeShow=$.datepicker._get(inst,"beforeShow");extendRemove(inst.settings,(beforeShow?beforeShow.apply(input,[input,inst]):{}));$.datepicker._hideDatepicker(null,"");$.datepicker._lastInput=input;$.datepicker._setDateFromField(inst);if($.datepicker._inDialog){input.value=""}if(!$.datepicker._pos){$.datepicker._pos=$.datepicker._findPos(input);$.datepicker._pos[1]+=input.offsetHeight}var isFixed=false;$(input).parents().each(function(){isFixed|=$(this).css("position")=="fixed";return !isFixed});if(isFixed&&$.browser.opera){$.datepicker._pos[0]-=document.documentElement.scrollLeft;$.datepicker._pos[1]-=document.documentElement.scrollTop}var offset={left:$.datepicker._pos[0],top:$.datepicker._pos[1]};$.datepicker._pos=null;inst.rangeStart=null;inst.dpDiv.css({position:"absolute",display:"block",top:"-1000px"});$.datepicker._updateDatepicker(inst);offset=$.datepicker._checkOffset(inst,offset,isFixed);inst.dpDiv.css({position:($.datepicker._inDialog&&$.blockUI?"static":(isFixed?"fixed":"absolute")),display:"none",left:offset.left+"px",top:offset.top+"px"});if(!inst.inline){var showAnim=$.datepicker._get(inst,"showAnim")||"show";var duration=$.datepicker._get(inst,"duration");var postProcess=function(){$.datepicker._datepickerShowing=true;if($.browser.msie&&parseInt($.browser.version,10)<7){$("iframe.ui-datepicker-cover").css({width:inst.dpDiv.width()+4,height:inst.dpDiv.height()+4})}};if($.effects&&$.effects[showAnim]){inst.dpDiv.show(showAnim,$.datepicker._get(inst,"showOptions"),duration,postProcess)}else{inst.dpDiv[showAnim](duration,postProcess)}if(duration==""){postProcess()}if(inst.input[0].type!="hidden"){inst.input[0].focus()}$.datepicker._curInst=inst}},_updateDatepicker:function(inst){var dims={width:inst.dpDiv.width()+4,height:inst.dpDiv.height()+4};var self=this;inst.dpDiv.empty().append(this._generateHTML(inst)).find("iframe.ui-datepicker-cover").css({width:dims.width,height:dims.height}).end().find("button, .ui-datepicker-prev, .ui-datepicker-next, .ui-datepicker-calendar td a").bind("mouseout",function(){$(this).removeClass("ui-state-hover");if(this.className.indexOf("ui-datepicker-prev")!=-1){$(this).removeClass("ui-datepicker-prev-hover")}if(this.className.indexOf("ui-datepicker-next")!=-1){$(this).removeClass("ui-datepicker-next-hover")}}).bind("mouseover",function(){if(!self._isDisabledDatepicker(inst.inline?inst.dpDiv.parent()[0]:inst.input[0])){$(this).parents(".ui-datepicker-calendar").find("a").removeClass("ui-state-hover");$(this).addClass("ui-state-hover");if(this.className.indexOf("ui-datepicker-prev")!=-1){$(this).addClass("ui-datepicker-prev-hover")}if(this.className.indexOf("ui-datepicker-next")!=-1){$(this).addClass("ui-datepicker-next-hover")}}}).end().find("."+this._dayOverClass+" a").trigger("mouseover").end();var numMonths=this._getNumberOfMonths(inst);var cols=numMonths[1];var width=17;if(cols>1){inst.dpDiv.addClass("ui-datepicker-multi-"+cols).css("width",(width*cols)+"em")}else{inst.dpDiv.removeClass("ui-datepicker-multi-2 ui-datepicker-multi-3 ui-datepicker-multi-4").width("")}inst.dpDiv[(numMonths[0]!=1||numMonths[1]!=1?"add":"remove")+"Class"]("ui-datepicker-multi");inst.dpDiv[(this._get(inst,"isRTL")?"add":"remove")+"Class"]("ui-datepicker-rtl");if(inst.input&&inst.input[0].type!="hidden"&&inst==$.datepicker._curInst){$(inst.input[0]).focus()}},_checkOffset:function(inst,offset,isFixed){var dpWidth=inst.dpDiv.outerWidth();var dpHeight=inst.dpDiv.outerHeight();var inputWidth=inst.input?inst.input.outerWidth():0;var inputHeight=inst.input?inst.input.outerHeight():0;var viewWidth=(window.innerWidth||document.documentElement.clientWidth||document.body.clientWidth)+$(document).scrollLeft();var viewHeight=(window.innerHeight||document.documentElement.clientHeight||document.body.clientHeight)+$(document).scrollTop();offset.left-=(this._get(inst,"isRTL")?(dpWidth-inputWidth):0);offset.left-=(isFixed&&offset.left==inst.input.offset().left)?$(document).scrollLeft():0;offset.top-=(isFixed&&offset.top==(inst.input.offset().top+inputHeight))?$(document).scrollTop():0;offset.left-=(offset.left+dpWidth>viewWidth&&viewWidth>dpWidth)?Math.abs(offset.left+dpWidth-viewWidth):0;offset.top-=(offset.top+dpHeight>viewHeight&&viewHeight>dpHeight)?Math.abs(offset.top+dpHeight+inputHeight*2-viewHeight):0;return offset},_findPos:function(obj){while(obj&&(obj.type=="hidden"||obj.nodeType!=1)){obj=obj.nextSibling}var position=$(obj).offset();return[position.left,position.top]},_hideDatepicker:function(input,duration){var inst=this._curInst;if(!inst||(input&&inst!=$.data(input,PROP_NAME))){return}if(inst.stayOpen){this._selectDate("#"+inst.id,this._formatDate(inst,inst.currentDay,inst.currentMonth,inst.currentYear))}inst.stayOpen=false;if(this._datepickerShowing){duration=(duration!=null?duration:this._get(inst,"duration"));var showAnim=this._get(inst,"showAnim");var postProcess=function(){$.datepicker._tidyDialog(inst)};if(duration!=""&&$.effects&&$.effects[showAnim]){inst.dpDiv.hide(showAnim,$.datepicker._get(inst,"showOptions"),duration,postProcess)}else{inst.dpDiv[(duration==""?"hide":(showAnim=="slideDown"?"slideUp":(showAnim=="fadeIn"?"fadeOut":"hide")))](duration,postProcess)}if(duration==""){this._tidyDialog(inst)}var onClose=this._get(inst,"onClose");if(onClose){onClose.apply((inst.input?inst.input[0]:null),[(inst.input?inst.input.val():""),inst])}this._datepickerShowing=false;this._lastInput=null;if(this._inDialog){this._dialogInput.css({position:"absolute",left:"0",top:"-100px"});if($.blockUI){$.unblockUI();$("body").append(this.dpDiv)}}this._inDialog=false}this._curInst=null},_tidyDialog:function(inst){inst.dpDiv.removeClass(this._dialogClass).unbind(".ui-datepicker-calendar")},_checkExternalClick:function(event){if(!$.datepicker._curInst){return}var $target=$(event.target);if(($target.parents("#"+$.datepicker._mainDivId).length==0)&&!$target.hasClass($.datepicker.markerClassName)&&!$target.hasClass($.datepicker._triggerClass)&&$.datepicker._datepickerShowing&&!($.datepicker._inDialog&&$.blockUI)){$.datepicker._hideDatepicker(null,"")}},_adjustDate:function(id,offset,period){var target=$(id);var inst=this._getInst(target[0]);if(this._isDisabledDatepicker(target[0])){return}this._adjustInstDate(inst,offset+(period=="M"?this._get(inst,"showCurrentAtPos"):0),period);this._updateDatepicker(inst)},_gotoToday:function(id){var target=$(id);var inst=this._getInst(target[0]);if(this._get(inst,"gotoCurrent")&&inst.currentDay){inst.selectedDay=inst.currentDay;inst.drawMonth=inst.selectedMonth=inst.currentMonth;inst.drawYear=inst.selectedYear=inst.currentYear}else{var date=new Date();inst.selectedDay=date.getDate();inst.drawMonth=inst.selectedMonth=date.getMonth();inst.drawYear=inst.selectedYear=date.getFullYear()}this._notifyChange(inst);this._adjustDate(target)},_selectMonthYear:function(id,select,period){var target=$(id);var inst=this._getInst(target[0]);inst._selectingMonthYear=false;inst["selected"+(period=="M"?"Month":"Year")]=inst["draw"+(period=="M"?"Month":"Year")]=parseInt(select.options[select.selectedIndex].value,10);this._notifyChange(inst);this._adjustDate(target)},_clickMonthYear:function(id){var target=$(id);var inst=this._getInst(target[0]);if(inst.input&&inst._selectingMonthYear&&!$.browser.msie){inst.input[0].focus()}inst._selectingMonthYear=!inst._selectingMonthYear},_selectDay:function(id,month,year,td){var target=$(id);if($(td).hasClass(this._unselectableClass)||this._isDisabledDatepicker(target[0])){return}var inst=this._getInst(target[0]);inst.selectedDay=inst.currentDay=$("a",td).html();inst.selectedMonth=inst.currentMonth=month;inst.selectedYear=inst.currentYear=year;if(inst.stayOpen){inst.endDay=inst.endMonth=inst.endYear=null}this._selectDate(id,this._formatDate(inst,inst.currentDay,inst.currentMonth,inst.currentYear));if(inst.stayOpen){inst.rangeStart=this._daylightSavingAdjust(new Date(inst.currentYear,inst.currentMonth,inst.currentDay));this._updateDatepicker(inst)}},_clearDate:function(id){var target=$(id);var inst=this._getInst(target[0]);inst.stayOpen=false;inst.endDay=inst.endMonth=inst.endYear=inst.rangeStart=null;this._selectDate(target,"")},_selectDate:function(id,dateStr){var target=$(id);var inst=this._getInst(target[0]);dateStr=(dateStr!=null?dateStr:this._formatDate(inst));if(inst.input){inst.input.val(dateStr)}this._updateAlternate(inst);var onSelect=this._get(inst,"onSelect");if(onSelect){onSelect.apply((inst.input?inst.input[0]:null),[dateStr,inst])}else{if(inst.input){inst.input.trigger("change")}}if(inst.inline){this._updateDatepicker(inst)}else{if(!inst.stayOpen){this._hideDatepicker(null,this._get(inst,"duration"));this._lastInput=inst.input[0];if(typeof(inst.input[0])!="object"){inst.input[0].focus()}this._lastInput=null}}},_updateAlternate:function(inst){var altField=this._get(inst,"altField");if(altField){var altFormat=this._get(inst,"altFormat")||this._get(inst,"dateFormat");var date=this._getDate(inst);dateStr=this.formatDate(altFormat,date,this._getFormatConfig(inst));$(altField).each(function(){$(this).val(dateStr)})}},noWeekends:function(date){var day=date.getDay();return[(day>0&&day<6),""]},iso8601Week:function(date){var checkDate=new Date(date.getFullYear(),date.getMonth(),date.getDate());var firstMon=new Date(checkDate.getFullYear(),1-1,4);var firstDay=firstMon.getDay()||7;firstMon.setDate(firstMon.getDate()+1-firstDay);if(firstDay<4&&checkDate<firstMon){checkDate.setDate(checkDate.getDate()-3);return $.datepicker.iso8601Week(checkDate)}else{if(checkDate>new Date(checkDate.getFullYear(),12-1,28)){firstDay=new Date(checkDate.getFullYear()+1,1-1,4).getDay()||7;if(firstDay>4&&(checkDate.getDay()||7)<firstDay-3){return 1}}}return Math.floor(((checkDate-firstMon)/86400000)/7)+1},parseDate:function(format,value,settings){if(format==null||value==null){throw"Invalid arguments"}value=(typeof value=="object"?value.toString():value+"");if(value==""){return null}var shortYearCutoff=(settings?settings.shortYearCutoff:null)||this._defaults.shortYearCutoff;var dayNamesShort=(settings?settings.dayNamesShort:null)||this._defaults.dayNamesShort;var dayNames=(settings?settings.dayNames:null)||this._defaults.dayNames;var monthNamesShort=(settings?settings.monthNamesShort:null)||this._defaults.monthNamesShort;var monthNames=(settings?settings.monthNames:null)||this._defaults.monthNames;var year=-1;var month=-1;var day=-1;var doy=-1;var literal=false;var lookAhead=function(match){var matches=(iFormat+1<format.length&&format.charAt(iFormat+1)==match);if(matches){iFormat++}return matches};var getNumber=function(match){lookAhead(match);var origSize=(match=="@"?14:(match=="y"?4:(match=="o"?3:2)));var size=origSize;var num=0;while(size>0&&iValue<value.length&&value.charAt(iValue)>="0"&&value.charAt(iValue)<="9"){num=num*10+parseInt(value.charAt(iValue++),10);size--}if(size==origSize){throw"Missing number at position "+iValue}return num};var getName=function(match,shortNames,longNames){var names=(lookAhead(match)?longNames:shortNames);var size=0;for(var j=0;j<names.length;j++){size=Math.max(size,names[j].length)}var name="";var iInit=iValue;while(size>0&&iValue<value.length){name+=value.charAt(iValue++);for(var i=0;i<names.length;i++){if(name==names[i]){return i+1}}size--}throw"Unknown name at position "+iInit};var checkLiteral=function(){if(value.charAt(iValue)!=format.charAt(iFormat)){throw"Unexpected literal at position "+iValue}iValue++};var iValue=0;for(var iFormat=0;iFormat<format.length;iFormat++){if(literal){if(format.charAt(iFormat)=="'"&&!lookAhead("'")){literal=false}else{checkLiteral()}}else{switch(format.charAt(iFormat)){case"d":day=getNumber("d");break;case"D":getName("D",dayNamesShort,dayNames);break;case"o":doy=getNumber("o");break;case"m":month=getNumber("m");break;case"M":month=getName("M",monthNamesShort,monthNames);break;case"y":year=getNumber("y");break;case"@":var date=new Date(getNumber("@"));year=date.getFullYear();month=date.getMonth()+1;day=date.getDate();break;case"'":if(lookAhead("'")){checkLiteral()}else{literal=true}break;default:checkLiteral()}}}if(year==-1){year=new Date().getFullYear()}else{if(year<100){year+=new Date().getFullYear()-new Date().getFullYear()%100+(year<=shortYearCutoff?0:-100)}}if(doy>-1){month=1;day=doy;do{var dim=this._getDaysInMonth(year,month-1);if(day<=dim){break}month++;day-=dim}while(true)}var date=this._daylightSavingAdjust(new Date(year,month-1,day));if(date.getFullYear()!=year||date.getMonth()+1!=month||date.getDate()!=day){throw"Invalid date"}return date},ATOM:"yy-mm-dd",COOKIE:"D, dd M yy",ISO_8601:"yy-mm-dd",RFC_822:"D, d M y",RFC_850:"DD, dd-M-y",RFC_1036:"D, d M y",RFC_1123:"D, d M yy",RFC_2822:"D, d M yy",RSS:"D, d M y",TIMESTAMP:"@",W3C:"yy-mm-dd",formatDate:function(format,date,settings){if(!date){return""}var dayNamesShort=(settings?settings.dayNamesShort:null)||this._defaults.dayNamesShort;var dayNames=(settings?settings.dayNames:null)||this._defaults.dayNames;var monthNamesShort=(settings?settings.monthNamesShort:null)||this._defaults.monthNamesShort;var monthNames=(settings?settings.monthNames:null)||this._defaults.monthNames;var lookAhead=function(match){var matches=(iFormat+1<format.length&&format.charAt(iFormat+1)==match);if(matches){iFormat++}return matches};var formatNumber=function(match,value,len){var num=""+value;if(lookAhead(match)){while(num.length<len){num="0"+num}}return num};var formatName=function(match,value,shortNames,longNames){return(lookAhead(match)?longNames[value]:shortNames[value])};var output="";var literal=false;if(date){for(var iFormat=0;iFormat<format.length;iFormat++){if(literal){if(format.charAt(iFormat)=="'"&&!lookAhead("'")){literal=false}else{output+=format.charAt(iFormat)}}else{switch(format.charAt(iFormat)){case"d":output+=formatNumber("d",date.getDate(),2);break;case"D":output+=formatName("D",date.getDay(),dayNamesShort,dayNames);break;case"o":var doy=date.getDate();for(var m=date.getMonth()-1;m>=0;m--){doy+=this._getDaysInMonth(date.getFullYear(),m)}output+=formatNumber("o",doy,3);break;case"m":output+=formatNumber("m",date.getMonth()+1,2);break;case"M":output+=formatName("M",date.getMonth(),monthNamesShort,monthNames);break;case"y":output+=(lookAhead("y")?date.getFullYear():(date.getYear()%100<10?"0":"")+date.getYear()%100);break;case"@":output+=date.getTime();break;case"'":if(lookAhead("'")){output+="'"}else{literal=true}break;default:output+=format.charAt(iFormat)}}}}return output},_possibleChars:function(format){var chars="";var literal=false;for(var iFormat=0;iFormat<format.length;iFormat++){if(literal){if(format.charAt(iFormat)=="'"&&!lookAhead("'")){literal=false}else{chars+=format.charAt(iFormat)}}else{switch(format.charAt(iFormat)){case"d":case"m":case"y":case"@":chars+="0123456789";break;case"D":case"M":return null;case"'":if(lookAhead("'")){chars+="'"}else{literal=true}break;default:chars+=format.charAt(iFormat)}}}return chars},_get:function(inst,name){return inst.settings[name]!==undefined?inst.settings[name]:this._defaults[name]},_setDateFromField:function(inst){var dateFormat=this._get(inst,"dateFormat");var dates=inst.input?inst.input.val():null;inst.endDay=inst.endMonth=inst.endYear=null;var date=defaultDate=this._getDefaultDate(inst);var settings=this._getFormatConfig(inst);try{date=this.parseDate(dateFormat,dates,settings)||defaultDate}catch(event){this.log(event);date=defaultDate}inst.selectedDay=date.getDate();inst.drawMonth=inst.selectedMonth=date.getMonth();inst.drawYear=inst.selectedYear=date.getFullYear();inst.currentDay=(dates?date.getDate():0);inst.currentMonth=(dates?date.getMonth():0);inst.currentYear=(dates?date.getFullYear():0);this._adjustInstDate(inst)},_getDefaultDate:function(inst){var date=this._determineDate(this._get(inst,"defaultDate"),new Date());var minDate=this._getMinMaxDate(inst,"min",true);var maxDate=this._getMinMaxDate(inst,"max");date=(minDate&&date<minDate?minDate:date);date=(maxDate&&date>maxDate?maxDate:date);return date},_determineDate:function(date,defaultDate){var offsetNumeric=function(offset){var date=new Date();date.setDate(date.getDate()+offset);return date};var offsetString=function(offset,getDaysInMonth){var date=new Date();var year=date.getFullYear();var month=date.getMonth();var day=date.getDate();var pattern=/([+-]?[0-9]+)\s*(d|D|w|W|m|M|y|Y)?/g;var matches=pattern.exec(offset);while(matches){switch(matches[2]||"d"){case"d":case"D":day+=parseInt(matches[1],10);break;case"w":case"W":day+=parseInt(matches[1],10)*7;break;case"m":case"M":month+=parseInt(matches[1],10);day=Math.min(day,getDaysInMonth(year,month));break;case"y":case"Y":year+=parseInt(matches[1],10);day=Math.min(day,getDaysInMonth(year,month));break}matches=pattern.exec(offset)}return new Date(year,month,day)};date=(date==null?defaultDate:(typeof date=="string"?offsetString(date,this._getDaysInMonth):(typeof date=="number"?(isNaN(date)?defaultDate:offsetNumeric(date)):date)));date=(date&&date.toString()=="Invalid Date"?defaultDate:date);if(date){date.setHours(0);date.setMinutes(0);date.setSeconds(0);date.setMilliseconds(0)}return this._daylightSavingAdjust(date)},_daylightSavingAdjust:function(date){if(!date){return null}date.setHours(date.getHours()>12?date.getHours()+2:0);return date},_setDate:function(inst,date,endDate){var clear=!(date);var origMonth=inst.selectedMonth;var origYear=inst.selectedYear;date=this._determineDate(date,new Date());inst.selectedDay=inst.currentDay=date.getDate();inst.drawMonth=inst.selectedMonth=inst.currentMonth=date.getMonth();inst.drawYear=inst.selectedYear=inst.currentYear=date.getFullYear();if(origMonth!=inst.selectedMonth||origYear!=inst.selectedYear){this._notifyChange(inst)}this._adjustInstDate(inst);if(inst.input){inst.input.val(clear?"":this._formatDate(inst))}},_getDate:function(inst){var startDate=(!inst.currentYear||(inst.input&&inst.input.val()=="")?null:this._daylightSavingAdjust(new Date(inst.currentYear,inst.currentMonth,inst.currentDay)));return startDate},_generateHTML:function(inst){var today=new Date();today=this._daylightSavingAdjust(new Date(today.getFullYear(),today.getMonth(),today.getDate()));var isRTL=this._get(inst,"isRTL");var showButtonPanel=this._get(inst,"showButtonPanel");var hideIfNoPrevNext=this._get(inst,"hideIfNoPrevNext");var navigationAsDateFormat=this._get(inst,"navigationAsDateFormat");var numMonths=this._getNumberOfMonths(inst);var showCurrentAtPos=this._get(inst,"showCurrentAtPos");var stepMonths=this._get(inst,"stepMonths");var stepBigMonths=this._get(inst,"stepBigMonths");var isMultiMonth=(numMonths[0]!=1||numMonths[1]!=1);var currentDate=this._daylightSavingAdjust((!inst.currentDay?new Date(9999,9,9):new Date(inst.currentYear,inst.currentMonth,inst.currentDay)));var minDate=this._getMinMaxDate(inst,"min",true);var maxDate=this._getMinMaxDate(inst,"max");var drawMonth=inst.drawMonth-showCurrentAtPos;var drawYear=inst.drawYear;if(drawMonth<0){drawMonth+=12;drawYear--}if(maxDate){var maxDraw=this._daylightSavingAdjust(new Date(maxDate.getFullYear(),maxDate.getMonth()-numMonths[1]+1,maxDate.getDate()));maxDraw=(minDate&&maxDraw<minDate?minDate:maxDraw);while(this._daylightSavingAdjust(new Date(drawYear,drawMonth,1))>maxDraw){drawMonth--;if(drawMonth<0){drawMonth=11;drawYear--}}}inst.drawMonth=drawMonth;inst.drawYear=drawYear;var prevText=this._get(inst,"prevText");prevText=(!navigationAsDateFormat?prevText:this.formatDate(prevText,this._daylightSavingAdjust(new Date(drawYear,drawMonth-stepMonths,1)),this._getFormatConfig(inst)));var prev=(this._canAdjustMonth(inst,-1,drawYear,drawMonth)?'<a class="ui-datepicker-prev ui-corner-all" onclick="DP_jQuery.datepicker._adjustDate(\'#'+inst.id+"', -"+stepMonths+", 'M');\" title=\""+prevText+'"><span class="ui-icon ui-icon-circle-triangle-'+(isRTL?"e":"w")+'">'+prevText+"</span></a>":(hideIfNoPrevNext?"":'<a class="ui-datepicker-prev ui-corner-all ui-state-disabled" title="'+prevText+'"><span class="ui-icon ui-icon-circle-triangle-'+(isRTL?"e":"w")+'">'+prevText+"</span></a>"));var nextText=this._get(inst,"nextText");nextText=(!navigationAsDateFormat?nextText:this.formatDate(nextText,this._daylightSavingAdjust(new Date(drawYear,drawMonth+stepMonths,1)),this._getFormatConfig(inst)));var next=(this._canAdjustMonth(inst,+1,drawYear,drawMonth)?'<a class="ui-datepicker-next ui-corner-all" onclick="DP_jQuery.datepicker._adjustDate(\'#'+inst.id+"', +"+stepMonths+", 'M');\" title=\""+nextText+'"><span class="ui-icon ui-icon-circle-triangle-'+(isRTL?"w":"e")+'">'+nextText+"</span></a>":(hideIfNoPrevNext?"":'<a class="ui-datepicker-next ui-corner-all ui-state-disabled" title="'+nextText+'"><span class="ui-icon ui-icon-circle-triangle-'+(isRTL?"w":"e")+'">'+nextText+"</span></a>"));var currentText=this._get(inst,"currentText");var gotoDate=(this._get(inst,"gotoCurrent")&&inst.currentDay?currentDate:today);currentText=(!navigationAsDateFormat?currentText:this.formatDate(currentText,gotoDate,this._getFormatConfig(inst)));var controls=(!inst.inline?'<button type="button" class="ui-datepicker-close ui-state-default ui-priority-primary ui-corner-all" onclick="DP_jQuery.datepicker._hideDatepicker();">'+this._get(inst,"closeText")+"</button>":"");var buttonPanel=(showButtonPanel)?'<div class="ui-datepicker-buttonpane ui-widget-content">'+(isRTL?controls:"")+(this._isInRange(inst,gotoDate)?'<button type="button" class="ui-datepicker-current ui-state-default ui-priority-secondary ui-corner-all" onclick="DP_jQuery.datepicker._gotoToday(\'#'+inst.id+"');\">"+currentText+"</button>":"")+(isRTL?"":controls)+"</div>":"";var firstDay=parseInt(this._get(inst,"firstDay"),10);firstDay=(isNaN(firstDay)?0:firstDay);var dayNames=this._get(inst,"dayNames");var dayNamesShort=this._get(inst,"dayNamesShort");var dayNamesMin=this._get(inst,"dayNamesMin");var monthNames=this._get(inst,"monthNames");var monthNamesShort=this._get(inst,"monthNamesShort");var beforeShowDay=this._get(inst,"beforeShowDay");var showOtherMonths=this._get(inst,"showOtherMonths");var calculateWeek=this._get(inst,"calculateWeek")||this.iso8601Week;var endDate=inst.endDay?this._daylightSavingAdjust(new Date(inst.endYear,inst.endMonth,inst.endDay)):currentDate;var defaultDate=this._getDefaultDate(inst);var html="";for(var row=0;row<numMonths[0];row++){var group="";for(var col=0;col<numMonths[1];col++){var selectedDate=this._daylightSavingAdjust(new Date(drawYear,drawMonth,inst.selectedDay));var cornerClass=" ui-corner-all";var calender="";if(isMultiMonth){calender+='<div class="ui-datepicker-group ui-datepicker-group-';switch(col){case 0:calender+="first";cornerClass=" ui-corner-"+(isRTL?"right":"left");break;case numMonths[1]-1:calender+="last";cornerClass=" ui-corner-"+(isRTL?"left":"right");break;default:calender+="middle";cornerClass="";break}calender+='">'}calender+='<div class="ui-datepicker-header ui-widget-header ui-helper-clearfix'+cornerClass+'">'+(/all|left/.test(cornerClass)&&row==0?(isRTL?next:prev):"")+(/all|right/.test(cornerClass)&&row==0?(isRTL?prev:next):"")+this._generateMonthYearHeader(inst,drawMonth,drawYear,minDate,maxDate,selectedDate,row>0||col>0,monthNames,monthNamesShort)+'</div><table class="ui-datepicker-calendar"><thead><tr>';var thead="";for(var dow=0;dow<7;dow++){var day=(dow+firstDay)%7;thead+="<th"+((dow+firstDay+6)%7>=5?' class="ui-datepicker-week-end"':"")+'><span title="'+dayNames[day]+'">'+dayNamesMin[day]+"</span></th>"}calender+=thead+"</tr></thead><tbody>";var daysInMonth=this._getDaysInMonth(drawYear,drawMonth);if(drawYear==inst.selectedYear&&drawMonth==inst.selectedMonth){inst.selectedDay=Math.min(inst.selectedDay,daysInMonth)}var leadDays=(this._getFirstDayOfMonth(drawYear,drawMonth)-firstDay+7)%7;var numRows=(isMultiMonth?6:Math.ceil((leadDays+daysInMonth)/7));var printDate=this._daylightSavingAdjust(new Date(drawYear,drawMonth,1-leadDays));for(var dRow=0;dRow<numRows;dRow++){calender+="<tr>";var tbody="";for(var dow=0;dow<7;dow++){var daySettings=(beforeShowDay?beforeShowDay.apply((inst.input?inst.input[0]:null),[printDate]):[true,""]);var otherMonth=(printDate.getMonth()!=drawMonth);var unselectable=otherMonth||!daySettings[0]||(minDate&&printDate<minDate)||(maxDate&&printDate>maxDate);tbody+='<td class="'+((dow+firstDay+6)%7>=5?" ui-datepicker-week-end":"")+(otherMonth?" ui-datepicker-other-month":"")+((printDate.getTime()==selectedDate.getTime()&&drawMonth==inst.selectedMonth&&inst._keyEvent)||(defaultDate.getTime()==printDate.getTime()&&defaultDate.getTime()==selectedDate.getTime())?" "+this._dayOverClass:"")+(unselectable?" "+this._unselectableClass+" ui-state-disabled":"")+(otherMonth&&!showOtherMonths?"":" "+daySettings[1]+(printDate.getTime()>=currentDate.getTime()&&printDate.getTime()<=endDate.getTime()?" "+this._currentClass:"")+(printDate.getTime()==today.getTime()?" ui-datepicker-today":""))+'"'+((!otherMonth||showOtherMonths)&&daySettings[2]?' title="'+daySettings[2]+'"':"")+(unselectable?"":" onclick=\"DP_jQuery.datepicker._selectDay('#"+inst.id+"',"+drawMonth+","+drawYear+', this);return false;"')+">"+(otherMonth?(showOtherMonths?printDate.getDate():"&#xa0;"):(unselectable?'<span class="ui-state-default">'+printDate.getDate()+"</span>":'<a class="ui-state-default'+(printDate.getTime()==today.getTime()?" ui-state-highlight":"")+(printDate.getTime()>=currentDate.getTime()&&printDate.getTime()<=endDate.getTime()?" ui-state-active":"")+'" href="#">'+printDate.getDate()+"</a>"))+"</td>";printDate.setDate(printDate.getDate()+1);printDate=this._daylightSavingAdjust(printDate)}calender+=tbody+"</tr>"}drawMonth++;if(drawMonth>11){drawMonth=0;drawYear++}calender+="</tbody></table>"+(isMultiMonth?"</div>"+((numMonths[0]>0&&col==numMonths[1]-1)?'<div class="ui-datepicker-row-break"></div>':""):"");group+=calender}html+=group}html+=buttonPanel+($.browser.msie&&parseInt($.browser.version,10)<7&&!inst.inline?'<iframe src="javascript:false;" class="ui-datepicker-cover" frameborder="0"></iframe>':"");inst._keyEvent=false;return html},_generateMonthYearHeader:function(inst,drawMonth,drawYear,minDate,maxDate,selectedDate,secondary,monthNames,monthNamesShort){minDate=(inst.rangeStart&&minDate&&selectedDate<minDate?selectedDate:minDate);var changeMonth=this._get(inst,"changeMonth");var changeYear=this._get(inst,"changeYear");var showMonthAfterYear=this._get(inst,"showMonthAfterYear");var html='<div class="ui-datepicker-title">';var monthHtml="";if(secondary||!changeMonth){monthHtml+='<span class="ui-datepicker-month">'+monthNames[drawMonth]+"</span> "}else{var inMinYear=(minDate&&minDate.getFullYear()==drawYear);var inMaxYear=(maxDate&&maxDate.getFullYear()==drawYear);monthHtml+='<select class="ui-datepicker-month" onchange="DP_jQuery.datepicker._selectMonthYear(\'#'+inst.id+"', this, 'M');\" onclick=\"DP_jQuery.datepicker._clickMonthYear('#"+inst.id+"');\">";for(var month=0;month<12;month++){if((!inMinYear||month>=minDate.getMonth())&&(!inMaxYear||month<=maxDate.getMonth())){monthHtml+='<option value="'+month+'"'+(month==drawMonth?' selected="selected"':"")+">"+monthNamesShort[month]+"</option>"}}monthHtml+="</select>"}if(!showMonthAfterYear){html+=monthHtml+((secondary||changeMonth||changeYear)&&(!(changeMonth&&changeYear))?"&#xa0;":"")}if(secondary||!changeYear){html+='<span class="ui-datepicker-year">'+drawYear+"</span>"}else{var years=this._get(inst,"yearRange").split(":");var year=0;var endYear=0;if(years.length!=2){year=drawYear-10;endYear=drawYear+10}else{if(years[0].charAt(0)=="+"||years[0].charAt(0)=="-"){year=drawYear+parseInt(years[0],10);endYear=drawYear+parseInt(years[1],10)}else{year=parseInt(years[0],10);endYear=parseInt(years[1],10)}}year=(minDate?Math.max(year,minDate.getFullYear()):year);endYear=(maxDate?Math.min(endYear,maxDate.getFullYear()):endYear);html+='<select class="ui-datepicker-year" onchange="DP_jQuery.datepicker._selectMonthYear(\'#'+inst.id+"', this, 'Y');\" onclick=\"DP_jQuery.datepicker._clickMonthYear('#"+inst.id+"');\">";for(;year<=endYear;year++){html+='<option value="'+year+'"'+(year==drawYear?' selected="selected"':"")+">"+year+"</option>"}html+="</select>"}if(showMonthAfterYear){html+=(secondary||changeMonth||changeYear?"&#xa0;":"")+monthHtml}html+="</div>";return html},_adjustInstDate:function(inst,offset,period){var year=inst.drawYear+(period=="Y"?offset:0);var month=inst.drawMonth+(period=="M"?offset:0);var day=Math.min(inst.selectedDay,this._getDaysInMonth(year,month))+(period=="D"?offset:0);var date=this._daylightSavingAdjust(new Date(year,month,day));var minDate=this._getMinMaxDate(inst,"min",true);var maxDate=this._getMinMaxDate(inst,"max");date=(minDate&&date<minDate?minDate:date);date=(maxDate&&date>maxDate?maxDate:date);inst.selectedDay=date.getDate();inst.drawMonth=inst.selectedMonth=date.getMonth();inst.drawYear=inst.selectedYear=date.getFullYear();if(period=="M"||period=="Y"){this._notifyChange(inst)}},_notifyChange:function(inst){var onChange=this._get(inst,"onChangeMonthYear");if(onChange){onChange.apply((inst.input?inst.input[0]:null),[inst.selectedYear,inst.selectedMonth+1,inst])}},_getNumberOfMonths:function(inst){var numMonths=this._get(inst,"numberOfMonths");return(numMonths==null?[1,1]:(typeof numMonths=="number"?[1,numMonths]:numMonths))},_getMinMaxDate:function(inst,minMax,checkRange){var date=this._determineDate(this._get(inst,minMax+"Date"),null);return(!checkRange||!inst.rangeStart?date:(!date||inst.rangeStart>date?inst.rangeStart:date))},_getDaysInMonth:function(year,month){return 32-new Date(year,month,32).getDate()},_getFirstDayOfMonth:function(year,month){return new Date(year,month,1).getDay()},_canAdjustMonth:function(inst,offset,curYear,curMonth){var numMonths=this._getNumberOfMonths(inst);var date=this._daylightSavingAdjust(new Date(curYear,curMonth+(offset<0?offset:numMonths[1]),1));if(offset<0){date.setDate(this._getDaysInMonth(date.getFullYear(),date.getMonth()))}return this._isInRange(inst,date)},_isInRange:function(inst,date){var newMinDate=(!inst.rangeStart?null:this._daylightSavingAdjust(new Date(inst.selectedYear,inst.selectedMonth,inst.selectedDay)));newMinDate=(newMinDate&&inst.rangeStart<newMinDate?inst.rangeStart:newMinDate);var minDate=newMinDate||this._getMinMaxDate(inst,"min");var maxDate=this._getMinMaxDate(inst,"max");return((!minDate||date>=minDate)&&(!maxDate||date<=maxDate))},_getFormatConfig:function(inst){var shortYearCutoff=this._get(inst,"shortYearCutoff");shortYearCutoff=(typeof shortYearCutoff!="string"?shortYearCutoff:new Date().getFullYear()%100+parseInt(shortYearCutoff,10));return{shortYearCutoff:shortYearCutoff,dayNamesShort:this._get(inst,"dayNamesShort"),dayNames:this._get(inst,"dayNames"),monthNamesShort:this._get(inst,"monthNamesShort"),monthNames:this._get(inst,"monthNames")}},_formatDate:function(inst,day,month,year){if(!day){inst.currentDay=inst.selectedDay;inst.currentMonth=inst.selectedMonth;inst.currentYear=inst.selectedYear}var date=(day?(typeof day=="object"?day:this._daylightSavingAdjust(new Date(year,month,day))):this._daylightSavingAdjust(new Date(inst.currentYear,inst.currentMonth,inst.currentDay)));return this.formatDate(this._get(inst,"dateFormat"),date,this._getFormatConfig(inst))}});function extendRemove(target,props){$.extend(target,props);for(var name in props){if(props[name]==null||props[name]==undefined){target[name]=props[name]}}return target}function isArray(a){return(a&&(($.browser.safari&&typeof a=="object"&&a.length)||(a.constructor&&a.constructor.toString().match(/\Array\(\)/))))}$.fn.datepicker=function(options){if(!$.datepicker.initialized){$(document).mousedown($.datepicker._checkExternalClick).find("body").append($.datepicker.dpDiv);$.datepicker.initialized=true}var otherArgs=Array.prototype.slice.call(arguments,1);if(typeof options=="string"&&(options=="isDisabled"||options=="getDate")){return $.datepicker["_"+options+"Datepicker"].apply($.datepicker,[this[0]].concat(otherArgs))}return this.each(function(){typeof options=="string"?$.datepicker["_"+options+"Datepicker"].apply($.datepicker,[this].concat(otherArgs)):$.datepicker._attachDatepicker(this,options)})};$.datepicker=new Datepicker();$.datepicker.initialized=false;$.datepicker.uuid=new Date().getTime();$.datepicker.version="1.7.1";window.DP_jQuery=$})(jQuery);;/*
 * jQuery UI Progressbar 1.7.1
 *
 * Copyright (c) 2009 AUTHORS.txt (http://jqueryui.com/about)
 * Dual licensed under the MIT (MIT-LICENSE.txt)
 * and GPL (GPL-LICENSE.txt) licenses.
 *
 * http://docs.jquery.com/UI/Progressbar
 *
 * Depends:
 *   ui.core.js
 */(function(a){a.widget("ui.progressbar",{_init:function(){this.element.addClass("ui-progressbar ui-widget ui-widget-content ui-corner-all").attr({role:"progressbar","aria-valuemin":this._valueMin(),"aria-valuemax":this._valueMax(),"aria-valuenow":this._value()});this.valueDiv=a('<div class="ui-progressbar-value ui-widget-header ui-corner-left"></div>').appendTo(this.element);this._refreshValue()},destroy:function(){this.element.removeClass("ui-progressbar ui-widget ui-widget-content ui-corner-all").removeAttr("role").removeAttr("aria-valuemin").removeAttr("aria-valuemax").removeAttr("aria-valuenow").removeData("progressbar").unbind(".progressbar");this.valueDiv.remove();a.widget.prototype.destroy.apply(this,arguments)},value:function(b){arguments.length&&this._setData("value",b);return this._value()},_setData:function(b,c){switch(b){case"value":this.options.value=c;this._refreshValue();this._trigger("change",null,{});break}a.widget.prototype._setData.apply(this,arguments)},_value:function(){var b=this.options.value;if(b<this._valueMin()){b=this._valueMin()}if(b>this._valueMax()){b=this._valueMax()}return b},_valueMin:function(){var b=0;return b},_valueMax:function(){var b=100;return b},_refreshValue:function(){var b=this.value();this.valueDiv[b==this._valueMax()?"addClass":"removeClass"]("ui-corner-right");this.valueDiv.width(b+"%");this.element.attr("aria-valuenow",b)}});a.extend(a.ui.progressbar,{version:"1.7.1",defaults:{value:0}})})(jQuery);;var canvasBounced;
var fixWidthElementList = [];

function $getQuerystring(key, default_) {
    if (default_ == null) default_ = "";
    key = key.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
    var regex = new RegExp("[\\?&]" + key + "=([^&#]*)","i");
    var qs = regex.exec(window.location.href);

    if (qs == null)
        return default_;
    else
        return qs[1];
}

function $setControlFocus(control) {
    if (control) {
        control.scrollIntoView();
        control.focus();
    }
}

function $keyCode(e) {
    var code = 0;
    if (!e) e = window.event;
    if (e.keyCode) code = e.keyCode;
    else if (e.which) code = e.which;
    else if (e.charCode) code = e.charCode;
    
    return code;
}

function $isFormValid(causesValidation) {
    var isValid = true;
    if (causesValidation == undefined || causesValidation)
        isValid = MvcValidation_Validate();
    return isValid;
}

function $submitForm(hiddenId, causesValidation) {
    var isValid = $isFormValid(causesValidation);
    if (isValid) {
        $('#' + hiddenId).val('true');
        $('#' + hiddenId)[0].form.submit();
    }
    return false;
}

function $submitFormElement(element, causesValidation) {
    if ($isFormValid(causesValidation)) {
        element.form.submit();
    }
    return false;
}

function $submitKeyPress(hiddenId, event) {
    if ($keyCode(event) == 13) {
        return $submitForm(hiddenId);
    }
}

function $submitKeyPressElement(element, event) {
    if ($keyCode(event) == 13) {
        return $submitFormElement(element);
    }
}

function addFixWidthElement(div){
    if (div) {
        fixWidthElementList.push(div);
    }
}

//called EVERY full or partial postback
function masterResourcesLoaded() {
    canvasBounced = false;

    if (typeof (SmartDate_Load) == 'function') SmartDate_Load();

    if (window.onbeforeunload_suppressed)
        window.onbeforeunload = window.onbeforeunload_suppressed;

    fixWikiContentWidth();
    fixWidthElementList = [];
}

// only called once per full postback, because it h8s us. (is body onload getting overwritten? no man knows.)
function masterLoad() {
    if(!self.logoImageLoaded)document.body.className+=' NoImages';
    else logoImageLoaded=null;
    CodePlex.UpdateProgress.initialize('UpdateProgressPanel');
    Sys.Net.WebRequestManager.add_completedRequest(masterErrorHandler);
    Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(function() { masterResourcesLoaded(); });
}

function masterErrorHandler(executor,args){
    if (executor.get_responseAvailable()&&executor.get_statusCode()!= 200) {
        var redirectUrl = executor.getResponseHeader("ErrorRedirect");
        if(redirectUrl)location.href = redirectUrl;
    }
}

function addCanvasBounceForFF(){
    if(CodePlex.UI.isFF){
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function(){ canvasBounce(); });
    }
}

function canvasBounce() {
    if(!canvasBounced) {
        CodePlex.UI.bounceFullHeightElement($('#Canvas').get(0));
        canvasBounced = true;
    }
}

function fixWidth(sourceDiv, forceRun) {
    if(!forceRun&&!CodePlex.UI.isIE)return;
    var skipMap={html:1,body:1,form:1,tbody:!forceRun,table:!forceRun,td:!forceRun};
    var canvas=$('#CanvasContent').get(0);
    var mainContent=(sourceDiv||canvas);
    while(mainContent&&mainContent.tagName){
        if(!skipMap[mainContent.tagName.toLowerCase()]&&mainContent.scrollWidth>(mainContent.clientWidth||mainContent.offsetWidth)){
            if (CodePlex.UI.isIE7) {
                var newWidth = mainContent.scrollWidth;
                if (mainContent.className == 'CanvasIEWidthFix') 
                    newWidth = newWidth - 7;
                if (newWidth > (mainContent.clientWidth||mainContent.offsetWidth))
                    mainContent.style.width=newWidth+'px';
            }
            else
                mainContent.style.width=mainContent.scrollWidth+'px';    
        }
        mainContent=mainContent.parentNode;
    }
    if(!sourceDiv){
        if(CodePlex.UI.isOpera){
            if(document.body.offsetHeight>canvas.scrollHeight)canvas.style.height=document.body.offsetHeight+'px';
        }
        CodePlex.UpdateProgress.reset();
    }
}

function fixWikiContentWidth() {
    if(CodePlex.UI.isIE) {
        for(i=0;i<fixWidthElementList.length;i++)
            fixWidth(fixWidthElementList[i],true);
        
        fixWidth(null, true);
    }
    else {
        for(i=0;i<fixWidthElementList.length;i++) {
            if (CodePlex.UI.isFF3)
                fixWidthElementList[i].style.width= (fixWidthElementList[i].scrollWidth - 17)+'px';
            else
                fixWidthElementList[i].style.width= fixWidthElementList[i].scrollWidth+'px';
        }
        if(fixWidthElementList.length > 0) 
            canvasBounce();
    }
}

function getResizeHandler(e){
    var _timer=0;
    return function(){
        if(_timer)clearTimeout(_timer);
        _timer=setTimeout(fixWidth,1);
    }
}


function EnterKeyFormSubmitHandler(triggeredBy, event, btnIdToSubmit) 
{
    if(event.keyCode == 13) // Handle Enter Key
    {
        event.returnValue = false;
        event.Cancel = true;
        document.getElementById(getFullIdForControl(triggeredBy, btnIdToSubmit)).click();
    }
}

function getElementByClass(tagCollection, className)
{
    var tagCount = tagCollection.length;
    for (i=0; i<tagCount; i++)
    {
        if (tagCollection[i].className==className)
        {
            return tagCollection[i];
        }
    }
    return null;
}

function expandAndCollapseDivs(divClass, divId, expandButtonClass, collapseButtonClass)
{
	var allTags = document.all ? document.all : document.getElementsByTagName("*");
	var divElement = getElementByClass(allTags, divClass);    
    var displayState = getCookie(divId);

    if (displayState == "expanded")
    {
        var expandButton = getElementByClass(allTags, expandButtonClass);
        displayContentDisplay(expandButton, "collapseButton", divId);
    }	
    else
    {
        var collapseButton = getElementByClass(allTags, collapseButtonClass);
        hideContentDisplay(collapseButton, "expandButton", divId);       
    }
}

function displayAllContentInlineDisplay(divClassToShow, divClassToHide, imgClassToShow, imgClassToHide, displayLink, hideLink)
{
    var divs = document.getElementsByTagName("div");
    var imgs = document.getElementsByTagName("img");
    
    var divsToShow = [], divsToHide = [];
    var imgsToShow = [], imgsToHide = [];
    
    for (var i = 0; i < divs.length; i++)
    {
        if (divs[i].className == divClassToShow)
            divsToShow.push(divs[i]);
        else if (divs[i].className == divClassToHide)
            divsToHide.push(divs[i]);
    }
    
    for (var i = 0; i < imgs.length; i++)
    {
        if (imgs[i].className == imgClassToShow)
            imgsToShow.push(imgs[i]);
        else if (imgs[i].className == imgClassToHide)
            imgsToHide.push(imgs[i]);
    }
    
    for (var i = 0; i < divsToShow.length; i++)
    {
        divsToShow[i].style.display = "inline";
    }
    
    for (var i = 0; i < divsToHide.length; i++)
    {
        divsToHide[i].style.display = "none";
    }
    
    for (var i = 0; i < imgsToShow.length; i++)
    {
        imgsToShow[i].style.display = "inline";
    }
    
    for (var i = 0; i < imgsToHide.length; i++)
    {
        imgsToHide[i].style.display = "none";
    }

    var displayLinkAnchor = document.getElementById(displayLink);
    if(displayLinkAnchor)
        displayLinkAnchor.style.display = "inline";
    var hideLinkAnchor = document.getElementById(hideLink);
    if(hideLinkAnchor)
        hideLinkAnchor.style.display = "none";

}

function getFullIdForControl(triggeredBy, controlId) 
{
    var fullId = triggeredBy.id.substring(0, triggeredBy.id.lastIndexOf('_')+1) + controlId;
    return fullId;
}

function displayContentDisplay(triggeredBy, buttonToDisplay, contentDiv) 
{
    var contentSection = document.getElementById(getFullIdForControl(triggeredBy, contentDiv));    
    var imageButtonToDisplay = document.getElementById(getFullIdForControl(triggeredBy, buttonToDisplay));
        
    triggeredBy.style.display = "none";
    imageButtonToDisplay.style.display = "inline";
    contentSection.style.display = "block";
    
    setCookie(contentDiv, "expanded");        
}

function hideContentDisplay(triggeredBy, buttonToDisplay, contentDiv)
{
    var contentSection = document.getElementById(getFullIdForControl(triggeredBy, contentDiv));
    var imageButtonToDisplay = document.getElementById(getFullIdForControl(triggeredBy, buttonToDisplay));

    triggeredBy.style.display = "none";
    imageButtonToDisplay.style.display = "inline";
    contentSection.style.display = "none";
    setCookie(contentDiv, "collapsed");        
}

function displayContentInlineDisplay(triggeredBy, buttonToDisplay, contentDiv, hideContentDiv) 
{
    var contentSection = document.getElementById(getFullIdForControl(triggeredBy, contentDiv));    
    var hideContentSection = document.getElementById(getFullIdForControl(triggeredBy, hideContentDiv));    
    var imageButtonToDisplay = document.getElementById(getFullIdForControl(triggeredBy, buttonToDisplay));
        
    triggeredBy.style.display = "none";
    imageButtonToDisplay.style.display = "inline";
    contentSection.style.display = "inline";
    hideContentSection.style.display = "none";
}

function setCookie(name, value, expires, path, domain, secure)
{
    document.cookie = name + "=" + escape(value) +
        ((expires) ? "; expires=" + expires.toGMTString() : "") +
        ((path) ? "; path=" + path : "") +
        ((domain) ? "; domain=" + domain : "") +
        ((secure) ? "; secure" : "");
}

function getCookie(name)
{
    var start = document.cookie.indexOf( name + "=" );
    var len = start + name.length + 1;
    if ( ( !start ) &&
         ( name != document.cookie.substring( 0, name.length ) ) )
    {
       return null;
    }
    if ( start == -1 ) return null;
    var end = document.cookie.indexOf( ";", len );
    if ( end == -1 ) end = document.cookie.length;
    return unescape( document.cookie.substring( len, end ) );
}
	
function deleteCookie(name, path, domain) 
{
    if (getCookie(name)) {
        document.cookie = name + "=" +
            ((path) ? "; path=" + path : "") +
            ((domain) ? "; domain=" + domain : "") +
            "; expires=Thu, 01-Jan-70 00:00:01 GMT";
    }
}
function getChildNodes(parentDiv,tagName,propertyName,propertyValue,fuzzyMatch){
    var ret=[];
    if(parentDiv&&parentDiv.childNodes){
        for(var i=0;i<parentDiv.childNodes.length;i++){
            if(parentDiv.childNodes[i].nodeType!=1)continue;
            if((!tagName||parentDiv.childNodes[i].tagName.toLowerCase()==tagName.toLowerCase())&&(!propertyName||(!fuzzyMatch&&parentDiv.childNodes[i][propertyName]==propertyValue)||(fuzzyMatch&&parentDiv.childNodes[i][propertyName]&&parentDiv.childNodes[i][propertyName].indexOf(propertyValue)>-1)))ret[ret.length]=parentDiv.childNodes[i];
            else ret=ret.concat(getChildNodes(parentDiv.childNodes[i],tagName,propertyName,propertyValue,fuzzyMatch));
        }
    }
    return ret;
}
function getParentNode(childNode,tagName,propertyName,propertyValue,fuzzyMatch){
    var ret=null;
    var tmp=childNode.parentNode;
    while(tmp){
        if((!tagName||tmp.tagName.toLowerCase()==tagName.toLowerCase())&&(!propertyName||(!fuzzyMatch&&tmp[propertyName]==propertyValue)||(fuzzyMatch&&tmp[propertyName]&&tmp[propertyName].indexOf(propertyValue)>-1))){
            ret=tmp;
            break;
        }
        tmp=tmp.parentNode;
    }
    return ret;
}
function createNode(parentNode,attributes,tagName){
    var elt=document.createElement(tagName?tagName:'div');
    if(attributes)for(var x in attributes)elt[x]=attributes[x];
    if(parentNode)parentNode.appendChild(elt);
    return elt;
}

// AJAX LIBRARY CORRECTIONS

if(typeof(WebForm_FireDefaultButton)!='undefined'){
    if(!self.CodePlex_Preserve_WebForm_FireDefaultButton)self.CodePlex_Preserve_WebForm_FireDefaultButton = WebForm_FireDefaultButton;
    WebForm_FireDefaultButton=function(event, target) {
        if(!event.srcElement&&event.target)event.srcElement=event.target;
        return CodePlex_Preserve_WebForm_FireDefaultButton(event, target);
    }
}

Type.registerNamespace("CodePlex");
CodePlex.UI=new function(){
    this.isIE=document.documentElement.className.indexOf('IE')>-1;
    this.isIE6=document.documentElement.className.indexOf('IE6')>-1;
    this.isIE7=document.documentElement.className.indexOf('IE7')>-1;
    this.isFF=document.documentElement.className.indexOf('FF')>-1;
    this.isFF3=document.documentElement.className.indexOf('FF3')>-1;
    this.isSafari=document.documentElement.className.indexOf('Safari')>-1;
    this.isOpera=document.documentElement.className.indexOf('Opera')>-1;
    this.Browser=document.documentElement.className.replace(/IE /,'');
    
    this.Point = function(x, y) {
        this.x = x;
        this.y = y;
    };
    this.getElementPosition = function(element) {
        var x = 0;
        var y = 0;
        while (element.offsetParent) {
            x += element.offsetLeft;
            y += element.offsetTop;
            
            element = element.offsetParent;
        }
        
        x += element.offsetLeft;
        y += element.offsetTop;
        
        return new CodePlex.UI.Point(x, y);
    }

    this.focus = function(elt) {
        if(!elt)return false;
        try{elt.focus();return true;}catch(e){return false;}
    }
    
    var _babel = null;
    this.encodeHtml = function(value, isAttribute) {
        if (!_babel) _babel = document.createElement("div");
        _babel.innerHTML = "";
        _babel.appendChild(document.createTextNode(value));
        return (isAttribute ? _babel.innerHTML.split('"').join("&quot;") : _babel.innerHTML).split(' ').join("&nbsp;");
    }
    
    var _bounceTimer=0;
    this.bounceElement=function(elt,callback){
        if(!elt)return;
        var width=elt.style.width;
        elt.style.width=(width!='auto'?'auto':'100%');
        if(_bounceTimer)clearTimeout(_bounceTimer);
        _bounceTimer=setTimeout(function(){
            elt.style.width=width;
            if(typeof(callback)=='function')callback();
        },1);
    }
    this.bounceFullHeightElement=function(elt,callback){
        if(!elt)return;
        elt.style.height='99.9%';
        if(_bounceTimer)clearTimeout(_bounceTimer);
        _bounceTimer=setTimeout(function(){
            elt.style.height='100%';
            if(typeof(callback)=='function')callback();
        },1);
    }
};

function createDelegate(instance, method) {
    return function() { method.apply(instance); }
}

// AJAX JS LIBRARY EXTENSIONS
Function.createDelegateWithParameters=function(instance,method){
    var args=[];
    for(var i=2;i<arguments.length;i++)args.push(arguments[i]);
    return function(){
        for(var i=0;i<arguments.length;i++)args.push(arguments[i]);
        return method.apply(instance,args);
    }
}

if(typeof(originalRegularExpressionValidatorEvaluateIsValid) == 'undefined' &&
   typeof(RegularExpressionValidatorEvaluateIsValid) == 'function')
{
    originalRegularExpressionValidatorEvaluateIsValid = RegularExpressionValidatorEvaluateIsValid;
    RegularExpressionValidatorEvaluateIsValid = 
        function (val) {
            if(val.invalidinIEifonlywhitespace)
            {
                if(val.invalidinIEifonlywhitespace == 'true')
                {
                    var value = ValidatorGetValue(val.controltovalidate);
                    if(value.replace(/\s*/g,'').length == 0 && value.length > 0) return false;
                    return originalRegularExpressionValidatorEvaluateIsValid(val);
                }
            }
            
            return originalRegularExpressionValidatorEvaluateIsValid(val);
        }
    }

//Safari and Chrome AJAX fix for Sys.ScriptLoadFailedException in ScriptResource.axd
Sys.Browser.WebKit = {}; //Safari 3 is considered WebKit
if (navigator.userAgent.indexOf('WebKit/') > -1) {
    Sys.Browser.agent = Sys.Browser.WebKit;
    Sys.Browser.version = parseFloat(navigator.userAgent.match(/WebKit\/(\d+(\.\d+)?)/)[1]);
    Sys.Browser.name = 'WebKit';
}

function suppressUnsavedData() {
    window.onbeforeunload_suppressed = window.onbeforeunload;
    window.onbeforeunload = null;
}

function allowUnsavedDataAlert() {
    if (window.onbeforeunload_suppressed && !window.onbeforeunload) {
        window.onbeforeunload = window.onbeforeunload_suppressed;
    }
}

function disableUnsavedData() {
    window.onbeforeunload = null;
    window.onbeforeunload_suppressed = null;
}

function OpenDialog(dialog, open, width, css, appendTo) {
    if ($(dialog) && !open) {
        $(dialog).dialog('close');
        return;
    }

    $(dialog).dialog('destroy');
    $(dialog).dialog({
    autoOpen: false,
    dialogClass: css,
        width: width,
        modal: true,
        overlay: {
            backgroundColor: '#CCC',
            opacity: 0.5
        }
    });

    if ($(dialog) && open) {        
        if (appendTo) 
            $(dialog).parent().appendTo(appendTo);            
        else
            $(dialog).parent().appendTo('form:last');
       
        $(dialog).dialog('open').show().focus();
    } 
}

function GetHashValue(hashValue, keyOnly) {
    var value = null;
    if (path.match('#')) {
        var key = path.split('#')[1];

        hashValue = hashValue.toLowerCase();
        
        if (!keyOnly)
            hashValue += '=';

        if (key.toLowerCase().indexOf(hashValue) >= 0) {
            if (!keyOnly)
                value = key.split('=')[1];
            else
                value = key;
        }
    }
    return value;
}

function HashValueExist(hashValue, keyOnly) {

    if (path.match('#')) {
    
        var key = path.split('#')[1];

        hashValue = hashValue.toLowerCase();
        
        if (!keyOnly)
            hashValue += '=';            

        if (key.toLowerCase().indexOf(hashValue) >= 0)
            return true;
        else
            return false;
    }
}

function BindButtonHover(id, onImageUrl, offImageUrl) {
    $('#' + id)
            .bind('mouseover', function() { $(this).attr('src', onImageUrl); })
            .bind('mouseout', function() { $(this).attr('src', offImageUrl); });
}function showSummaryValidator(source, e) 
{
    for (i  = 0; i<Page_Validators.length; i++) 
    {
        if (Page_Validators[i].validationGroup == source.validationGroup &&
            Page_Validators[i].id != source.id) 
        {
            ValidatorValidate(Page_Validators[i]);
            if (Page_Validators[i].isvalid == false) {
                e.IsValid = false;
                break;
            }
        }
    }
}(function(jQuery) {
    jQuery.fn.enhancedTextBox = function(options) {
        return this.each(function() {
            new jQuery.enhancedTextBox(this, options);
        });
    };

    jQuery.enhancedTextBox = function(element, options) {
        this.textArea = element;
        this.options = options;
        this.init();
    }

    jQuery.enhancedTextBox.fn = jQuery.enhancedTextBox.prototype = {};
    jQuery.enhancedTextBox.fn.extend = jQuery.enhancedTextBox.extend = jQuery.extend;
    jQuery.enhancedTextBox.fn.extend({
        init: function() {
            this.limit = this.options.limit && this.options.limit > 0 ? parseInt(this.options.limit) : 0;
            this.multiline = $('textarea#' + this.textArea.id).length > 0;
            this.defaultButton = $('#' + this.options.defaultButtonId)[0];
            this.rowState = this.options.rowState;
            this.showUnsavedDataAlert = this.options.showUnsavedDataAlert && Boolean.parse(this.options.showUnsavedDataAlert);
            if (this.showUnsavedDataAlert) {
                if (this.options.hiddenFieldValue == 'true') {
                    $(window).bind('onbeforeunload', function() { return 'You are navigating away from the page you were editing; Your changes may be lost.'; });
                    $('#' + this.options.hiddenFieldId).val('true');
                }
            }

            this.initTextArea();
        },

        initTextArea: function() {
            $(this.textArea)
                .bind('keydown', this, this.onKeyDown)
                .bind('keyup', this, this.onKeyUp)
                .bind('keypress', this, this.onKeyPress)
                .bind('change', this, this.onChange)
                .bind('enlarge', this, this.onEnlarge);
            if (this.loadTimer) clearTimeout(this.loadTimer);
            this.loadTimer = setTimeout(createDelegate(this, this.populateTextArea), 250);
        },

        populateTextArea: function() {
            if (!this.textArea) return;
            this.textArea.disabled = false;
            if (!$.browser.mozilla) $(this.textarea).css('display', 'block');
            var txt = $(this.textArea).attr('txt');
            if (txt) {
                $(this.textArea)
                .removeAttr('txt')
                .val(txt);
                this.textAreaChanged();
            }
        },

        enlarge: function(rows) {
            if (!this.textArea || !this.rowState) return;
            this.rowState.value = this.textArea.rows = parseInt(this.textArea.rows) + (rows || 5);
        },

        autoEnlargeTextBox: function(textBox, state, maxAutoRows) {
            var increment = 5;
            if (textBox && textBox.rows) {
                if (document.all) { // IE
                    while (textBox.scrollHeight > textBox.clientHeight && this.isLessThanMaxRows(textBox, maxAutoRows)) {
                        textBox.rows += increment;
                    }
                    textBox.scrollTop = 0;
                } else {
                    var lineBreaks = countLineBreaks(textBox.value);
                    var wrap = textBox.getAttribute('wrap');
                    if (lineBreaks > parseInt(textBox.rows)) {
                        while (lineBreaks > parseInt(textBox.rows) && this.isLessThanMaxRows(textBox, maxAutoRows)) {
                            textBox.rows += increment
                        }
                    } else if (wrap.toLowerCase() == 'soft' || wrap.toLowerCase() == 'hard') {
                        while (parseInt(textBox.rows) * parseInt(textBox.cols) <= textBox.value.length &&
                           isLessThanMaxRows(textBox, maxAutoRows)) {
                            textBox.rows += increment;
                        }
                    }
                }
                if (state) state.value = textBox.rows;
            }
        },

        updateCounter: function(text, delay) {
            if (!this.options.showCounter) return;
            if (!this.options.limit) return this.setCounter(text.length, delay);
            return this.setCounter(text.length < this.options.limit ? this.options.limit - text.length : 0, delay);
        },

        setCounter: function(count, delay) {
            var $counter = $('#' + this.options.counterId);
            if (!$counter.length) return;
            if (delay && $counter.html() - count < 10) return;
            $counter.html(count);
        },

        countLineBreaks: function(text) {
            return text.split('\n').length - 1;
        },

        isLessThanMaxRows: function(maxRows) {
            if (!maxRows) return true;
            return (parseInt(this.textArea.rows) < maxRows);
        },

        isLessThanLimit: function(text) {
            if (!this.options.limit) return true;
            var textValue = $(this.textArea).val();
            return textValue.length < this.options.limit || (text || this.normalize(textValue).length < this.options.limit);
        },

        isGreaterThanLimit: function(text) {
            if (!this.options.limit) return false;
            var textValue = $(this.textArea).val();
            return textValue.length > this.options.limit && (text || this.normalize(textValue).length > this.options.limit);
        },

        normalize: function(text) {
            if (text.indexOf("\r") > -1) {
                if (text.indexOf("\r\n") > -1) text = text.split("\r\n").join("\n");
                if (text.indexOf("\r") > -1) text = text.split("\r").join("\n");
            }
            return text;
        },

        getSelection: function() {
            if (typeof (this.textArea.createTextRange) != 'undefined') {
                var sel = document.selection && document.activeElement == this.textArea ? document.selection.createRange() : null;
                return sel ? sel.text : '';
            }
            if (typeof (this.textArea.selectionStart) != 'undefined') return this.textArea.value.substring(this.textArea.selectionStart, this.textArea.selectionEnd);
            return '';
        },

        textAreaChanged: function() {
            if (!this.textArea) return;
            var normalizedText = this.normalize(this.textArea.value);
            if (this.isGreaterThanLimit(normalizedText)) {
                normalizedText = normalizedText.substring(0, this.options.limit || Infinity);
                this.textArea.value = normalizedText;
            }
            this.updateCounter(normalizedText);
            if (this.options.autoEnlarge) {
                autoEnlargeTextBox(this.textarea, state);
            }
        },

        cancelEvent: function(e) {
            if (e && e.preventDefault) { e.preventDefault(); }
            if (e && e.stopPropagation) { e.stopPropagation(); }
            return false;
        },

        onKeyDown: function(e) {
            e.data.updateCounter(e.data.normalize(e.data.textArea.value), 1);
            if (e.data.showUnsavedDataAlert) {
                $('#' + e.data.options.hiddenFieldId).val('true');
                window.onbeforeunload = function() { return 'You are navigating away from the page you were editing; Your changes may be lost.'; }
            }
        },

        onKeyUp: function(e) {
            if (e.data.keyTimer) clearTimeout(e.data.keyTimer);
            e.data.keyTimer = setTimeout(function() { e.data.onChange(e); }, 250);
        },

        onKeyPress: function(e) {
            var doCancel = false;
            if (e.data.multiline) doCancel = !e.data.onKeyPress_MultiLine(e);
            else doCancel = !e.data.onKeyPress_SingleLine(e);
            if (doCancel) return e.data.cancelEvent(e);
        },

        onKeyPress_SingleLine: function(e) {
            var keyCode = e.keyCode;
            if (keyCode == 13 && e.data.defaultButton) {
                e.data.click();
                return false;
            }
            return true;
        },

        onKeyPress_MultiLine: function(e) {
            var keyCode = e.keyCode;

            if ($.browser.mozilla && keyCode == 13 && e && e.stopPropagation) e.stopPropagation();
            if (!e.data.options.limit) return true; // don't spin if there is no limit
            if (e.ctrlKey || e.shiftKey || e.altKey) return true; // always allow ctrl, shift or alt combos
            var allowMap = {         // always allow these keys to be pressed
                8: true,  // backspace
                34: true, 33: true, 35: true, 36: true,  // page down, page up, end, home
                37: true, 38: true, 39: true, 40: true,  // arrow keys
                46: true  // delete
            };
            if (allowMap[keyCode]) return true;
            if (e.data.getSelection().length) return true; // always allow selected text to be overwritten
            return e.data.isLessThanLimit();
        },

        onChange: function(e) {
            e.data.textAreaChanged();
        },

        onEnlarge: function(e, rows) {
            e.data.enlarge(rows);
        }
    });
})(jQuery);    CodePlexPaging = new function() {
        var _this, _parameterCallbackMethod;
        this.initialize = function(parameterCallbackMethod) {
            _this = this;
            _parameterCallbackMethod = parameterCallbackMethod;
        }

        this.gotoPage = function(url, index) {
            var parameters = this._getParameters();
            var separator = this._getQuerystringSeparator(parameters);
            CodePlex.UpdateProgress.displayForPaging('Loading...');
            if (index > 0)
                location.href = url + parameters + separator + 'page=' + index;
            else
                location.href = url + parameters;
        }

        this.loadPage = function(url, container, index) {
            var parameters = this._getParameters();
            var separator = this._getQuerystringSeparator(parameters);
            CodePlex.UpdateProgress.displayForPaging('Loading...');
            if (index > 0)
                url = url + parameters + separator + 'page=' + index;
            else
                url = url + parameters;
            $('#' + container).load(url, null, function() {
                CodePlex.UpdateProgress.hide();
            });
        }

        this.gotoPageSize = function(url, size) {
            var parameters = this._getParameters();
            var separator = this._getQuerystringSeparator(parameters);
            CodePlex.UpdateProgress.displayForPaging('Loading...');
            location.href = url + parameters + separator + 'size=' + size;
        }

        this._getParameters = function() {
            var parameters = '';
            if (_parameterCallbackMethod != null) {
                eval('parameters = ' + _parameterCallbackMethod + '();');
            }
            return parameters;
        }

        this._getQuerystringSeparator = function(parameters) {
            var separator = '?';
            if (parameters.length > 0) separator = '&';
            return separator;
        }
    }

MultiFileUpload = function(namePrefix,iconPathFormat) {
    this._namePrefix = namePrefix;
    this._iconPathFormat = iconPathFormat;
}

MultiFileUpload.prototype = {
    render: function(container) {
        this.baseContainer = container;
        var row = document.createElement("tr");
        this._renderToTableRow(container.appendChild(row), this._namePrefix);
    },

    _renderToTableRow: function(row, namePrefix) {
        var typeIconCell = row.insertCell(0);
        var filenameCell = row.insertCell(1);
        var typeCell = row.insertCell(2);
        var locationCell = row.insertCell(3);

        this._renderFilenameTemplate(filenameCell, namePrefix);
        this._renderLocationTemplate(locationCell, namePrefix);
        this._renderTypeTemplate(typeCell, namePrefix);
        this._renderTypeIconTemplate(typeIconCell, namePrefix);
    },

    _renderFilenameTemplate: function(container, namePrefix) {
        var input = document.createElement("input");

        input.setAttribute("type", "text");
        input.id = input.name = namePrefix + "_Filename";
        input.className = "MultiFileHeight";
        input.maxLength = 80;

        container.appendChild(document.createTextNode("Filename"));
        container.appendChild(document.createElement("br"));
        container.appendChild(input);
    },

    _renderLocationTemplate: function(container, namePrefix) {
        var input = document.createElement("input");

        input.setAttribute("type", "file");
        input.id = input.name = namePrefix + "_Location";
        input.className = "MultiFileHeight FileUploadHeight";

        container.appendChild(document.createTextNode("Location"));
        container.appendChild(document.createElement("br"));
        container.appendChild(input);
    },
    _renderTypeIconTemplate: function(container, namePrefix) {
        if (!this.typeIcon) {
            this.typeIcon = container.appendChild(document.createElement("img"));
            this.typeIcon.className = 'FileTypeImage';
        }
        this.typeIcon.src = this._iconPathFormat.replace('{0}', this.typeSelect.value + '.gif');
    },
    _renderTypeTemplate: function(container, namePrefix) {
        var select = this.typeSelect = document.createElement("select");

        select.multiple = false;
        select.id = select.name = namePrefix + "_Type";
        select.className = "MultiFileHeight";
        select.onchange = Function.createDelegate(this, this._renderTypeIconTemplate);

        this._renderTypeItem(select, "Application (Runtime Binary)", "RuntimeBinary");
        this._renderTypeItem(select, "Source Code", "SourceCode");
        this._renderTypeItem(select, "Documentation", "Documentation");
        this._renderTypeItem(select, "Example", "Example");

        container.appendChild(document.createTextNode("Type"));
        container.appendChild(document.createElement("br"));
        container.appendChild(select);
    },

    _renderTypeItem: function(select, text, value) {
        var option = document.createElement("option");

        option.text = text;
        option.value = value;

        select.options.add(option);
    }
}

MultiFileUpload._fileCount = 1;

MultiFileUpload.getFileCount = function() {
    return MultiFileUpload._fileCount;
}

MultiFileUpload.getNextNamePrefix = function(base) {
    return base + "_" + MultiFileUpload.getFileCount();
}

MultiFileUpload.addFileToTable = function(tableId, namePrefix, iconPathFormat) {
    var table;
    if (tableId)
        table = $('#' + tableId)[0];
    if (table) {
        if (table.tBodies && table.tBodies.length) table = table.tBodies[0];
        MultiFileUpload._fileCount++;
        var template = new MultiFileUpload(namePrefix, iconPathFormat);
        template.render(table);
    }
};



(function(jQuery) {
    jQuery.fn.promptBox = function(watermarkText) {
        return this.each(function() {
            new jQuery.promptBox(this, watermarkText);
        });
    };

    jQuery.promptBox = function(element, watermarkText) {
        this.textBox = element;
        this.watermarkText = watermarkText;
        this.init();
    }

    jQuery.promptBox.fn = jQuery.promptBox.prototype = {};
    jQuery.promptBox.fn.extend = jQuery.promptBox.extend = jQuery.extend;
    jQuery.promptBox.fn.extend({
        init: function() {
            this.css = 'EmptyTextBox';
            this.initTextBox();
            this.registerEvents();
        },

        initTextBox: function() {
            $(this.textBox).addClass(this.css);
            $(this.textBox).val(this.watermarkText);
        },

        focusHandler: function(e) {
            if (!e.data.dirty) $(e.data.textBox).val('');
            $(e.data.textBox).removeClass(e.data.css);
        },

        blurHandler: function(e) {
            var val = $(e.data.textBox).val();
            e.data.dirty = val.length > 0 && val != e.data.watermarkText;
            $(e.data.textBox).attr('dirty', e.data.dirty);
            if (!e.data.dirty) {
                e.data.initTextBox();
            }
        },

        submitHandler: function(e) {
            if ($keyCode(e) == 13 && !e.data.dirty) {
                $(e.data.textBox).val('');
            }
        },

        keyHandler: function(e) {
            switch (e.keyCode) {
                case 13:
                    e.data.blurHandler(e);
                    break;
                case 27:
                    $(e.data.textBox).val('');
                    $(e.data.textBox).blur();
                    break;
            }
        },

        registerEvents: function() {
            $(this.textBox)
                .bind('focus', this, this.focusHandler)
                .bind('blur', this, this.blurHandler)
                .bind('keydown', this, this.keyHandler)
                .bind('keypress', this, this.submitHandler);
        }

    });
})(jQuery);
function TimezoneCookieExists() {
    var result = false;
    var date = new Date();
    var timezoneOffsetCookie = getCookie("TimezoneOffset");
    
    if (timezoneOffsetCookie != null &&
        timezoneOffsetCookie == (date.getTimezoneOffset() / 60 * -1) &&
        !firstTimeSetTimezoneCookie) //firstTimeSetTimezoneCookie from base.Master
            result = true;
    
    return result;
}

function SmartDate_Load()
{
    if (TimezoneCookieExists()) return;

    var templiterals = document.getElementsByTagName("abbr");
    var literals = new Array(templiterals.length);
    for (var i = 0; i < templiterals.length; i++)
    {
        literals[i] = templiterals[i].parentNode;        
    }    
    
    for (var i = 0; i < literals.length; i++)
    {
        var classNames = literals[i].className.split(' ');
        var isSmartDate = false;
        for (var index = 0; index < classNames.length; index++)
        {
            if (classNames[index] == "smartDate")
            {
                isSmartDate = true;
                break;
            }                    
        }
        
        if (isSmartDate)
        {
            var showFullDate = false;
            var showDateOnly = false;
            var showTimeOnly = false;
            var noShortDate = false;
            for (var index = 0; index < classNames.length; index++)
            {
                if (classNames[index] == "full")
                {
                    showFullDate = true;
                    break;
                }
                if (classNames[index] == "dateOnly")
                {
                    showDateOnly = true;
                    break;
                }
                if (classNames[index] == "timeOnly")
                {
                    showTimeOnly = true;
                    break;
                }
                if (classNames[index] == "dateOnlyNoShort")
                {
                    showDateOnly = true;
                    noShortDate = true;
                }
            }
         
            var timestamp = parseInt(literals[i].attributes['LocalTimeTicks'].value);
            if (timestamp != Number.NaN) {
                var breakDateTime = false;
                var breakAfterAtDateTime = false;
                
                if (literals[i].getAttribute("BreakDateTime"))
                    breakDateTime = true;
                
                if (literals[i].getAttribute("BreakAfterAtDateTime"))
                    breakAfterAtDateTime = true;
                
                literals[i].innerHTML = SmartDate_FormatTimestamp(timestamp, showFullDate, showDateOnly, showTimeOnly, noShortDate, breakDateTime, breakAfterAtDateTime);
            }
        }
    }
    var templiterals = null;
    var literals = null;
}

//
// Keep this method in sync with SmartDateTimeLiteral.FormatTimestamp!!
//
function SmartDate_FormatTimestamp(timestamp, showFullDate, showDateOnly, showTimeOnly, noShortDate, breakDateTime, breakAfterAtDateTime)
{
    var date = new Date(timestamp * 1000);
    
    // We get more granular as we get further from the date
    var timeText = SmartDate_FormatTime(date, showFullDate);
    var dateText = SmartDate_FormatDate(date, showFullDate, noShortDate);
    
    var text = "";
    if (dateText.length > 0)
        text = dateText;
    
    if (showDateOnly)
        return text;
    if (showTimeOnly)
        text = "";
    
    if (timeText.length > 0)
    {
        if (text.length > 0) {

            if (breakDateTime)
                text += "<br />";

            text += " at ";
            
            if (breakAfterAtDateTime)
                text += "<br />";
        }
        text += timeText;
    }
    
    return text;
}

function SmartDate_FormatTime(dateObject, showFullDate)
{
    var h = dateObject.getHours();
    var m = dateObject.getMinutes();
    var ampm = h < 12 ? "AM" : "PM";
    
    if (h == 0 && m == 0 && dateObject.getSeconds() == 0 && dateObject.getMilliseconds() == 0)
        return "";
  
    if (h > 12)
        h = h - 12;
    
    if (h == 0)
        h = 12;
    
    return h + ":" + SmartDate_ZeroPadNumber(dateObject.getMinutes(), 2) + " " + ampm;
}

function SmartDate_FormatDate(dateObject, showFullDate, noShortDate)
{
    var Y = dateObject.getFullYear();
    var M = dateObject.getMonth();
    var D = dateObject.getDate();
    var d = dateObject.getDay();
    
    var days = ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"];
    var months = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];

    if (showFullDate)
    {
        return days[d] + " " + months[M] + " " + D + " " + Y;
    }
    else
    {
        var now = new Date();
        
        // We get more granular as we get further from the date
        var dateText = "";
        if(!noShortDate)
        {
            if (now.getFullYear() == Y && now.getMonth() == M && now.getDate() == D)
                dateText = "Today";
            else if (now.getFullYear() == Y && now.getMonth() == M && ((now.getDate() > D && (now.getDate() - 6) < D) || (D > now.getDate() && (D - 6) < now.getDate())))
                dateText = days[d];
            else if (now.getFullYear() == Y)
                dateText = months[M] + " " + D;
            else
                dateText = months[M] + " " + D + " " + Y;
                
            return dateText;
        }
        
        dateText = months[M] + " " + D + " " + Y;
        return dateText;
    }
}

function SmartDate_ZeroPadNumber(num, places)
{
    var numString = "" + num + "";
    
    for (var i = 0; i < places - numString.length; i++)
    {
        numString = "0" + numString;
    }
    
    return numString;
}


function SmartDate_AjaxLoad()
{
    var page = $object("_PageRequestManager");
    page.propertyChanged.add(SmartDate_RequestManagerPropertyChanged);    
}

// handles the property changed event to listen for the ajax callback
// and re-run the date load method    
function SmartDate_RequestManagerPropertyChanged(sender, args)
{
    if(args.get_propertyName() == "inPostBack" && !sender.get_inPostBack())
    {
        SmartDate_Load();
    }
}Type.registerNamespace('CodePlex');

var UpdateProgress_Hide;
var UpdateProgress_PagingTimer;

// UpdateProgress Singleton
CodePlex.UpdateProgress = new function() {
    var _this, _ie6, _ff, _rootEl, _panel, _clientID, _resizeTimer, _disabledControls;
    var _manager;
    this.initialize = function(clientID) {
        _this = this;
        _manager = Sys.WebForms.PageRequestManager.getInstance();
        _manager.add_pageLoading(onEndRequest);
        _manager.add_endRequest(onEndRequest);
        _disabledControls = [];
        _clientID = clientID;
        _ff = $.browser.mozilla;
        _ie6 = $.browser.msie && $.browser.version < 7;
        _rootEl = !_ie6 && typeof (document.documentElement.scrollLeft) != "undefined" ? document.documentElement : document.body;
        Sys.UI._UpdateProgress.prototype._startRequest = getDelegate(this, updateProgressPanel, [Sys.UI._UpdateProgress.prototype._startRequest]);
        Sys.UI._UpdateProgress.prototype._handleEndRequest = getDelegate(this, resetProgressPanel, [Sys.UI._UpdateProgress.prototype._handleEndRequest]);
        if (ensurePanel()) {
            registerEvents();
            updateProgressPanel.apply(_panel, [null]);
        }
    }

    this.show = show;
    this.reset = resetProgressPanel;
    this.displayPanelAndDisableControls = displayPanelAndDisableControls;
    this.displayForPaging = displayForPaging;
    this.hidePanelAndEnableControls = onEndRequest;
    this.hide = hide;
    this.showWithCustomText = showWithCustomText;

    // protected exposed

    function onEndRequest(sender, args) {
        hide();
        for (var i = 0; i < _disabledControls.length; i++) {
            if (_disabledControls[i]) {
                if (_disabledControls[i].disabled) _disabledControls[i].disabled = false;
                var saveClick = _disabledControls[i].saveClick;
                if (saveClick) {
                    _disabledControls[i].onclick = saveClick;
                    _disabledControls[i].saveClick = null;
                }
            }
        }
    }


    function displayPanelAndDisableControls(text) {
        setUpdateProgressText(text);
        show(true);

        //disable the variable amount of controls passed in
        for (var i = 1; i < arguments.length; i++) {
            try {
                if (arguments[i]) {
                    arguments[i].disabled = true;
                    if (arguments[i].onclick) {
                        arguments[i].saveClick = arguments[i].onclick;
                        arguments[i].onclick = function() { return false; };
                    }
                    _disabledControls[i - 1] = arguments[i];
                }
            }
            catch (err) {

            }
        }

    }

    function displayForPaging() {
        if (UpdateProgress_PagingTimer != null) {
            window.clearTimeout(UpdateProgress_PagingTimer);
            UpdateProgress_PagingTimer = null;
        }

        setUpdateProgressText('Loading...');
        UpdateProgress_PagingTimer = window.setTimeout(this.show, 2000);
    }

    // Hide update progess and restore default text
    function hide() {
        if (UpdateProgress_PagingTimer != null) {
            window.clearTimeout(UpdateProgress_PagingTimer);
            UpdateProgress_PagingTimer = null;
        }
        show(false);
        setUpdateProgressText("Updating...");
    }

    function setUpdateProgressText(text) {
        $('#UpdateProgressText').html(text);
    }

    function show(visible) {
        updateProgressPanel();

        visibleStyle = 'visible';
        if (!visible) visibleStyle = 'hidden';

        if (ensurePanel()) {
            _panel.style.visibility = visibleStyle;
            if (visibleStyle == 'hidden') {
                _panel.style.top = "-1000px";
                _panel.style.left = "-1000px";
            }
        }
    }

    function showWithCustomText(visible, text) {
        setUpdateProgressText(text);
        show(visible);
    }

    //protected methods
    function ensurePanel() {
        if (!_clientID) return false;
        if (!_panel) _panel = $('#' + _clientID)[0];
        return !!_panel;
    }

    function updateProgressPanel(callback, sender, args) {
        if (ensurePanel()) {
            if (!_panel) return;
            var root = !CodePlex.UI.isOpera ? document.documentElement : document.body;
            var dims = [((root.offsetWidth - _panel.offsetWidth) / 2) + root.scrollLeft, ((root.clientHeight - _panel.clientHeight) / 2) + root.scrollTop];
            _panel.style.left = Math.round(dims[0]) + "px";
            _panel.style.top = Math.round(dims[1]) + "px";
        }
        fireCallback(callback, sender, args);
    }
    function resetProgressPanel(callback, force) {
        hide();
        fireCallback(callback);
        if (!ensurePanel()) return;
    }
    function getDelegate(obj, fnc, args) {
        return function() {
            for (var i = 0; i < arguments.length; i++) args[args.length] = arguments[i];
            if (fnc) return fnc.apply(obj, args);
        };
    }
    function fireCallback(callback) {
        if (typeof (callback) != 'function') return;
        var args = [];
        for (var i = 1; i < arguments.length; i++) args[args.length] = arguments[i];
        if (ensurePanel()) getDelegate(_panel, callback, args)();
    }
    function getChild(id, elt) {
        if (!elt || !elt.nodeType) return null;
        for (var i = 0; i < elt.childNodes.length; i++) {
            if (elt.childNodes[i].id == id) return elt.childNodes[i];
            var child = getChild(id, elt.childNodes[i]);
            if (child) return child;
        }
        return null;
    }
    
    function registerEvents() {
        $(document).bind('keydown', keyHandler);
    }

    // events
    function showHandler(e) {
        if (UpdateProgress_Hide) {
            UpdateProgress_Hide = false;
            return;
        }
        if (typeof (Page_IsValid) == 'undefined' || Page_IsValid)
            show(true);
    }
    function scrollHandler(e) {
        if (!ensurePanel() || _panel.style.visibility == 'hidden') return;
        updateProgressPanel.apply(_panel, [null]);
    }
    function resizeHandler(e) {
        if (_resizeTimer) clearTimeout(_resizeTimer);
        _resizeTimer = setTimeout(resetProgressPanel, 1);
    }
    function keyHandler(e) {
        if (ensurePanel() && _panel.style.visibility == 'visible') {
            if (e.preventDefault) e.preventDefault();
            if (e.stopPropagation) e.stopPropagation();
            return e.returnValue = false;
        }
    }
};
function openPreviewWikiWindow(url) {
    newWindow = window.open(url, 'Print', 'toolbar=0, menubar=0, location=0, status=0, scrollbars=1, resizable=1');
}

function printWiki() {
    parent.print()
}

function closeWiki() {
    try {
        parent.focus();
        parent.close();
    } catch (e) { }
}

// jQuery plugin: wikiContent 
(function($) {
    var defaults = {
        allowJsonSave: false,
        allowEmpty: false,
        version: 0,
        initialParentWikiTitle: 'Home',
        cancelRedirectUrl: '',
        wikiActionUrl: '',
        initialWikiContent: ''
    };

    $.fn.wikiContent = function(settings) {
        settings = $.extend(defaults, settings);
        return this.each(function() {
            new $.wikiContent(this, settings);
        });
    };

    $.wikiContent = function(element, settings) {
        this.element = element;
        this.settings = settings;
        this.init();
    }

    $.wikiContent.fn = $.wikiContent.prototype = {};
    $.wikiContent.fn.extend = $.wikiContent.extend = $.extend;
    $.wikiContent.fn.extend({
        init: function() {
            this.initContent();

            $(this.element).bind('editWiki', this, function(e) { e.data.edit(); });
            $(this.element).bind('previewWiki', this, function(e) { e.data.preview(); });
            $(this.element).bind('saveWiki', this, function(e) { e.data.save(); });
            $(this.element).bind('cancelWiki', this, function(e) { e.data.cancel(); });
            this.bindContainerEvents();
        },

        initContent: function() {
            this.wikiContent = this.settings.initialWikiContent;
            this.parentWikiTitle = this.settings.initialParentWikiTitle;
        },

        edit: function() {
            this.post('Edit');
        },

        onGenericSuccess: function(data) {
            this.sourceObject.genericSuccess(data);
        },

        genericSuccess: function(data) {
            $(this.element).empty().html(data);
            this.bindContainerEvents();
            this.completeSuccess();
        },

        completeSuccess: function() {
            CodePlex.UpdateProgress.hide();
        },

        bindContainerEvents: function() {
            $('#composeLink', $(this.element)).bind('click', this, function(e) { $(e.data.element).trigger('editWiki'); return false; });
            $('#previewLink', $(this.element)).bind('click', this, function(e) { $(e.data.element).trigger('previewWiki'); return false; });
            $('#saveWikiButton', $(this.element)).bind('click', this, function(e) { $(e.data.element).trigger('saveWiki'); return false; });
            $('#cancelWikiButton', $(this.element)).bind('click', this, function(e) { $(e.data.element).trigger('cancelWiki'); return false; });
        },

        preview: function() {
            if (this.validate())
                this.post('Preview');
        },

        validate: function() {
            this.updateContentFromForm();
            if (!this.settings.allowEmpty && this.wikiContent.trim().length === 0) {
                $('#wikiError').html('Wiki content cannot be empty. Please enter content for the wiki page.').show();
                return false;
            }

            return true;
        },

        updateContentFromForm: function() {
            var wikiContentFromForm = $('#inputText', $(this.element)).val();
            if (wikiContentFromForm != undefined)
                this.wikiContent = wikiContentFromForm;

            var parentWikiTitleFromForm = $('#parentWikiTitle', $(this.element)).val();
            if (parentWikiTitleFromForm && parentWikiTitleFromForm.length)
                this.parentWikiTitle = parentWikiTitleFromForm;
        },

        post: function(wikiAction, override, progressMessage, successCallback) {
            if (override === undefined)
                override = false;
            if (progressMessage === undefined)
                progressMessage = 'Loading...';
            if (successCallback === undefined)
                successCallback = this.onGenericSuccess;

            var dataType = 'html';
            if (this.settings.allowJsonSave && wikiAction === 'Save')
                dataType = 'json';

            CodePlex.UpdateProgress.showWithCustomText(true, progressMessage);

            this.updateContentFromForm();
            $.ajax({
                data: { WikiAction: wikiAction, Version: this.settings.version, WikiContent: this.wikiContent, ParentWikiTitle: this.parentWikiTitle, ForceOverwrite: override },
                type: 'POST',
                url: this.settings.wikiActionUrl,
                success: successCallback,
                sourceObject: this,
                dataType: dataType
            });
        },

        save: function(override) {
            if (override === undefined)
                override = false;

            $('#wikiError').hide();

            if (this.validate())
                this.post('Save', override, 'Saving...', function(data) { this.sourceObject.saveSuccess(data); });
        },

        saveSuccess: function(data) {
            if (typeof (data) === 'object') {
                this.completeSuccess();
                if (data.IsError) {
                    $('#wikiError').text(data.Message).show();
                } else if (data.RequireConcurrencyPrompt) {
                    if (confirm('You are not editing the latest version of this page. If you continue, you will overwrite the latest version. Are you sure you want to overwrite the changes?')) {
                        this.save(true);
                    }
                } else {
                    disableUnsavedData();
                    location.href = data.RedirectUrl;
                }
            } else {
                this.genericSuccess(data);
            }
        },

        cancel: function() {
            if (this.settings.cancelRedirectUrl && this.settings.cancelRedirectUrl.length) {
                disableUnsavedData();
                location.href = this.settings.cancelRedirectUrl;
            } else {
                this.post('View');
                this.initContent();
            }
        }
    });
})(jQuery)
if(!window.CanvasRenderingContext2D){(function(){var m=Math;var mr=m.round;var ms=m.sin;var mc=m.cos;var Z=10;var Z2=Z/2;var G_vmlCanvasManager_={init:function(opt_doc){var doc=opt_doc||document;if(/MSIE/.test(navigator.userAgent)&&!window.opera){var self=this;doc.attachEvent("onreadystatechange",function(){self.init_(doc)})}},init_:function(doc){if(doc.readyState=="complete"){if(!doc.namespaces["g_vml_"]){doc.namespaces.add("g_vml_","urn:schemas-microsoft-com:vml")}var ss=doc.createStyleSheet();ss.cssText="canvas{display:inline-block;overflow:hidden;"+"text-align:left;width:300px;height:150px}"+"g_vml_\\:*{behavior:url(#default#VML)}";var els=doc.getElementsByTagName("canvas");for(var i=0;i<els.length;i++){if(!els[i].getContext){this.initElement(els[i])}}}},fixElement_:function(el){var outerHTML=el.outerHTML;var newEl=el.ownerDocument.createElement(outerHTML);if(outerHTML.slice(-2)!="/>"){var tagName="/"+el.tagName;var ns;while((ns=el.nextSibling)&&ns.tagName!=tagName){ns.removeNode()}if(ns){ns.removeNode()}}el.parentNode.replaceChild(newEl,el);return newEl},initElement:function(el){el=this.fixElement_(el);el.getContext=function(){if(this.context_){return this.context_}return this.context_=new CanvasRenderingContext2D_(this)};el.attachEvent('onpropertychange',onPropertyChange);el.attachEvent('onresize',onResize);var attrs=el.attributes;if(attrs.width&&attrs.width.specified){el.style.width=attrs.width.nodeValue+"px"}else{el.width=el.clientWidth}if(attrs.height&&attrs.height.specified){el.style.height=attrs.height.nodeValue+"px"}else{el.height=el.clientHeight}return el}};function onPropertyChange(e){var el=e.srcElement;switch(e.propertyName){case'width':el.style.width=el.attributes.width.nodeValue+"px";el.getContext().clearRect();break;case'height':el.style.height=el.attributes.height.nodeValue+"px";el.getContext().clearRect();break}}function onResize(e){var el=e.srcElement;if(el.firstChild){el.firstChild.style.width=el.clientWidth+'px';el.firstChild.style.height=el.clientHeight+'px'}}G_vmlCanvasManager_.init();var dec2hex=[];for(var i=0;i<16;i++){for(var j=0;j<16;j++){dec2hex[i*16+j]=i.toString(16)+j.toString(16)}}function createMatrixIdentity(){return[[1,0,0],[0,1,0],[0,0,1]]}function matrixMultiply(m1,m2){var result=createMatrixIdentity();for(var x=0;x<3;x++){for(var y=0;y<3;y++){var sum=0;for(var z=0;z<3;z++){sum+=m1[x][z]*m2[z][y]}result[x][y]=sum}}return result}function copyState(o1,o2){o2.fillStyle=o1.fillStyle;o2.lineCap=o1.lineCap;o2.lineJoin=o1.lineJoin;o2.lineWidth=o1.lineWidth;o2.miterLimit=o1.miterLimit;o2.shadowBlur=o1.shadowBlur;o2.shadowColor=o1.shadowColor;o2.shadowOffsetX=o1.shadowOffsetX;o2.shadowOffsetY=o1.shadowOffsetY;o2.strokeStyle=o1.strokeStyle;o2.arcScaleX_=o1.arcScaleX_;o2.arcScaleY_=o1.arcScaleY_}function processStyle(styleString){var str,alpha=1;styleString=String(styleString);if(styleString.substring(0,3)=="rgb"){var start=styleString.indexOf("(",3);var end=styleString.indexOf(")",start+1);var guts=styleString.substring(start+1,end).split(",");str="#";for(var i=0;i<3;i++){str+=dec2hex[Number(guts[i])]}if((guts.length==4)&&(styleString.substr(3,1)=="a")){alpha=guts[3]}}else{str=styleString}return[str,alpha]}function processLineCap(lineCap){switch(lineCap){case"butt":return"flat";case"round":return"round";case"square":default:return"square"}}function CanvasRenderingContext2D_(surfaceElement){this.m_=createMatrixIdentity();this.mStack_=[];this.aStack_=[];this.currentPath_=[];this.strokeStyle="#000";this.fillStyle="#000";this.lineWidth=1;this.lineJoin="miter";this.lineCap="butt";this.miterLimit=Z*1;this.globalAlpha=1;this.canvas=surfaceElement;var el=surfaceElement.ownerDocument.createElement('div');el.style.width=surfaceElement.clientWidth+'px';el.style.height=surfaceElement.clientHeight+'px';el.style.overflow='hidden';el.style.position='absolute';surfaceElement.appendChild(el);this.element_=el;this.arcScaleX_=1;this.arcScaleY_=1}var contextPrototype=CanvasRenderingContext2D_.prototype;contextPrototype.clearRect=function(){this.element_.innerHTML="";this.currentPath_=[]};contextPrototype.beginPath=function(){this.currentPath_=[]};contextPrototype.moveTo=function(aX,aY){this.currentPath_.push({type:"moveTo",x:aX,y:aY});this.currentX_=aX;this.currentY_=aY};contextPrototype.lineTo=function(aX,aY){this.currentPath_.push({type:"lineTo",x:aX,y:aY});this.currentX_=aX;this.currentY_=aY};contextPrototype.bezierCurveTo=function(aCP1x,aCP1y,aCP2x,aCP2y,aX,aY){this.currentPath_.push({type:"bezierCurveTo",cp1x:aCP1x,cp1y:aCP1y,cp2x:aCP2x,cp2y:aCP2y,x:aX,y:aY});this.currentX_=aX;this.currentY_=aY};contextPrototype.quadraticCurveTo=function(aCPx,aCPy,aX,aY){var cp1x=this.currentX_+2.0/3.0*(aCPx-this.currentX_);var cp1y=this.currentY_+2.0/3.0*(aCPy-this.currentY_);var cp2x=cp1x+(aX-this.currentX_)/3.0;var cp2y=cp1y+(aY-this.currentY_)/3.0;this.bezierCurveTo(cp1x,cp1y,cp2x,cp2y,aX,aY)};contextPrototype.arc=function(aX,aY,aRadius,aStartAngle,aEndAngle,aClockwise){aRadius*=Z;var arcType=aClockwise?"at":"wa";var xStart=aX+(mc(aStartAngle)*aRadius)-Z2;var yStart=aY+(ms(aStartAngle)*aRadius)-Z2;var xEnd=aX+(mc(aEndAngle)*aRadius)-Z2;var yEnd=aY+(ms(aEndAngle)*aRadius)-Z2;if(xStart==xEnd&&!aClockwise){xStart+=0.125}this.currentPath_.push({type:arcType,x:aX,y:aY,radius:aRadius,xStart:xStart,yStart:yStart,xEnd:xEnd,yEnd:yEnd})};contextPrototype.rect=function(aX,aY,aWidth,aHeight){this.moveTo(aX,aY);this.lineTo(aX+aWidth,aY);this.lineTo(aX+aWidth,aY+aHeight);this.lineTo(aX,aY+aHeight);this.closePath()};contextPrototype.strokeRect=function(aX,aY,aWidth,aHeight){this.beginPath();this.moveTo(aX,aY);this.lineTo(aX+aWidth,aY);this.lineTo(aX+aWidth,aY+aHeight);this.lineTo(aX,aY+aHeight);this.closePath();this.stroke()};contextPrototype.fillRect=function(aX,aY,aWidth,aHeight){this.beginPath();this.moveTo(aX,aY);this.lineTo(aX+aWidth,aY);this.lineTo(aX+aWidth,aY+aHeight);this.lineTo(aX,aY+aHeight);this.closePath();this.fill()};contextPrototype.createLinearGradient=function(aX0,aY0,aX1,aY1){var gradient=new CanvasGradient_("gradient");return gradient};contextPrototype.createRadialGradient=function(aX0,aY0,aR0,aX1,aY1,aR1){var gradient=new CanvasGradient_("gradientradial");gradient.radius1_=aR0;gradient.radius2_=aR1;gradient.focus_.x=aX0;gradient.focus_.y=aY0;return gradient};contextPrototype.drawImage=function(image,var_args){var dx,dy,dw,dh,sx,sy,sw,sh;var oldRuntimeWidth=image.runtimeStyle.width;var oldRuntimeHeight=image.runtimeStyle.height;image.runtimeStyle.width='auto';image.runtimeStyle.height='auto';var w=image.width;var h=image.height;image.runtimeStyle.width=oldRuntimeWidth;image.runtimeStyle.height=oldRuntimeHeight;if(arguments.length==3){dx=arguments[1];dy=arguments[2];sx=sy=0;sw=dw=w;sh=dh=h}else if(arguments.length==5){dx=arguments[1];dy=arguments[2];dw=arguments[3];dh=arguments[4];sx=sy=0;sw=w;sh=h}else if(arguments.length==9){sx=arguments[1];sy=arguments[2];sw=arguments[3];sh=arguments[4];dx=arguments[5];dy=arguments[6];dw=arguments[7];dh=arguments[8]}else{throw"Invalid number of arguments";}var d=this.getCoords_(dx,dy);var w2=sw/2;var h2=sh/2;var vmlStr=[];var W=10;var H=10;vmlStr.push(' <g_vml_:group',' coordsize="',Z*W,',',Z*H,'"',' coordorigin="0,0"',' style="width:',W,';height:',H,';position:absolute;');if(this.m_[0][0]!=1||this.m_[0][1]){var filter=[];filter.push("M11='",this.m_[0][0],"',","M12='",this.m_[1][0],"',","M21='",this.m_[0][1],"',","M22='",this.m_[1][1],"',","Dx='",mr(d.x/Z),"',","Dy='",mr(d.y/Z),"'");var max=d;var c2=this.getCoords_(dx+dw,dy);var c3=this.getCoords_(dx,dy+dh);var c4=this.getCoords_(dx+dw,dy+dh);max.x=Math.max(max.x,c2.x,c3.x,c4.x);max.y=Math.max(max.y,c2.y,c3.y,c4.y);vmlStr.push("padding:0 ",mr(max.x/Z),"px ",mr(max.y/Z),"px 0;filter:progid:DXImageTransform.Microsoft.Matrix(",filter.join(""),", sizingmethod='clip');")}else{vmlStr.push("top:",mr(d.y/Z),"px;left:",mr(d.x/Z),"px;")}vmlStr.push(' ">','<g_vml_:image src="',image.src,'"',' style="width:',Z*dw,';',' height:',Z*dh,';"',' cropleft="',sx/w,'"',' croptop="',sy/h,'"',' cropright="',(w-sx-sw)/w,'"',' cropbottom="',(h-sy-sh)/h,'"',' />','</g_vml_:group>');this.element_.insertAdjacentHTML("BeforeEnd",vmlStr.join(""))};contextPrototype.stroke=function(aFill){var lineStr=[];var lineOpen=false;var a=processStyle(aFill?this.fillStyle:this.strokeStyle);var color=a[0];var opacity=a[1]*this.globalAlpha;var W=10;var H=10;lineStr.push('<g_vml_:shape',' fillcolor="',color,'"',' filled="',Boolean(aFill),'"',' style="position:absolute;width:',W,';height:',H,';"',' coordorigin="0 0" coordsize="',Z*W,' ',Z*H,'"',' stroked="',!aFill,'"',' strokeweight="',this.lineWidth,'"',' strokecolor="',color,'"',' path="');var newSeq=false;var min={x:null,y:null};var max={x:null,y:null};for(var i=0;i<this.currentPath_.length;i++){var p=this.currentPath_[i];if(p.type=="moveTo"){lineStr.push(" m ");var c=this.getCoords_(p.x,p.y);lineStr.push(mr(c.x),",",mr(c.y))}else if(p.type=="lineTo"){lineStr.push(" l ");var c=this.getCoords_(p.x,p.y);lineStr.push(mr(c.x),",",mr(c.y))}else if(p.type=="close"){lineStr.push(" x ")}else if(p.type=="bezierCurveTo"){lineStr.push(" c ");var c=this.getCoords_(p.x,p.y);var c1=this.getCoords_(p.cp1x,p.cp1y);var c2=this.getCoords_(p.cp2x,p.cp2y);lineStr.push(mr(c1.x),",",mr(c1.y),",",mr(c2.x),",",mr(c2.y),",",mr(c.x),",",mr(c.y))}else if(p.type=="at"||p.type=="wa"){lineStr.push(" ",p.type," ");var c=this.getCoords_(p.x,p.y);var cStart=this.getCoords_(p.xStart,p.yStart);var cEnd=this.getCoords_(p.xEnd,p.yEnd);lineStr.push(mr(c.x-this.arcScaleX_*p.radius),",",mr(c.y-this.arcScaleY_*p.radius)," ",mr(c.x+this.arcScaleX_*p.radius),",",mr(c.y+this.arcScaleY_*p.radius)," ",mr(cStart.x),",",mr(cStart.y)," ",mr(cEnd.x),",",mr(cEnd.y))}if(c){if(min.x==null||c.x<min.x){min.x=c.x}if(max.x==null||c.x>max.x){max.x=c.x}if(min.y==null||c.y<min.y){min.y=c.y}if(max.y==null||c.y>max.y){max.y=c.y}}}lineStr.push(' ">');if(typeof this.fillStyle=="object"){var focus={x:"50%",y:"50%"};var width=(max.x-min.x);var height=(max.y-min.y);var dimension=(width>height)?width:height;focus.x=mr((this.fillStyle.focus_.x/width)*100+50)+"%";focus.y=mr((this.fillStyle.focus_.y/height)*100+50)+"%";var colors=[];if(this.fillStyle.type_=="gradientradial"){var inside=(this.fillStyle.radius1_/dimension*100);var expansion=(this.fillStyle.radius2_/dimension*100)-inside}else{var inside=0;var expansion=100}var insidecolor={offset:null,color:null};var outsidecolor={offset:null,color:null};this.fillStyle.colors_.sort(function(cs1,cs2){return cs1.offset-cs2.offset});for(var i=0;i<this.fillStyle.colors_.length;i++){var fs=this.fillStyle.colors_[i];colors.push((fs.offset*expansion)+inside,"% ",fs.color,",");if(fs.offset>insidecolor.offset||insidecolor.offset==null){insidecolor.offset=fs.offset;insidecolor.color=fs.color}if(fs.offset<outsidecolor.offset||outsidecolor.offset==null){outsidecolor.offset=fs.offset;outsidecolor.color=fs.color}}colors.pop();lineStr.push('<g_vml_:fill',' color="',outsidecolor.color,'"',' color2="',insidecolor.color,'"',' type="',this.fillStyle.type_,'"',' focusposition="',focus.x,', ',focus.y,'"',' colors="',colors.join(""),'"',' opacity="',opacity,'" />')}else if(aFill){lineStr.push('<g_vml_:fill color="',color,'" opacity="',opacity,'" />')}else{lineStr.push('<g_vml_:stroke',' opacity="',opacity,'"',' joinstyle="',this.lineJoin,'"',' miterlimit="',this.miterLimit,'"',' endcap="',processLineCap(this.lineCap),'"',' weight="',this.lineWidth,'px"',' color="',color,'" />')}lineStr.push("</g_vml_:shape>");this.element_.insertAdjacentHTML("beforeEnd",lineStr.join(""))};contextPrototype.fill=function(){this.stroke(true)};contextPrototype.closePath=function(){this.currentPath_.push({type:"close"})};contextPrototype.getCoords_=function(aX,aY){return{x:Z*(aX*this.m_[0][0]+aY*this.m_[1][0]+this.m_[2][0])-Z2,y:Z*(aX*this.m_[0][1]+aY*this.m_[1][1]+this.m_[2][1])-Z2}};contextPrototype.save=function(){var o={};copyState(this,o);this.aStack_.push(o);this.mStack_.push(this.m_);this.m_=matrixMultiply(createMatrixIdentity(),this.m_)};contextPrototype.restore=function(){copyState(this.aStack_.pop(),this);this.m_=this.mStack_.pop()};contextPrototype.translate=function(aX,aY){var m1=[[1,0,0],[0,1,0],[aX,aY,1]];this.m_=matrixMultiply(m1,this.m_)};contextPrototype.rotate=function(aRot){var c=mc(aRot);var s=ms(aRot);var m1=[[c,s,0],[-s,c,0],[0,0,1]];this.m_=matrixMultiply(m1,this.m_)};contextPrototype.scale=function(aX,aY){this.arcScaleX_*=aX;this.arcScaleY_*=aY;var m1=[[aX,0,0],[0,aY,0],[0,0,1]];this.m_=matrixMultiply(m1,this.m_)};contextPrototype.clip=function(){};contextPrototype.arcTo=function(){};contextPrototype.createPattern=function(){return new CanvasPattern_};function CanvasGradient_(aType){this.type_=aType;this.radius1_=0;this.radius2_=0;this.colors_=[];this.focus_={x:0,y:0}}CanvasGradient_.prototype.addColorStop=function(aOffset,aColor){aColor=processStyle(aColor);this.colors_.push({offset:1-aOffset,color:aColor})};function CanvasPattern_(){}G_vmlCanvasManager=G_vmlCanvasManager_;CanvasRenderingContext2D=CanvasRenderingContext2D_;CanvasGradient=CanvasGradient_;CanvasPattern=CanvasPattern_})()}
(function(F){function D(AO,e,f){var W=[],o={colors:["#edc240","#afd8f8","#cb4b4b","#4da74d","#9440ed"],legend:{show:true,noColumns:1,labelFormatter:null,labelBoxBorderColor:"#ccc",container:null,position:"ne",margin:5,backgroundColor:null,backgroundOpacity:0.85},xaxis:{mode:null,min:null,max:null,autoscaleMargin:null,ticks:null,tickFormatter:null,labelWidth:null,labelHeight:null,tickDecimals:null,tickSize:null,minTickSize:null,monthNames:null,timeformat:null},yaxis:{autoscaleMargin:0.02},x2axis:{autoscaleMargin:null},y2axis:{autoscaleMargin:0.02},points:{show:false,radius:3,lineWidth:2,fill:true,fillColor:"#ffffff"},lines:{show:false,lineWidth:2,fill:false,fillColor:null},bars:{show:false,lineWidth:2,barWidth:1,fill:true,fillColor:null,align:"left"},grid:{color:"#545454",backgroundColor:null,tickColor:"#dddddd",labelMargin:5,borderWidth:2,markings:null,markingsColor:"#f4f4f4",markingsLineWidth:2,clickable:false,hoverable:false,autoHighlight:true,mouseActiveRadius:10},selection:{mode:null,color:"#e8cfac"},shadowSize:4},X=null,AP=null,AQ=null,g=null,AX=null,K=AO,AA={xaxis:{},yaxis:{},x2axis:{},y2axis:{}},m={left:0,right:0,top:0,bottom:0},AI=0,Z=0,N=0,AB=0,S={};this.setData=n;this.setupGrid=s;this.draw=AU;this.clearSelection=I;this.setSelection=AC;this.getCanvas=function(){return X};this.getPlotOffset=function(){return m};this.getData=function(){return W};this.getAxes=function(){return AA};this.highlight=AS;this.unhighlight=AH;y(f);n(e);j();s();AU();function n(AY){W=U(AY);c();t()}function U(Ac){var Aa=[];for(var AZ=0;AZ<Ac.length;++AZ){var Ab;if(Ac[AZ].data){Ab={};for(var AY in Ac[AZ]){Ab[AY]=Ac[AZ][AY]}}else{Ab={data:Ac[AZ]}}Aa.push(Ab)}return Aa}function y(AY){F.extend(true,o,AY);if(o.xaxis.noTicks&&o.xaxis.ticks==null){o.xaxis.ticks=o.xaxis.noTicks}if(o.yaxis.noTicks&&o.yaxis.ticks==null){o.yaxis.ticks=o.yaxis.noTicks}if(o.grid.coloredAreas){o.grid.markings=o.grid.coloredAreas}if(o.grid.coloredAreasColor){o.grid.markingsColor=o.grid.coloredAreasColor}}function c(){var Ad;var Ai=W.length,AY=[],Ab=[];for(Ad=0;Ad<W.length;++Ad){var Ah=W[Ad].color;if(Ah!=null){--Ai;if(typeof Ah=="number"){Ab.push(Ah)}else{AY.push(E(W[Ad].color))}}}for(Ad=0;Ad<Ab.length;++Ad){Ai=Math.max(Ai,Ab[Ad]+1)}var AZ=[],Ac=0;Ad=0;while(AZ.length<Ai){var Ag;if(o.colors.length==Ad){Ag=new G(100,100,100)}else{Ag=E(o.colors[Ad])}var Aa=Ac%2==1?-1:1;var Af=1+Aa*Math.ceil(Ac/2)*0.2;Ag.scale(Af,Af,Af);AZ.push(Ag);++Ad;if(Ad>=o.colors.length){Ad=0;++Ac}}var Ae=0,Aj;for(Ad=0;Ad<W.length;++Ad){Aj=W[Ad];if(Aj.color==null){Aj.color=AZ[Ae].toString();++Ae}else{if(typeof Aj.color=="number"){Aj.color=AZ[Aj.color].toString()}}Aj.lines=F.extend(true,{},o.lines,Aj.lines);Aj.points=F.extend(true,{},o.points,Aj.points);Aj.bars=F.extend(true,{},o.bars,Aj.bars);if(Aj.shadowSize==null){Aj.shadowSize=o.shadowSize}if(Aj.xaxis&&Aj.xaxis==2){Aj.xaxis=AA.x2axis}else{Aj.xaxis=AA.xaxis}if(Aj.yaxis&&Aj.yaxis==2){Aj.yaxis=AA.y2axis}else{Aj.yaxis=AA.yaxis}}}function t(){var Aa=Number.POSITIVE_INFINITY,AZ=Number.NEGATIVE_INFINITY,Ab;for(Ab in AA){AA[Ab].datamin=Aa;AA[Ab].datamax=AZ;AA[Ab].used=false}for(var Ae=0;Ae<W.length;++Ae){var Ad=W[Ae].data,Aj=W[Ae].xaxis,Ai=W[Ae].yaxis,AY=0,Ah=0;if(W[Ae].bars.show){AY=W[Ae].bars.align=="left"?0:-W[Ae].bars.barWidth/2;Ah=AY+W[Ae].bars.barWidth}Aj.used=Ai.used=true;for(var Ac=0;Ac<Ad.length;++Ac){if(Ad[Ac]==null){continue}var Ag=Ad[Ac][0],Af=Ad[Ac][1];if(Ag!=null&&!isNaN(Ag=+Ag)){if(Ag+AY<Aj.datamin){Aj.datamin=Ag+AY}if(Ag+Ah>Aj.datamax){Aj.datamax=Ag+Ah}}if(Af!=null&&!isNaN(Af=+Af)){if(Af<Ai.datamin){Ai.datamin=Af}if(Af>Ai.datamax){Ai.datamax=Af}}if(Ag==null||Af==null||isNaN(Ag)||isNaN(Af)){Ad[Ac]=null}}}for(Ab in AA){if(AA[Ab].datamin==Aa){AA[Ab].datamin=0}if(AA[Ab].datamax==AZ){AA[Ab].datamax=1}}}function j(){AI=K.width();Z=K.height();K.html("");K.css("position","relative");if(AI<=0||Z<=0){throw"Invalid dimensions for plot, width = "+AI+", height = "+Z}X=F('<canvas width="'+AI+'" height="'+Z+'"></canvas>').appendTo(K).get(0);if(F.browser.msie){X=window.G_vmlCanvasManager.initElement(X)}g=X.getContext("2d");AP=F('<canvas style="position:absolute;left:0px;top:0px;" width="'+AI+'" height="'+Z+'"></canvas>').appendTo(K).get(0);if(F.browser.msie){AP=window.G_vmlCanvasManager.initElement(AP)}AX=AP.getContext("2d");AQ=F([AP,X]);if(o.selection.mode!=null||o.grid.hoverable){AQ.each(function(){this.onmousemove=J});if(o.selection.mode!=null){AQ.mousedown(AN)}}if(o.grid.clickable){AQ.click(k)}}function s(){function AY(Ab,Aa){Q(Ab,Aa);L(Ab,Aa);w(Ab,Aa);if(Ab==AA.xaxis||Ab==AA.x2axis){Ab.p2c=function(Ac){return(Ac-Ab.min)*Ab.scale};Ab.c2p=function(Ac){return Ab.min+Ac/Ab.scale}}else{Ab.p2c=function(Ac){return(Ab.max-Ac)*Ab.scale};Ab.c2p=function(Ac){return Ab.max-Ac/Ab.scale}}}for(var AZ in AA){AY(AA[AZ],o[AZ])}AW();p();AV()}function Q(Ab,Ad){var Aa=Ad.min!=null?Ad.min:Ab.datamin;var AY=Ad.max!=null?Ad.max:Ab.datamax;if(AY-Aa==0){var AZ;if(AY==0){AZ=1}else{AZ=0.01}Aa-=AZ;AY+=AZ}else{var Ac=Ad.autoscaleMargin;if(Ac!=null){if(Ad.min==null){Aa-=(AY-Aa)*Ac;if(Aa<0&&Ab.datamin>=0){Aa=0}}if(Ad.max==null){AY+=(AY-Aa)*Ac;if(AY>0&&Ab.datamax<=0){AY=0}}}}Ab.min=Aa;Ab.max=AY}function L(Ad,Ag){var Ac;if(typeof Ag.ticks=="number"&&Ag.ticks>0){Ac=Ag.ticks}else{if(Ad==AA.xaxis||Ad==AA.x2axis){Ac=AI/100}else{Ac=Z/60}}var Al=(Ad.max-Ad.min)/Ac;var Ao,Ah,Aj,Ak,Af,Aa,AZ;if(Ag.mode=="time"){function An(Av,Ap,Ar){var Aq=function(Ax){Ax=""+Ax;return Ax.length==1?"0"+Ax:Ax};var Au=[];var At=false;if(Ar==null){Ar=["Jan","Feb","Mar","Apr","May","Jun","Jul","Aug","Sep","Oct","Nov","Dec"]}for(var As=0;As<Ap.length;++As){var Aw=Ap.charAt(As);if(At){switch(Aw){case"h":Aw=""+Av.getUTCHours();break;case"H":Aw=Aq(Av.getUTCHours());break;case"M":Aw=Aq(Av.getUTCMinutes());break;case"S":Aw=Aq(Av.getUTCSeconds());break;case"d":Aw=""+Av.getUTCDate();break;case"m":Aw=""+(Av.getUTCMonth()+1);break;case"y":Aw=""+Av.getUTCFullYear();break;case"b":Aw=""+Ar[Av.getUTCMonth()];break}Au.push(Aw);At=false}else{if(Aw=="%"){At=true}else{Au.push(Aw)}}}return Au.join("")}var Ai={second:1000,minute:60*1000,hour:60*60*1000,day:24*60*60*1000,month:30*24*60*60*1000,year:365.2425*24*60*60*1000};var Am=[[1,"second"],[2,"second"],[5,"second"],[10,"second"],[30,"second"],[1,"minute"],[2,"minute"],[5,"minute"],[10,"minute"],[30,"minute"],[1,"hour"],[2,"hour"],[4,"hour"],[8,"hour"],[12,"hour"],[1,"day"],[2,"day"],[3,"day"],[0.25,"month"],[0.5,"month"],[1,"month"],[2,"month"],[3,"month"],[6,"month"],[1,"year"]];var Ab=0;if(Ag.minTickSize!=null){if(typeof Ag.tickSize=="number"){Ab=Ag.tickSize}else{Ab=Ag.minTickSize[0]*Ai[Ag.minTickSize[1]]}}for(Af=0;Af<Am.length-1;++Af){if(Al<(Am[Af][0]*Ai[Am[Af][1]]+Am[Af+1][0]*Ai[Am[Af+1][1]])/2&&Am[Af][0]*Ai[Am[Af][1]]>=Ab){break}}Ao=Am[Af][0];Aj=Am[Af][1];if(Aj=="year"){Aa=Math.pow(10,Math.floor(Math.log(Al/Ai.year)/Math.LN10));AZ=(Al/Ai.year)/Aa;if(AZ<1.5){Ao=1}else{if(AZ<3){Ao=2}else{if(AZ<7.5){Ao=5}else{Ao=10}}}Ao*=Aa}if(Ag.tickSize){Ao=Ag.tickSize[0];Aj=Ag.tickSize[1]}Ah=function(Ar){var Aw=[],Au=Ar.tickSize[0],Ax=Ar.tickSize[1],Av=new Date(Ar.min);var Aq=Au*Ai[Ax];if(Ax=="second"){Av.setUTCSeconds(C(Av.getUTCSeconds(),Au))}if(Ax=="minute"){Av.setUTCMinutes(C(Av.getUTCMinutes(),Au))}if(Ax=="hour"){Av.setUTCHours(C(Av.getUTCHours(),Au))}if(Ax=="month"){Av.setUTCMonth(C(Av.getUTCMonth(),Au))}if(Ax=="year"){Av.setUTCFullYear(C(Av.getUTCFullYear(),Au))}Av.setUTCMilliseconds(0);if(Aq>=Ai.minute){Av.setUTCSeconds(0)}if(Aq>=Ai.hour){Av.setUTCMinutes(0)}if(Aq>=Ai.day){Av.setUTCHours(0)}if(Aq>=Ai.day*4){Av.setUTCDate(1)}if(Aq>=Ai.year){Av.setUTCMonth(0)}var Az=0,Ay=Number.NaN,As;do{As=Ay;Ay=Av.getTime();Aw.push({v:Ay,label:Ar.tickFormatter(Ay,Ar)});if(Ax=="month"){if(Au<1){Av.setUTCDate(1);var Ap=Av.getTime();Av.setUTCMonth(Av.getUTCMonth()+1);var At=Av.getTime();Av.setTime(Ay+Az*Ai.hour+(At-Ap)*Au);Az=Av.getUTCHours();Av.setUTCHours(0)}else{Av.setUTCMonth(Av.getUTCMonth()+Au)}}else{if(Ax=="year"){Av.setUTCFullYear(Av.getUTCFullYear()+Au)}else{Av.setTime(Ay+Aq)}}}while(Ay<Ar.max&&Ay!=As);return Aw};Ak=function(Ap,As){var At=new Date(Ap);if(Ag.timeformat!=null){return An(At,Ag.timeformat,Ag.monthNames)}var Aq=As.tickSize[0]*Ai[As.tickSize[1]];var Ar=As.max-As.min;if(Aq<Ai.minute){fmt="%h:%M:%S"}else{if(Aq<Ai.day){if(Ar<2*Ai.day){fmt="%h:%M"}else{fmt="%b %d %h:%M"}}else{if(Aq<Ai.month){fmt="%b %d"}else{if(Aq<Ai.year){if(Ar<Ai.year){fmt="%b"}else{fmt="%b %y"}}else{fmt="%y"}}}}return An(At,fmt,Ag.monthNames)}}else{var AY=Ag.tickDecimals;var Ae=-Math.floor(Math.log(Al)/Math.LN10);if(AY!=null&&Ae>AY){Ae=AY}Aa=Math.pow(10,-Ae);AZ=Al/Aa;if(AZ<1.5){Ao=1}else{if(AZ<3){Ao=2;if(AZ>2.25&&(AY==null||Ae+1<=AY)){Ao=2.5;++Ae}}else{if(AZ<7.5){Ao=5}else{Ao=10}}}Ao*=Aa;if(Ag.minTickSize!=null&&Ao<Ag.minTickSize){Ao=Ag.minTickSize}if(Ag.tickSize!=null){Ao=Ag.tickSize}Ad.tickDecimals=Math.max(0,(AY!=null)?AY:Ae);Ah=function(Ar){var At=[];var Au=C(Ar.min,Ar.tickSize),Aq=0,Ap=Number.NaN,As;do{As=Ap;Ap=Au+Aq*Ar.tickSize;At.push({v:Ap,label:Ar.tickFormatter(Ap,Ar)});++Aq}while(Ap<Ar.max&&Ap!=As);return At};Ak=function(Ap,Aq){return Ap.toFixed(Aq.tickDecimals)}}Ad.tickSize=Aj?[Ao,Aj]:Ao;Ad.tickGenerator=Ah;if(F.isFunction(Ag.tickFormatter)){Ad.tickFormatter=function(Ap,Aq){return""+Ag.tickFormatter(Ap,Aq)}}else{Ad.tickFormatter=Ak}if(Ag.labelWidth!=null){Ad.labelWidth=Ag.labelWidth}if(Ag.labelHeight!=null){Ad.labelHeight=Ag.labelHeight}}function w(Ac,Ae){Ac.ticks=[];if(!Ac.used){return }if(Ae.ticks==null){Ac.ticks=Ac.tickGenerator(Ac)}else{if(typeof Ae.ticks=="number"){if(Ae.ticks>0){Ac.ticks=Ac.tickGenerator(Ac)}}else{if(Ae.ticks){var Ad=Ae.ticks;if(F.isFunction(Ad)){Ad=Ad({min:Ac.min,max:Ac.max})}var Ab,AY;for(Ab=0;Ab<Ad.length;++Ab){var AZ=null;var Aa=Ad[Ab];if(typeof Aa=="object"){AY=Aa[0];if(Aa.length>1){AZ=Aa[1]}}else{AY=Aa}if(AZ==null){AZ=Ac.tickFormatter(AY,Ac)}Ac.ticks[Ab]={v:AY,label:AZ}}}}}if(Ae.autoscaleMargin!=null&&Ac.ticks.length>0){if(Ae.min==null){Ac.min=Math.min(Ac.min,Ac.ticks[0].v)}if(Ae.max==null&&Ac.ticks.length>1){Ac.max=Math.min(Ac.max,Ac.ticks[Ac.ticks.length-1].v)}}}function AW(){function AZ(Ac){if(Ac.labelWidth==null){Ac.labelWidth=AI/6}if(Ac.labelHeight==null){labels=[];for(i=0;i<Ac.ticks.length;++i){l=Ac.ticks[i].label;if(l){labels.push('<div class="tickLabel" style="float:left;width:'+Ac.labelWidth+'px">'+l+"</div>")}}Ac.labelHeight=0;if(labels.length>0){var Ab=F('<div style="position:absolute;top:-10000px;width:10000px;font-size:smaller">'+labels.join("")+'<div style="clear:left"></div></div>').appendTo(K);Ac.labelHeight=Ab.height();Ab.remove()}}}function AY(Ae){if(Ae.labelWidth==null||Ae.labelHeight==null){var Ad,Af=[],Ac;for(Ad=0;Ad<Ae.ticks.length;++Ad){Ac=Ae.ticks[Ad].label;if(Ac){Af.push('<div class="tickLabel">'+Ac+"</div>")}}if(Af.length>0){var Ab=F('<div style="position:absolute;top:-10000px;font-size:smaller">'+Af.join("")+"</div>").appendTo(K);if(Ae.labelWidth==null){Ae.labelWidth=Ab.width()}if(Ae.labelHeight==null){Ae.labelHeight=Ab.find("div").height()}Ab.remove()}if(Ae.labelWidth==null){Ae.labelWidth=0}if(Ae.labelHeight==null){Ae.labelHeight=0}}}AZ(AA.xaxis);AY(AA.yaxis);AZ(AA.x2axis);AY(AA.y2axis);var Aa=o.grid.borderWidth/2;for(i=0;i<W.length;++i){Aa=Math.max(Aa,2*(W[i].points.radius+W[i].points.lineWidth/2))}m.left=m.right=m.top=m.bottom=Aa;if(AA.xaxis.labelHeight>0){m.bottom=Math.max(Aa,AA.xaxis.labelHeight+o.grid.labelMargin)}if(AA.yaxis.labelWidth>0){m.left=Math.max(Aa,AA.yaxis.labelWidth+o.grid.labelMargin)}if(AA.x2axis.labelHeight>0){m.top=Math.max(Aa,AA.x2axis.labelHeight+o.grid.labelMargin)}if(AA.y2axis.labelWidth>0){m.right=Math.max(Aa,AA.y2axis.labelWidth+o.grid.labelMargin)}N=AI-m.left-m.right;AB=Z-m.bottom-m.top;AA.xaxis.scale=N/(AA.xaxis.max-AA.xaxis.min);AA.yaxis.scale=AB/(AA.yaxis.max-AA.yaxis.min);AA.x2axis.scale=N/(AA.x2axis.max-AA.x2axis.min);AA.y2axis.scale=AB/(AA.y2axis.max-AA.y2axis.min)}function AU(){a();for(var AY=0;AY<W.length;AY++){AK(W[AY])}}function V(AZ,Af){var Ac=Af+"axis",AY=Af+"2axis",Ab,Ae,Ad,Aa;if(AZ[Ac]){Ab=AA[Ac];Ae=AZ[Ac].from;Ad=AZ[Ac].to}else{if(AZ[AY]){Ab=AA[AY];Ae=AZ[AY].from;Ad=AZ[AY].to}else{Ab=AA[Ac];Ae=AZ[Af+"1"];Ad=AZ[Af+"2"]}}if(Ae!=null&&Ad!=null&&Ae>Ad){return{from:Ad,to:Ae,axis:Ab}}return{from:Ae,to:Ad,axis:Ab}}function a(){var Ac;g.save();g.clearRect(0,0,AI,Z);g.translate(m.left,m.top);if(o.grid.backgroundColor){g.fillStyle=o.grid.backgroundColor;g.fillRect(0,0,N,AB)}if(o.grid.markings){var AZ=o.grid.markings;if(F.isFunction(AZ)){AZ=AZ({xmin:AA.xaxis.min,xmax:AA.xaxis.max,ymin:AA.yaxis.min,ymax:AA.yaxis.max,xaxis:AA.xaxis,yaxis:AA.yaxis,x2axis:AA.x2axis,y2axis:AA.y2axis})}for(Ac=0;Ac<AZ.length;++Ac){var AY=AZ[Ac],Ae=V(AY,"x"),Ab=V(AY,"y");if(Ae.from==null){Ae.from=Ae.axis.min}if(Ae.to==null){Ae.to=Ae.axis.max}if(Ab.from==null){Ab.from=Ab.axis.min}if(Ab.to==null){Ab.to=Ab.axis.max}if(Ae.to<Ae.axis.min||Ae.from>Ae.axis.max||Ab.to<Ab.axis.min||Ab.from>Ab.axis.max){continue}Ae.from=Math.max(Ae.from,Ae.axis.min);Ae.to=Math.min(Ae.to,Ae.axis.max);Ab.from=Math.max(Ab.from,Ab.axis.min);Ab.to=Math.min(Ab.to,Ab.axis.max);if(Ae.from==Ae.to&&Ab.from==Ab.to){continue}Ae.from=Ae.axis.p2c(Ae.from);Ae.to=Ae.axis.p2c(Ae.to);Ab.from=Ab.axis.p2c(Ab.from);Ab.to=Ab.axis.p2c(Ab.to);if(Ae.from==Ae.to||Ab.from==Ab.to){g.strokeStyle=AY.color||o.grid.markingsColor;g.lineWidth=AY.lineWidth||o.grid.markingsLineWidth;g.moveTo(Math.floor(Ae.from),Math.floor(Ab.from));g.lineTo(Math.floor(Ae.to),Math.floor(Ab.to));g.stroke()}else{g.fillStyle=AY.color||o.grid.markingsColor;g.fillRect(Math.floor(Ae.from),Math.floor(Ab.to),Math.floor(Ae.to-Ae.from),Math.floor(Ab.from-Ab.to))}}}g.lineWidth=1;g.strokeStyle=o.grid.tickColor;g.beginPath();var Aa,Ad=AA.xaxis;for(Ac=0;Ac<Ad.ticks.length;++Ac){Aa=Ad.ticks[Ac].v;if(Aa<=Ad.min||Aa>=AA.xaxis.max){continue}g.moveTo(Math.floor(Ad.p2c(Aa))+g.lineWidth/2,0);g.lineTo(Math.floor(Ad.p2c(Aa))+g.lineWidth/2,AB)}Ad=AA.yaxis;for(Ac=0;Ac<Ad.ticks.length;++Ac){Aa=Ad.ticks[Ac].v;if(Aa<=Ad.min||Aa>=Ad.max){continue}g.moveTo(0,Math.floor(Ad.p2c(Aa))+g.lineWidth/2);g.lineTo(N,Math.floor(Ad.p2c(Aa))+g.lineWidth/2)}Ad=AA.x2axis;for(Ac=0;Ac<Ad.ticks.length;++Ac){Aa=Ad.ticks[Ac].v;if(Aa<=Ad.min||Aa>=Ad.max){continue}g.moveTo(Math.floor(Ad.p2c(Aa))+g.lineWidth/2,-5);g.lineTo(Math.floor(Ad.p2c(Aa))+g.lineWidth/2,5)}Ad=AA.y2axis;for(Ac=0;Ac<Ad.ticks.length;++Ac){Aa=Ad.ticks[Ac].v;if(Aa<=Ad.min||Aa>=Ad.max){continue}g.moveTo(N-5,Math.floor(Ad.p2c(Aa))+g.lineWidth/2);g.lineTo(N+5,Math.floor(Ad.p2c(Aa))+g.lineWidth/2)}g.stroke();if(o.grid.borderWidth){g.lineWidth=o.grid.borderWidth;g.strokeStyle=o.grid.color;g.lineJoin="round";g.strokeRect(0,0,N,AB)}g.restore()}function p(){K.find(".tickLabels").remove();var AY='<div class="tickLabels" style="font-size:smaller;color:'+o.grid.color+'">';function AZ(Ac,Ad){for(var Ab=0;Ab<Ac.ticks.length;++Ab){var Aa=Ac.ticks[Ab];if(!Aa.label||Aa.v<Ac.min||Aa.v>Ac.max){continue}AY+=Ad(Aa,Ac)}}AZ(AA.xaxis,function(Aa,Ab){return'<div style="position:absolute;top:'+(m.top+AB+o.grid.labelMargin)+"px;left:"+(m.left+Ab.p2c(Aa.v)-Ab.labelWidth/2)+"px;width:"+Ab.labelWidth+'px;text-align:center" class="tickLabel">'+Aa.label+"</div>"});AZ(AA.yaxis,function(Aa,Ab){return'<div style="position:absolute;top:'+(m.top+Ab.p2c(Aa.v)-Ab.labelHeight/2)+"px;right:"+(m.right+N+o.grid.labelMargin)+"px;width:"+Ab.labelWidth+'px;text-align:right" class="tickLabel">'+Aa.label+"</div>"});AZ(AA.x2axis,function(Aa,Ab){return'<div style="position:absolute;bottom:'+(m.bottom+AB+o.grid.labelMargin)+"px;left:"+(m.left+Ab.p2c(Aa.v)-Ab.labelWidth/2)+"px;width:"+Ab.labelWidth+'px;text-align:center" class="tickLabel">'+Aa.label+"</div>"});AZ(AA.y2axis,function(Aa,Ab){return'<div style="position:absolute;top:'+(m.top+Ab.p2c(Aa.v)-Ab.labelHeight/2)+"px;left:"+(m.left+N+o.grid.labelMargin)+"px;width:"+Ab.labelWidth+'px;text-align:left" class="tickLabel">'+Aa.label+"</div>"});AY+="</div>";K.append(AY)}function AK(AY){if(AY.lines.show||(!AY.bars.show&&!AY.points.show)){h(AY)}if(AY.bars.show){u(AY)}if(AY.points.show){v(AY)}}function h(Aa){function AZ(Aj,Ah,An,Am){var Ag,Ao=null,Ad=null,Ap=null;g.beginPath();for(var Ai=0;Ai<Aj.length;++Ai){Ag=Ao;Ao=Aj[Ai];if(Ag==null||Ao==null){continue}var Af=Ag[0],Al=Ag[1],Ae=Ao[0],Ak=Ao[1];if(Al<=Ak&&Al<Am.min){if(Ak<Am.min){continue}Af=(Am.min-Al)/(Ak-Al)*(Ae-Af)+Af;Al=Am.min}else{if(Ak<=Al&&Ak<Am.min){if(Al<Am.min){continue}Ae=(Am.min-Al)/(Ak-Al)*(Ae-Af)+Af;Ak=Am.min}}if(Al>=Ak&&Al>Am.max){if(Ak>Am.max){continue}Af=(Am.max-Al)/(Ak-Al)*(Ae-Af)+Af;Al=Am.max}else{if(Ak>=Al&&Ak>Am.max){if(Al>Am.max){continue}Ae=(Am.max-Al)/(Ak-Al)*(Ae-Af)+Af;Ak=Am.max}}if(Af<=Ae&&Af<An.min){if(Ae<An.min){continue}Al=(An.min-Af)/(Ae-Af)*(Ak-Al)+Al;Af=An.min}else{if(Ae<=Af&&Ae<An.min){if(Af<An.min){continue}Ak=(An.min-Af)/(Ae-Af)*(Ak-Al)+Al;Ae=An.min}}if(Af>=Ae&&Af>An.max){if(Ae>An.max){continue}Al=(An.max-Af)/(Ae-Af)*(Ak-Al)+Al;Af=An.max}else{if(Ae>=Af&&Ae>An.max){if(Af>An.max){continue}Ak=(An.max-Af)/(Ae-Af)*(Ak-Al)+Al;Ae=An.max}}if(Ad!=An.p2c(Af)||Ap!=Am.p2c(Al)+Ah){g.moveTo(An.p2c(Af),Am.p2c(Al)+Ah)}Ad=An.p2c(Ae);Ap=Am.p2c(Ak)+Ah;g.lineTo(Ad,Ap)}g.stroke()}function Ab(Aj,Aq,Ao){var Ah,Ar=null;var Ad=Math.min(Math.max(0,Ao.min),Ao.max);var Am,Ag=0;var Ap=false;for(var Ai=0;Ai<Aj.length;++Ai){Ah=Ar;Ar=Aj[Ai];if(Ap&&Ah!=null&&Ar==null){g.lineTo(Aq.p2c(Ag),Ao.p2c(Ad));g.fill();Ap=false;continue}if(Ah==null||Ar==null){continue}var Af=Ah[0],An=Ah[1],Ae=Ar[0],Al=Ar[1];if(Af<=Ae&&Af<Aq.min){if(Ae<Aq.min){continue}An=(Aq.min-Af)/(Ae-Af)*(Al-An)+An;Af=Aq.min}else{if(Ae<=Af&&Ae<Aq.min){if(Af<Aq.min){continue}Al=(Aq.min-Af)/(Ae-Af)*(Al-An)+An;Ae=Aq.min}}if(Af>=Ae&&Af>Aq.max){if(Ae>Aq.max){continue}An=(Aq.max-Af)/(Ae-Af)*(Al-An)+An;Af=Aq.max}else{if(Ae>=Af&&Ae>Aq.max){if(Af>Aq.max){continue}Al=(Aq.max-Af)/(Ae-Af)*(Al-An)+An;Ae=Aq.max}}if(!Ap){g.beginPath();g.moveTo(Aq.p2c(Af),Ao.p2c(Ad));Ap=true}if(An>=Ao.max&&Al>=Ao.max){g.lineTo(Aq.p2c(Af),Ao.p2c(Ao.max));g.lineTo(Aq.p2c(Ae),Ao.p2c(Ao.max));continue}else{if(An<=Ao.min&&Al<=Ao.min){g.lineTo(Aq.p2c(Af),Ao.p2c(Ao.min));g.lineTo(Aq.p2c(Ae),Ao.p2c(Ao.min));continue}}var As=Af,Ak=Ae;if(An<=Al&&An<Ao.min&&Al>=Ao.min){Af=(Ao.min-An)/(Al-An)*(Ae-Af)+Af;An=Ao.min}else{if(Al<=An&&Al<Ao.min&&An>=Ao.min){Ae=(Ao.min-An)/(Al-An)*(Ae-Af)+Af;Al=Ao.min}}if(An>=Al&&An>Ao.max&&Al<=Ao.max){Af=(Ao.max-An)/(Al-An)*(Ae-Af)+Af;An=Ao.max}else{if(Al>=An&&Al>Ao.max&&An<=Ao.max){Ae=(Ao.max-An)/(Al-An)*(Ae-Af)+Af;Al=Ao.max}}if(Af!=As){if(An<=Ao.min){Am=Ao.min}else{Am=Ao.max}g.lineTo(Aq.p2c(As),Ao.p2c(Am));g.lineTo(Aq.p2c(Af),Ao.p2c(Am))}g.lineTo(Aq.p2c(Af),Ao.p2c(An));g.lineTo(Aq.p2c(Ae),Ao.p2c(Al));if(Ae!=Ak){if(Al<=Ao.min){Am=Ao.min}else{Am=Ao.max}g.lineTo(Aq.p2c(Ak),Ao.p2c(Am));g.lineTo(Aq.p2c(Ae),Ao.p2c(Am))}Ag=Math.max(Ae,Ak)}if(Ap){g.lineTo(Aq.p2c(Ag),Ao.p2c(Ad));g.fill()}}g.save();g.translate(m.left,m.top);g.lineJoin="round";var Ac=Aa.lines.lineWidth;var AY=Aa.shadowSize;if(AY>0){g.lineWidth=AY/2;g.strokeStyle="rgba(0,0,0,0.1)";AZ(Aa.data,Ac/2+AY/2+g.lineWidth/2,Aa.xaxis,Aa.yaxis);g.lineWidth=AY/2;g.strokeStyle="rgba(0,0,0,0.2)";AZ(Aa.data,Ac/2+g.lineWidth/2,Aa.xaxis,Aa.yaxis)}g.lineWidth=Ac;g.strokeStyle=Aa.color;AD(Aa.lines,Aa.color);if(Aa.lines.fill){Ab(Aa.data,Aa.xaxis,Aa.yaxis)}AZ(Aa.data,0,Aa.xaxis,Aa.yaxis);g.restore()}function v(AZ){function Ac(Ag,Ae,Ah,Ak,Ai){for(var Af=0;Af<Ag.length;++Af){if(Ag[Af]==null){continue}var Ad=Ag[Af][0],Aj=Ag[Af][1];if(Ad<Ak.min||Ad>Ak.max||Aj<Ai.min||Aj>Ai.max){continue}g.beginPath();g.arc(Ak.p2c(Ad),Ai.p2c(Aj),Ae,0,2*Math.PI,true);if(Ah){g.fill()}g.stroke()}}function Ab(Ag,Ai,Ae,Ak,Ah){for(var Af=0;Af<Ag.length;++Af){if(Ag[Af]==null){continue}var Ad=Ag[Af][0],Aj=Ag[Af][1];if(Ad<Ak.min||Ad>Ak.max||Aj<Ah.min||Aj>Ah.max){continue}g.beginPath();g.arc(Ak.p2c(Ad),Ah.p2c(Aj)+Ai,Ae,0,Math.PI,false);g.stroke()}}g.save();g.translate(m.left,m.top);var Aa=AZ.lines.lineWidth;var AY=AZ.shadowSize;if(AY>0){g.lineWidth=AY/2;g.strokeStyle="rgba(0,0,0,0.1)";Ab(AZ.data,AY/2+g.lineWidth/2,AZ.points.radius,AZ.xaxis,AZ.yaxis);g.lineWidth=AY/2;g.strokeStyle="rgba(0,0,0,0.2)";Ab(AZ.data,g.lineWidth/2,AZ.points.radius,AZ.xaxis,AZ.yaxis)}g.lineWidth=AZ.points.lineWidth;g.strokeStyle=AZ.color;AD(AZ.points,AZ.color);Ac(AZ.data,AZ.points.radius,AZ.points.fill,AZ.xaxis,AZ.yaxis);g.restore()}function AM(Aj,Ah,Ac,Ai,Aa,Ao,An,Ak,Af){var Am=true,Ae=true,Ab=true,Ad=false,AZ=Aj+Ac,Al=Aj+Ai,AY=0,Ag=Ah;if(Ag<AY){Ag=0;AY=Ah;Ad=true;Ab=false}if(Al<An.min||AZ>An.max||Ag<Ak.min||AY>Ak.max){return }if(AZ<An.min){AZ=An.min;Am=false}if(Al>An.max){Al=An.max;Ae=false}if(AY<Ak.min){AY=Ak.min;Ad=false}if(Ag>Ak.max){Ag=Ak.max;Ab=false}if(Ao){Af.beginPath();Af.moveTo(An.p2c(AZ),Ak.p2c(AY)+Aa);Af.lineTo(An.p2c(AZ),Ak.p2c(Ag)+Aa);Af.lineTo(An.p2c(Al),Ak.p2c(Ag)+Aa);Af.lineTo(An.p2c(Al),Ak.p2c(AY)+Aa);Af.fill()}if(Am||Ae||Ab||Ad){Af.beginPath();AZ=An.p2c(AZ);AY=Ak.p2c(AY);Al=An.p2c(Al);Ag=Ak.p2c(Ag);Af.moveTo(AZ,AY+Aa);if(Am){Af.lineTo(AZ,Ag+Aa)}else{Af.moveTo(AZ,Ag+Aa)}if(Ab){Af.lineTo(Al,Ag+Aa)}else{Af.moveTo(Al,Ag+Aa)}if(Ae){Af.lineTo(Al,AY+Aa)}else{Af.moveTo(Al,AY+Aa)}if(Ad){Af.lineTo(AZ,AY+Aa)}else{Af.moveTo(AZ,AY+Aa)}Af.stroke()}}function u(Aa){function AZ(Ae,Ab,Ad,Ah,Af,Ai,Ag){for(var Ac=0;Ac<Ae.length;Ac++){if(Ae[Ac]==null){continue}AM(Ae[Ac][0],Ae[Ac][1],Ab,Ad,Ah,Af,Ai,Ag,g)}}g.save();g.translate(m.left,m.top);g.lineJoin="round";g.lineWidth=Aa.bars.lineWidth;g.strokeStyle=Aa.color;AD(Aa.bars,Aa.color);var AY=Aa.bars.align=="left"?0:-Aa.bars.barWidth/2;AZ(Aa.data,AY,AY+Aa.bars.barWidth,0,Aa.bars.fill,Aa.xaxis,Aa.yaxis);g.restore()}function AD(Aa,AY){var AZ=Aa.fill;if(!AZ){return }if(Aa.fillColor){g.fillStyle=Aa.fillColor}else{var Ab=E(AY);Ab.a=typeof AZ=="number"?AZ:0.4;Ab.normalize();g.fillStyle=Ab.toString()}}function AV(){K.find(".legend").remove();if(!o.legend.show){return }var Ae=[];var Ac=false;for(i=0;i<W.length;++i){if(!W[i].label){continue}if(i%o.legend.noColumns==0){if(Ac){Ae.push("</tr>")}Ae.push("<tr>");Ac=true}var Ag=W[i].label;if(o.legend.labelFormatter!=null){Ag=o.legend.labelFormatter(Ag)}Ae.push('<td class="legendColorBox"><div style="border:1px solid '+o.legend.labelBoxBorderColor+';padding:1px"><div style="width:14px;height:10px;background-color:'+W[i].color+';overflow:hidden"></div></div></td><td class="legendLabel">'+Ag+"</td>")}if(Ac){Ae.push("</tr>")}if(Ae.length==0){return }var Ai='<table style="font-size:smaller;color:'+o.grid.color+'">'+Ae.join("")+"</table>";if(o.legend.container!=null){o.legend.container.html(Ai)}else{var Af="";var AZ=o.legend.position,Aa=o.legend.margin;if(AZ.charAt(0)=="n"){Af+="top:"+(Aa+m.top)+"px;"}else{if(AZ.charAt(0)=="s"){Af+="bottom:"+(Aa+m.bottom)+"px;"}}if(AZ.charAt(1)=="e"){Af+="right:"+(Aa+m.right)+"px;"}else{if(AZ.charAt(1)=="w"){Af+="left:"+(Aa+m.left)+"px;"}}var Ah=F('<div class="legend">'+Ai.replace('style="','style="position:absolute;'+Af+";")+"</div>").appendTo(K);if(o.legend.backgroundOpacity!=0){var Ad=o.legend.backgroundColor;if(Ad==null){var Ab;if(o.grid.backgroundColor){Ab=o.grid.backgroundColor}else{Ab=A(Ah)}Ad=E(Ab).adjust(null,null,null,1).toString()}var AY=Ah.children();F('<div style="position:absolute;width:'+AY.width()+"px;height:"+AY.height()+"px;"+Af+"background-color:"+Ad+';"> </div>').prependTo(Ah).css("opacity",o.legend.backgroundOpacity)}}}var AG={pageX:null,pageY:null},d={first:{x:-1,y:-1},second:{x:-1,y:-1},show:false,active:false},AF=[],P=false,O=null,z=null;function AT(Ae,Ac){var Al=o.grid.mouseActiveRadius,Ar=Al*Al+1,At=null,An=false;function Ai(Ay,Ax){return{datapoint:W[Ay].data[Ax],dataIndex:Ax,series:W[Ay],seriesIndex:Ay}}for(var Aq=0;Aq<W.length;++Aq){var Aw=W[Aq].data,Ad=W[Aq].xaxis,Ab=W[Aq].yaxis,Am=Ad.c2p(Ae),Ak=Ab.c2p(Ac),AZ=Al/Ad.scale,AY=Al/Ab.scale,Av=W[Aq].bars.show,Au=!(W[Aq].bars.show&&!(W[Aq].lines.show||W[Aq].points.show)),Aa=W[Aq].bars.align=="left"?0:-W[Aq].bars.barWidth/2,As=Aa+W[Aq].bars.barWidth;for(var Ap=0;Ap<Aw.length;++Ap){if(Aw[Ap]==null){continue}var Ag=Aw[Ap][0],Af=Aw[Ap][1];if(Av){if(!An&&Am>=Ag+Aa&&Am<=Ag+As&&Ak>=Math.min(0,Af)&&Ak<=Math.max(0,Af)){At=Ai(Aq,Ap)}}if(Au){if((Ag-Am>AZ||Ag-Am<-AZ)||(Af-Ak>AY||Af-Ak<-AY)){continue}var Aj=Math.abs(Ad.p2c(Ag)-Ae),Ah=Math.abs(Ab.p2c(Af)-Ac),Ao=Aj*Aj+Ah*Ah;if(Ao<Ar){Ar=Ao;An=true;At=Ai(Aq,Ap)}}}}return At}function J(AZ){var Aa=AZ||window.event;if(Aa.pageX==null&&Aa.clientX!=null){var Ab=document.documentElement,AY=document.body;AG.pageX=Aa.clientX+(Ab&&Ab.scrollLeft||AY.scrollLeft||0);AG.pageY=Aa.clientY+(Ab&&Ab.scrollTop||AY.scrollTop||0)}else{AG.pageX=Aa.pageX;AG.pageY=Aa.pageY}if(o.grid.hoverable&&!z){z=setTimeout(R,100)}if(d.active){AL(AG)}}function AN(AY){if(AY.which!=1){return }document.body.focus();if(document.onselectstart!==undefined&&S.onselectstart==null){S.onselectstart=document.onselectstart;document.onselectstart=function(){return false}}if(document.ondrag!==undefined&&S.ondrag==null){S.ondrag=document.ondrag;document.ondrag=function(){return false}}AR(d.first,AY);AG.pageX=null;d.active=true;F(document).one("mouseup",Y)}function k(AY){if(P){P=false;return }M("plotclick",AY)}function R(){M("plothover",AG);z=null}function M(AZ,AY){var Aa=AQ.offset(),Af={pageX:AY.pageX,pageY:AY.pageY},Ad=AY.pageX-Aa.left-m.left,Ab=AY.pageY-Aa.top-m.top;if(AA.xaxis.used){Af.x=AA.xaxis.c2p(Ad)}if(AA.yaxis.used){Af.y=AA.yaxis.c2p(Ab)}if(AA.x2axis.used){Af.x2=AA.x2axis.c2p(Ad)}if(AA.y2axis.used){Af.y2=AA.y2axis.c2p(Ab)}var Ag=AT(Ad,Ab);if(Ag){Ag.pageX=parseInt(Ag.series.xaxis.p2c(Ag.datapoint[0])+Aa.left+m.left);Ag.pageY=parseInt(Ag.series.yaxis.p2c(Ag.datapoint[1])+Aa.top+m.top)}if(o.grid.autoHighlight){for(var Ac=0;Ac<AF.length;++Ac){var Ae=AF[Ac];if(Ae.auto&&!(Ag&&Ae.series==Ag.series&&Ae.point==Ag.datapoint)){AH(Ae.series,Ae.point)}}if(Ag){AS(Ag.series,Ag.datapoint,true)}}K.trigger(AZ,[Af,Ag])}function x(){if(!O){O=setTimeout(T,50)}}function T(){O=null;AX.save();AX.clearRect(0,0,AI,Z);AX.translate(m.left,m.top);var Ab,Aa;for(Ab=0;Ab<AF.length;++Ab){Aa=AF[Ab];if(Aa.series.bars.show){AJ(Aa.series,Aa.point)}else{AE(Aa.series,Aa.point)}}AX.restore();if(d.show&&b()){AX.strokeStyle=E(o.selection.color).scale(null,null,null,0.8).toString();AX.lineWidth=1;g.lineJoin="round";AX.fillStyle=E(o.selection.color).scale(null,null,null,0.4).toString();var AY=Math.min(d.first.x,d.second.x),Ad=Math.min(d.first.y,d.second.y),AZ=Math.abs(d.second.x-d.first.x),Ac=Math.abs(d.second.y-d.first.y);AX.fillRect(AY+m.left,Ad+m.top,AZ,Ac);AX.strokeRect(AY+m.left,Ad+m.top,AZ,Ac)}}function AS(Aa,AY,Ab){if(typeof Aa=="number"){Aa=W[Aa]}if(typeof AY=="number"){AY=Aa.data[AY]}var AZ=q(Aa,AY);if(AZ==-1){AF.push({series:Aa,point:AY,auto:Ab});x()}else{if(!Ab){AF[AZ].auto=false}}}function AH(Aa,AY){if(typeof Aa=="number"){Aa=W[Aa]}if(typeof AY=="number"){AY=Aa.data[AY]}var AZ=q(Aa,AY);if(AZ!=-1){AF.splice(AZ,1);x()}}function q(Aa,Ab){for(var AY=0;AY<AF.length;++AY){var AZ=AF[AY];if(AZ.series==Aa&&AZ.point[0]==Ab[0]&&AZ.point[1]==Ab[1]){return AY}}return -1}function AE(Ab,Aa){var AZ=Aa[0],Af=Aa[1],Ae=Ab.xaxis,Ad=Ab.yaxis;if(AZ<Ae.min||AZ>Ae.max||Af<Ad.min||Af>Ad.max){return }var Ac=Ab.points.radius+Ab.points.lineWidth/2;AX.lineWidth=Ac;AX.strokeStyle=E(Ab.color).scale(1,1,1,0.5).toString();var AY=1.5*Ac;AX.beginPath();AX.arc(Ae.p2c(AZ),Ad.p2c(Af),AY,0,2*Math.PI,true);AX.stroke()}function AJ(Aa,AY){AX.lineJoin="round";AX.lineWidth=Aa.bars.lineWidth;AX.strokeStyle=E(Aa.color).scale(1,1,1,0.5).toString();AX.fillStyle=E(Aa.color).scale(1,1,1,0.5).toString();var AZ=Aa.bars.align=="left"?0:-Aa.bars.barWidth/2;AM(AY[0],AY[1],AZ,AZ+Aa.bars.barWidth,0,true,Aa.xaxis,Aa.yaxis,AX)}function r(){var AZ=Math.min(d.first.x,d.second.x),AY=Math.max(d.first.x,d.second.x),Ab=Math.max(d.first.y,d.second.y),Aa=Math.min(d.first.y,d.second.y);var Ac={};if(AA.xaxis.used){Ac.xaxis={from:AA.xaxis.c2p(AZ),to:AA.xaxis.c2p(AY)}}if(AA.x2axis.used){Ac.x2axis={from:AA.x2axis.c2p(AZ),to:AA.x2axis.c2p(AY)}}if(AA.yaxis.used){Ac.yaxis={from:AA.yaxis.c2p(Ab),to:AA.yaxis.c2p(Aa)}}if(AA.y2axis.used){Ac.yaxis={from:AA.y2axis.c2p(Ab),to:AA.y2axis.c2p(Aa)}}K.trigger("plotselected",[Ac]);if(AA.xaxis.used&&AA.yaxis.used){K.trigger("selected",[{x1:Ac.xaxis.from,y1:Ac.yaxis.from,x2:Ac.xaxis.to,y2:Ac.yaxis.to}])}}function Y(AY){if(document.onselectstart!==undefined){document.onselectstart=S.onselectstart}if(document.ondrag!==undefined){document.ondrag=S.ondrag}d.active=false;AL(AY);if(b()){r();P=true}return false}function AR(Aa,AY){var AZ=AQ.offset();if(o.selection.mode=="y"){if(Aa==d.first){Aa.x=0}else{Aa.x=N}}else{Aa.x=AY.pageX-AZ.left-m.left;Aa.x=Math.min(Math.max(0,Aa.x),N)}if(o.selection.mode=="x"){if(Aa==d.first){Aa.y=0}else{Aa.y=AB}}else{Aa.y=AY.pageY-AZ.top-m.top;Aa.y=Math.min(Math.max(0,Aa.y),AB)}}function AL(AY){if(AY.pageX==null){return }AR(d.second,AY);if(b()){d.show=true;x()}else{I()}}function I(){if(d.show){d.show=false;x()}}function AC(AZ,AY){var Aa;if(o.selection.mode=="y"){d.first.x=0;d.second.x=N}else{Aa=V(AZ,"x");d.first.x=Aa.axis.p2c(Aa.from);d.second.x=Aa.axis.p2c(Aa.to)}if(o.selection.mode=="x"){d.first.y=0;d.second.y=AB}else{Aa=V(AZ,"y");d.first.y=Aa.axis.p2c(Aa.from);d.second.y=Aa.axis.p2c(Aa.to)}d.show=true;x();if(!AY){r()}}function b(){var AY=5;return Math.abs(d.second.x-d.first.x)>=AY&&Math.abs(d.second.y-d.first.y)>=AY}}F.plot=function(L,J,I){var K=new D(L,J,I);return K};function C(J,I){return I*Math.floor(J/I)}function H(J,K,I){if(K<J){return K}else{if(K>I){return I}else{return K}}}function G(O,N,J,L){var M=["r","g","b","a"];var I=4;while(-1<--I){this[M[I]]=arguments[I]||((I==3)?1:0)}this.toString=function(){if(this.a>=1){return"rgb("+[this.r,this.g,this.b].join(",")+")"}else{return"rgba("+[this.r,this.g,this.b,this.a].join(",")+")"}};this.scale=function(R,Q,S,P){I=4;while(-1<--I){if(arguments[I]!=null){this[M[I]]*=arguments[I]}}return this.normalize()};this.adjust=function(R,Q,S,P){I=4;while(-1<--I){if(arguments[I]!=null){this[M[I]]+=arguments[I]}}return this.normalize()};this.clone=function(){return new G(this.r,this.b,this.g,this.a)};var K=function(Q,P,R){return Math.max(Math.min(Q,R),P)};this.normalize=function(){this.r=K(parseInt(this.r),0,255);this.g=K(parseInt(this.g),0,255);this.b=K(parseInt(this.b),0,255);this.a=K(this.a,0,1);return this};this.normalize()}var B={aqua:[0,255,255],azure:[240,255,255],beige:[245,245,220],black:[0,0,0],blue:[0,0,255],brown:[165,42,42],cyan:[0,255,255],darkblue:[0,0,139],darkcyan:[0,139,139],darkgrey:[169,169,169],darkgreen:[0,100,0],darkkhaki:[189,183,107],darkmagenta:[139,0,139],darkolivegreen:[85,107,47],darkorange:[255,140,0],darkorchid:[153,50,204],darkred:[139,0,0],darksalmon:[233,150,122],darkviolet:[148,0,211],fuchsia:[255,0,255],gold:[255,215,0],green:[0,128,0],indigo:[75,0,130],khaki:[240,230,140],lightblue:[173,216,230],lightcyan:[224,255,255],lightgreen:[144,238,144],lightgrey:[211,211,211],lightpink:[255,182,193],lightyellow:[255,255,224],lime:[0,255,0],magenta:[255,0,255],maroon:[128,0,0],navy:[0,0,128],olive:[128,128,0],orange:[255,165,0],pink:[255,192,203],purple:[128,0,128],violet:[128,0,128],red:[255,0,0],silver:[192,192,192],white:[255,255,255],yellow:[255,255,0]};function A(J){var I,K=J;do{I=K.css("background-color").toLowerCase();if(I!=""&&I!="transparent"){break}K=K.parent()}while(!F.nodeName(K.get(0),"body"));if(I=="rgba(0, 0, 0, 0)"){return"transparent"}return I}function E(K){var I;if(I=/rgb\(\s*([0-9]{1,3})\s*,\s*([0-9]{1,3})\s*,\s*([0-9]{1,3})\s*\)/.exec(K)){return new G(parseInt(I[1],10),parseInt(I[2],10),parseInt(I[3],10))}if(I=/rgba\(\s*([0-9]{1,3})\s*,\s*([0-9]{1,3})\s*,\s*([0-9]{1,3})\s*,\s*([0-9]+(?:\.[0-9]+)?)\s*\)/.exec(K)){return new G(parseInt(I[1],10),parseInt(I[2],10),parseInt(I[3],10),parseFloat(I[4]))}if(I=/rgb\(\s*([0-9]+(?:\.[0-9]+)?)\%\s*,\s*([0-9]+(?:\.[0-9]+)?)\%\s*,\s*([0-9]+(?:\.[0-9]+)?)\%\s*\)/.exec(K)){return new G(parseFloat(I[1])*2.55,parseFloat(I[2])*2.55,parseFloat(I[3])*2.55)}if(I=/rgba\(\s*([0-9]+(?:\.[0-9]+)?)\%\s*,\s*([0-9]+(?:\.[0-9]+)?)\%\s*,\s*([0-9]+(?:\.[0-9]+)?)\%\s*,\s*([0-9]+(?:\.[0-9]+)?)\s*\)/.exec(K)){return new G(parseFloat(I[1])*2.55,parseFloat(I[2])*2.55,parseFloat(I[3])*2.55,parseFloat(I[4]))}if(I=/#([a-fA-F0-9]{2})([a-fA-F0-9]{2})([a-fA-F0-9]{2})/.exec(K)){return new G(parseInt(I[1],16),parseInt(I[2],16),parseInt(I[3],16))}if(I=/#([a-fA-F0-9])([a-fA-F0-9])([a-fA-F0-9])/.exec(K)){return new G(parseInt(I[1]+I[1],16),parseInt(I[2]+I[2],16),parseInt(I[3]+I[3],16))}var J=F.trim(K).toLowerCase();if(J=="transparent"){return new G(255,255,255,0)}else{I=B[J];return new G(I[0],I[1],I[2])}}})(jQuery);

function InitializeDiffPanes() {
    var diffBox = $("#diffBox");
    var parent = diffBox.parent();
    var diffPane = $(".diffPane", diffBox);
    var leftTable = $(".diffTable", diffPane[0]);
    var rightTable = $(".diffTable", diffPane[1]);
    var diffPaneLinesLeft = $("td.line", leftTable);
    var diffPaneLineHeight = diffPaneLinesLeft.outerHeight() + 1;
    var footer = $("#CanvasFooter");
    $("#CanvasContent").css("padding-bottom", "0px");
                
               
    SizeDiffTablesEqually();
    SizeDiffPanesToWindow();


    // synchronize the scroll bars
    $(diffPane[0]).scroll(OnLeftDiffPaneScroll);
    $(diffPane[1]).scroll(OnRightDiffPaneScroll);


    $(window).resizeComplete(function() {
        SizeDiffTablesEqually();
        SizeDiffPanesToWindow();
    });


    function SizeDiffPanesToWindow() {
        var lineCount = diffPaneLinesLeft.length;
        var contentHeight = lineCount * diffPaneLineHeight + 3;

        diffPane.hide();
        var pageFooterTop = footer.offset().top;
        var parentHeight = parent.outerHeight(true);
        var parentTop = parent.offset().top;
        var windowHeight = $(window).height();
        var newHeight = (pageFooterTop  - parentTop) - 30;
        diffPane.show();

        if (contentHeight < newHeight) {
            newHeight = contentHeight;
        }

        if (newHeight > 0)
            diffPane.height(newHeight);
    }

    function SizeDiffTablesEqually() {
        var windowWidth = ($.browser.msie) ? document.documentElement.clientWidth : window.innerWidth;
        var diffPaneWidth = Math.max(Math.floor(windowWidth / 2), 480) - 2;
        var tableHeight = Math.max(leftTable.height(), rightTable.height());

        leftTable.height(tableHeight);
        rightTable.height(tableHeight);
        diffPane.width(diffPaneWidth);
    }

    function OnLeftDiffPaneScroll(e) {
        var left = this.scrollLeft;
        var top = this.scrollTop;
        if (top != diffPane[1].scrollTop) diffPane[1].scrollTop = top;
        if (left != diffPane[1].scrollLeft) diffPane[1].scrollLeft = left;
    }

    function OnRightDiffPaneScroll(e) {
        var left = this.scrollLeft;
        var top = this.scrollTop;
        if (top != diffPane[0].scrollTop) diffPane[0].scrollTop = top;
        if (left != diffPane[0].scrollLeft) diffPane[0].scrollLeft = left;
    }
}


jQuery.fn.resizeComplete = function(callback) {

    var element = this;
    var height = element.height();
    var width = element.width();
    var monitoring = false;
    var timer;

    function monitorResizing() {

        if (!same()) {
            height = element.height();
            width = element.width();
            timer = setTimeout(function() { monitorResizing() }, 500);
        }
        else {
            clearTimeout(timer);
            callback();
            monitoring = false;
        }
    }

    function same() {
        var newHeight = element.height();
        var newWidth = element.width();

        return newHeight == height && newWidth == width;
    }

    function onResize() {
        if (monitoring) return;
        if (same()) return;
        monitoring = true;
        monitorResizing();
    }

    if ($.browser.mozilla) {
        element.resize(callback);
    }
    else {
        element.resize(onResize);
    }
};(function(jQuery) {

    jQuery.fn.sourceCodeBrowser = function(options) {
        return this.each(function() {
            new jQuery.sourceCodeBrowser(options);
        });
    };

    jQuery.sourceCodeBrowser = function(options) {
        this.options = options;
        this.init();
    }

    jQuery.sourceCodeBrowser.fn = jQuery.sourceCodeBrowser.prototype = {};
    jQuery.sourceCodeBrowser.fn.extend = jQuery.sourceCodeBrowser.extend = jQuery.extend;
    jQuery.sourceCodeBrowser.fn.extend({
        init: function() {
            var self = this;
            jQuery(window).resize(function() {
                self.fixOffsets();
            });

            self.setTreeEvents();

            if (location.hash.length > 1) {
                self.loadFile(null, location.hash.substr(1), self.loadTree);
            } else {
                $('#dir_0').focus();
            }

            self.fixOffsets();
        },

        setTreeEvents: function() {
            var self = this;
            jQuery('.browserDir')
                .unbind('click', this.toggleTreeNodeHandler)
                .bind('click', this, this.toggleTreeNodeHandler);

            jQuery('.browseFile')
                .unbind('click', this.loadFileHandler)
                .bind('click', this, this.loadFileHandler);
        },

        fixOffsets: function() {
            var dirView = $('div#DirectoryView'), fileView = $('div#FileView'), footer = $('div#CanvasFooter');
            if (!(dirView.length && fileView.length && footer.length)) {
                return;
            }

            var footerHeight = footer.height() + parseInt(footer.css("padding-top"), 10) + parseInt(footer.css("padding-bottom"), 10) + 2;
            var viewHeight = $(window).height() - dirView.offset().top - footerHeight;
            dirView.css('height', viewHeight);

            if (fileView.is(':visible')) {
                var maxWidth = $(window).width() - 25;
                var containerWidth = $('div#sourceBrowserContainer').outerWidth(true);
                if (containerWidth > maxWidth) {
                    containerWidth = maxWidth;
                }

                fileView.css('height', viewHeight);
                fileView.css('width', containerWidth - $('div#leftPane').outerWidth(true));
            }
        },

        loadTree: function(self) {
            var path = $('#FilePath').val();
            var pathParts = path.split('/');
            var currentPath = '$/' + pathParts[1];
            self.recurseLoadTree(2, currentPath, pathParts);
        },

        recurseLoadTree: function(index, currentPath, pathParts) {
            var self = this;
            if (index < pathParts.length - 1) {
                currentPath += '/' + pathParts[index];
                var anchor = $("a[path='" + currentPath + "']");
                if (anchor.length == 1) {
                    this.toggleTreeNode(anchor[0], function() {
                        self.recurseLoadTree(index + 1, currentPath, pathParts);
                        if (CodePlex.UI.isIE6) self.fixOffsets();
                    });
                }
            }
        },

        toggleTreeNodeHandler: function(e) {
            e.data.toggleTreeNode(this, null);
        },

        toggleTreeNode: function(node, callback) {
            var self = this;
            var key = node.id.substr(3);
            var span = $('#dir' + key + '_span');
            if (span.html() === '') {
                if (!callback) this.showLoading();
                span.load(this.options.listUrl.replace(/xXPathXx/, escape($(node).attr('path'))).replace(/xXKeyXx/, escape(key)), function() {
                    span.scrollTo();
                    span.css('display', 'block');
                    $(node).children('div:first').toggleClass('sbfc').toggleClass('sbfe');
                    self.setTreeEvents();
                    if (!callback) self.hideLoading();
                    if (callback) callback();
                });
            } else {
                $(node).children('div:first').toggleClass('sbfc').toggleClass('sbfe');
                span.css('display') == 'block' ? span.css('display', 'none') : span.css('display', 'block');
            }

            return false;
        },

        loadFileHandler: function(e) {
            e.data.loadFile(this, $(this).attr('fileId'), null);
        },

        loadFile: function(node, id, callback) {
            var self = this;
            if (node && $(node).children('div:first').hasClass('sbsf')) {
                return true;
            }

            this.showLoading();
            $('#beginInstructions').hide();
            $('div#FileView').show();
            $('#DirectoryView div.sbsf').removeClass('sbsf');
            if (node) {
                $(node).children('div:first').addClass('sbsf');
                node.focus();
            }
            $('#OlderVersionsPanel').remove();
            $('#rightPane').load(this.options.fileUrl.replace(/-1/, id), null, function(responseText) {
                self.hideLoading();
                self.fixOffsets();
                self.setDiffEvents();
                if (callback) callback(self);
            });

            return true;
        },

        setDiffEvents: function() {
            $('#previousVersions')
                .unbind('change', this.handleDiffSelection)
                .bind('change', this, this.handleDiffSelection);
        },

        setChangesetEvents: function() {
            $('#OlderVersionsPanel .Body .oldChangeSet')
                .unbind('click', this.handleDiffOlder)
                .bind('click', this, this.handleDiffOlder);
        },

        handleDiffSelection: function(e) {
            var self = e.data;
            if (this.selectedIndex === 1) {
                self.diff(this);
            } else if (this.selectedIndex === 2) {
                self.showLoading();
                $.getJSON(self.options.changesetsUrl.replace(/-1/, $('#FileID').val()),
                    function(data) {
                        OpenDialog('#OlderVersionsPanel', true, '36em', 'ClosePanel');
                        self.hideLoading();

                        var html = '';
                        if (data && data.length) {
                            for (var i = 0; i < data.length; i++) {
                                html = html + "<a href='#' class='oldChangeSet' changeSetId='" + data[i].Id + "'>Change set " + data[i].Id + "</a> by " +
                                data[i].Author + " on " + data[i].Date + "<br/>";
                            }
                        } else {
                            html = 'There are no older change sets for <br/><strong>' + $('#OlderVersionsPanel #Header').html() + '</strong>.';
                        }
                        $('#OlderVersionsPanel .Body').html(html);
                        self.setChangesetEvents();
                    });
            }

            // reset selected index so if user closes window they can re-select the same option.
            this.selectedIndex = 0;
        },

        diff: function(element) {
            var url = this.options.diffUrl.replace(/-1/, $('#FileID').val());
            this.showDiffWindow(url);
        },

        handleDiffOlder: function(e) {
            e.data.diffOlder(this);
        },

        diffOlder: function(element) {
            var url = this.options.oldDiffUrl.replace(/-1/, $('#FileID').val());
            url = url.replace(/-2/, $(element).attr('changesetId'));
            this.showDiffWindow(url);
            OpenDialog('#OlderVersionsPanel', false);
        },

        showLoading: function() {
            CodePlex.UpdateProgress.showWithCustomText(true, 'Loading...');
        },

        hideLoading: function() {
            CodePlex.UpdateProgress.hide();
        },

        showDiffWindow: function(url) {
            newwindow = window.open(url, 'name', 'height=700,width=1000,toolbar=0,scrollbars=0,location=0,statusbar=0,menubar=0,resizable=1');
            if (window.focus) { newwindow.focus(); }
        }
    });

})(jQuery);

$.fn.scrollTo = function() {
    var offset = this.offset().top;
    var scrollTop = (CodePlex.UI.isSafari ? document.body.scrollTop : $('html,body')[0].scrollTop);
    var totalH = scrollTop + document.body.clientHeight;
    var outerHeight = this.outerHeight();
    if ((offset + outerHeight) > totalH && outerHeight < totalH) {
        window.scrollBy(0, ((offset + this.outerHeight()) - totalH));
    } else if (offset < scrollTop) {
        $('html,body').animate({ scrollTop: offset }, 'fast');
    }
};(function(jQuery) {

    jQuery.fn.hoverDelete = function(options) {
        return this.each(function() {
            new jQuery.hoverDelete($(this), options);
        });
    };

    jQuery.hoverDelete = function(element, options) {
        this.element = element;
        this.options = options;
        this.init();
    }

    jQuery.hoverDelete.fn = jQuery.hoverDelete.prototype = {};
    jQuery.hoverDelete.fn.extend = jQuery.hoverDelete.extend = jQuery.extend;
    jQuery.hoverDelete.fn.extend({
        init: function() {
            var self = this;

            var removeHtml = '<div class="remove" />';
            var imageHtml = '<img src="' + this.options.inactiveImageUrl + '" title="' + this.options.hoverText + '" />';
            var clearHtml = '<div class="ClearBoth" />';

            this.element.append(removeHtml).append(clearHtml);
            this.removeElement = this.element.children('.remove');
            this.removeElement.append(imageHtml);
            this.removeImage = this.removeElement.children('img');

            this.element.hover(
                function() { self.removeElement.show(); },
                function() { self.removeElement.hide(); }
            );
            this.removeImage.hover(
                function() { $(this).attr('src', self.options.activeImageUrl); },
                function() { $(this).attr('src', self.options.inactiveImageUrl); }
            );
            this.removeImage.bind('click', this, this.options.callback);
        }
    });

})(jQuery);
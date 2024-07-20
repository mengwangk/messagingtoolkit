;===============================================================================
; OSML - Open Source Messaging Library
;
;===============================================================================
; Copyright © TWIT88.COM.  All rights reserved.
;
; This file is part of Open Source Messaging Library.
;
;; Open Source Messaging Library is free software: you can redistribute it
; and/or modify it under the terms of the GNU General Public License version 3.
;
; Open Source Messaging Library is distributed in the hope that it will be useful,
; but WITHOUT ANY WARRANTY; without even the implied warranty of
; MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
; GNU General Public License for more details.
;
; You should have received a copy of the GNU General Public License
; along with this software.  If not, see <http:;www.gnu.org/licenses/>.
;===============================================================================

SetCompressor /SOLID lzma

RequestExecutionLevel admin

!include "LogicLib.nsh"
!include "MUI2.nsh"

;-------------------------------------------

;Functions
!ifdef VER_MAJOR & VER_MINOR & VER_REVISION & VER_BUILD
  !insertmacro VersionCompare
!endif


!define PRODUCT_NAME "MessagingToolkit-QRCode"
!define PRODUCT_VERSION "1.3.0.0"
!define PRODUCT_PUBLISHER "TWIT88.COM"
;!define PRODUCT_LICENSE_SITE "http://www.twit88.com/platform/wiki/messagingtoolkit/WiKi_-_Licensing"
!define PRODUCT_DIR_REGKEY "Software\Microsoft\Windows\CurrentVersion\App Paths"
!define PRODUCT_UNINST_KEY "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}"
!define PRODUCT_UNINST_ROOT_KEY "HKLM"

;Additional components
;!define PRODUCT_BULK_GATEWAY "Bulk Gateway"
;!define PRODUCT_SMART_GATEWAY "Smart Gateway"
;!define PRODUCT_RECOVERY_MANAGER "Recovery Manager"


;Names
Name "MessagingToolkit QRCode"
Caption "MessagingToolkit QRCode ${PRODUCT_VERSION} Setup"

; MUI Settings

!define MUI_ICON "icons\modern-install.ico"
!define MUI_UNICON "icons\modern-uninstall.ico"

; Welcome page
!insertmacro MUI_PAGE_WELCOME

; License page
!insertmacro MUI_PAGE_LICENSE "license\mit.txt"

!insertmacro MUI_PAGE_COMPONENTS

; Directory page
!insertmacro MUI_PAGE_DIRECTORY

; Instfiles page
!insertmacro MUI_PAGE_INSTFILES

; Finish page
;!define MUI_FINISHPAGE_RUN "$INSTDIR\MessagingToolkit.Core.Utilities.exe"

!insertmacro MUI_PAGE_FINISH

!insertmacro MUI_UNPAGE_CONFIRM
  
; Uninstaller pages
!insertmacro MUI_UNPAGE_INSTFILES

!define MUI_ABORTWARNING

!insertmacro MUI_LANGUAGE "English"

; The name of the installer
;Name "${PRODUCT_NAME} ${PRODUCT_VERSION}"

; The file to write
;OutFile "${PRODUCT_NAME}_setup.exe"
OutFile "setup-qrcode.exe"

; The default installation directory
InstallDir $PROGRAMFILES\MessagingToolkit\${PRODUCT_NAME}

; Registry key to check for directory (so if you install again, it will 
; overwrite the old one automatically)
InstallDirRegKey HKLM "${PRODUCT_DIR_REGKEY}" "Install_Dir"

ShowInstDetails show

ShowUnInstDetails show

;--------------------------------

; Pages

Page components

;Page directory
;Page instfiles

;UninstPage uninstConfirm
;UninstPage instfiles

Section "${PRODUCT_NAME}" SEC01

  SectionIn RO

  ; Set output path to the installation directory.
  SetOutPath $INSTDIR
  
  ;SetOverwrite ifnewer

  ; Put file there
  ;File "MessagingToolkit.Core.Utilities.exe"
  ;File "MessagingToolkit.Core.dll"
  ;File "MessagingToolkit.Barcode.dll"
  ;File "MessagingToolkit.Barcode.Demo.exe"
  File /r "QRCode"
  File /r "QRCodeMobile"
  File /r "QRCodeSource"
  ;File /r "MessagingToolkit.Barcode.Demo"

  CreateDirectory "$SMPROGRAMS\MessagingToolkit\${PRODUCT_NAME}"
  CreateShortCut "$SMPROGRAMS\MessagingToolkit\${PRODUCT_NAME}\WinForms Demo - QRCode.lnk" "$INSTDIR\QRCode\QRCodeSample.exe"
  CreateShortCut "$SMPROGRAMS\MessagingToolkit\${PRODUCT_NAME}\WPF Demo - QRCode.lnk" "$INSTDIR\QRCode\QRCodeWPFSampleApp.exe"
  CreateShortCut "$SMPROGRAMS\MessagingToolkit\${PRODUCT_NAME}\Windows Mobile Demo.lnk" "$INSTDIR\QRCodeMobile\QRCodeWindowsMobile6.exe"
  CreateShortCut "$SMPROGRAMS\MessagingToolkit\${PRODUCT_NAME}\Source Code.lnk" "$INSTDIR\QRCodeSource"  
  CreateShortCut "$SMPROGRAMS\MessagingToolkit\${PRODUCT_NAME}\Uninstall.lnk" "$INSTDIR\uninstall.exe" "" "$INSTDIR\uninstall.exe" 0

SectionEnd


;Section "${PRODUCT_BULK_GATEWAY}" SEC02

  ; Set output path to the installation directory.
 ; SetOutPath $INSTDIR

  ;SetOverwrite ifnewer

  ; Put file there
  ;File "MessagingToolkit.BulkGateway.exe"

  ;CreateShortCut "$SMPROGRAMS\${PRODUCT_NAME}\Bulk Gateway.lnk" "$INSTDIR\MessagingToolkit.BulkGateway.exe"

;SectionEnd

;Section "${PRODUCT_SMART_GATEWAY}" SEC03

  ; Set output path to the installation directory.
;  SetOutPath $INSTDIR

;  SetOverwrite ifnewer

  ; Put file there

;SectionEnd

;Section "${PRODUCT_RECOVERY_MANAGER}" SEC04

  ; Set output path to the installation directory.
;  SetOutPath $INSTDIR

;  SetOverwrite ifnewer

  ; Put file there

;SectionEnd


Function .onInit

  ReadRegStr $R0 HKLM \
  "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}" \
  "UninstallString"
  StrCmp $R0 "" done

  MessageBox MB_OKCANCEL|MB_ICONEXCLAMATION \
  "${PRODUCT_NAME} is already installed. $\n$\nClick OK to remove the \
  previous version or Cancel to cancel this upgrade." \
  IDOK uninst
  Abort

;Run the uninstaller
uninst:
  ClearErrors
  ExecWait '$R0 _?=$INSTDIR' ;Do not copy the uninstaller to a temp file

  IfErrors no_remove_uninstaller
    ;You can either use Delete /REBOOTOK in the uninstaller or add some code
    ;here to remove the uninstaller. Use a registry key to check
    ;whether the user has chosen to uninstall. If you are using an uninstaller
    ;components page, make sure all sections are uninstalled.
  no_remove_uninstaller:

done:

FunctionEnd

;--------------------------------

; Uninstaller

Section "Uninstall"
  ; ----------------------------------------------
  ;Delete "$INSTDIR\Purchase.url"
  ;Delete "$INSTDIR\uninstall.exe"
  ;Delete "$INSTDIR\MessagingToolkit.Core.Utilities.exe"
  ;Delete "$INSTDIR\MessagingToolkit.Core.dll"
  ;Delete "$INSTDIR\MessagingToolkit.Pdu.dll"
  ;Delete "$INSTDIR\MessagingToolkit.Barcode.dll"
  ;Delete "$INSTDIR\MessagingToolkit.Barcode.Demo.exe"


  Delete "$SMPROGRAMS\MessagingToolkit\${PRODUCT_NAME}\*.lnk"
  ;Delete "$SMPROGRAMS\MessagingToolkit\${PRODUCT_NAME}\Uninstall.lnk"
  ;Delete "$SMPROGRAMS\MessagingToolkit\${PRODUCT_NAME}\WinForms Demo - QRCode.lnk"
  ;Delete "$SMPROGRAMS\MessagingToolkit\${PRODUCT_NAME}\Windows Mobile Demo.lnk"
  ;Delete "$SMPROGRAMS\MessagingToolkit\${PRODUCT_NAME}\WPF Demo - QRCode.lnk"
  ;Delete "$SMPROGRAMS\MessagingToolkit\${PRODUCT_NAME}\Source Code.lnk"

  RMDir "$SMPROGRAMS\MessagingToolkit\${PRODUCT_NAME}"
  RMDir /r "$INSTDIR"
  ;RMDir /r "$INSTDIR\QRCode\"
  ;RMDir /r "$INSTDIR\QRCodeMobile\"
  ;RMDir /r "$INSTDIR\QRCodeSource\"
  
  DeleteRegKey ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}"
  DeleteRegKey HKLM SOFTWARE\${PRODUCT_NAME}
  DeleteRegKey HKLM "${PRODUCT_DIR_REGKEY}"
  SetAutoClose true

SectionEnd

Section -AdditionalIcons
  ;WriteIniStr "$INSTDIR\Purchase.url" "InternetShortcut" "URL" "${PRODUCT_LICENSE_SITE}"
  ;CreateShortCut "$SMPROGRAMS\${PRODUCT_NAME}\Purchase.lnk" "$INSTDIR\Purchase.url"
  ;CreateShortCut "$SMPROGRAMS\${PRODUCT_NAME}\Uninstall.lnk" "$INSTDIR\uninst.exe"
SectionEnd

Section -Post
  WriteUninstaller "$INSTDIR\uninstall.exe"
  ;WriteRegStr HKLM "${PRODUCT_DIR_REGKEY}" "" "$INSTDIR"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayName" "$(^Name)"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "UninstallString" "$INSTDIR\uninst.exe"
  ;WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayIcon" "$INSTDIR"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayVersion" "${PRODUCT_VERSION}"
  ;WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "URLInfoAbout" "${PRODUCT_LICENSE_SITE}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "Publisher" "${PRODUCT_PUBLISHER}"
SectionEnd

Function un.onUninstSuccess
  HideWindow
  MessageBox MB_ICONINFORMATION|MB_OK "${PRODUCT_NAME} was successfully removed from your computer."
FunctionEnd

Function un.onInit
  MessageBox MB_ICONQUESTION|MB_YESNO|MB_DEFBUTTON2 "Are you sure you want to completely remove ${PRODUCT_NAME} and all of its components?" IDYES +2
  Abort
FunctionEnd


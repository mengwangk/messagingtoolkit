<%@ Page Title="Encoder" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Encoder.aspx.cs" Inherits="MessagingToolkit.Barcode.Web.Encoder" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>
        Encoder
    </h2>
    <table width="100%" cellpadding="10px" cellspacing="10px">
        <tr>
            <td>
                Barcode Data
            </td>
            <td>
                <asp:TextBox ID="txtBarcodeData" TextMode="MultiLine" runat="server" Width="255px" Height="150px">Demo for barcode library</asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                Barcode Type
            </td>
            <td>
                <asp:DropDownList ID="cboBarcodeType" runat="server">
                    <asp:ListItem>QR Code</asp:ListItem>
                    <asp:ListItem>Data Matrix</asp:ListItem>
                    <asp:ListItem>PDF417</asp:ListItem>
                    <asp:ListItem>Bookland/ISBN</asp:ListItem>
                    <asp:ListItem>Codabar</asp:ListItem>
                    <asp:ListItem>Code 11</asp:ListItem>
                    <asp:ListItem>Code 128</asp:ListItem>
                    <asp:ListItem>Code 128-A</asp:ListItem>
                    <asp:ListItem>Code 128-B</asp:ListItem>
                    <asp:ListItem>Code 128-C</asp:ListItem>
                    <asp:ListItem>Code 39</asp:ListItem>
                    <asp:ListItem>Code 39 Extended</asp:ListItem>
                    <asp:ListItem>Code 93</asp:ListItem>
                    <asp:ListItem>EAN-8</asp:ListItem>
                    <asp:ListItem>EAN-13</asp:ListItem>
                    <asp:ListItem>FIM</asp:ListItem>
                    <asp:ListItem>Interleaved 2 of 5</asp:ListItem>
                    <asp:ListItem>ITF-14</asp:ListItem>
                    <asp:ListItem>LOGMARS</asp:ListItem>
                    <asp:ListItem>MSI 2 Mod 10</asp:ListItem>
                    <asp:ListItem>MSI Mod 10</asp:ListItem>
                    <asp:ListItem>MSI Mod 11</asp:ListItem>
                    <asp:ListItem>MSI Mod 11 Mod 10</asp:ListItem>
                    <asp:ListItem>PostNet</asp:ListItem>
                    <asp:ListItem>Standard 2 of 5</asp:ListItem>
                    <asp:ListItem>Telepen</asp:ListItem>
                    <asp:ListItem>UPC 2 Digit Ext.</asp:ListItem>
                    <asp:ListItem>UPC 5 Digit Ext.</asp:ListItem>
                    <asp:ListItem>UPC-A</asp:ListItem>
                    <asp:ListItem>UPC-E</asp:ListItem>

                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
            <td>
                <asp:Button ID="btnEncode" runat="server" Text="Encode" OnClick="btnEncode_Click" />&nbsp;<asp:Button
                    ID="btnReset" runat="server" Text="Reset" UseSubmitBehavior="False" 
                    onclick="btnReset_Click" />
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Image ID="picEncodedBarcode" runat="server" Visible="false" />
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Label ID="lblMsg" runat="server" Text="" Visible="false"></asp:Label>
            </td>
        </tr>
    </table>
</asp:Content>

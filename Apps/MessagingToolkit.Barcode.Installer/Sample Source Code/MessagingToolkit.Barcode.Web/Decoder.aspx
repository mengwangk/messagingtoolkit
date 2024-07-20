<%@ Page Title="Decoder" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Decoder.aspx.cs" Inherits="MessagingToolkit.Barcode.Web.Decoder" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>
        Decoder
    </h2>
    <table width="100%" cellpadding="10px" cellspacing="10px">
        <tr>
            <td>
                File Location
            </td>
            <td>
                <asp:FileUpload ID="fileUploadImage" runat="server" />
                &nbsp;
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
            <td>
                <asp:Button ID="btnDecode" runat="server" Text="Decode" 
                    onclick="btnDecode_Click" />&nbsp;<asp:Button ID="btnReset"
                    runat="server" Text="Reset" UseSubmitBehavior="False" 
                    onclick="btnReset_Click" />
            </td>
        </tr>
          <tr>
            <td>
                Barcode Type
            </td>
            <td colspan="2">
                <asp:TextBox ID="txtBarcodeType" runat="server" Width="255px"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                Result
            </td>
            <td colspan="2">
                <asp:TextBox ID="txtDecodedResult" runat="server" TextMode="MultiLine"  Width="255px" Rows="8"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Label ID="lblMsg" runat="server" Text="" Visible="false"></asp:Label>
            </td>
        </tr>
    </table>
</asp:Content>

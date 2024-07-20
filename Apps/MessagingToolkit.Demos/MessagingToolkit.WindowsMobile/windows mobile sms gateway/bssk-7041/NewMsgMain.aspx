<%@ Page  MasterPageFile="~/MasterPage.master" Title="New MsgMain" %>

<asp:Content ID="Content1" ContentPlaceHolderID="mainContentPlaceholder" Runat="Server">
   <div class="pagecontent">
        <h1>New MsgMain</h1>      

<asp:DetailsView runat="server" DefaultMode="Insert" PageIndex="-1" DataSourceID="MsgMainsDataSource" EmptyDataText="The requested record was not found." ID="MsgMainsDetailsView" AutoGenerateRows="False" DataKeyNames="Msgid"><Fields>
<asp:BoundField ReadOnly="True" DataField="Msgid" HeaderText="Msgid"></asp:BoundField>
<asp:TemplateField SortExpression="Username" HeaderText="Username"><InsertItemTemplate>
<asp:ObjectDataSource runat="server" DeleteMethod="Delete" EnableCaching="True" ConflictDetection="CompareAllValues" SelectMethod="GetAllUsers" ID="User_UsernameDataSource" DataObjectTypeName="bulksms.User" UpdateMethod="Update" TypeName="bulksms.User" OldValuesParameterFormatString="original_{0}"></asp:ObjectDataSource>

<asp:DropDownList runat="server" ID="User_UsernameList" DataTextField="Username" DataValueField="Username" SelectedValue='<%# Bind("Username") %>' DataSourceID="User_UsernameDataSource" >
</asp:DropDownList>
</InsertItemTemplate>
</asp:TemplateField>
<asp:BoundField DataField="Message" HeaderText="Message"></asp:BoundField>
<asp:BoundField DataField="Type" HeaderText="Type"></asp:BoundField>
<asp:TemplateField SortExpression="Sender" HeaderText="Sender"><InsertItemTemplate>
<asp:ObjectDataSource runat="server" DeleteMethod="Delete" EnableCaching="True" ConflictDetection="CompareAllValues" SelectMethod="GetAllSenderIds" ID="SenderId_SenderDataSource" DataObjectTypeName="bulksms.SenderId" UpdateMethod="Update" TypeName="bulksms.SenderId" OldValuesParameterFormatString="original_{0}"></asp:ObjectDataSource>

<asp:DropDownList runat="server" ID="SenderId_SenderList" DataTextField="Tableid" DataValueField="Tableid" SelectedValue='<%# Bind("Sender") %>' DataSourceID="SenderId_SenderDataSource" >
</asp:DropDownList>
</InsertItemTemplate>
</asp:TemplateField>
<asp:BoundField DataField="Date" HeaderText="Date"></asp:BoundField>
<asp:HyperLinkField Text="View MsgDetails" DataNavigateUrlFormatString="~/MsgDetails.aspx?Table=MsgMain_MsgDetails&amp;MsgDetails_Msgid={0}" DataNavigateUrlFields="Msgid,"></asp:HyperLinkField>
<asp:HyperLinkField Text="View MsgHistories" DataNavigateUrlFormatString="~/MsgHistories.aspx?Table=MsgMain_MsgHistories&amp;MsgHistories_Msgid={0}" DataNavigateUrlFields="Msgid,"></asp:HyperLinkField>
<asp:HyperLinkField Text="View MsgOutboxes" DataNavigateUrlFormatString="~/MsgOutboxes.aspx?Table=MsgMain_MsgOutboxes&amp;MsgOutboxes_Msgid={0}" DataNavigateUrlFields="Msgid,"></asp:HyperLinkField>
<asp:CommandField CancelText="Clear Values" ShowInsertButton="True"></asp:CommandField>
</Fields>
</asp:DetailsView>

<asp:ObjectDataSource runat="server" DeleteMethod="Delete" EnableCaching="True" ConflictDetection="CompareAllValues" InsertMethod="Insert" SelectMethod="GetMsgMain" ID="MsgMainsDataSource" DataObjectTypeName="bulksms.MsgMain" UpdateMethod="Update" TypeName="bulksms.MsgMain" OldValuesParameterFormatString="original_{0}"><SelectParameters>
<asp:QueryStringParameter ConvertEmptyStringToNull="False" Name="Msgid" QueryStringField="Msgid"></asp:QueryStringParameter>
</SelectParameters>
</asp:ObjectDataSource>

    </div>
</asp:Content>

<%@ Page  MasterPageFile="~/MasterPage.master" Title="MsgMain Detail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="mainContentPlaceholder" Runat="Server">
   <div class="pagecontent">
        <h1>MsgMain Detail</h1>      

<asp:DetailsView runat="server" DataSourceID="MsgMainsDataSource" EmptyDataText="The requested record was not found." ID="MsgMainsDetailsView" AutoGenerateRows="False" DataKeyNames="Msgid"><Fields>
<asp:BoundField ReadOnly="True" DataField="Msgid" InsertVisible="False" HeaderText="Msgid"></asp:BoundField>
<asp:TemplateField SortExpression="Username" HeaderText="Username"><EditItemTemplate>
<asp:ObjectDataSource runat="server" DeleteMethod="Delete" EnableCaching="True" ConflictDetection="CompareAllValues" SelectMethod="GetAllUsers" ID="User_UsernameDataSource" DataObjectTypeName="bulksms.User" UpdateMethod="Update" TypeName="bulksms.User" OldValuesParameterFormatString="original_{0}"></asp:ObjectDataSource>

<asp:DropDownList runat="server" ID="User_UsernameList" DataTextField="Username" DataValueField="Username" SelectedValue='<%# Bind("Username") %>' DataSourceID="User_UsernameDataSource" >
</asp:DropDownList>
</EditItemTemplate>
<ItemTemplate>
<asp:HyperLink ID="UsernameLink" runat="server" Text='<%# Eval("Username") %>' NavigateUrl='<%# Eval("Username", "~/UserDetail.aspx?Username={0}")%>' />
</ItemTemplate>
</asp:TemplateField>
<asp:BoundField DataField="Message" InsertVisible="False" HeaderText="Message"></asp:BoundField>
<asp:BoundField DataField="Type" InsertVisible="False" HeaderText="Type"></asp:BoundField>
<asp:TemplateField SortExpression="Sender" HeaderText="Sender"><EditItemTemplate>
<asp:ObjectDataSource runat="server" DeleteMethod="Delete" EnableCaching="True" ConflictDetection="CompareAllValues" SelectMethod="GetAllSenderIds" ID="SenderId_SenderDataSource" DataObjectTypeName="bulksms.SenderId" UpdateMethod="Update" TypeName="bulksms.SenderId" OldValuesParameterFormatString="original_{0}"></asp:ObjectDataSource>

<asp:DropDownList runat="server" ID="SenderId_SenderList" DataTextField="Tableid" DataValueField="Tableid" SelectedValue='<%# Bind("Sender") %>' DataSourceID="SenderId_SenderDataSource" >
</asp:DropDownList>
</EditItemTemplate>
<ItemTemplate>
<asp:HyperLink ID="SenderLink" runat="server" Text='<%# Eval("Sender") %>' NavigateUrl='<%# Eval("Sender", "~/SenderIdDetail.aspx?Tableid={0}")%>' />
</ItemTemplate>
</asp:TemplateField>
<asp:BoundField DataField="Date" InsertVisible="False" HeaderText="Date"></asp:BoundField>
<asp:HyperLinkField Text="View MsgDetails" DataNavigateUrlFormatString="~/MsgDetails.aspx?Table=MsgMain_MsgDetails&amp;MsgDetails_Msgid={0}" DataNavigateUrlFields="Msgid,"></asp:HyperLinkField>
<asp:HyperLinkField Text="View MsgHistories" DataNavigateUrlFormatString="~/MsgHistories.aspx?Table=MsgMain_MsgHistories&amp;MsgHistories_Msgid={0}" DataNavigateUrlFields="Msgid,"></asp:HyperLinkField>
<asp:HyperLinkField Text="View MsgOutboxes" DataNavigateUrlFormatString="~/MsgOutboxes.aspx?Table=MsgMain_MsgOutboxes&amp;MsgOutboxes_Msgid={0}" DataNavigateUrlFields="Msgid,"></asp:HyperLinkField>
<asp:CommandField ShowDeleteButton="True" ShowEditButton="True"></asp:CommandField>
</Fields>
</asp:DetailsView>

<asp:ObjectDataSource runat="server" DeleteMethod="Delete" EnableCaching="True" ConflictDetection="CompareAllValues" InsertMethod="Insert" SelectMethod="GetMsgMain" ID="MsgMainsDataSource" DataObjectTypeName="bulksms.MsgMain" UpdateMethod="Update" TypeName="bulksms.MsgMain" OldValuesParameterFormatString="original_{0}"><SelectParameters>
<asp:QueryStringParameter ConvertEmptyStringToNull="False" Name="Msgid" QueryStringField="Msgid"></asp:QueryStringParameter>
</SelectParameters>
</asp:ObjectDataSource>

    </div>
</asp:Content>

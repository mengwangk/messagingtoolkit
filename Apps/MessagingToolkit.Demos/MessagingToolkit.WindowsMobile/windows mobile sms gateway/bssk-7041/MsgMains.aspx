<%@ Page  MasterPageFile="~/MasterPage.master" Title="MsgMains"%>

<asp:Content ID="Content1" ContentPlaceHolderID="mainContentPlaceholder" Runat="Server">
   <div class="pagecontent">
        <h1>MsgMains</h1>
<asp:GridView runat="server" DataSourceID="MsgMainsDataSource" ID="MsgMainsGridView" AllowSorting="True" EmptyDataText="No records were returned." AutoGenerateColumns="False" AllowPaging="True" DataKeyNames="Msgid"><Columns>
<asp:CommandField ShowDeleteButton="True" ShowEditButton="True"></asp:CommandField>
<asp:BoundField ReadOnly="True" DataField="Msgid" SortExpression="Msgid" HeaderText="Msgid"></asp:BoundField>
<asp:TemplateField SortExpression="Username" HeaderText="Username"><EditItemTemplate>
<asp:ObjectDataSource runat="server" DeleteMethod="Delete" EnableCaching="True" ConflictDetection="CompareAllValues" SelectMethod="GetAllUsers" ID="User_UsernameDataSource" DataObjectTypeName="bulksms.User" UpdateMethod="Update" TypeName="bulksms.User" OldValuesParameterFormatString="original_{0}"></asp:ObjectDataSource>

<asp:DropDownList runat="server" ID="User_UsernameList" DataTextField="Username" DataValueField="Username" SelectedValue='<%# Bind("Username") %>' DataSourceID="User_UsernameDataSource" >
</asp:DropDownList>
</EditItemTemplate>
<ItemTemplate>
<asp:HyperLink ID="UsernameLink" runat="server" Text='<%# Eval("Username") %>' NavigateUrl='<%# Eval("Username", "~/UserDetail.aspx?Username={0}")%>' />
</ItemTemplate>
</asp:TemplateField>
<asp:BoundField DataField="Message" SortExpression="Message" HeaderText="Message"></asp:BoundField>
<asp:BoundField DataField="Type" SortExpression="Type" HeaderText="Type"></asp:BoundField>
<asp:TemplateField SortExpression="Sender" HeaderText="Sender"><EditItemTemplate>
<asp:ObjectDataSource runat="server" DeleteMethod="Delete" EnableCaching="True" ConflictDetection="CompareAllValues" SelectMethod="GetAllSenderIds" ID="SenderId_SenderDataSource" DataObjectTypeName="bulksms.SenderId" UpdateMethod="Update" TypeName="bulksms.SenderId" OldValuesParameterFormatString="original_{0}"></asp:ObjectDataSource>

<asp:DropDownList runat="server" ID="SenderId_SenderList" DataTextField="Tableid" DataValueField="Tableid" SelectedValue='<%# Bind("Sender") %>' DataSourceID="SenderId_SenderDataSource" >
</asp:DropDownList>
</EditItemTemplate>
<ItemTemplate>
<asp:HyperLink ID="SenderLink" runat="server" Text='<%# Eval("Sender") %>' NavigateUrl='<%# Eval("Sender", "~/SenderIdDetail.aspx?Tableid={0}")%>' />
</ItemTemplate>
</asp:TemplateField>
<asp:BoundField DataField="Date" SortExpression="Date" HeaderText="Date"></asp:BoundField>
<asp:HyperLinkField Text="View MsgDetails" DataNavigateUrlFormatString="~/MsgDetails.aspx?Table=MsgMain_MsgDetails&amp;MsgDetails_Msgid={0}" DataNavigateUrlFields="Msgid,"></asp:HyperLinkField>
<asp:HyperLinkField Text="View MsgHistories" DataNavigateUrlFormatString="~/MsgHistories.aspx?Table=MsgMain_MsgHistories&amp;MsgHistories_Msgid={0}" DataNavigateUrlFields="Msgid,"></asp:HyperLinkField>
<asp:HyperLinkField Text="View MsgOutboxes" DataNavigateUrlFormatString="~/MsgOutboxes.aspx?Table=MsgMain_MsgOutboxes&amp;MsgOutboxes_Msgid={0}" DataNavigateUrlFields="Msgid,"></asp:HyperLinkField>
<asp:HyperLinkField Text="View Details" DataNavigateUrlFormatString="~/MsgMainDetail.aspx?Msgid={0}" DataNavigateUrlFields="Msgid"></asp:HyperLinkField>
</Columns>
</asp:GridView>

<asp:ObjectDataSource runat="server" SelectCountMethod="GetMsgMainsCount" DeleteMethod="Delete" ConflictDetection="CompareAllValues" SortParameterName="sortExpression" SelectMethod="GetMsgMains" ID="MsgMainsDataSource" DataObjectTypeName="bulksms.MsgMain" EnablePaging="True" UpdateMethod="Update" TypeName="bulksms.MsgMain" OldValuesParameterFormatString="original_{0}"><SelectParameters>
<asp:QueryStringParameter Name="tableName" QueryStringField="Table"></asp:QueryStringParameter>
<asp:QueryStringParameter ConvertEmptyStringToNull="False" Name="MsgMains_Tableid" QueryStringField="MsgMains_Tableid"></asp:QueryStringParameter>
<asp:QueryStringParameter ConvertEmptyStringToNull="False" Name="MsgMains_Username" QueryStringField="MsgMains_Username"></asp:QueryStringParameter>
</SelectParameters>
</asp:ObjectDataSource>

        <div class="link">
        <asp:HyperLink runat="server" CssClass="button" ID="NewRecordLink" NavigateUrl="~/NewMsgMain.aspx">Create New MsgMain</asp:HyperLink>

        </div>    
    </div>
</asp:Content>

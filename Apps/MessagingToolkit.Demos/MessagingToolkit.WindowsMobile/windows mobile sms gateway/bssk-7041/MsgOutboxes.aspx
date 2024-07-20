<%@ Page  MasterPageFile="~/MasterPage.master" Title="MsgOutboxes"%>

<asp:Content ID="Content1" ContentPlaceHolderID="mainContentPlaceholder" Runat="Server">
   <div class="pagecontent">
        <h1>MsgOutboxes</h1>
<asp:GridView runat="server" DataSourceID="MsgOutboxesDataSource" ID="MsgOutboxesGridView" AllowSorting="True" EmptyDataText="No records were returned." AutoGenerateColumns="False" AllowPaging="True" DataKeyNames="MsgOutboxId"><Columns>
<asp:CommandField ShowDeleteButton="True" ShowEditButton="True"></asp:CommandField>
<asp:BoundField DataField="Mobile" SortExpression="Mobile" HeaderText="Mobile"></asp:BoundField>
<asp:TemplateField SortExpression="Username" HeaderText="Username"><EditItemTemplate>
<asp:ObjectDataSource runat="server" DeleteMethod="Delete" EnableCaching="True" ConflictDetection="CompareAllValues" SelectMethod="GetAllUsers" ID="User_UsernameDataSource" DataObjectTypeName="bulksms.User" UpdateMethod="Update" TypeName="bulksms.User" OldValuesParameterFormatString="original_{0}"></asp:ObjectDataSource>

<asp:DropDownList runat="server" ID="User_UsernameList" DataTextField="Username" DataValueField="Username" SelectedValue='<%# Bind("Username") %>' DataSourceID="User_UsernameDataSource" >
</asp:DropDownList>
</EditItemTemplate>
<ItemTemplate>
<asp:HyperLink ID="UsernameLink" runat="server" Text='<%# Eval("Username") %>' NavigateUrl='<%# Eval("Username", "~/UserDetail.aspx?Username={0}")%>' />
</ItemTemplate>
</asp:TemplateField>
<asp:TemplateField SortExpression="Msgid" HeaderText="Msgid"><EditItemTemplate>
<asp:ObjectDataSource runat="server" DeleteMethod="Delete" EnableCaching="True" ConflictDetection="CompareAllValues" SelectMethod="GetAllMsgMains" ID="MsgMain_MsgidDataSource" DataObjectTypeName="bulksms.MsgMain" UpdateMethod="Update" TypeName="bulksms.MsgMain" OldValuesParameterFormatString="original_{0}"></asp:ObjectDataSource>

<asp:DropDownList runat="server" ID="MsgMain_MsgidList" DataTextField="Msgid" DataValueField="Msgid" SelectedValue='<%# Bind("Msgid") %>' DataSourceID="MsgMain_MsgidDataSource" >
</asp:DropDownList>
</EditItemTemplate>
<ItemTemplate>
<asp:HyperLink ID="MsgidLink" runat="server" Text='<%# Eval("Msgid") %>' NavigateUrl='<%# Eval("Msgid", "~/MsgMainDetail.aspx?Msgid={0}")%>' />
</ItemTemplate>
</asp:TemplateField>
<asp:BoundField DataField="Groupid" SortExpression="Groupid" HeaderText="Groupid"></asp:BoundField>
<asp:BoundField DataField="Statusid" SortExpression="Statusid" HeaderText="Statusid"></asp:BoundField>
<asp:BoundField ReadOnly="True" DataField="MsgOutboxId" InsertVisible="False" SortExpression="MsgOutboxId" HeaderText="MsgOutboxId"></asp:BoundField>
<asp:HyperLinkField Text="View Details" DataNavigateUrlFormatString="~/MsgOutboxDetail.aspx?MsgOutboxId={0}" DataNavigateUrlFields="MsgOutboxId"></asp:HyperLinkField>
</Columns>
</asp:GridView>

<asp:ObjectDataSource runat="server" SelectCountMethod="GetMsgOutboxesCount" DeleteMethod="Delete" ConflictDetection="CompareAllValues" SortParameterName="sortExpression" SelectMethod="GetMsgOutboxes" ID="MsgOutboxesDataSource" DataObjectTypeName="bulksms.MsgOutbox" EnablePaging="True" UpdateMethod="Update" TypeName="bulksms.MsgOutbox" OldValuesParameterFormatString="original_{0}"><SelectParameters>
<asp:QueryStringParameter Name="tableName" QueryStringField="Table"></asp:QueryStringParameter>
<asp:QueryStringParameter ConvertEmptyStringToNull="False" Name="MsgOutboxes_Msgid" QueryStringField="MsgOutboxes_Msgid"></asp:QueryStringParameter>
<asp:QueryStringParameter ConvertEmptyStringToNull="False" Name="MsgOutboxes_Username" QueryStringField="MsgOutboxes_Username"></asp:QueryStringParameter>
</SelectParameters>
</asp:ObjectDataSource>

        <div class="link">
        <asp:HyperLink runat="server" CssClass="button" ID="NewRecordLink" NavigateUrl="~/NewMsgOutbox.aspx">Create New MsgOutbox</asp:HyperLink>

        </div>    
    </div>
</asp:Content>

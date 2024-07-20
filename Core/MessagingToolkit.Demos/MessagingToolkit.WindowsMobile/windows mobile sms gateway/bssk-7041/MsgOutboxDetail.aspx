<%@ Page  MasterPageFile="~/MasterPage.master" Title="MsgOutbox Detail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="mainContentPlaceholder" Runat="Server">
   <div class="pagecontent">
        <h1>MsgOutbox Detail</h1>      

<asp:DetailsView runat="server" DataSourceID="MsgOutboxesDataSource" EmptyDataText="The requested record was not found." ID="MsgOutboxesDetailsView" AutoGenerateRows="False" DataKeyNames="MsgOutboxId"><Fields>
<asp:BoundField DataField="Mobile" InsertVisible="False" HeaderText="Mobile"></asp:BoundField>
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
<asp:BoundField DataField="Groupid" InsertVisible="False" HeaderText="Groupid"></asp:BoundField>
<asp:BoundField DataField="Statusid" InsertVisible="False" HeaderText="Statusid"></asp:BoundField>
<asp:BoundField ReadOnly="True" DataField="MsgOutboxId" InsertVisible="False" HeaderText="MsgOutboxId"></asp:BoundField>
<asp:CommandField ShowDeleteButton="True" ShowEditButton="True"></asp:CommandField>
</Fields>
</asp:DetailsView>

<asp:ObjectDataSource runat="server" DeleteMethod="Delete" EnableCaching="True" ConflictDetection="CompareAllValues" InsertMethod="Insert" SelectMethod="GetMsgOutbox" ID="MsgOutboxesDataSource" DataObjectTypeName="bulksms.MsgOutbox" UpdateMethod="Update" TypeName="bulksms.MsgOutbox" OldValuesParameterFormatString="original_{0}"><SelectParameters>
<asp:QueryStringParameter ConvertEmptyStringToNull="False" Name="MsgOutboxId" QueryStringField="MsgOutboxId"></asp:QueryStringParameter>
</SelectParameters>
</asp:ObjectDataSource>

    </div>
</asp:Content>

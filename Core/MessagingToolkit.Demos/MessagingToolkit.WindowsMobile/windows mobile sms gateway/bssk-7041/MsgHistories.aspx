<%@ Page  MasterPageFile="~/MasterPage.master" Title="MsgHistories"%>

<asp:Content ID="Content1" ContentPlaceHolderID="mainContentPlaceholder" Runat="Server">
   <div class="pagecontent">
        <h1>MsgHistories</h1>
<asp:GridView runat="server" DataSourceID="MsgHistoriesDataSource" ID="MsgHistoriesGridView" AllowSorting="True" EmptyDataText="No records were returned." AutoGenerateColumns="False" AllowPaging="True" DataKeyNames="MsgHistoryId"><Columns>
<asp:CommandField ShowDeleteButton="True" ShowEditButton="True"></asp:CommandField>
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
<asp:TemplateField SortExpression="Groupid" HeaderText="Groupid"><EditItemTemplate>
<asp:ObjectDataSource runat="server" DeleteMethod="Delete" EnableCaching="True" ConflictDetection="CompareAllValues" SelectMethod="GetAllGrpMains" ID="GrpMain_GroupidDataSource" DataObjectTypeName="bulksms.GrpMain" UpdateMethod="Update" TypeName="bulksms.GrpMain" OldValuesParameterFormatString="original_{0}"></asp:ObjectDataSource>

<asp:DropDownList runat="server" ID="GrpMain_GroupidList" DataTextField="Groupid" DataValueField="Groupid" SelectedValue='<%# Bind("Groupid") %>' DataSourceID="GrpMain_GroupidDataSource" >
</asp:DropDownList>
</EditItemTemplate>
<ItemTemplate>
<asp:HyperLink ID="GroupidLink" runat="server" Text='<%# Eval("Groupid") %>' NavigateUrl='<%# Eval("Groupid", "~/GrpMainDetail.aspx?Groupid={0}")%>' />
</ItemTemplate>
</asp:TemplateField>
<asp:BoundField DataField="Total" SortExpression="Total" HeaderText="Total"></asp:BoundField>
<asp:BoundField DataField="Hide" SortExpression="Hide" HeaderText="Hide"></asp:BoundField>
<asp:BoundField DataField="Statusid" SortExpression="Statusid" HeaderText="Statusid"></asp:BoundField>
<asp:BoundField ReadOnly="True" DataField="MsgHistoryId" SortExpression="MsgHistoryId" HeaderText="MsgHistoryId"></asp:BoundField>
<asp:HyperLinkField Text="View Details" DataNavigateUrlFormatString="~/MsgHistoryDetail.aspx?MsgHistoryId={0}" DataNavigateUrlFields="MsgHistoryId"></asp:HyperLinkField>
</Columns>
</asp:GridView>

<asp:ObjectDataSource runat="server" SelectCountMethod="GetMsgHistoriesCount" DeleteMethod="Delete" ConflictDetection="CompareAllValues" SortParameterName="sortExpression" SelectMethod="GetMsgHistories" ID="MsgHistoriesDataSource" DataObjectTypeName="bulksms.MsgHistory" EnablePaging="True" UpdateMethod="Update" TypeName="bulksms.MsgHistory" OldValuesParameterFormatString="original_{0}"><SelectParameters>
<asp:QueryStringParameter Name="tableName" QueryStringField="Table"></asp:QueryStringParameter>
<asp:QueryStringParameter ConvertEmptyStringToNull="False" Name="MsgHistories_Msgid" QueryStringField="MsgHistories_Msgid"></asp:QueryStringParameter>
<asp:QueryStringParameter ConvertEmptyStringToNull="False" Name="MsgHistories_Groupid" QueryStringField="MsgHistories_Groupid"></asp:QueryStringParameter>
<asp:QueryStringParameter ConvertEmptyStringToNull="False" Name="MsgHistories_Username" QueryStringField="MsgHistories_Username"></asp:QueryStringParameter>
</SelectParameters>
</asp:ObjectDataSource>

        <div class="link">
        <asp:HyperLink runat="server" CssClass="button" ID="NewRecordLink" NavigateUrl="~/NewMsgHistory.aspx">Create New MsgHistory</asp:HyperLink>

        </div>    
    </div>
</asp:Content>

<%@ Page  MasterPageFile="~/MasterPage.master" Title="PointHistories"%>

<asp:Content ID="Content1" ContentPlaceHolderID="mainContentPlaceholder" Runat="Server">
   <div class="pagecontent">
        <h1>PointHistories</h1>
<asp:GridView runat="server" DataSourceID="PointHistoriesDataSource" ID="PointHistoriesGridView" AllowSorting="True" EmptyDataText="No records were returned." AutoGenerateColumns="False" AllowPaging="True" DataKeyNames="PointHistoryId"><Columns>
<asp:CommandField ShowDeleteButton="True" ShowEditButton="True"></asp:CommandField>
<asp:TemplateField SortExpression="Username" HeaderText="Username"><EditItemTemplate>
<asp:ObjectDataSource runat="server" DeleteMethod="Delete" EnableCaching="True" ConflictDetection="CompareAllValues" SelectMethod="GetAllUsers" ID="User_UsernameDataSource" DataObjectTypeName="bulksms.User" UpdateMethod="Update" TypeName="bulksms.User" OldValuesParameterFormatString="original_{0}"></asp:ObjectDataSource>

<asp:DropDownList runat="server" ID="User_UsernameList" DataTextField="Username" DataValueField="Username" SelectedValue='<%# Bind("Username") %>' DataSourceID="User_UsernameDataSource" AppendDataBoundItems="true" >
<asp:ListItem Text="<null>" Value="" />
</asp:DropDownList>
</EditItemTemplate>
<ItemTemplate>
<asp:HyperLink ID="UsernameLink" runat="server" Text='<%# Eval("Username") %>' NavigateUrl='<%# Eval("Username", "~/UserDetail.aspx?Username={0}")%>' />
</ItemTemplate>
</asp:TemplateField>
<asp:BoundField DataField="Date" SortExpression="Date" HeaderText="Date"></asp:BoundField>
<asp:BoundField DataField="Debit" SortExpression="Debit" HeaderText="Debit"></asp:BoundField>
<asp:BoundField DataField="Credit" SortExpression="Credit" HeaderText="Credit"></asp:BoundField>
<asp:BoundField DataField="Details" SortExpression="Details" HeaderText="Details"></asp:BoundField>
<asp:BoundField ReadOnly="True" DataField="PointHistoryId" InsertVisible="False" SortExpression="PointHistoryId" HeaderText="PointHistoryId"></asp:BoundField>
<asp:HyperLinkField Text="View Details" DataNavigateUrlFormatString="~/PointHistoryDetail.aspx?PointHistoryId={0}" DataNavigateUrlFields="PointHistoryId"></asp:HyperLinkField>
</Columns>
</asp:GridView>

<asp:ObjectDataSource runat="server" SelectCountMethod="GetPointHistoriesCount" DeleteMethod="Delete" ConflictDetection="CompareAllValues" SortParameterName="sortExpression" SelectMethod="GetPointHistories" ID="PointHistoriesDataSource" DataObjectTypeName="bulksms.PointHistory" EnablePaging="True" UpdateMethod="Update" TypeName="bulksms.PointHistory" OldValuesParameterFormatString="original_{0}"><SelectParameters>
<asp:QueryStringParameter Name="tableName" QueryStringField="Table"></asp:QueryStringParameter>
<asp:QueryStringParameter ConvertEmptyStringToNull="False" Name="PointHistories_Username" QueryStringField="PointHistories_Username"></asp:QueryStringParameter>
</SelectParameters>
</asp:ObjectDataSource>

        <div class="link">
        <asp:HyperLink runat="server" CssClass="button" ID="NewRecordLink" NavigateUrl="~/NewPointHistory.aspx">Create New PointHistory</asp:HyperLink>

        </div>    
    </div>
</asp:Content>

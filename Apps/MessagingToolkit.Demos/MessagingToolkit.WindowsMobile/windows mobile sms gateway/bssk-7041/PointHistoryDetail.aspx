<%@ Page  MasterPageFile="~/MasterPage.master" Title="PointHistory Detail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="mainContentPlaceholder" Runat="Server">
   <div class="pagecontent">
        <h1>PointHistory Detail</h1>      

<asp:DetailsView runat="server" DataSourceID="PointHistoriesDataSource" EmptyDataText="The requested record was not found." ID="PointHistoriesDetailsView" AutoGenerateRows="False" DataKeyNames="PointHistoryId"><Fields>
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
<asp:BoundField DataField="Date" InsertVisible="False" HeaderText="Date"></asp:BoundField>
<asp:BoundField DataField="Debit" InsertVisible="False" HeaderText="Debit"></asp:BoundField>
<asp:BoundField DataField="Credit" InsertVisible="False" HeaderText="Credit"></asp:BoundField>
<asp:BoundField DataField="Details" InsertVisible="False" HeaderText="Details"></asp:BoundField>
<asp:BoundField ReadOnly="True" DataField="PointHistoryId" InsertVisible="False" HeaderText="PointHistoryId"></asp:BoundField>
<asp:CommandField ShowDeleteButton="True" ShowEditButton="True"></asp:CommandField>
</Fields>
</asp:DetailsView>

<asp:ObjectDataSource runat="server" DeleteMethod="Delete" EnableCaching="True" ConflictDetection="CompareAllValues" InsertMethod="Insert" SelectMethod="GetPointHistory" ID="PointHistoriesDataSource" DataObjectTypeName="bulksms.PointHistory" UpdateMethod="Update" TypeName="bulksms.PointHistory" OldValuesParameterFormatString="original_{0}"><SelectParameters>
<asp:QueryStringParameter ConvertEmptyStringToNull="False" Name="PointHistoryId" QueryStringField="PointHistoryId"></asp:QueryStringParameter>
</SelectParameters>
</asp:ObjectDataSource>

    </div>
</asp:Content>

<%@ Page  MasterPageFile="~/MasterPage.master" Title="New PointHistory" %>

<asp:Content ID="Content1" ContentPlaceHolderID="mainContentPlaceholder" Runat="Server">
   <div class="pagecontent">
        <h1>New PointHistory</h1>      

<asp:DetailsView runat="server" DefaultMode="Insert" PageIndex="-1" DataSourceID="PointHistoriesDataSource" EmptyDataText="The requested record was not found." ID="PointHistoriesDetailsView" AutoGenerateRows="False" DataKeyNames="PointHistoryId"><Fields>
<asp:TemplateField SortExpression="Username" HeaderText="Username"><InsertItemTemplate>
<asp:ObjectDataSource runat="server" DeleteMethod="Delete" EnableCaching="True" ConflictDetection="CompareAllValues" SelectMethod="GetAllUsers" ID="User_UsernameDataSource" DataObjectTypeName="bulksms.User" UpdateMethod="Update" TypeName="bulksms.User" OldValuesParameterFormatString="original_{0}"></asp:ObjectDataSource>

<asp:DropDownList runat="server" ID="User_UsernameList" DataTextField="Username" DataValueField="Username" SelectedValue='<%# Bind("Username") %>' DataSourceID="User_UsernameDataSource" AppendDataBoundItems="true" >
<asp:ListItem Text="<null>" Value="" />
</asp:DropDownList>
</InsertItemTemplate>
</asp:TemplateField>
<asp:BoundField DataField="Date" HeaderText="Date"></asp:BoundField>
<asp:BoundField DataField="Debit" HeaderText="Debit"></asp:BoundField>
<asp:BoundField DataField="Credit" HeaderText="Credit"></asp:BoundField>
<asp:BoundField DataField="Details" HeaderText="Details"></asp:BoundField>
<asp:BoundField ReadOnly="True" DataField="PointHistoryId" InsertVisible="False" HeaderText="PointHistoryId"></asp:BoundField>
<asp:CommandField CancelText="Clear Values" ShowInsertButton="True"></asp:CommandField>
</Fields>
</asp:DetailsView>

<asp:ObjectDataSource runat="server" DeleteMethod="Delete" EnableCaching="True" ConflictDetection="CompareAllValues" InsertMethod="Insert" SelectMethod="GetPointHistory" ID="PointHistoriesDataSource" DataObjectTypeName="bulksms.PointHistory" UpdateMethod="Update" TypeName="bulksms.PointHistory" OldValuesParameterFormatString="original_{0}"><SelectParameters>
<asp:QueryStringParameter ConvertEmptyStringToNull="False" Name="PointHistoryId" QueryStringField="PointHistoryId"></asp:QueryStringParameter>
</SelectParameters>
</asp:ObjectDataSource>

    </div>
</asp:Content>

<%@ Page  MasterPageFile="~/MasterPage.master" Title="New Admin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="mainContentPlaceholder" Runat="Server">
   <div class="pagecontent">
        <h1>New Admin</h1>      

<asp:DetailsView runat="server" DefaultMode="Insert" PageIndex="-1" DataSourceID="AdminsDataSource" EmptyDataText="The requested record was not found." ID="AdminsDetailsView" AutoGenerateRows="False" DataKeyNames="Username"><Fields>
<asp:BoundField ReadOnly="True" DataField="Username" HeaderText="Username"></asp:BoundField>
<asp:BoundField DataField="Password" HeaderText="Password"></asp:BoundField>
<asp:BoundField DataField="Lastvisit" HeaderText="Lastvisit"></asp:BoundField>
<asp:BoundField DataField="Sold" HeaderText="Sold"></asp:BoundField>
<asp:CommandField CancelText="Clear Values" ShowInsertButton="True"></asp:CommandField>
</Fields>
</asp:DetailsView>

<asp:ObjectDataSource runat="server" DeleteMethod="Delete" EnableCaching="True" ConflictDetection="CompareAllValues" InsertMethod="Insert" SelectMethod="GetAdmin" ID="AdminsDataSource" DataObjectTypeName="bulksms.Admin" UpdateMethod="Update" TypeName="bulksms.Admin" OldValuesParameterFormatString="original_{0}"><SelectParameters>
<asp:QueryStringParameter ConvertEmptyStringToNull="False" Name="Username" QueryStringField="Username"></asp:QueryStringParameter>
</SelectParameters>
</asp:ObjectDataSource>

    </div>
</asp:Content>

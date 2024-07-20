<%@ Page  MasterPageFile="~/MasterPage.master" Title="Admin Detail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="mainContentPlaceholder" Runat="Server">
   <div class="pagecontent">
        <h1>Admin Detail</h1>      

<asp:DetailsView runat="server" DataSourceID="AdminsDataSource" EmptyDataText="The requested record was not found." ID="AdminsDetailsView" AutoGenerateRows="False" DataKeyNames="Username"><Fields>
<asp:BoundField ReadOnly="True" DataField="Username" InsertVisible="False" HeaderText="Username"></asp:BoundField>
<asp:BoundField DataField="Password" InsertVisible="False" HeaderText="Password"></asp:BoundField>
<asp:BoundField DataField="Lastvisit" InsertVisible="False" HeaderText="Lastvisit"></asp:BoundField>
<asp:BoundField DataField="Sold" InsertVisible="False" HeaderText="Sold"></asp:BoundField>
<asp:CommandField ShowDeleteButton="True" ShowEditButton="True"></asp:CommandField>
</Fields>
</asp:DetailsView>

<asp:ObjectDataSource runat="server" DeleteMethod="Delete" EnableCaching="True" ConflictDetection="CompareAllValues" InsertMethod="Insert" SelectMethod="GetAdmin" ID="AdminsDataSource" DataObjectTypeName="bulksms.Admin" UpdateMethod="Update" TypeName="bulksms.Admin" OldValuesParameterFormatString="original_{0}"><SelectParameters>
<asp:QueryStringParameter ConvertEmptyStringToNull="False" Name="Username" QueryStringField="Username"></asp:QueryStringParameter>
</SelectParameters>
</asp:ObjectDataSource>

    </div>
</asp:Content>

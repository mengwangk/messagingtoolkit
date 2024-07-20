<%@ Page  MasterPageFile="~/MasterPage.master" Title="New Detail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="mainContentPlaceholder" Runat="Server">
   <div class="pagecontent">
        <h1>New Detail</h1>      

<asp:DetailsView runat="server" DataSourceID="NewsDataSource" EmptyDataText="The requested record was not found." ID="NewsDetailsView" AutoGenerateRows="False" DataKeyNames="NewsId"><Fields>
<asp:BoundField ReadOnly="True" DataField="NewsId" InsertVisible="False" HeaderText="NewsId"></asp:BoundField>
<asp:BoundField DataField="Title" InsertVisible="False" HeaderText="Title"></asp:BoundField>
<asp:BoundField DataField="News" InsertVisible="False" HeaderText="News"></asp:BoundField>
<asp:BoundField DataField="NewsDate" InsertVisible="False" HeaderText="NewsDate"></asp:BoundField>
<asp:CommandField ShowDeleteButton="True" ShowEditButton="True"></asp:CommandField>
</Fields>
</asp:DetailsView>

<asp:ObjectDataSource runat="server" DeleteMethod="Delete" EnableCaching="True" ConflictDetection="CompareAllValues" InsertMethod="Insert" SelectMethod="GetNew" ID="NewsDataSource" DataObjectTypeName="bulksms.New" UpdateMethod="Update" TypeName="bulksms.New" OldValuesParameterFormatString="original_{0}"><SelectParameters>
<asp:QueryStringParameter ConvertEmptyStringToNull="False" Name="NewsId" QueryStringField="NewsId"></asp:QueryStringParameter>
</SelectParameters>
</asp:ObjectDataSource>

    </div>
</asp:Content>

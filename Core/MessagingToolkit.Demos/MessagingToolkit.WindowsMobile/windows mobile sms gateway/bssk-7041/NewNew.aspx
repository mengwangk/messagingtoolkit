<%@ Page  MasterPageFile="~/MasterPage.master" Title="New New" %>

<asp:Content ID="Content1" ContentPlaceHolderID="mainContentPlaceholder" Runat="Server">
   <div class="pagecontent">
        <h1>New New</h1>      

<asp:DetailsView runat="server" DefaultMode="Insert" PageIndex="-1" DataSourceID="NewsDataSource" EmptyDataText="The requested record was not found." ID="NewsDetailsView" AutoGenerateRows="False" DataKeyNames="NewsId"><Fields>
<asp:BoundField ReadOnly="True" DataField="NewsId" HeaderText="NewsId"></asp:BoundField>
<asp:BoundField DataField="Title" HeaderText="Title"></asp:BoundField>
<asp:BoundField DataField="News" HeaderText="News"></asp:BoundField>
<asp:BoundField DataField="NewsDate" HeaderText="NewsDate"></asp:BoundField>
<asp:CommandField CancelText="Clear Values" ShowInsertButton="True"></asp:CommandField>
</Fields>
</asp:DetailsView>

<asp:ObjectDataSource runat="server" DeleteMethod="Delete" EnableCaching="True" ConflictDetection="CompareAllValues" InsertMethod="Insert" SelectMethod="GetNew" ID="NewsDataSource" DataObjectTypeName="bulksms.New" UpdateMethod="Update" TypeName="bulksms.New" OldValuesParameterFormatString="original_{0}"><SelectParameters>
<asp:QueryStringParameter ConvertEmptyStringToNull="False" Name="NewsId" QueryStringField="NewsId"></asp:QueryStringParameter>
</SelectParameters>
</asp:ObjectDataSource>

    </div>
</asp:Content>

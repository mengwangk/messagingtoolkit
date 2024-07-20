<%@ Page  MasterPageFile="~/MasterPage.master" Title="Error Detail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="mainContentPlaceholder" Runat="Server">
   <div class="pagecontent">
        <h1>Error Detail</h1>      

<asp:DetailsView runat="server" DataSourceID="ErrorsDataSource" EmptyDataText="The requested record was not found." ID="ErrorsDetailsView" AutoGenerateRows="False" DataKeyNames="Id"><Fields>
<asp:BoundField ReadOnly="True" DataField="Id" InsertVisible="False" HeaderText="Id"></asp:BoundField>
<asp:BoundField DataField="Username" InsertVisible="False" HeaderText="Username"></asp:BoundField>
<asp:BoundField DataField="Date" InsertVisible="False" HeaderText="Date"></asp:BoundField>
<asp:BoundField DataField="Num" InsertVisible="False" HeaderText="Num"></asp:BoundField>
<asp:BoundField DataField="Msg" InsertVisible="False" HeaderText="Msg"></asp:BoundField>
<asp:BoundField DataField="File" InsertVisible="False" HeaderText="File"></asp:BoundField>
<asp:BoundField DataField="Line" InsertVisible="False" HeaderText="Line"></asp:BoundField>
<asp:CommandField ShowDeleteButton="True" ShowEditButton="True"></asp:CommandField>
</Fields>
</asp:DetailsView>

<asp:ObjectDataSource runat="server" DeleteMethod="Delete" EnableCaching="True" ConflictDetection="CompareAllValues" InsertMethod="Insert" SelectMethod="GetError" ID="ErrorsDataSource" DataObjectTypeName="bulksms.Error" UpdateMethod="Update" TypeName="bulksms.Error" OldValuesParameterFormatString="original_{0}"><SelectParameters>
<asp:QueryStringParameter ConvertEmptyStringToNull="False" Name="Id" QueryStringField="Id"></asp:QueryStringParameter>
</SelectParameters>
</asp:ObjectDataSource>

    </div>
</asp:Content>

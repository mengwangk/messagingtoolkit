<%@ Page  MasterPageFile="~/MasterPage.master" Title="New Error" %>

<asp:Content ID="Content1" ContentPlaceHolderID="mainContentPlaceholder" Runat="Server">
   <div class="pagecontent">
        <h1>New Error</h1>      

<asp:DetailsView runat="server" DefaultMode="Insert" PageIndex="-1" DataSourceID="ErrorsDataSource" EmptyDataText="The requested record was not found." ID="ErrorsDetailsView" AutoGenerateRows="False" DataKeyNames="Id"><Fields>
<asp:BoundField ReadOnly="True" DataField="Id" InsertVisible="False" HeaderText="Id"></asp:BoundField>
<asp:BoundField DataField="Username" HeaderText="Username"></asp:BoundField>
<asp:BoundField DataField="Date" HeaderText="Date"></asp:BoundField>
<asp:BoundField DataField="Num" HeaderText="Num"></asp:BoundField>
<asp:BoundField DataField="Msg" HeaderText="Msg"></asp:BoundField>
<asp:BoundField DataField="File" HeaderText="File"></asp:BoundField>
<asp:BoundField DataField="Line" HeaderText="Line"></asp:BoundField>
<asp:CommandField CancelText="Clear Values" ShowInsertButton="True"></asp:CommandField>
</Fields>
</asp:DetailsView>

<asp:ObjectDataSource runat="server" DeleteMethod="Delete" EnableCaching="True" ConflictDetection="CompareAllValues" InsertMethod="Insert" SelectMethod="GetError" ID="ErrorsDataSource" DataObjectTypeName="bulksms.Error" UpdateMethod="Update" TypeName="bulksms.Error" OldValuesParameterFormatString="original_{0}"><SelectParameters>
<asp:QueryStringParameter ConvertEmptyStringToNull="False" Name="Id" QueryStringField="Id"></asp:QueryStringParameter>
</SelectParameters>
</asp:ObjectDataSource>

    </div>
</asp:Content>

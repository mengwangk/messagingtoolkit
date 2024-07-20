<%@ Page  MasterPageFile="~/MasterPage.master" Title="Sysdiagram Detail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="mainContentPlaceholder" Runat="Server">
   <div class="pagecontent">
        <h1>Sysdiagram Detail</h1>      

<asp:DetailsView runat="server" DataSourceID="SysdiagramsDataSource" EmptyDataText="The requested record was not found." ID="SysdiagramsDetailsView" AutoGenerateRows="False" DataKeyNames="DiagramId"><Fields>
<asp:BoundField DataField="Name" InsertVisible="False" HeaderText="Name"></asp:BoundField>
<asp:BoundField DataField="PrincipalId" InsertVisible="False" HeaderText="PrincipalId"></asp:BoundField>
<asp:BoundField ReadOnly="True" DataField="DiagramId" InsertVisible="False" HeaderText="DiagramId"></asp:BoundField>
<asp:BoundField DataField="Version" InsertVisible="False" HeaderText="Version"></asp:BoundField>
<asp:CommandField ShowDeleteButton="True" ShowEditButton="True"></asp:CommandField>
</Fields>
</asp:DetailsView>

<asp:ObjectDataSource runat="server" DeleteMethod="Delete" EnableCaching="True" ConflictDetection="CompareAllValues" InsertMethod="Insert" SelectMethod="GetSysdiagram" ID="SysdiagramsDataSource" DataObjectTypeName="bulksms.Sysdiagram" UpdateMethod="Update" TypeName="bulksms.Sysdiagram" OldValuesParameterFormatString="original_{0}"><SelectParameters>
<asp:QueryStringParameter ConvertEmptyStringToNull="False" Name="DiagramId" QueryStringField="DiagramId"></asp:QueryStringParameter>
</SelectParameters>
</asp:ObjectDataSource>

    </div>
</asp:Content>
